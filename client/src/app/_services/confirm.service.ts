import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

// Handles confirm dialog
@Injectable({
	providedIn: 'root',
})
export class ConfirmService {
	bsModalRef: BsModalRef;

	constructor(private modalServide: BsModalService) {}

	confirm(
		title = 'Confirmation',
		message = 'Are you sure to do this?',
		btnOkText = 'Ok',
		btnCancelText = 'Cancel'
	): Observable<boolean> {
		const config = {
			initialState: {
				title,
				message,
				btnOkText,
				btnCancelText,
			},
		};

		// show modal
		this.bsModalRef = this.modalServide.show(
			ConfirmDialogComponent,
			config
		);

		// return an observable which the component can subscribe to
		return new Observable<boolean>(this.getResult());
	}

	private getResult() {
		return (observer) => {
			// emit event when the modal behind the ref finishes or starts hiding
			const subscription = this.bsModalRef.onHidden.subscribe(() => {
				observer.next(this.bsModalRef.content.result);
				observer.complete();
			});

			return {
				unsubscribe() {
					subscription.unsubscribe();
				},
			};
		};
	}
}
