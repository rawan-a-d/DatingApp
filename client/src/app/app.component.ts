import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

// Decorator
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating app';
  users: any;
  
  // Dependency injection http client to send HTTP requests
  constructor(private http: HttpClient) {
  }
  
  
  // Lifecycle hook: called after Angular has initialized all data-bound property of a directive
  ngOnInit() {
    this.getUsers();
  }
  
  
  getUsers() {
    this.http.get('https://localhost:5001/api/users')
      .subscribe(response => {
        this.users = response;
      }, error => {
        console.log(error);
      })
  }
  
  
}
