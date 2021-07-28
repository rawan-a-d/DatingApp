import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

// Reusable date input used in registration
// allows us to access form control from other components in this component
@Component({
	selector: 'app-date-input',
	templateUrl: './date-input.component.html',
	styleUrls: ['./date-input.component.css'],
})
export class DateInputComponent implements ControlValueAccessor {
	// label and max date of input
	@Input() label: string;
	@Input() maxDate: Date;

	// configuration
	// Partial means that every configuration is optional -> we don't have to provide all of them
	bsConfig: Partial<BsDatepickerConfig>;

	constructor(@Self() public ngControl: NgControl) {
		this.ngControl.valueAccessor = this;
		this.bsConfig = {
			containerClass: 'theme-red',
			dateInputFormat: 'DD MMMM YYYY',
		};
	}

	writeValue(obj: any): void {}
	registerOnChange(fn: any): void {}
	registerOnTouched(fn: any): void {}
}
