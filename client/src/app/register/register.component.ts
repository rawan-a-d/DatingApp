import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  // Parent to child communication
  // @Input() usersFromHomeComponent: any;
  // Child to parent communication
  @Output() cancelRegister = new EventEmitter();

  constructor(private accountService: AccountService,
              private toastr: ToastrService) {
  }

  ngOnInit(): void {
  }

  // Register
  register() {
    this.accountService.register(this.model)
      .subscribe(response => {
        this.cancel();
      }, error => {
        console.log(error);
        
        this.toastr.error(error.error);
      });
  }
  
  // Cancel register
  cancel() {    
    // inform parent
    this.cancelRegister.emit(false);
  }
  
  
}
