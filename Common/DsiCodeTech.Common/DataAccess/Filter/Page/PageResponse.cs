using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace DsiCodeTech.Common.DataAccess.Filter.Page
{
    public class PageResponse<T> where T : class
    {
        [JsonProperty(propertyName: "content")]
        public List<T> Content { get; set; }

        [JsonProperty("total_elements")]
        public int TotalElements { get; set; }

        [JsonProperty(propertyName: "total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty(propertyName: "size")]
        public int Size { get; set; }

        [JsonProperty(propertyName: "number")]
        public int Number { get; set; }

        public PageResponse(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalElements = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Size = pageSize;
            Number = pageNumber;
            Content = items;
        }

        public PageResponse() { }
    }
}
