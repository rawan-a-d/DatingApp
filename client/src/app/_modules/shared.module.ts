import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { NgxSpinnerModule } from 'ngx-spinner';
import { FileUploadModule } from 'ng2-file-upload';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { ModalModule } from 'ngx-bootstrap/modal';

// Tidying up the app module by using a shared module for non angular modules
@NgModule({
	declarations: [],
	imports: [
		CommonModule,
		// Drop down
		// forRoot: initializes services, components with the root module
		// https://valor-software.com/ngx-bootstrap/#/dropdowns
		BsDropdownModule.forRoot(),
		// Toastr module to notify client
		ToastrModule.forRoot({
			positionClass: 'toast-bottom-right',
		}),
		// Tabs
		TabsModule.forRoot(),
		// Photo gallery
		// https://www.npmjs.com/package/@kolkov/ngx-gallery
		NgxGalleryModule,
		// Loading spinner
		// https://www.npmjs.com/package/ngx-spinner
		// https://labs.danielcardoso.net/load-awesome/animations.html
		// if error: npm i @angular/cdk
		NgxSpinnerModule,
		// https://valor-software.com/ng2-file-upload/
		FileUploadModule,
		// Bootstrap date picker (HTML date picker is different on each browser)
		// https://valor-software.com/ngx-bootstrap/#/datepicker
		BsDatepickerModule.forRoot(),
		// Pagination
		// https://valor-software.com/ngx-bootstrap/#/pagination
		PaginationModule.forRoot(),
		// Sorting buttons
		// https://valor-software.com/ngx-bootstrap/#/buttons
		ButtonsModule.forRoot(),
		// Time ago
		// https://www.npmjs.com/package/ngx-timeago
		TimeagoModule.forRoot(),
		// Modal to edit roles
		// https://valor-software.com/ngx-bootstrap/#/modals
		ModalModule.forRoot(),
	],
	// Need to export the modules in order to work in app.module
	exports: [
		BsDropdownModule,
		ToastrModule,
		TabsModule,
		NgxGalleryModule,
		NgxSpinnerModule,
		FileUploadModule,
		BsDatepickerModule,
		PaginationModule,
		ButtonsModule,
		TimeagoModule,
		ModalModule,
	],
})
export class SharedModule {}
