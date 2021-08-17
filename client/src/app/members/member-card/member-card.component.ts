import { ThisReceiver } from '@angular/compiler';
import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
	selector: 'app-member-card',
	templateUrl: './member-card.component.html',
	styleUrls: ['./member-card.component.css'],
	// encapsulation style
	//encapsulation:
})
export class MemberCardComponent implements OnInit {
	@Input() member: Member;
	//onlineUsers: string[];

	constructor(
		private membersService: MembersService,
		public presenceService: PresenceService,
		private toastr: ToastrService
	) {}

	ngOnInit(): void {
		// subscribe to presence service or use async pipe
		//this.presenceService.onlineUsers$.subscribe((usernames) => {
		//	this.onlineUsers = usernames;
		//	console.log(this.onlineUsers);
		//});
	}

	// Add like
	addLike(member: Member) {
		this.membersService.addLike(member.username).subscribe(() => {
			this.toastr.success('You have liked ' + member.knownAs);
		});
	}
}
