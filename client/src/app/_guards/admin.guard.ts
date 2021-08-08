import { Injectable } from '@angular/core';
import {
	ActivatedRouteSnapshot,
	CanActivate,
	RouterStateSnapshot,
	UrlTree,
} from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

// Protect admin routes
// guards subscribe to observables automatically when a route is accessed
@Injectable({
	providedIn: 'root',
})
export class AdminGuard implements CanActivate {
	constructor(
		private accountService: AccountService,
		private toastr: ToastrService
	) {}

	canActivate(): Observable<boolean> {
		return this.accountService.currentUser$.pipe(
			map((user) => {
				console.log(user.roles);

				// if user logged in
				if (
					user.roles.includes('Admin') ||
					user.roles.includes('Moderator')
				) {
					return true;
				}
				// otherwise
				this.toastr.error('You cannot enter this area!');
				return false;
			})
		);
	}
}
