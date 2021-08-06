import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagination';

// Methods for pagination
// get paginated result of generic type
export function getPaginatedResult<T>(
	url: string,
	params: HttpParams,
	http: HttpClient
) {
	// Paginated result
	const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

	return http
		.get<T>(url, {
			observe: 'response',
			params, // add params
		})
		.pipe(
			// save members to members list
			map((response) => {
				paginatedResult.result = response.body;

				// get pagination info from header
				if (response.headers.get('Pagination') !== null) {
					paginatedResult.pagination = JSON.parse(
						response.headers.get('Pagination')
					);
				}

				return paginatedResult;
			})
		);
}

// Add pagination headers
export function getPaginationHeaders(pageNumber: number, pageSize: number) {
	// create http params
	let params = new HttpParams();

	params = params.append('pageNumber', pageNumber);
	params = params.append('pageSize', pageSize);

	return params;
}
