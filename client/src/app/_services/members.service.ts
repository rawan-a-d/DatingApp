import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

//const httpOptions = {
//	headers: new HttpHeaders({
//		Authorization:
//			'Bearer ' + JSON.parse(localStorage.getItem('user'))?.token,
//	}),
//};

@Injectable({
	providedIn: 'root',
})
export class MembersService {
	// Api url in environment file
	baseUrl = environment.apiUrl;

	// Using the service to store state
	members: Member[] = [];

	constructor(private http: HttpClient) {}

	getMembers() {
		if (this.members.length > 0) {
			// observable of members list
			return of(this.members);
		}
		return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
			// save members to members list
			map((members) => {
				this.members = members;
				return members;
			})
		);
	}

	getMember(username: string) {
		// find user
		const member = this.members.find((x) => x.username === username);

		if (member !== undefined) {
			return of(member);
		}
		return this.http.get<Member>(this.baseUrl + 'users/' + username);
	}

	updateMember(member: Member) {
		return this.http.put(this.baseUrl + 'users/', member).pipe(
			map(() => {
				// update member in members list
				const index = this.members.indexOf(member);

				this.members[index] = member;
			})
		);
	}
}
