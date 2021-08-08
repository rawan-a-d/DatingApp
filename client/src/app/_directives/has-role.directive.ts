import {
	Directive,
	Input,
	OnInit,
	TemplateRef,
	ViewContainerRef,
} from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

// Structural directive: displays, hides based on user role
@Directive({
	// *appHasRole='["Admin"]'
	selector: '[appHasRole]', // other directives: *ngIf, *ngFor, bsRadio
})
export class HasRoleDirective implements OnInit {
	@Input() appHasRole: string[];
	user: User;

	constructor(
		private viewContainerRef: ViewContainerRef,
		private templateRef: TemplateRef<any>,
		private accountService: AccountService
	) {
		this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
			this.user = user;
		});
	}

	ngOnInit(): void {
		// clear view if no roles
		if (!this.user.roles || this.user == null) {
			this.viewContainerRef.clear();
			return;
		}
		// if user has the required role
		if (this.user?.roles.some((r) => this.appHasRole.includes(r))) {
			// display element
			this.viewContainerRef.createEmbeddedView(this.templateRef);
		} else {
			// don't display element
			this.viewContainerRef.clear();
		}
	}
}
