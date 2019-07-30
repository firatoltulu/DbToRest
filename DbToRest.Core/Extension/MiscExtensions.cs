using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DbToRest.Core
{
    /// <remarks>codehint: sm-add</remarks>
    public static class MiscExtensions
    {
        private const string TablePrefixName = "SmartForm_{0}";

        public static void Dump(this System.Exception exc)
        {
            try
            {
                exc.StackTrace.Dump();
                exc.Message.Dump();
            }
            catch (System.Exception)
            {
            }
        }

        public static string ToElapsedMinutes(this Stopwatch watch)
        {
            return "{0:0.0}".FormatWith(TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalMinutes);
        }

        public static string ToElapsedSeconds(this Stopwatch watch)
        {
            return "{0:0.0}".FormatWith(TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds);
        }

        public static bool HasColumn(this DataView dv, string columnName)
        {
            dv.RowFilter = "ColumnName='" + columnName + "'";
            return dv.Count > 0;
        }

        public static string GetDataType(this DataTable dt, string columnName)
        {
            dt.DefaultView.RowFilter = "ColumnName='" + columnName + "'";
            return dt.Rows[0]["DataType"].ToString();
        }

        public static object SafeConvert(this TypeConverter converter, string value)
        {
            try
            {
                if (converter != null && value.HasValue() && converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFromString(value);
                }
            }
            catch (System.Exception exc)
            {
                exc.Dump();
            }
            return null;
        }

        public static bool IsEqual(this TypeConverter converter, string value, object compareWith)
        {
            object convertedObject = converter.SafeConvert(value);

            if (convertedObject != null && compareWith != null)
                return convertedObject.Equals(compareWith);

            return false;
        }

        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static string ToJson<T>(this T value)
        {
            return Serialize.ToJson(value);
        }

        public static T FromJson<T>(this string value)
        {
            return Serialize.FromJson<T>(value);
        }

        public static T FromJson<T>(this Stream s)
        {
            using (StreamReader reader = new StreamReader(s, Encoding.UTF8))
            {
                var jsonString = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
            }
        }

        public static string ToDbTableName(this string value)
        {
            Guard.ArgumentNotEmpty(value, "ToDbTableName");

            return TablePrefixName.FormatWith(value).Clean(true, false, false, "_", true);
        }

        public static string ToDbTableNameForArray(this string value, string parent)
        {
            Guard.ArgumentNotEmpty(value, "ToDbTableName");
            return string.Format("{0}_{1}", TablePrefixName.FormatWith(parent), value).Clean(true, false, false, "_", true);
        }
    }	// class
}