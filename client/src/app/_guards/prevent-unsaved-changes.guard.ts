import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

// Prevents information lost when a user changes data in his/her profile then clicks away without saving
@Injectable({
	providedIn: 'root',
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
	constructor(private confirmService: ConfirmService) {}

	//canDeactivate(component: MemberEditComponent): boolean {
	canDeactivate(
		component: MemberEditComponent
	): Observable<boolean> | boolean {
		// if edit form is dirty (information was changed)
		if (component.editForm.dirty) {
			//return confirm(
			//	'Are you sure you want to continue? Any unsaved changes will be lost'
			//);
			return this.confirmService.confirm();
		}

		return true;
	}
}
