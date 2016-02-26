using System;

namespace CorsairLink4.Module.HidDevices.Management.Filters
{
    public class WhereFilter
    {
        private string field;

        public WhereFilter(string fieldName)
        {
            field = fieldName;
        }

        public static readonly IManagementObjectFilter All = new WhereAllFilter();

        public IManagementObjectFilter Like(string value)
        {
            return new WhereLikeFilter(field, value);
        }

        public IManagementObjectFilter Eq(string value)
        {
            return new WhereEqFilter(field, value);
        }

        public IManagementObjectFilter StartsWith(string value)
        {
            return new WhereStartsWithFilter(field, value);
        }

        private class WhereAllFilter : IManagementObjectFilter
        {
            public string QueryString
            {
                get { return string.Empty; }
            }
        }

        private class WhereEqFilter : IManagementObjectFilter
        {
            private string queryString;

            internal WhereEqFilter(string field, string value)
            {
                queryString = string.Format(" WHERE {0} = '{1}'", field, value).Replace("\\", "\\\\");
            }

            public string QueryString
            {
                get { return queryString; }
            }
        }

        private class WhereLikeFilter : IManagementObjectFilter
        {
            private string queryString;

            internal WhereLikeFilter(string field, string value)
            {
                queryString = string.Format(" WHERE {0} LIKE '%{1}%'", field, value).Replace("\\", "\\\\");
            }

            public string QueryString
            {
                get { return queryString; }
            }
        }

        private class WhereStartsWithFilter : IManagementObjectFilter
        {
            private string queryString;

            internal WhereStartsWithFilter(string field, string value)
            {
                queryString = string.Format(" WHERE {0} LIKE '{1}%'", field, value).Replace("\\", "\\\\");
            }

            public string QueryString
            {
                get { return queryString; }
            }
        }

    }
}
