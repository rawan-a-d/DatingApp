import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

// Use web socket to connect to SignalR presence hub
// when the browser window is closed, it automatically disconnects the client
@Injectable({
	providedIn: 'root',
})
export class PresenceService {
	hubUrl = environment.hubUrl;
	private hubConnection: HubConnection;

	// online users generic observable
	private onlineUsersSource = new BehaviorSubject<string[]>([]);
	onlineUsers$ = this.onlineUsersSource.asObservable();

	constructor(private toastr: ToastrService, private router: Router) {}

	// Create connection with hub
	// called in app.component.ts and account.service.ts
	createHubConnection(user: User) {
		// create connection
		this.hubConnection = new HubConnectionBuilder()
			.withUrl(this.hubUrl + 'presence', {
				accessTokenFactory: () => user.token,
			})
			.withAutomaticReconnect()
			.build(); // if network problem, client will automatically try to reconnect to hub

		// start connection
		this.hubConnection.start().catch((error) => {
			console.log(error);
		});

		// listen for UserIsOnline event/method from server
		// informing us that a user has connected
		this.hubConnection.on('UserIsOnline', (username) => {
			//this.toastr.info(username + ' has connected');
			// add new online user to onlineUsersSource
			this.onlineUsers$.pipe(take(1)).subscribe((usernames) => {
				this.onlineUsersSource.next([...usernames, username]);
			});
		});

		// listen for UserIsOffline event from server
		this.hubConnection.on('UserIsOffline', (username) => {
			//this.toastr.warning(username + ' has disconnected');
			// remove user from onlineUsersSource
			this.onlineUsers$.pipe(take(1)).subscribe((usernames) => {
				this.onlineUsersSource.next([
					...usernames.filter((x) => x !== username),
				]);
			});
		});

		// listen for GetOnlineUsers event from server
		this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
			this.onlineUsersSource.next(usernames);
		});

		// listen for NewMessageReceived event from server
		this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
			// notify user
			// on tab navigate user to the message thread
			this.toastr
				.info(knownAs + ' has sent you a new message!')
				.onTap.pipe(take(1))
				.subscribe(() => {
					this.router.navigateByUrl(
						'/members/' + username + '?tab=3'
					);
				});
		});
	}

	// Stop hub connection
	stopHubConnection() {
		this.hubConnection.stop().catch((error) => {
			console.log(error);
		});
	}
}
