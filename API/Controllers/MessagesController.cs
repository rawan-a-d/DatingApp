using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class MessagesController : BaseApiController
	{
		private readonly IMessageRepository _messageRepository;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;

		public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
		{
			_userRepository = userRepository;
			_messageRepository = messageRepository;
			_mapper = mapper;
		}


		/// <summary>
		/// Create new message
		/// </summary>
		/// <param name="createMessageDto">Created message which container recipientUsername and content</param>
		/// <returns>the created message</returns>
		[HttpPost]
		public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto) {
			var username = User.GetUsername();

			// If user is sending a message to himself
			if(username == createMessageDto.RecipientUsername.ToLower()) {
				return BadRequest("You cannot send a message to yourself");
			}

			// get users
			var sender = await _userRepository.GetUserByUsernameAsync(username);
			var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

			// if no recipient
			if(recipient == null) {
				return NotFound("Recipient cannot be found");
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

			// add message
			_messageRepository.AddMessage(message);

			// save to db
			if(await _messageRepository.SaveAllAsync()) {
				// should return create at route
				return Ok(_mapper.Map<MessageDto>(message));
			}

			return BadRequest("Failed to sendd message");
		}


		/// <summary>
		/// Get messages for a specific user
		/// </summary>
		/// <param name="messageParams">message query params</param>
		/// <returns>List of messages</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams) {
			// get current username
			var username = User.GetUsername();

			// add current username to MessageParams
			messageParams.Username = username;

			// get messages
			var messages = await _messageRepository.GetMessagesForUser(messageParams);

			Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

			return messages;
		}


		/// <summary>
		/// Get conversation between two users
		/// </summary>
		/// <param name="username">other user</param>
		/// <returns>list of messages</returns>
		[HttpGet("thread/{username}")]
		public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username) {
			var currentUsername = User.GetUsername();

			var messages = await _messageRepository.GetMessageThread(currentUsername, username);

			return Ok(messages);
		}


		/// <summary>
		/// Delete a message
		/// </summary>
		/// <param name="id">message id</param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteMessage(int id) {
			// get user
			var username = User.GetUsername();
			var user = _userRepository.GetUserByUsernameAsync(username);

			// get message
			var message = await _messageRepository.GetMessage(id);

			// if not owner
			if(message.Sender.UserName != username && message.Recipient.UserName != username) {
				return Unauthorized();
			}

			// check sender or recipient and delete
			if(message.Sender.UserName == username) {
				message.SenderDeleted = true;
			}
			else if(message.Recipient.UserName == username) {
				message.RecipientDeleted = true;
			}

			// if both users deleted the message -> delete from db
			if(message.SenderDeleted && message.RecipientDeleted) {
				_messageRepository.DeleteMessage(message);
			}

			// save to db
			if(await _messageRepository.SaveAllAsync()) {
				return Ok();
			}

			return BadRequest("Problems deleting the message");
		}
	}
}