using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MMDB.DataService.Web.Models
{
	//Mostly borrowed from MvcContrib: https://code.google.com/p/mvccontrib/
	//	However, I don't want the object itself to be IEnumerable like IPagination is, 
	//		because when it gets serialized to JSON, it will serialize as an array instead of an object
	//		and we would lose the important properties
	public class PagedViewModel<T>
	{
		private IList<T> DataSource { get; set; }
		public int PageNumber { get; private set; }
		public int PageSize { get; private set; }
		public int TotalItems { get; private set; }


		public PagedViewModel(IEnumerable<T> dataSource, int pageNumber, int pageSize, int totalItems)
		{
			this.DataSource = dataSource.ToList();
			this.PageNumber = pageNumber;
			this.PageSize = pageSize;
			this.TotalItems = totalItems;
		}

		public int TotalPages
		{
			get { return (int)Math.Ceiling(((double)this.TotalItems) / this.PageSize); }
		}

		public int FirstItem
		{
			get
			{
				return ((this.PageNumber - 1) * this.PageSize) + 1;
			}
		}

		public int LastItem
		{
			get { return this.FirstItem + this.DataSource.Count - 1; }
		}

		public bool HasPreviousPage
		{
			get { return this.PageNumber > 1; }
		}

		public bool HasNextPage
		{
			get { return this.PageNumber < this.TotalPages; }
		}

	}
}