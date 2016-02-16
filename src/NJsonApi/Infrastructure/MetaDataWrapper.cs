using System;
using System.Collections.Generic;

namespace NJsonApi.Infrastructure
{
    public class MetaDataWrapper<T> : IMetaDataWrapper where T : class
    {
        private readonly T value;
        public T Value { get { return value; } }

        public Dictionary<string, object> MetaData { get; private set; }

        public object GetValue()
        {
            return Value;
        }

        public MetaDataWrapper(T value)
        {
            MetaData = new Dictionary<string, object>();
            this.value = value;
        }
    }

    public static class MetaDataConstants
    {
        public const string PageIndex = "pageIndex";
        public const string PageSize = "pageSize";
        public const string TotalItemCount = "totalItemCount";
        public const string TotalPageCount = "totalPageCount";
        public const string Filtering = "filtering";
        public const string Sorting = "sorting";
        public const string SortingAsc = "asc";
        public const string SortingDesc = "desc";
    }

    public enum MetaDataSortingDirection
    {
        Asc,
        Desc
    }
}