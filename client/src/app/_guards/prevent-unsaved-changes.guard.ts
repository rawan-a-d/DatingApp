import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

// Prevents information lost when a user changes data in his/her profile then clicks away without saving
@Injectable({
	providedIn: 'root',
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
	canDeactivate(component: MemberEditComponent): boolean {
		// if edit form is dirty (information was changed)
		if (component.editForm.dirty) {
			return confirm(
				'Are you sure you want to continue? Any unsaved changes will be lost'
			);
		}

		return true;
	}
}
