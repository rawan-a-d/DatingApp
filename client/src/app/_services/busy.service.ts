import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
	providedIn: 'root',
})
export class BusyService {
	busyRequestCount = 0;

	constructor(private spinnerService: NgxSpinnerService) {}

	// new request
	busy() {
		this.busyRequestCount++;
		this.spinnerService.show(undefined, {
			//type: 'line-scale-party',
			type: 'line-scale-pulse-out-rapid',
			bdColor: 'rgba(255, 255, 255, 0)',
			color: '#333333',
		});
	}

	// request is done
	idle() {
		this.busyRequestCount--;
		if (this.busyRequestCount <= 0) {
			this.busyRequestCount = 0;

			this.spinnerService.hide();
		}
	}
}
