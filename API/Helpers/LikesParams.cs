namespace API.Helpers
{
    // Like query params received with the http request
	// pagination, selection of either the people the user liked or those who liked him
    public class LikesParams: PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}