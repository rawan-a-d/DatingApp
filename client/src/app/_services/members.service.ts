import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { pipe } from 'rxjs';
import { Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

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

	// Save the member cache as a map with
	// 1. a key of userParams with dashes in between
	// 2. a value of the result from the server
	memberCache = new Map();

	user: User;
	userParams: UserParams;

	constructor(
		private http: HttpClient,
		private accountService: AccountService
	) {
		this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
			this.user = user;
			this.userParams = new UserParams(user);
		});
	}

	getUserParams() {
		return this.userParams;
	}

	setUserParams(params: UserParams) {
		this.userParams = params;
	}

	resetUserParams() {
		this.userParams = new UserParams(this.user);
		return this.userParams;
	}

	getMembers(userParams: UserParams) {
		var response = this.memberCache.get(
			Object.values(userParams).join('-')
		);

		// if result is already in cache
		if (response) {
			// an observable of response
			return of(response);
		}

		// Query params:
		// pagination
		let params = getPaginationHeaders(
			userParams.pageNumber,
			userParams.pageSize
		);

		// age
		params = params.append('minAge', userParams.minAge);
		params = params.append('maxAge', userParams.maxAge);

		// gender
		params = params.append('gender', userParams.gender);

		// order by
		params = params.append('orderBy', userParams.orderBy);

		// get paginated result of Member array
		return getPaginatedResult<Member[]>(
			this.baseUrl + 'users',
			params,
			this.http
		).pipe(
			map((res) => {
				// save in cache
				this.memberCache.set(Object.values(userParams).join('-'), res);

				return res;
			})
		);
	}

	getMember(username: string) {
		// find user
		const member = [...this.memberCache.values()]
			.reduce(
				(arr, elem) =>
					// flat array into array of members
					arr.concat(elem.result),
				[]
			) // initial value for arr
			.find((member: Member) => member.username === username);

		if (member) {
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

	// Photos
	setMainPhoto(photoId: number) {
		return this.http.put(
			this.baseUrl + 'users/set-main-photo/' + photoId,
			{}
		);
	}

	deletePhoto(photoId: number) {
		return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
	}

	// Likes
	addLike(username: string) {
		return this.http.post(this.baseUrl + 'likes/' + username, {});
	}

	getLikes(predicate: string, pageNumber: number, pageSize: number) {
		let params = getPaginationHeaders(pageNumber, pageSize);
		params = params.append('predicate', predicate);
		return getPaginatedResult<Partial<Member[]>>(
			this.baseUrl + 'likes',
			params,
			this.http
		);
	}
}
