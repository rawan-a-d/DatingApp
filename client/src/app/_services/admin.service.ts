import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

// Service for Admin
@Injectable({
	providedIn: 'root',
})
export class AdminService {
	baseUrl = environment.apiUrl;

	constructor(private http: HttpClient) {}

	// for Admin
	getUsersWithRoles() {
		return this.http.get<Partial<User[]>>(
			this.baseUrl + 'admin/users-with-roles'
		);
	}

	// for Admin and Moderator
	getPhotos() {
		return this.http.get(this.baseUrl + 'admin/photos-to-moderate');
	}

	// update user roles
	updateUserRoles(username: string, roles: string[]) {
		return this.http.post(
			this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles,
			{}
		);
	}

	// get unapproved photos
	getPhotosForApproval() {
		return this.http.get(this.baseUrl + 'admin/photos-to-moderate');
	}

	// Approve photo
	approvePhoto(photoId: number) {
		return this.http.post(
			this.baseUrl + 'admin/approve-photo/' + photoId,
			{}
		);
	}

	// Reject photo
	rejectPhoto(photoId: number) {
		return this.http.post(
			this.baseUrl + 'admin/reject-photo/' + photoId,
			{}
		);
	}
}
