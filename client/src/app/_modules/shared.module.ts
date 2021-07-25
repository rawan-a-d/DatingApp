import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';

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
	],
	// Need to export the modules in order to work in app.module
	exports: [BsDropdownModule, ToastrModule, TabsModule, NgxGalleryModule],
})
export class SharedModule {}
