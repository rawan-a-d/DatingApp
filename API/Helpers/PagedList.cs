using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
	/// <summary>
	/// Responsible for pagination
	/// get number items, skip number items
	/// </summary>
	/// <typeparam name="T">any type of entity</typeparam>
	public class PagedList<T> : List<T>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items">list of items</param>
		/// <param name="count">number of items</param>
		/// <param name="pageNumber">current page number</param>
		/// <param name="pageSize">how many items per page</param>
		public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
		{
			CurrentPage = pageNumber;
			TotalPages = (int) Math.Ceiling(count / (double) pageSize);
			PageSize = pageSize;
			TotalCount = count;

			// Add the entire collection of elements in the list
			// https://www.tutorialspoint.com/What-is-the-AddRange-method-in-Chash-lists
			AddRange(items);
		}

		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PageSize { get; set; }
		public int TotalCount { get; set; }

		/// <summary>
		/// Create paginated async query
		/// </summary>
		/// <param name="source">source query</param>
		/// <param name="pageNumber">which page number</param>
		/// <param name="pageSize">how many items per page</param>
		/// <returns>paged list of an entity</returns>
		public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize) {
			// 1. get count from the db (1st query)
			var count = await source.CountAsync();

			// 2. add pagination and execute query (2nd query)
			var items = await source.Skip((pageNumber - 1) * pageSize)
									.Take(pageSize)
									.ToListAsync(); // perform query

			// 2. return the result
			return new PagedList<T>(items, count, pageNumber, pageSize);
		}
	}
}