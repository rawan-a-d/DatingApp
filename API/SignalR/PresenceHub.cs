using System;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
	/// <summary>
	/// Manage the presense of users
	/// </summary>
	[Authorize]
	public class PresenceHub : Hub
	{
		private readonly PresenceTracker _tracker;
		public PresenceHub(PresenceTracker tracker)
		{
			_tracker = tracker;
		}

		/// <summary>
		/// Inform all users except for this one that this user is online
		/// </summary>
		/// <returns></returns>
		public override async Task OnConnectedAsync()
		{
			// track user
			await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);

			// send to everybody except the client who triggered it that the user is online
			// UserIsOnline: name of the method in the client
			await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

			// send online users to everybody who is connected
			var currentUsers = await _tracker.GetOnlineUsers();
			await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
		}

		/// <summary>
		/// Inform all users except for this one that this user is offline
		/// </summary>
		/// <returns></returns>
		public override async Task OnDisconnectedAsync(Exception ex)
		{
			// untrack user
			await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

			await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

			await base.OnDisconnectedAsync(ex);

			// send online users to everybody who is connected
			var currentUsers = await _tracker.GetOnlineUsers();
			await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
		}
	}
}