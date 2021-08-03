import { Injectable } from '@angular/core';
import {
	HttpRequest,
	HttpHandler,
	HttpEvent,
	HttpInterceptor,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
	constructor(private router: Router, private toastr: ToastrService) {}

	// Intercepts request and response from to the server
	intercept(
		request: HttpRequest<unknown>,
		next: HttpHandler
	): Observable<HttpEvent<unknown>> {
		return next.handle(request).pipe(
			catchError((error) => {
				if (error) {
					// check type of error
					switch (error.status) {
						case 400:
							// validation errors (for example in register)
							if (error.error.errors) {
								const modalStateErrors = [];
								for (const key in error.error.errors) {
									if (error.error.errors[key]) {
										// Flatten errors array
										// gather errors into an array
										modalStateErrors.push(
											error.error.errors[key]
										);
									}
								}

								// throw it to be displayed in the component
								// flat: creates an array of values instead of array of objects
								// need to add es2019 to tsconfig.json like this ("lib": ["es2019", "es2018", "dom"])
								throw modalStateErrors.flat();
							} else {
								//this.toastr.error(error.statusText, error.status);
								this.toastr.error(error.error, error.status);
							}
							// check if error.error is an object
							//else if (typeof error.error === 'object') {
							//	this.toastr.error(
							//		error.statusText,
							//		error.status
							//	);
							//}
							//else {
							//	this.toastr.error(error.error, error.status);
							//}
							break;
						case 401:
							// this.toastr.error(error.statusText, error.status);
							this.toastr.error(
								error.error === null
									? 'Unauthorized'
									: error.error,
								error.status
							);
							break;
						case 404:
							this.router.navigateByUrl('/not-found');
							break;
						case 500:
							const navigationExtras: NavigationExtras = {
								state: { error: error.error },
							};
							this.router.navigateByUrl(
								'/server-error',
								navigationExtras
							);
							break;
						default:
							this.toastr.error(
								'Something unexpected went wrong'
							);
							console.log(error);
							break;
					}
				}

				return throwError(error);
			})
		);
	}
}
