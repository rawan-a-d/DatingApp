import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
	selector: 'app-photo-editor',
	templateUrl: './photo-editor.component.html',
	styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
	@Input() member: Member;

	// photo uploader
	uploader: FileUploader;
	hasBaseDropzoneOver = false;

	baseUrl = environment.apiUrl;
	user: User;

	constructor(
		private accountService: AccountService,
		private memberService: MembersService
	) {
		this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
			this.user = user;
		});
	}

	ngOnInit(): void {
		this.initializeUploader();
	}

	fileOverBase(e: any) {
		this.hasBaseDropzoneOver = e;
	}

	// change main photo
	setMainPhoto(photo: Photo) {
		this.memberService.setMainPhoto(photo.id).subscribe(() => {
			this.user.photoUrl = photo.url;

			// update user object
			this.accountService.setCurrentUser(this.user);

			this.member.photoUrl = photo.url;

			// switch isMain to selected photo
			this.member.photos.forEach((p) => {
				if (p.isMain) {
					p.isMain = false;
				}
				if (p.id == photo.id) {
					p.isMain = true;
				}
			});
		});
	}

	// Delete photo
	deletePhoto(photo: Photo) {
		this.memberService.deletePhoto(photo.id).subscribe(() => {
			// 1. allow deletion of main
			// delete photo
			//let index = this.member.photos.indexOf(photo);
			//this.member.photos.splice(index, 1);

			// set main photo to first
			//this.member.photoUrl = this.member.photos[0].url;
			//this.member.photos[0].isMain = true;
			//this.user.photoUrl = this.member.photos[0].url;

			// update user object
			//this.accountService.setCurrentUser(this.user);

			// 2. don't allow deletion of main
			// get all photos except for deleted
			this.member.photos = this.member.photos.filter(
				(x) => x.id !== photo.id
			);
		});
	}

	// Uploaded configuration
	initializeUploader() {
		this.uploader = new FileUploader({
			url: this.baseUrl + 'users/add-photo', // end point
			authToken: 'Bearer ' + this.user.token, // JSON token, this doesn't go through the interceptor
			isHTML5: true,
			allowedFileType: ['image'],
			removeAfterUpload: true, // empty file upload
			autoUpload: false, // use button to confirm
			maxFileSize: 10 * 1024 * 1024,
		});

		this.uploader.onAfterAddingFile = (file) => {
			file.withCredentials = false;
		};

		// if photo was uploaded successfully
		this.uploader.onSuccessItem = (item, response, status, headers) => {
			if (response) {
				const photo: Photo = JSON.parse(response);
				this.member.photos.push(photo);

				// update user object with the new photo
				//if (this.user.photoUrl == null) {er
				if (photo.isMain) {
					this.user.photoUrl = photo.url;
					this.member.photoUrl = photo.url;
					this.accountService.setCurrentUser(this.user);
				}
			}
		};
	}
}
