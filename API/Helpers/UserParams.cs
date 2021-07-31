namespace API.Helpers
{
	// User query params received with the http request
	// pagination, selection of gender and excluding of current user
	public class UserParams
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

		// used to exclude current user from list
		public string CurrentUsename { get; set; }

		// selected gender
		public string Gender { get; set; }

		// age
		public int MinAge { get; set; } = 18;
		public int MaxAge { get; set; } = 150;

		// sorting
		// last active or created at
		public string OrderBy { get; set; } = "lastActive";
	}
}