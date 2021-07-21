import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';

// Decorator
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating app';
  
  // Dependency injection http client to send HTTP requests
  constructor(private http: HttpClient,
              private accountService: AccountService) {
  }
  
  
  // Lifecycle hook: called after Angular has initialized all data-bound property of a directive
  ngOnInit() {    
    // Set current user in account service when app starts
    this.setCurrentUser();
  }
  
  
  // Set current user in account service
  setCurrentUser() {
    // get user from local storage
    const user: User = JSON.parse(localStorage.getItem('user'));
    
    // set current user in account service
    this.accountService.setCurrentUser(user);
  }
}