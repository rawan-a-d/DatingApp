import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  // loggedIn: boolean = false;

  constructor(public accountService: AccountService) { 
    
  }

  ngOnInit(): void {
    // Get current user from account service
    //this.getCurrentUser();
  }
  
  
  login() {    
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      
      //this.loggedIn = true;
    }, error => {
      console.log(error);
    })
  }
  
  // Logout
  logout() {
    this.accountService.logout();
  
    //this.loggedIn = false;
  }
  
  // Get current user from account service
  /*getCurrentUser() {
    // this is observable is never completes as it is not an HTTP request ->
    // 1. we will remain subscribed to it
    // 2. it will stay in memory -> memory leak
    // This can be avoided by using the async pipe in HTML which will automatically unsubscribe to observables if our nav is no longer visible
    this.accountService.currentUser$.subscribe(user => {
      // turn object into boolean
      // if user is null -> false; otherwise true
      this.loggedIn = !!user;
    }, error => {
      console.log(error);
    })
  }*/

}
