using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class MessageRepository : IMessageRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public MessageRepository(DataContext context, IMapper mapper)
		{
			_mapper = mapper;
			_context = context;
		}

		/// <summary>
		/// Add a group for two users
		/// </summary>
		/// <param name="group">the group</param>
		public void AddGroup(Group group)
		{
			_context.Groups.Add(group);
		}


		/// <summary>
		/// Add message
		/// </summary>
		/// <param name="message">the message</param>
		public void AddMessage(Message message)
		{
			_context.Messages.Add(message);
		}


		/// <summary>
		/// Delete message
		/// </summary>
		/// <param name="message">the message</param>
		public void DeleteMessage(Message message)
		{
			_context.Messages.Remove(message);
		}

		/// <summary>
		/// Get connection by id
		/// </summary>
		/// <param name="connectionId">the id</param>
		/// <returns>a connection</returns>
		public async Task<Connection> GetConnection(string connectionId)
		{
			return await _context.Connections.FindAsync(connectionId);
		}


		/// <summary>
		/// Get a specific message
		/// </summary>
		/// <param name="id">message id</param>
		/// <returns></returns>
		public async Task<Message> GetMessage(int id)
		{
			//return await _context.Messages.FindAsync(id);

			return await _context.Messages
				.Include(u => u.Sender)
				.Include(u => u.Recipient)
				.SingleOrDefaultAsync(x => x.Id == id);
		}

		/// <summary>
		/// Get message group by group name
		/// </summary>
		/// <param name="groupName">the group name</param>
		/// <returns>a group</returns>
		public async Task<Group> GetMessageGroup(string groupName)
		{
			return await _context.Groups
				.Include(x => x.Connections) // include connections
				.FirstOrDefaultAsync(x => x.Name == groupName);
		}


		/// <summary>
		/// Get messages for a specific user
		/// </summary>
		/// <param name="messageParams">message query params</param>
		/// <returns>List of messages</returns>
		public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
		{
			var query = _context.Messages
				.OrderByDescending(m => m.MessageSent) // order by most recent
				.AsQueryable();

			// check container
			query = messageParams.Container switch
			{
				// received messages
				"Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false),
				// Sent messaged
				"Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username  && u.SenderDeleted == false),
				// Recipient and didn't read message
				_ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
			};

			// Project message to messageDto
			var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

			// return paged messages
			return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
		}


		/// <summary>
		/// Get conversation between two users
		/// </summary>
		/// <param name="currentUsername">logged in user</param>
		/// <param name="recipientUsername">other user</param>
		/// <returns>list of messages</returns>
		public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
		{
			// get conversation of current user
			var messages = await _context.Messages
				// include users and photos
				.Include(u => u.Sender).ThenInclude(p => p.Photos)
				.Include(u => u.Recipient).ThenInclude(p => p.Photos)				
				.Where(m =>
					(m.SenderUsername == currentUsername && m.RecipientUsername == recipientUsername && m.SenderDeleted == false) ||
					(m.SenderUsername == recipientUsername && m.RecipientUsername == currentUsername && m.RecipientDeleted == false)
				)
				.OrderBy(m => m.MessageSent)
				.ToListAsync();

			// mark messages as read
			var unreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList();
			if(unreadMessages.Any()) {
				foreach(var message in unreadMessages) {
					//message.DateRead = DateTime.Now;
					message.DateRead = DateTime.UtcNow;
				}
			}

			// save to db
			await _context.SaveChangesAsync();

			return _mapper.Map<IEnumerable<MessageDto>>(messages);
		}

		/// <summary>
		/// Remove connection
		/// </summary>
		/// <param name="connection">the connection</param>
		public void RemoveConnection(Connection connection)
		{
			_context.Connections.Remove(connection);
		}


		/// <summary>
		/// Save to db
		/// </summary>
		/// <returns>true or false</returns>
		public async Task<bool> SaveAllAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}
	}
}