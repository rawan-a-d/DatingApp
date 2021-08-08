import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
	selector: 'app-user-management',
	templateUrl: './user-management.component.html',
	styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent implements OnInit {
	users: Partial<User[]>;

	// Modal reference
	bsModalRef: BsModalRef;

	constructor(
		private adminService: AdminService,
		private modalService: BsModalService
	) {}

	ngOnInit(): void {
		this.getUsersWithRoles();
	}

	// Get users with roles
	getUsersWithRoles() {
		this.adminService.getUsersWithRoles().subscribe((users) => {
			this.users = users;
		});
	}

	// Open roles edit modal
	openRolesModal(user: User) {
		// pass data to modal
		const config = {
			class: 'modal-dialog-centered',
			initialState: {
				user,
				roles: this.getRolesArray(user),
			},
		};
		// open modal
		this.bsModalRef = this.modalService.show(RolesModalComponent, config);

		// handle with roles update
		this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
			const rolesToUpdate = {
				// get roles which were checked and include only the name
				roles: [
					...values
						.filter((el) => el.checked === true)
						.map((el) => el.name),
				],
			};
			if (rolesToUpdate) {
				// update roles
				this.adminService
					.updateUserRoles(user.username, rolesToUpdate.roles)
					.subscribe(() => {
						user.roles = [...rolesToUpdate.roles];
					});
			}
		});

		//this.bsModalRef = this.modalService.show(RolesModalComponent);
	}

	// Get roles
	private getRolesArray(user: User) {
		const roles = [];

		// user roles
		const userRoles = user.roles;

		const availableRoles: any[] = [
			{ name: 'Admin', value: 'Admin' },
			{ name: 'Moderator', value: 'Moderator' },
			{ name: 'Member', value: 'Member' },
		];

		// loop over available roles
		// if user has role set it as checked and push to roles array
		// if not set checked to false and push it to roles array
		availableRoles.forEach((role) => {
			let isMatch = false;
			for (const userRole of userRoles) {
				if (role.name === userRole) {
					isMatch = true;
					role.checked = true;
					roles.push(role);

					break;
				}
			}
			if (!isMatch) {
				role.checked = false;
				roles.push(role);
			}
		});

		return roles;
	}
}
