import { Injectable } from '@angular/core';
import {
	HttpRequest,
	HttpHandler,
	HttpEvent,
	HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { delay, finalize } from 'rxjs/operators';

// Show a loading spinner when a request is sent
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
	constructor(private busyService: BusyService) {}

	intercept(
		request: HttpRequest<unknown>,
		next: HttpHandler
	): Observable<HttpEvent<unknown>> {
		// new request
		this.busyService.busy();

		// when the request comes back, is completed
		return next.handle(request).pipe(
			//delay(1000),
			// when completed
			finalize(() => {
				this.busyService.idle();
			})
		);
	}
}
