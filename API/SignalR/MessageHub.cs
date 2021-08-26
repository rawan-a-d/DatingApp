using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
	public class MessageHub : Hub
	{
		private readonly IUnitOfWork _unitOfWork;

		//private readonly IMessageRepository _messageRepository;
		private readonly IMapper _mapper;
		//private readonly IUserRepository _userRepository;
		private readonly IHubContext<PresenceHub> _presenceHub;
		private readonly PresenceTracker _tracker;

		//public MessageHub(IMessageRepository messageRepository, 
		//					IMapper mapper, 
		//					IUserRepository userRepository, 
		//					IHubContext<PresenceHub> presenceHub,
		//					PresenceTracker tracker)
		//{
		//	_presenceHub = presenceHub;
		//	_tracker = tracker;
		//	_userRepository = userRepository;
		//	_messageRepository = messageRepository;
		//	_mapper = mapper;
		//}
		public MessageHub(IUnitOfWork unitOfWork,
							IMapper mapper, 
							IHubContext<PresenceHub> presenceHub,
							PresenceTracker tracker)
		{
			_presenceHub = presenceHub;
			_tracker = tracker;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override async Task OnConnectedAsync()
		{
			var httpContext = Context.GetHttpContext();

			// caller user
			//var callerUser = httpContext.User.GetUsername();
			var callerUser = Context.User.GetUsername();

			// other user
			var otherUser = httpContext.Request.Query["user"].ToString();

			// get group name
			var groupName = GetGroupName(callerUser, otherUser);

			// create group for users
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

			// group tracking
			// add user to group
			var group = await AddToGroup(groupName);
			// send the updated group back, if it's empty SignalR doesn't send anything back
			await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

			//var messages = await _messageRepository.GetMessageThread(callerUser, otherUser);
			var messages = await _unitOfWork.MessageRepository.GetMessageThread(callerUser, otherUser);

			// if updates (messages marked as read) => save to db
			if (_unitOfWork.HasChanges()) {
				await _unitOfWork.Complete();
			}

			// send message thread to both of the connected users
			//await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);

			// send message thread to the user who connected
			await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="ex">Exception</param>
		/// <returns></returns>
		public override async Task OnDisconnectedAsync(Exception ex)
		{
			// group tracking
			// remove user from group
			var group = await RemoveFromMessageGroup();

			// send the updated group back, if it's empty SignalR doesn't send anything back
			await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

			// will be removed automatically
			await base.OnDisconnectedAsync(ex);
		}


		public async Task SendMessage(CreateMessageDto createMessageDto)
		{
			var username = Context.User.GetUsername();

			// If user is sending a message to himself
			if (username == createMessageDto.RecipientUsername.ToLower())
			{
				throw new HubException("You cannot send a message to yourself");
			}

			// get users
			//var sender = await _userRepository.GetUserByUsernameAsync(username);
			//var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
			var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
			var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

			// if no recipient
			if (recipient == null)
			{
				throw new HubException("Recipient cannot be found");
			}

			// create message
			var message = new Message
			{
				Sender = sender,
				SenderUsername = sender.UserName,
				Recipient = recipient,
				RecipientUsername = recipient.UserName,
				Content = createMessageDto.Content
			};

			// get group name
			var groupName = GetGroupName(sender.UserName, recipient.UserName);

			// get message group
			//var group = await _messageRepository.GetMessageGroup(groupName);
			var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

			// if recipient is connected and in the messages tab
			if (group.Connections.Any(x => x.Username == recipient.UserName))
			{
				message.DateRead = DateTime.UtcNow;
			}
			else
			{
				// get user connections
				var connections = await _tracker.GetConnectionsForUser(recipient.UserName);

				// user is online but not a part of the group (not connected to the same group)
				if(connections != null) {
					await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new {
						username = sender.UserName,
						knownAs = sender.KnownAs
					});
				}
			}

			// add message
			//_messageRepository.AddMessage(message);
			_unitOfWork.MessageRepository.AddMessage(message);

			// save to db
			//if (await _messageRepository.SaveAllAsync())
			//{
			//	// Send NewMessage to sender and recipient
			//	await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
			//}
			if (await _unitOfWork.Complete())
			{
				// Send NewMessage to sender and recipient
				await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
			}

			//throw new HubException("Failed to send message");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="caller"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		private string GetGroupName(string caller, string other)
		{
			// check alphabetical order of the usernames
			var stringCompare = string.CompareOrdinal(caller, other) < 0;

			//return stringCompare ? string.Concat(caller, other) : string.Concat(other, caller);

			return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
		}

		/// <summary>
		/// Add user to group
		/// </summary>
		/// <param name="groupName">group name</param>
		/// <returns>the group</returns>
		private async Task<Group> AddToGroup(string groupName)
		{
			// get group
			//var group = await _messageRepository.GetMessageGroup(groupName);
			var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

			// create new connection
			var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

			// if no group
			if (group == null)
			{
				group = new Group(groupName);
				//_messageRepository.AddGroup(group);
				_unitOfWork.MessageRepository.AddGroup(group);
			}

			// add new connection to the group
			group.Connections.Add(connection);

			//if(await _messageRepository.SaveAllAsync()) {
			//	return group;
			//}
			if(await _unitOfWork.Complete()) {
				return group;
			}

			throw new HubException("Failed to join group");
		}

		/// <summary>
		/// Remove connection from group
		/// </summary>
		/// <returns>the group</returns>
		private async Task<Group> RemoveFromMessageGroup()
		{
			//var connection = await _messageRepository.GetConnection(Context.ConnectionId);

			// get group
			//var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
			var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
			var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

			// remove connection
			//_messageRepository.RemoveConnection(connection);
			_unitOfWork.MessageRepository.RemoveConnection(connection);

			//if(await _messageRepository.SaveAllAsync()) {
			//	return group;
			//}
			if(await _unitOfWork.Complete()) {
				return group;
			}

			throw new HubException("Failed to remove from group");
		}
	}
}