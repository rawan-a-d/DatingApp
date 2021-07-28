import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {
	AbstractControl,
	FormBuilder,
	FormControl,
	FormGroup,
	ValidatorFn,
	Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
	selector: 'app-register',
	templateUrl: './register.component.html',
	styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
	// Parent to child communication
	// @Input() usersFromHomeComponent: any;
	// Child to parent communication
	@Output() cancelRegister = new EventEmitter();

	// Reactive form
	registerForm: FormGroup;
	maxDate: Date;

	// Errors from server
	validationErrors: string[] = [];

	constructor(
		private accountService: AccountService,
		private toastr: ToastrService,
		private fb: FormBuilder,
		private router: Router
	) {}

	ngOnInit(): void {
		this.initializeForm();

		// set max date (age 18 and above)
		this.maxDate = new Date();
		this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
	}

	initializeForm() {
		// Create form
		// 1. FormGroup
		//this.registerForm = new FormGroup({
		//	username: new FormControl('', Validators.required),
		//	password: new FormControl('', [
		//		Validators.required,
		//		Validators.minLength(4),
		//		Validators.maxLength(8),
		//	]),
		//	confirmPassword: new FormControl('', [
		//		Validators.required,
		//		this.matchValues('password'),
		//	]),
		//});

		//2. Form builder
		this.registerForm = this.fb.group({
			gender: ['male'],
			username: ['', Validators.required],
			knownAs: ['', Validators.required],
			dateOfBirth: ['', Validators.required],
			city: ['', Validators.required],
			country: ['', Validators.required],
			password: [
				'',
				[
					Validators.required,
					Validators.minLength(4),
					Validators.maxLength(8),
				],
			],
			confirmPassword: [
				'',
				[Validators.required, this.matchValues('password')],
			],
		});

		// when password changes, validate confirm password again
		this.registerForm.controls.password.valueChanges.subscribe(() => {
			if (this.registerForm.controls.confirmPassword.value != '') {
				this.registerForm.controls.confirmPassword.updateValueAndValidity();
			}
		});
	}

	// Custom validator
	// attach this to confirmPassword control
	matchValues(matchTo: string): ValidatorFn {
		return (control: AbstractControl) => {
			// if confirmPassword == password
			return control?.value === control?.parent?.controls[matchTo].value
				? null
				: { isMatching: true };
		};
	}

	// Register
	register() {
		this.accountService.register(this.registerForm.value).subscribe(
			(response) => {
				this.router.navigateByUrl('/members');
			},
			(error) => {
				this.validationErrors = error;
			}
		);
	}

	// Cancel register
	cancel() {
		// inform parent
		this.cancelRegister.emit(false);
	}
}
