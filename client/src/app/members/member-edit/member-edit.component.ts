import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
	selector: 'app-member-edit',
	templateUrl: './member-edit.component.html',
	styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
	// user information
	member: Member;
	// current user
	user: User;

	// Host listener
	// access browser events, before browser/tab is closed
	@HostListener('window:beforeunload', ['$event']) unloadNotification(
		$event: any
	) {
		if (this.editForm.dirty) {
			$event.returnValue = true;
		}
	}

	// form, used in prevent-unsaved-changes.guard and here
	@ViewChild('editForm')
	editForm: NgForm;

	constructor(
		private accountService: AccountService,
		private memberServie: MembersService,
		private toastr: ToastrService
	) {
		// get current user
		this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
			this.user = user;
		});
	}

	ngOnInit(): void {
		this.loadMember();
	}

	loadMember() {
		this.memberServie.getMember(this.user.username).subscribe((member) => {
			this.member = member;
		});
	}

	updateMember() {
		this.memberServie.updateMember(this.member).subscribe(() => {
			this.toastr.success('Profile updated successfully');

			// reset form to the updated member
			this.editForm.reset(this.member);
		});
	}
}
