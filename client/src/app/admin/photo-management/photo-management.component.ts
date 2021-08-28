import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
	selector: 'app-photo-management',
	templateUrl: './photo-management.component.html',
	styleUrls: ['./photo-management.component.css'],
})
export class PhotoManagementComponent implements OnInit {
	photosForApproval: Photo[];

	constructor(private adminServce: AdminService) {}

	ngOnInit(): void {
		this.getPhotosForApproval();
	}

	getPhotosForApproval() {
		this.adminServce.getPhotosForApproval().subscribe((photos) => {
			this.photosForApproval = <Photo[]>photos;
		});
	}

	approvePhoto(photoId: number) {
		this.adminServce.approvePhoto(photoId).subscribe(() => {
			// update photos array
			this.photosForApproval.splice(
				this.photosForApproval.findIndex((p) => p.id === photoId),
				1
			);
		});
	}

	rejectPhoto(photoId: number) {
		this.adminServce.rejectPhoto(photoId).subscribe(() => {
			// update photos array
			this.photosForApproval.splice(
				this.photosForApproval.findIndex((p) => p.id === photoId),
				1
			);
		});
	}
}
