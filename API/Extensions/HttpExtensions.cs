using System.Text.Json;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    /// <summary>
    /// Extension for HTTP responses
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Add custom pagination header to response
        /// </summary>
        /// <param name="response">HTTP response</param>
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages) {
			// 1. create pagination header
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            // use camel case for properties
			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			// 2. add custom pagination header
			response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));

            // 3. expose pagination header (because it's custom)
			response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
		}
    }
}