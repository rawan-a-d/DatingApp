import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

// Confirmation dialog uses confirm service
@Component({
	selector: 'app-confirm-dialog',
	templateUrl: './confirm-dialog.component.html',
	styleUrls: ['./confirm-dialog.component.css'],
})
export class ConfirmDialogComponent implements OnInit {
	title: string;
	message: string;
	btnOkText: string;
	btnCancelText: string;
	// store selection as result
	result: boolean;

	constructor(public bsModalRef: BsModalRef) {}

	ngOnInit(): void {}

	// Confirm
	confirm() {
		this.result = true;
		this.bsModalRef.hide();
	}

	// Decline
	decline() {
		this.result = false;
		this.bsModalRef.hide();
	}
}
