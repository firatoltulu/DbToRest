namespace DbToRest.Core.Infrastructure.SmartForm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class SmartFilterCommand
    {
        public SmartFilterCommand()
        {
            Top = -1;
            Skip = -1;
            SortDescriptors = new List<SortDescriptor>();
            FilterDescriptors = new List<IFilterDescriptor>();
            SelectDescriptors = new List<SelectDescriptor>();
        }

        public SmartFilterCommand(string query) : base()
        {
            var newInstance = Parse(query);
            this.Top = newInstance.Top;
            this.Skip = newInstance.Skip;
            this.From = newInstance.From;
            this.FilterDescriptors = newInstance.FilterDescriptors;
            this.SortDescriptors = newInstance.SortDescriptors;
            this.SelectDescriptors = newInstance.SelectDescriptors;
        }

        public int Skip
        {
            get;
            set;
        }

        public int Top
        {
            get;
            set;
        }

        public IList<SortDescriptor> SortDescriptors
        {
            get;
            private set;
        }

        public IList<IFilterDescriptor> FilterDescriptors
        {
            get;
            private set;
        }

        public IList<SelectDescriptor> SelectDescriptors
        {
            get;
            private set;
        }

        public static SmartFilterCommand Parse(string queryString, bool? isShowingSoftDeleted = null)
        {
            if (queryString.StartsWith("?"))
                queryString = queryString.Substring(1);

            var queries = HttpUtility.ParseQueryString(queryString);
            queries.Remove(null);

            var top = -1;
            var skip = -1;
            var filter = string.Empty;
            var select = string.Empty;
            var from = string.Empty;
            var orderby = string.Empty;

            try
            {
                if (queries.AllKeys.Contains("$top") && string.IsNullOrEmpty(queries["$top"]) == false)
                    top = int.Parse(queries["$top"]);

                if (queries.AllKeys.Contains("$skip") && string.IsNullOrEmpty(queries["$skip"]) == false)
                    skip = int.Parse(queries["$skip"]);

                if (queries.AllKeys.Contains("$filter") && string.IsNullOrEmpty(queries["$filter"]) == false)
                {
                    filter = queries["$filter"].ToString();

                }

                if (queries.AllKeys.Contains("$select") && string.IsNullOrEmpty(queries["$select"]) == false)
                    select = queries["$select"].ToString();

                if (queries.AllKeys.Contains("$orderby") && string.IsNullOrEmpty(queries["$orderby"]) == false)
                    orderby = queries["$orderby"].ToString();

                if (queries.AllKeys.Contains("$from") && string.IsNullOrEmpty(queries["$from"]) == false)
                    from = queries["$from"].ToString();
            }
            catch (Exception)
            {
            }

            if (isShowingSoftDeleted != null)
            {
                filter += string.Format("{0}Deleted~eq~{1}", string.IsNullOrEmpty(filter) ? "" : "~", isShowingSoftDeleted.Value.ToString().ToLower());
            }

            var removeoDataKeys = queries.AllKeys.Where(i => i.StartsWith("$")).ToList();
            foreach (var item in removeoDataKeys)
                queries.Remove(item);

           /* 
            *if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                if (HttpContext.Current.Request.Form.Count > 0)
                {
                    foreach (var item in HttpContext.Current.Request.Form.AllKeys)
                        queries.Add(item, HttpContext.Current.Request.Form[item]);
                }
            }
            */


            return Parse(from, skip, top, select, orderby, filter, queries);
        }

        public static SmartFilterCommand Parse(int skip, int top, string orderBy, string filter, string select)
        {
            return Parse(string.Empty, skip, top, select, orderBy, filter, null);
        }

        public static SmartFilterCommand Parse(int skip, int top, string orderBy, string filter)
        {
            return Parse(string.Empty, skip, top, string.Empty, orderBy, filter, null);
        }

        public static SmartFilterCommand Parse(string fromBy, int skip, int top)
        {
            return Parse(fromBy, skip, top);
        }

        public static SmartFilterCommand Parse(string fromBy, int skip, int top, string select)
        {
            return Parse(fromBy, skip, top, select);
        }

        public static SmartFilterCommand Parse(string fromBy, int skip, int top, string select, string orderBy)
        {
            return Parse(fromBy, skip, top, select, orderBy);
        }

        public static SmartFilterCommand Parse(string fromBy, int skip, int top, string select, string orderBy, string filter, System.Collections.Specialized.NameValueCollection parameters)
        {
            SmartFilterCommand result = new SmartFilterCommand
            {
                Skip = skip,
                Top = top,
                From = fromBy,
                SortDescriptors = SmartFilterDescriptorSerializer.Deserialize<SortDescriptor>(orderBy),
                SelectDescriptors = SmartFilterDescriptorSerializer.Deserialize<SelectDescriptor>(select),
                FilterDescriptors = FilterDescriptorFactory.Create(filter),
                Parameters = parameters
            };
            return result;
        }

        public string From { get; set; }

        public System.Collections.Specialized.NameValueCollection Parameters { get; set; }

        private static string FindQueryKey(List<string> arr, Func<string, bool> find)
        {
            string result = string.Empty;
            var value = arr.FirstOrDefault(find);
            if (value != null)
                result = value.Split('=')[1];

            return result;
        }
    }
}