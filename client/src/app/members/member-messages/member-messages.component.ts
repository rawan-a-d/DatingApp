import {
	ChangeDetectionStrategy,
	Component,
	Input,
	OnInit,
	ViewChild,
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
	changeDetection: ChangeDetectionStrategy.OnPush,
	selector: 'app-member-messages',
	templateUrl: './member-messages.component.html',
	styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
	@ViewChild('messageForm') messageForm: NgForm;
	// username of the chosen user
	@Input() username: string;
	@Input() messages: Message[];
	messageContent: string;
	loading = false;

	constructor(public messageService: MessageService) {}

	ngOnInit(): void {
		//this.loadMessages();
	}

	//loadMessages() {
	//	this.messageService
	//		.getMessageThread(this.username)
	//		.subscribe((messages) => {
	//			this.messages = messages;
	//		});
	//}

	// Send a message
	sendMessage() {
		this.loading = true;

		this.messageService
			.sendMessage(this.username, this.messageContent)
			.then(() => {
				// reset form
				this.messageForm.reset();
			})
			.finally(() => {
				this.loading = false;
			});
		//.subscribe((message) => {
		//	this.messages.push(message);

		//	// reset form
		//	this.messageForm.reset();
		//});
	}
}
