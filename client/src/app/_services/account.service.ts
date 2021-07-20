import { HttpClient } from '@angular/common/http';
import { ThrowStmt } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';

// This service can be injected into other components or services
// It is also a Singleton
@Injectable({
  providedIn: 'root' // same as adding it to app.module file providers
})
export class AccountService {
  private baseUrl = 'https://localhost:5001/api/';
  
  // ReplaySubject: buffer object which stores the value inside it
  // anytime a subscriber subscribe to this observable, it emits the last value inside it, or however many values inside it we want to emit.
  // this object stores only one user (current user)
  private currentUserSource = new ReplaySubject<User>(1);
  // create an observable
  currentUser$ = this.currentUserSource.asObservable();
  
  constructor(private http: HttpClient) { }
  
  // Login
  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model)
      .pipe(
        // apply a given function to each value
        map((response: User) => {
          // const user = <User>response;
          const user = response;
          if(user) {
            // save user in local storage
            localStorage.setItem('user', JSON.stringify(user));
            
            // set the currentUserSource
            this.currentUserSource.next(user);
          }
        })
        
      );
  }
  
  // set the currentUserSource
  setCurrentUser(user: User) {
    this.currentUserSource.next(user)
  }
  
  // Logout
  logout() {
    localStorage.removeItem('user');
    
    // empty the currentUserSource
    this.currentUserSource.next(null);
    //this.currentUserSource.next(undefined);
  }
  
  // Register
  register(model: any) {
   return this.http.post(this.baseUrl + 'account/register', model)
      .pipe(
        // apply a given function to each value
        map((user: User) => {
          if (user) {
            // save user in local storage
            localStorage.setItem('user', JSON.stringify(user));

            // set the currentUserSource
            this.currentUserSource.next(user);
          }
        })

      );
  }
}
