import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { MembersService } from 'src/app/_services/members.service';

@Component({
	selector: 'app-member-list',
	templateUrl: './member-list.component.html',
	styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
	members: Member[];

	// Pagination
	pagination: Pagination;

	// Remember filters
	userParams: UserParams;
	user: User;

	// Genders
	genderList = [
		{ value: 'male', display: 'Males' },
		{ value: 'female', display: 'Females' },
	];

	constructor(private membersService: MembersService) {
		// get saved user and user params
		this.user = membersService.user;
		this.userParams = membersService.getUserParams();
	}

	ngOnInit(): void {
		this.loadMembers();
	}

	loadMembers() {
		// change user params to save filters
		this.membersService.setUserParams(this.userParams);

		this.membersService
			.getMembers(this.userParams)
			.subscribe((response: PaginatedResult<Member[]>) => {
				this.members = response.result;
				this.pagination = response.pagination;
			});
	}

	// Reset user params
	resetFilters() {
		// reset user params
		this.userParams = this.membersService.resetUserParams();

		//this.userParams = new UserParams(this.user);
		this.loadMembers();
	}

	// When page is changed
	pageChanged(event: any) {
		console.log(event);
		this.userParams.pageNumber = event.page;

		// change user params to save filters
		this.membersService.setUserParams(this.userParams);

		// fetch new users
		this.loadMembers();
	}
}
