import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
	selector: 'app-roles-modal',
	templateUrl: './roles-modal.component.html',
	styleUrls: ['./roles-modal.component.css'],
})
export class RolesModalComponent implements OnInit {
	// Passed from the component that opened the modal
	// inform parent of update (output works too)
	@Input() updateSelectedRoles = new EventEmitter();
	user: User;
	roles: any[];

	constructor(public bsModalRef: BsModalRef) {}

	ngOnInit(): void {}

	// Notify parent about updating roles and close modal
	updateRoles() {
		this.updateSelectedRoles.emit(this.roles);
		this.bsModalRef.hide();
	}
}
