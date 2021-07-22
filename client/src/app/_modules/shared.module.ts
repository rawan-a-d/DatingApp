import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

// Tidying up the app module by using a shared module for non angular modules
@NgModule({
	declarations: [],
	imports: [
		CommonModule,
		// drop down
		// forRoot: initializes services, components with the root module
		// https://valor-software.com/ngx-bootstrap/#/dropdowns
		BsDropdownModule.forRoot(),
		// Toastr module to notify client
		ToastrModule.forRoot({
			positionClass: 'toast-bottom-right',
		}),
	],
	// Need to export the modules in order to work in app.module
	exports: [BsDropdownModule, ToastrModule],
})
export class SharedModule { }
