import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { ConfirmService } from '../_services/confirm.service';
import { MessageService } from '../_services/message.service';

@Component({
	selector: 'app-messages',
	templateUrl: './messages.component.html',
	styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
	messages: Message[];
	pagination: Pagination;
	container = 'Unread'; // Inbox, Outbox, Unread

	pageNumber = 1;
	pageSize = 5;

	loading = false;

	constructor(
		private messageService: MessageService,
		private confirmService: ConfirmService
	) {}

	ngOnInit(): void {
		this.loadMessages();
	}

	// get messages from server
	loadMessages() {
		this.loading = true;

		this.messageService
			.getMessages(this.pageNumber, this.pageSize, this.container)
			.subscribe((response) => {
				this.messages = response.result;
				this.pagination = response.pagination;

				this.loading = false;
			});
	}

	// delete message
	deleteMessage(id: number) {
		// confirm deletion
		this.confirmService
			.confirm('Confirm delete message', 'This cannot be undone')
			.subscribe((result) => {
				if (result) {
					// delete message
					this.messageService.deleteMessage(id).subscribe(() => {
						this.messages.splice(
							this.messages.findIndex((m) => m.id === id),
							1
						);
					});
				}
			});
	}

	// page changed event
	pageChanged(event: any) {
		this.pageNumber = event.page;
		this.loadMessages();
	}
}
