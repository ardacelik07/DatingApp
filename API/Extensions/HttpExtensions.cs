using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using API.Extensions;
using API.Helpers;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse Response, PaginationHeader header){

            var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            Response.Headers.Add("Pagination",JsonSerializer.Serialize(header,jsonOptions));
            Response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
    }
}
