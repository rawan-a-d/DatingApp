import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

// Reusable text input used in registration
// allows us to access form control from other components in this component
@Component({
	selector: 'app-text-input',
	templateUrl: './text-input.component.html',
	styleUrls: ['./text-input.component.css'],
})
export class TextInputComponent implements ControlValueAccessor {
	// label and type of input
	@Input() label: string;
	@Input() type: 'text';

	// Inject control in the constructor
	constructor(@Self() public ngControl: NgControl) {
		this.ngControl.valueAccessor = this;
	}

	// Methods implementations will come from ControlValueAccessor
	writeValue(obj: any): void {}

	registerOnChange(fn: any): void {}

	registerOnTouched(fn: any): void {}

	//setDisabledState?(isDisabled: boolean): void {
	//	throw new Error('Method not implemented.');
	//}
}
