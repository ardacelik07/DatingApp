using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Helpers
{
    public class PagedList<T> :List<T>
    {
      

        public PagedList(IEnumerable<T> items,int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count/(double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,int pageNumber,int PageSize){

            var count = await source.CountAsync();
            //listedeki ürünlerin toplam sayısı

            var items = await source.Skip((pageNumber - 1)* PageSize).Take(PageSize).ToListAsync();
            //

            return new PagedList<T>(items,count,pageNumber,PageSize);

        }
    }
}