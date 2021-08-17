import { HttpClient } from '@angular/common/http';
import { ThrowStmt } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { isArray } from 'ngx-bootstrap/chronos';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

// This service can be injected into other components or services
// It is also a Singleton
@Injectable({
	providedIn: 'root', // same as adding it to app.module file providers
})
export class AccountService {
	// Api url in environment file
	baseUrl = environment.apiUrl;

	// ReplaySubject: buffer object which stores the value inside it
	// anytime a subscriber subscribe to this observable, it emits the last value inside it, or however many values inside it we want to emit.
	// this object stores only one user (current user)
	private currentUserSource = new ReplaySubject<User>(1);
	// create an observable to be observed by other components, classes
	currentUser$ = this.currentUserSource.asObservable();

	constructor(private http: HttpClient, private presence: PresenceService) {}

	// Login
	login(model: any) {
		return this.http.post(this.baseUrl + 'account/login', model).pipe(
			// apply a given function to each value
			map((response: User) => {
				// const user = <User>response;
				const user = response;
				if (user) {
					// set the currentUserSource and save the user into local storage
					this.setCurrentUser(user);

					// connect to hub
					this.presence.createHubConnection(user);
				}
			})
		);
	}

	// set the currentUserSource and save the user into local storage
	setCurrentUser(user: User) {
		// get user roles
		user.roles = [];
		const roles = this.getDecodedToken(user.token).role;
		// if array (more than one role) or just a string (one role)
		Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);

		// save user in local storage
		localStorage.setItem('user', JSON.stringify(user));

		this.currentUserSource.next(user);

		//// connect to hub
		//this.presence.createHubConnection(user);
	}

	// Logout
	logout() {
		localStorage.removeItem('user');

		// empty the currentUserSource
		this.currentUserSource.next(null);
		//this.currentUserSource.next(undefined);

		// stop hub connection
		this.presence.stopHubConnection();
	}

	// Register
	register(model: any) {
		return this.http.post(this.baseUrl + 'account/register', model).pipe(
			// apply a given function to each value
			map((user: User) => {
				if (user) {
					// set the currentUserSource and save the user into local storage
					this.setCurrentUser(user);

					// connect to hub
					this.presence.createHubConnection(user);
				}
			})
		);
	}

	// Get decoded token
	getDecodedToken(token) {
		// get payload (data)
		return JSON.parse(atob(token.split('.')[1]));
	}
}
