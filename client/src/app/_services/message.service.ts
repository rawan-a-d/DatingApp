import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { BusyService } from './busy.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
	providedIn: 'root',
})
export class MessageService {
	baseUrl = environment.apiUrl;
	hubUrl = environment.hubUrl;
	private hubConnection: HubConnection;

	private messageThreadSource = new BehaviorSubject<Message[]>([]);
	messageThread$ = this.messageThreadSource.asObservable();

	constructor(private http: HttpClient, private busyService: BusyService) {}

	createHubConnection(user: User, otherUsername: string) {
		// loading
		this.busyService.busy();

		// create connection
		this.hubConnection = new HubConnectionBuilder()
			.withUrl(this.hubUrl + 'message?user=' + otherUsername, {
				accessTokenFactory: () => user.token,
			}) // send token in url because hub don't work through http -> no Authorization header
			.withAutomaticReconnect() // reconnect client when there is a problem
			.build();

		// start connection
		this.hubConnection
			.start()
			.catch((error) => console.log(error))
			.finally(() => {
				// turn off loading
				this.busyService.idle();
			});

		// when a user is connected to hub send messages
		this.hubConnection.on('ReceiveMessageThread', (messages) => {
			console.log('ReceiveMessageThread');
			this.messageThreadSource.next(messages);
		});

		// create new message
		this.hubConnection.on('NewMessage', (message) => {
			this.messageThread$.pipe(take(1)).subscribe((messages) => {
				// update array in the BehaviorSubject with a new array containing the new message
				console.log('NewMessage');
				this.messageThreadSource.next([...messages, message]);
			});
		});

		this.hubConnection.on('UpdatedGroup', (group: Group) => {
			// if there are unread messages for the current user, mark them as read
			if (group.connections.some((x) => x.username == otherUsername)) {
				this.messageThread$.pipe(take(1)).subscribe((messages) => {
					messages.forEach((message) => {
						if (!message.dateRead) {
							message.dateRead = new Date(Date.now());
						}
					});
					this.messageThreadSource.next([...messages]);
				});
			}
		});
	}

	stopHubConnection() {
		// if there is a connection
		if (this.hubConnection) {
			// clear message thread
			this.messageThreadSource.next([]);

			this.hubConnection.stop();
		}
	}

	// Get paginated messages based on container (Unread, Inbox, Outbox)
	getMessages(pageNumber: number, pageSize: number, container: string) {
		// params
		let params = getPaginationHeaders(pageNumber, pageSize);

		params = params.append('Container', container);

		// get paginated result
		return getPaginatedResult<Message[]>(
			this.baseUrl + 'messages',
			params,
			this.http
		);
	}

	// Get conversation between two users
	getMessageThread(username: string) {
		return this.http.get<Message[]>(
			this.baseUrl + 'messages/thread/' + username
		);
	}

	// Send message
	async sendMessage(username: string, content: string) {
		//return this.http.post<Message>(this.baseUrl + 'messages', {
		//	recipientUsername: username,
		//	content,
		//});
		// send message via the hub
		return this.hubConnection
			.invoke('SendMessage', {
				// invoke this method in hub
				recipientUsername: username,
				content,
			})
			.catch((error) => {
				console.log(error);
			});
	}

	// Delete message
	deleteMessage(id: number) {
		return this.http.delete(this.baseUrl + 'messages/' + id);
	}
}
