import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
	NgxGalleryAnimation,
	NgxGalleryImage,
	NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
	selector: 'app-member-detail',
	templateUrl: './member-detail.component.html',
	styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
	// Tabs
	@ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
	activeTab: TabDirective;

	member: Member;

	galleryOptions: NgxGalleryOptions[];
	galleryImages: NgxGalleryImage[] = [];

	// list of messages for message tab
	messages: Message[] = [];

	constructor(
		private memberService: MembersService,
		private messageService: MessageService,
		private route: ActivatedRoute
	) {}

	ngOnInit(): void {
		// Get member
		//this.loadMember();
		// OR
		// Route resolver
		// allow us to get access to the data before the component is constructed
		// no (Cannot read property 'photoUrl' of undefined) errors
		this.route.data.subscribe((data) => {
			this.member = data.member;
		});

		// Get tab from url
		this.route.queryParams.subscribe((params) => {
			params.tab ? this.selectTab(params.tab) : this.selectTab(0);
		});

		// Gallery
		this.galleryOptions = [
			{
				width: '500px',
				height: '500px',
				imagePercent: 100,
				thumbnailsColumns: 4,
				imageAnimation: NgxGalleryAnimation.Slide,
				preview: false,
			},
		];

		// Load images
		this.loadImages();
	}

	// Load images of user
	loadImages() {
		this.member.photos.forEach((photo) => {
			this.galleryImages.push({
				small: photo.url,
				medium: photo.url,
				big: photo.url,
			});
		});
	}

	// Load member
	//loadMember() {
	//	let username = this.route.snapshot.paramMap.get('username');
	//	this.memberService.getMember(username).subscribe((member) => {
	//		this.member = member;

	//		this.loadImages();
	//	});
	//}

	// Load messages
	loadMessages() {
		this.messageService
			.getMessageThread(this.member.username)
			.subscribe((messages) => {
				this.messages = messages;
			});
	}

	// when tab is changed
	onTabActivated(data: TabDirective) {
		this.activeTab = data;

		// messages tab
		if (
			this.activeTab.heading === 'Messages' &&
			this.messages.length === 0
		) {
			this.loadMessages();
		}
	}

	selectTab(tabId: number) {
		this.memberTabs.tabs[tabId].active = true;
	}
}
