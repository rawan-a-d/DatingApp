using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
	/// <summary>
	/// Track presence of users
	/// can be done in many ways:
	/// 1. DB
	/// 2. In a Dictionary on server (works for one server only)
	/// 3. Reader: operates in memory and can be distributed across different servers (OPTIMAL)
	/// </summary>
	public class PresenceTracker
	{
		/// <summary>
		/// Dictionary with the username as key, and list of ids (connections) of user as value
		/// not thread safe (if two users where connecting at the same time, we'll run into problems) => use lock
		/// </summary>
		private static readonly Dictionary<string, List<string>> OnlineUsers = 
			new Dictionary<string, List<string>>();


		/// <summary>
		/// When a user connects
		/// </summary>
		/// <param name="username">the username</param>
		/// <param name="connectionId">the connection id</param>
		/// <returns>true if first connection, otherwise false</returns>
		public Task<bool> UserConnected(string username, string connectionId) {
			bool isOnline = false;
			// Lock dictionary until we're done
			lock (OnlineUsers) {
				// if an entry was made for this user
				if(OnlineUsers.ContainsKey(username)) {
					OnlineUsers[username].Add(connectionId);
				}
				else {
					OnlineUsers.Add(username, new List<string>(){ connectionId });

					isOnline = true;
				}
			}
			
			return Task.FromResult(isOnline);
			//return Task.CompletedTask;
		}

		/// <summary>
		/// When a user disconnects
		/// </summary>
		/// <param name="username">the username</param>
		/// <param name="connectionId">the connection id</param>
		/// <returns></returns>
		public Task<bool> UserDisconnected(string username, string connectionId) {
			bool isOffile = false;

			// Lock dictionary until we're done
			lock (OnlineUsers)
			{
				// if no key
				if (!OnlineUsers.ContainsKey(username)) {
					//return Task.CompletedTask;
					return Task.FromResult(isOffile);
				}

				// remove connection id
				OnlineUsers[username].Remove(connectionId);
				// if no more connections for this user
				if(OnlineUsers[username].Count == 0) {
					OnlineUsers.Remove(username);

					isOffile = true;
				}
			}

			//return Task.CompletedTask;
			return Task.FromResult(isOffile);
		}

		/// <summary>
		/// Get online users
		/// </summary>
		/// <returns>List of usernames</returns>
		public Task<string[]> GetOnlineUsers() {
			string[] onlineUsers;
			
			lock(OnlineUsers) {
				onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
			}

			return Task.FromResult(onlineUsers);
		}


		/// <summary>
		/// Get connections for a user
		/// </summary>
		/// <param name="username">the username</param>
		/// <returns>list of connections</returns>
		public Task<List<string>> GetConnectionsForUser(string username) {
			List<string> connectionIds;

			lock(OnlineUsers) {
				// found user or null if not found
				connectionIds = OnlineUsers.GetValueOrDefault(username);
			}

			return Task.FromResult(connectionIds);
		}
	}
}