import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
	providedIn: 'root',
})
export class MessageService {
	baseUrl = environment.apiUrl;

	constructor(private http: HttpClient) {}

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
	sendMessage(username: string, content: string) {
		return this.http.post<Message>(this.baseUrl + 'messages', {
			recipientUsername: username,
			content,
		});
	}

	// Delete message
	deleteMessage(id: number) {
		return this.http.delete(this.baseUrl + 'messages/' + id);
	}
}
