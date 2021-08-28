import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
	NgxGalleryAnimation,
	NgxGalleryImage,
	NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
	selector: 'app-member-detail',
	templateUrl: './member-detail.component.html',
	styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
	// Tabs
	@ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
	activeTab: TabDirective;

	member: Member;

	galleryOptions: NgxGalleryOptions[];
	galleryImages: NgxGalleryImage[] = [];

	// current user
	user: User;

	// list of messages for message tab
	messages: Message[] = [];

	constructor(
		private messageService: MessageService,
		public presenceService: PresenceService,
		private accountService: AccountService,
		private membersService: MembersService,
		private route: ActivatedRoute,
		private router: Router,
		private toastr: ToastrService
	) {
		this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
			this.user = user;
		});

		// Disable reusing route when visiting someone else's profile and received a message
		// by clicking on the message, destroy page and rebuild it (to receive the messages)
		this.router.routeReuseStrategy.shouldReuseRoute = () => false;
	}

	ngOnDestroy(): void {
		// disconnect from hub if this component is closed/destroyed
		this.messageService.stopHubConnection();
	}

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
			// Load messages
			// 1. using service (http)
			//this.loadMessages();
			// 2. using hub
			this.messageService.createHubConnection(
				this.user,
				this.member.username
			);
		} else {
			// disconnect from hub
			this.messageService.stopHubConnection();
		}
	}

	selectTab(tabId: number) {
		this.memberTabs.tabs[tabId].active = true;
	}

	// Like user
	addLike(member: Member) {
		this.membersService.addLike(member.username).subscribe(() => {
			this.toastr.success('You have liked ' + member.knownAs);
		});
	}
}
