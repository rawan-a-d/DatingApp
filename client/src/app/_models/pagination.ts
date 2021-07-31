// Information in response header
export interface Pagination {
	currentPage: number;
	itemsPerPage: number;
	totalItems: number;
	totalPages: number;
}

// T here represent an array of members
export class PaginatedResult<T> {
	result: T; // list of members
	pagination: Pagination; // pagination details
}
