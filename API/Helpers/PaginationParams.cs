namespace API.Helpers
{
    // Pagination query params received with the http request
    public class PaginationParams
    {
        // maximum allowed page size
		private const int MaxPageSize = 50;

		public int PageNumber { get; set; } = 1;

		// default page size
		private int _pageSize = 10; 

		public int PageSize { 
			get { 
				return _pageSize; 
			} 
			set {
				if(value > MaxPageSize) {
					_pageSize = 50;
				}
				else {
					_pageSize = value;
				}
			} 
			//get =>  _pageSize;
			//set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
		}
    }
}