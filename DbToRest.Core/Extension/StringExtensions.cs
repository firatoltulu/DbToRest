using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DbToRest.Core
{
    public static partial class StringExtensions
    {
        public const string CarriageReturnLineFeed = "\r\n";
        public const string Empty = "";
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';
        public const char Tab = '\t';

        private delegate void ActionLine(TextWriter textWriter, string line);

        #region Char extensions

        [DebuggerStepThrough]
        public static int ToInt(this char value)
        {
            if ((value >= '0') && (value <= '9'))
            {
                return (value - '0');
            }
            if ((value >= 'a') && (value <= 'f'))
            {
                return ((value - 'a') + 10);
            }
            if ((value >= 'A') && (value <= 'F'))
            {
                return ((value - 'A') + 10);
            }
            return -1;
        }

        [DebuggerStepThrough]
        public static string ToUnicode(this char c)
        {
            using (StringWriter w = new StringWriter(CultureInfo.InvariantCulture))
            {
                WriteCharAsUnicode(c, w);
                return w.ToString();
            }
        }

        internal static void WriteCharAsUnicode(char c, TextWriter writer)
        {
            Guard.ArgumentNotNull(writer, "writer");

            char h1 = ((c >> 12) & '\x000f').ToHex();
            char h2 = ((c >> 8) & '\x000f').ToHex();
            char h3 = ((c >> 4) & '\x000f').ToHex();
            char h4 = (c & '\x000f').ToHex();

            writer.Write('\\');
            writer.Write('u');
            writer.Write(h1);
            writer.Write(h2);
            writer.Write(h3);
            writer.Write(h4);
        }

        #endregion Char extensions

        #region String extensions

        [DebuggerStepThrough]
        public static string ToSafe(this string value, string defaultValue = null)
        {
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }
            return (defaultValue ?? String.Empty);
        }

        [DebuggerStepThrough]
        public static string EmptyNull(this string value)
        {
            return (value ?? string.Empty).Trim();
        }

        [DebuggerStepThrough]
        public static string NullEmpty(this string value)
        {
            return (string.IsNullOrEmpty(value)) ? null : value;
        }

        /// <summary>
        /// Formats a string to an invariant culture
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatInvariant(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.InvariantCulture, format, objects);
        }

        /// <summary>
        /// Formats a string to the current culture.
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatCurrent(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.CurrentCulture, format, objects);
        }

        /// <summary>
        /// Formats a string to the current UI culture.
        /// </summary>
        /// <param name="formatString">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatCurrentUI(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.CurrentUICulture, format, objects);
        }

        [DebuggerStepThrough]
        public static string FormatWith(this string format, params object[] args)
        {
            return FormatWith(format, CultureInfo.CurrentCulture, args);
        }

        [DebuggerStepThrough]
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            return string.Format(provider, format, args);
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="instance">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsCaseSensitiveEqual(this string value, string comparing)
        {
            return string.CompareOrdinal(value, comparing) == 0;
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="instance">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsCaseInsensitiveEqual(this string value, string comparing)
        {
            return string.Compare(value, comparing, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Determines whether the string is null, empty or all whitespace.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsEmpty(this string value)
        {
            if (value == null || value.Length == 0)
                return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the string is all white space. Empty string will return false.
        /// </summary>
        /// <param name="s">The string to test whether it is all white space.</param>
        /// <returns>
        /// 	<c>true</c> if the string is all white space; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsWhiteSpace(this string value)
        {
            Guard.ArgumentNotNull(value, "value");

            if (value.Length == 0)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }

            return true;
        }

        [DebuggerStepThrough]
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <remarks>codehint: sm-edit</remarks>
        /// <remarks>to get equivalent result to PHPs md5 function call Hash("my value", false, false).</remarks>
        [DebuggerStepThrough]
        public static string Hash(this string value, bool toBase64 = false, bool unicode = false)
        {
            Guard.ArgumentNotEmpty(value, "value");

            using (MD5 md5 = MD5.Create())
            {
                byte[] data = null;
                if (unicode)
                    data = Encoding.Unicode.GetBytes(value);
                else
                    data = Encoding.ASCII.GetBytes(value);

                if (toBase64)
                {
                    byte[] hash = md5.ComputeHash(data);
                    return Convert.ToBase64String(hash);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    byte[] hashBytes = md5.ComputeHash(data);
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2").ToLower());
                    }

                    return sb.ToString();
                }
            }
        }

        [DebuggerStepThrough]
        public static bool IsWebUrl(this string value)
        {
            return !String.IsNullOrEmpty(value) && RegularExpressions.IsWebUrl.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static bool IsEmail(this string value)
        {
            return !String.IsNullOrEmpty(value) && RegularExpressions.IsEmail.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static bool IsNumeric(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            return !RegularExpressions.IsNotNumber.IsMatch(value) &&
                   !RegularExpressions.HasTwoDot.IsMatch(value) &&
                   !RegularExpressions.HasTwoMinus.IsMatch(value) &&
                   RegularExpressions.IsNumeric.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static bool IsAlpha(this string value)
        {
            return RegularExpressions.IsAlpha.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static bool IsAlphaNumeric(this string value)
        {
            return RegularExpressions.IsAlphaNumeric.IsMatch(value);
        }

        [DebuggerStepThrough]
        public static string Truncate(this string value, int maxLength)
        {
            return Truncate(value, maxLength, "...");
        }

        [DebuggerStepThrough]
        public static string Truncate(this string value, int maxLength, string suffix)
        {
            Guard.ArgumentNotNull(suffix, "suffix");
            Guard.ArgumentIsPositive(maxLength, "maxLength");

            int subStringLength = maxLength - suffix.Length;

            if (subStringLength <= 0)
                throw Error.Argument("maxLength", "Length of suffix string is greater or equal to maximumLength");

            if (value != null && value.Length > maxLength)
            {
                string truncatedString = value.Substring(0, subStringLength);
                // incase the last character is a space
                truncatedString = truncatedString.Trim();
                truncatedString += suffix;

                return truncatedString;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Determines whether the string contains white space.
        /// </summary>
        /// <param name="s">The string to test for white space.</param>
        /// <returns>
        /// 	<c>true</c> if the string contains white space; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool ContainsWhiteSpace(this string value)
        {
            Guard.ArgumentNotNull(value, "value");

            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsWhiteSpace(value[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Ensures the target string ends with the specified string.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        /// <returns>The target string with the value string at the end.</returns>
        [DebuggerStepThrough]
        public static string EnsureEndsWith(this string value, string endWith)
        {
            Guard.ArgumentNotNull(value, "value");
            Guard.ArgumentNotNull(endWith, "endWith");

            if (value.Length >= endWith.Length)
            {
                if (string.Compare(value, value.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;

                string trimmedString = value.TrimEnd(null);

                if (string.Compare(trimmedString, trimmedString.Length - endWith.Length, endWith, 0, endWith.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    return value;
            }

            return value + endWith;
        }

        [DebuggerStepThrough]
        public static int? GetLength(this string value)
        {
            if (value == null)
                return null;
            else
                return value.Length;
        }

        [DebuggerStepThrough]
        public static string RemoveHtml(this string value)
        {
            return RemoveHtmlInternal(value, null);
        }

        public static string RemoveHtml(this string value, ICollection<string> removeTags)
        {
            return RemoveHtmlInternal(value, removeTags);
        }

        private static string RemoveHtmlInternal(string s, ICollection<string> removeTags)
        {
            List<string> removeTagsUpper = null;
            if (removeTags != null)
            {
                removeTagsUpper = new List<string>(removeTags.Count);

                foreach (string tag in removeTags)
                {
                    removeTagsUpper.Add(tag.ToUpperInvariant());
                }
            }

            return RegularExpressions.RemoveHTML.Replace(s, delegate (Match match)
            {
                string tag = match.Groups["tag"].Value.ToUpperInvariant();

                if (removeTagsUpper == null)
                    return string.Empty;
                else if (removeTagsUpper.Contains(tag))
                    return string.Empty;
                else
                    return match.Value;
            });
        }

        /// <summary>
        /// Replaces pascal casing with spaces. For example "CustomerId" would become "Customer Id".
        /// Strings that already contain spaces are ignored.
        /// </summary>
        /// <param name="input">String to split</param>
        /// <returns>The string after being split</returns>
        [DebuggerStepThrough]
        public static string SplitPascalCase(this string value)
        {
            //return Regex.Replace(input, "([A-Z][a-z])", " $1", RegexOptions.Compiled).Trim();
            StringBuilder sb = new StringBuilder();
            char[] ca = value.ToCharArray();
            sb.Append(ca[0]);
            for (int i = 1; i < ca.Length - 1; i++)
            {
                char c = ca[i];
                if (char.IsUpper(c) && (char.IsLower(ca[i + 1]) || char.IsLower(ca[i - 1])))
                {
                    sb.Append(" ");
                }
                sb.Append(c);
            }
            if (ca.Length > 1)
            {
                sb.Append(ca[ca.Length - 1]);
            }

            return sb.ToString();
        }

        /// <remarks>codehint: sm-add</remarks>
        [DebuggerStepThrough]
        public static string[] SplitSafe(this string value, string separator)
        {
            if (string.IsNullOrEmpty(value))
                return new string[0];
            return value.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>Splits a string into two strings</summary>
        /// <remarks>codehint: sm-add</remarks>
        /// <returns>true: success, false: failure</returns>
        [DebuggerStepThrough]
        public static bool SplitToPair(this string value, out string strLeft, out string strRight, string delimiter)
        {
            int idx = -1;
            if (value.IsNullOrEmpty() || delimiter.IsNullOrEmpty() || (idx = value.IndexOf(delimiter)) == -1)
            {
                strLeft = value;
                strRight = "";
                return false;
            }
            strLeft = value.Substring(0, idx);
            strRight = value.Substring(idx + delimiter.Length);
            return true;
        }

        [DebuggerStepThrough]
        public static string ToCamelCase(this string instance)
        {
            char ch = instance[0];
            return (ch.ToString().ToLowerInvariant() + instance.Substring(1));
        }

        [DebuggerStepThrough]
        public static string ReplaceNewLines(this string value, string replacement)
        {
            StringReader sr = new StringReader(value);
            StringBuilder sb = new StringBuilder();

            bool first = true;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (first)
                    first = false;
                else
                    sb.Append(replacement);

                sb.Append(line);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Indents the specified string.
        /// </summary>
        /// <param name="s">The string to indent.</param>
        /// <param name="indentation">The number of characters to indent by.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string Indent(this string value, int indentation)
        {
            return Indent(value, indentation, ' ');
        }

        /// <summary>
        /// Indents the specified string.
        /// </summary>
        /// <param name="s">The string to indent.</param>
        /// <param name="indentation">The number of characters to indent by.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string Indent(this string value, int indentation, char indentChar)
        {
            Guard.ArgumentNotNull(value, "value");
            Guard.ArgumentIsPositive(indentation, "indentation");

            StringReader sr = new StringReader(value);
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

            ActionTextReaderLine(sr, sw, delegate (TextWriter tw, string line)
            {
                tw.Write(new string(indentChar, indentation));
                tw.Write(line);
            });

            return sw.ToString();
        }

        /// <summary>
        /// Numbers the lines.
        /// </summary>
        /// <param name="s">The string to number.</param>
        /// <returns></returns>
        public static string NumberLines(this string value)
        {
            Guard.ArgumentNotNull(value, "value");

            StringReader sr = new StringReader(value);
            StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

            int lineNumber = 1;

            ActionTextReaderLine(sr, sw, delegate (TextWriter tw, string line)
            {
                tw.Write(lineNumber.ToString(CultureInfo.InvariantCulture).PadLeft(4));
                tw.Write(". ");
                tw.Write(line);

                lineNumber++;
            });

            return sw.ToString();
        }

        [DebuggerStepThrough]
        public static string EncodeJsString(this string value)
        {
            return EncodeJsString(value, '"', true);
        }

        [DebuggerStepThrough]
        public static string EncodeJsString(this string value, char delimiter, bool appendDelimiters)
        {
            StringBuilder sb = new StringBuilder(value.GetLength() ?? 16);
            using (StringWriter w = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                EncodeJsString(w, value, delimiter, appendDelimiters);
                return w.ToString();
            }
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string enclosedIn)
        {
            return value.IsEnclosedIn(enclosedIn, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string enclosedIn, StringComparison comparisonType)
        {
            if (String.IsNullOrEmpty(enclosedIn))
                return false;

            if (enclosedIn.Length == 1)
                return value.IsEnclosedIn(enclosedIn, enclosedIn, comparisonType);

            if (enclosedIn.Length % 2 == 0)
            {
                int len = enclosedIn.Length / 2;
                return value.IsEnclosedIn(
                    enclosedIn.Substring(0, len),
                    enclosedIn.Substring(len, len),
                    comparisonType);
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string start, string end)
        {
            return value.IsEnclosedIn(start, end, StringComparison.CurrentCulture);
        }

        [DebuggerStepThrough]
        public static bool IsEnclosedIn(this string value, string start, string end, StringComparison comparisonType)
        {
            return value.StartsWith(start, comparisonType) && value.EndsWith(end, comparisonType);
        }

        public static string RemoveEncloser(this string value, string encloser)
        {
            return value.RemoveEncloser(encloser, StringComparison.CurrentCulture);
        }

        public static string RemoveEncloser(this string value, string encloser, StringComparison comparisonType)
        {
            if (value.IsEnclosedIn(encloser, comparisonType))
            {
                int len = encloser.Length / 2;
                return value.Substring(
                    len,
                    value.Length - (len * 2));
            }

            return value;
        }

        public static string RemoveEncloser(this string value, string start, string end)
        {
            return value.RemoveEncloser(start, end, StringComparison.CurrentCulture);
        }

        public static string RemoveEncloser(this string value, string start, string end, StringComparison comparisonType)
        {
            if (value.IsEnclosedIn(start, end, comparisonType))
                return value.Substring(
                    start.Length,
                    value.Length - (start.Length + end.Length));

            return value;
        }

        // codehint: sm-add (begin)

        /// <summary>Debug.WriteLine</summary>
        /// <remarks>codehint: sm-add</remarks>
        [DebuggerStepThrough]
        public static void Dump(this string value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>Appends grow and uses delimiter if the string is not empty.</summary>
        /// <remarks>codehint: sm-add</remarks>
        [DebuggerStepThrough]
        public static string Grow(this string value, string grow, string delimiter)
        {
            if (string.IsNullOrEmpty(value))
                return (string.IsNullOrEmpty(grow) ? "" : grow);

            if (string.IsNullOrEmpty(grow))
                return (string.IsNullOrEmpty(value) ? "" : value);

            return string.Format("{0}{1}{2}", value, delimiter, grow);
        }

        /// <summary>Returns n/a if string is empty else self.</summary>
        /// <remarks>codehint: sm-add</remarks>
        [DebuggerStepThrough]
        public static string NaIfEmpty(this string value)
        {
            return (value.HasValue() ? value : "n/a");
        }

        /// <summary>Replaces substring with position x1 to x2 by replaceBy.</summary>
        /// <remarks>codehint: sm-add</remarks>
        [DebuggerStepThrough]
        public static string Replace(this string value, int x1, int x2, string replaceBy = null)
        {
            if (value.HasValue() && x1 > 0 && x2 > x1 && x2 < value.Length)
            {
                return value.Substring(0, x1) + (replaceBy == null ? "" : replaceBy) + value.Substring(x2 + 1);
            }
            return value;
        }

        [DebuggerStepThrough]
        public static string TrimSafe(this string value)
        {
            return (value.HasValue() ? value.Trim() : value);
        }

        [DebuggerStepThrough]
        public static string Prettify(this string value, bool allowSpace = false, char[] allowChars = null)
        {
            string res = "";
            try
            {
                if (value.HasValue())
                {
                    StringBuilder sb = new StringBuilder();
                    bool space = false;
                    char ch;

                    for (int i = 0; i < value.Length; ++i)
                    {
                        ch = value[i];

                        if (ch == ' ' || ch == '-')
                        {
                            if (allowSpace && ch == ' ')
                                sb.Append(' ');
                            else if (!space)
                                sb.Append('-');
                            space = true;
                            continue;
                        }

                        space = false;

                        if ((ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 90) || (ch >= 97 && ch <= 122))
                        {
                            sb.Append(ch);
                            continue;
                        }

                        if (allowChars != null && allowChars.Contains(ch))
                        {
                            sb.Append(ch);
                            continue;
                        }

                        switch (ch)
                        {
                            case '_': sb.Append(ch); break;

                            case 'ä': sb.Append("ae"); break;
                            case 'ö': sb.Append("oe"); break;
                            case 'ü': sb.Append("ue"); break;
                            case 'ß': sb.Append("ss"); break;
                            case 'Ä': sb.Append("AE"); break;
                            case 'Ö': sb.Append("OE"); break;
                            case 'Ü': sb.Append("UE"); break;

                            case 'é':
                            case 'è':
                            case 'ê': sb.Append('e'); break;
                            case 'á':
                            case 'à':
                            case 'â': sb.Append('a'); break;
                            case 'ú':
                            case 'ù':
                            case 'û': sb.Append('u'); break;
                            case 'ó':
                            case 'ò':
                            case 'ô': sb.Append('o'); break;
                        }   // switch
                    }   // for

                    if (sb.Length > 0)
                    {
                        res = sb.ToString().Trim(new char[] { ' ', '-' });

                        Regex pat = new Regex(@"(-{2,})");      // remove double SpaceChar
                        res = pat.Replace(res, "-");
                        res = res.Replace("__", "_");
                    }
                }
            }
            catch (System.Exception exp)
            {
                exp.Dump();
            }
            return (res.Length > 0 ? res : "null");
        }

        public static string SanitizeHtmlId(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            StringBuilder builder = new StringBuilder(value.Length);
            int index = value.IndexOf("#");
            int num2 = value.LastIndexOf("#");
            if (num2 > index)
            {
                ReplaceInvalidHtmlIdCharacters(value.Substring(0, index), builder);
                builder.Append(value.Substring(index, (num2 - index) + 1));
                ReplaceInvalidHtmlIdCharacters(value.Substring(num2 + 1), builder);
            }
            else
            {
                ReplaceInvalidHtmlIdCharacters(value, builder);
            }
            return builder.ToString();
        }

        private static bool IsValidHtmlIdCharacter(char c)
        {
            bool invalid = (c == '?' || c == '!' || c == '#' || c == '.' || c == ' ' || c == ';' || c == ':');
            return !invalid;
        }

        private static void ReplaceInvalidHtmlIdCharacters(string part, StringBuilder builder)
        {
            for (int i = 0; i < part.Length; i++)
            {
                char c = part[i];
                if (IsValidHtmlIdCharacter(c))
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append('_');
                }
            }
        }

        public static string Sha(this string value)
        {
            if (value.HasValue())
            {
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    byte[] data = Encoding.ASCII.GetBytes(value);
                    StringBuilder sb = new StringBuilder();

                    foreach (byte b in sha1.ComputeHash(data))
                        sb.Append(b.ToString("x2"));

                    return sb.ToString();
                }
            }
            return "";
        }

        [DebuggerStepThrough]
        public static bool IsMatch(this string input, string pattern, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            return Regex.IsMatch(input, pattern, options);
        }

        [DebuggerStepThrough]
        public static bool IsMatch(this string input, string pattern, out Match match, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            match = Regex.Match(input, pattern, options);
            return match.Success;
        }

        public static string RegexRemove(this string input, string pattern, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            return Regex.Replace(input, pattern, string.Empty, options);
        }

        public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
        {
            return Regex.Replace(input, pattern, replacement, options);
        }

        [DebuggerStepThrough]
        public static string ToValidFileName(this string input, string replacement = "-")
        {
            return input.ToValidPathInternal(false, replacement);
        }

        [DebuggerStepThrough]
        public static string ToValidPath(this string input, string replacement = "-")
        {
            return input.ToValidPathInternal(true, replacement);
        }

        private static string ToValidPathInternal(this string input, bool isPath, string replacement)
        {
            var result = input.ToSafe();

            char[] invalidChars = isPath ? Path.GetInvalidPathChars() : Path.GetInvalidFileNameChars();

            foreach (var c in invalidChars)
            {
                result = result.Replace(c.ToString(), replacement ?? "-");
            }

            return result;
        }

        [DebuggerStepThrough]
        public static int[] ToIntArray(this string s)
        {
            return Array.ConvertAll(s.SplitSafe(","), v => int.Parse(v));
        }

        [DebuggerStepThrough]
        public static bool ToIntArrayContains(this string s, int value, bool defaultValue)
        {
            var arr = s.ToIntArray();
            if (arr == null || arr.Count() <= 0)
                return defaultValue;
            return arr.Contains(value);
        }

        [DebuggerStepThrough]
        public static string RemoveInvalidXmlChars(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            return Regex.Replace(s, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "", RegexOptions.Compiled);
        }

        // codehint: sm-add (end)

        #endregion String extensions

        #region Helper

        private static void EncodeJsChar(TextWriter writer, char c, char delimiter)
        {
            switch (c)
            {
                case '\t':
                    writer.Write(@"\t");
                    break;

                case '\n':
                    writer.Write(@"\n");
                    break;

                case '\r':
                    writer.Write(@"\r");
                    break;

                case '\f':
                    writer.Write(@"\f");
                    break;

                case '\b':
                    writer.Write(@"\b");
                    break;

                case '\\':
                    writer.Write(@"\\");
                    break;
                //case '<':
                //case '>':
                //case '\'':
                //  StringUtils.WriteCharAsUnicode(writer, c);
                //  break;
                case '\'':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '\'') ? @"\'" : @"'");
                    break;

                case '"':
                    // only escape if this charater is being used as the delimiter
                    writer.Write((delimiter == '"') ? "\\\"" : @"""");
                    break;

                default:
                    if (c > '\u001f')
                        writer.Write(c);
                    else
                        WriteCharAsUnicode(c, writer);
                    break;
            }
        }

        private static void EncodeJsString(TextWriter writer, string value, char delimiter, bool appendDelimiters)
        {
            // leading delimiter
            if (appendDelimiters)
                writer.Write(delimiter);

            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    EncodeJsChar(writer, value[i], delimiter);
                }
            }

            // trailing delimiter
            if (appendDelimiters)
                writer.Write(delimiter);
        }

        private static void ActionTextReaderLine(TextReader textReader, TextWriter textWriter, ActionLine lineAction)
        {
            string line;
            bool firstLine = true;
            while ((line = textReader.ReadLine()) != null)
            {
                if (!firstLine)
                    textWriter.WriteLine();
                else
                    firstLine = false;

                lineAction(textWriter, line);
            }
        }

        [DebuggerStepThrough]
        public static string ReplaceMicroExpression(this string value)
        {
            Guard.ArgumentNotNull(value, "value");

            if (value.IndexOf("*") > -1)
                value = value.Replace("*", "%");
            else
                value += "%";

            return value;
        }

        #endregion Helper

        public static string StripString(this string input)
        {
            input = input.ToLower(new CultureInfo("tr-TR"));
            input = StripTags(input);
            input = RemoveAccents(input);
            input = Regex.Replace(input, "&.+?;", "");
            input = Regex.Replace(input, "[^.a-z0-9 _-]", "");
            input = Regex.Replace(input, @"\.|\s+", "");
            input = Regex.Replace(input, "-+", "");
            input = input.Trim('-');
            return input;
        }

        public static string RemoveAccents(string input)
        {
            string normalized = input.Replace('ı', 'i').Normalize(NormalizationForm.FormKD);
            char[] array = new char[input.Length];
            int arrayIndex = 0;
            foreach (char c in normalized)
            {
                if (char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    array[arrayIndex] = c;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string StripTags(string input)
        {
            char[] array = new char[input.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < input.Length; i++)
            {
                char let = input[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// String Ayır Örneğin InProc yazdığınızda size In Proc Olarak Dönücektir
        /// </summary>
        /// <param name="camelCase"></param>
        /// <returns></returns>
        public static string FromCamelCase(this string camelCase)
        {
            if (camelCase == null)
                throw new ArgumentException("Lütfen Değer Giriniz");

            var sb = new StringBuilder(camelCase.Length + 10);
            bool first = true;
            char lastChar = '\0';

            foreach (char ch in camelCase)
            {
                if (!first &&
                     (char.IsUpper(ch) ||
                       char.IsDigit(ch) && !char.IsDigit(lastChar)))
                    sb.Append(' ');

                sb.Append(ch);
                first = false;
                lastChar = ch;
            }

            return sb.ToString(); ;
        }

        public static bool FindStartWith(this string key, string value)
        {
            return key.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool FindEndWith(this string key, string value)
        {
            return key.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string Highlight(this string s, string Search_str, string StartTag, string EndTag)
        {
            Regex RegExp = new Regex(Search_str.Replace(" ", "|").Trim(), RegexOptions.IgnoreCase);

            // Highlight keywords by calling the delegate each time a keyword is found.
            return RegExp.Replace(s, new MatchEvaluator(pk => StartTag + pk.Value + EndTag));
        }

        public static string FixHTMLForDisplay(this string Html)
        {
            Html = Html.Replace("<", "&lt;");
            Html = Html.Replace(">", "&gt;");
            Html = Html.Replace("\"", "&quot;");
            return Html;
        }

        #region StringConstantsToHelpWithComparisons

        private const string m_Letters = "abcdefghijklmnopqrstuvwxyz";
        private const string m_Digits = "0123456789";
        private const string m_ForwardSlash = "/";
        private const string m_BackSlash = "\\";
        private const string m_Period = ".";
        private const string m_DollarSign = "$";
        private const string m_PercentSign = "%";
        private const string m_Comma = ",";
        private const string m_Yes = "yes";
        private const string m_No = "no";
        private const string m_True = "true";
        private const string m_False = "false";
        private const string m_1 = "1";
        private const string m_0 = "0";
        private const string m_y = "y";
        private const string m_n = "n";
        private const string m_special = "-()+*~";

        #endregion StringConstantsToHelpWithComparisons

        #region DataTypeStringConstants

        public const string m_GUID = "guid";
        public const string m_Boolean1 = "boolean";
        public const string m_Boolean2 = "bool";
        public const string m_Byte = "byte";
        public const string m_Char = "char";
        public const string m_DateTime = "datetime";
        public const string m_DBNull = "dbnull";
        public const string m_Decimal = "decimal";
        public const string m_Double = "double";
        public const string m_Empty = "empty";
        public const string m_Int16_1 = "int16";
        public const string m_Int16_2 = "short";
        public const string m_Int32_1 = "int32";
        public const string m_Int32_2 = "int";
        public const string m_Int32_3 = "integer";
        public const string m_Int64_1 = "int64";
        public const string m_Int64_2 = "long";
        public const string m_Object = "object";
        public const string m_SByte = "sbyte";
        public const string m_Single = "single";
        public const string m_String = "string";
        public const string m_UInt16 = "uint16";
        public const string m_UInt32 = "uint32";
        public const string m_UInt64 = "uint64";

        #endregion DataTypeStringConstants

        #region MethodsThatCheckDataType

        /// <summary>
        /// Evaluates whether passed-in string can be converted to a bool
        /// </summary>
        /// <param name="stream">string to check</param>
        /// <returns>
        /// bool indicating whether stream is a bool (0, 1, true/True,
        /// false/False)
        /// </returns>
        public static bool IsStandardBool(string stream)
        {
            try
            {
                if (stream == null || stream == string.Empty)
                    return false;
                stream = stream.Trim().ToLower();
                switch (stream)
                {
                    case m_0:
                        return true;

                    case m_1:
                        return true;

                    case m_True:
                        return true;

                    case m_False:
                        return true;

                    default:
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return false;
            }
        }

        /// <summary>
        /// Evaluates whether string can can be COERCED to a bool
        /// </summary>
        /// <param name="stream">string to check</param>
        /// <returns>
        /// bool indicating whether argument is a standard or custom bool
        /// (0, 1, true/True, false/False) OR (y/Y, yes/Yes, n/N, no/NO)
        /// </returns>
        public static bool IsFriendlyBool(string stream)
        {
            try
            {
                if (stream == null || stream == string.Empty)
                    return false;
                stream = stream.Trim().ToLower();
                switch (stream)
                {
                    case m_0:
                        return true;

                    case m_1:
                        return true;

                    case m_True:
                        return true;

                    case m_False:
                        return true;

                    case m_n:
                        return true;

                    case m_y:
                        return true;

                    case m_No:
                        return true;

                    case m_Yes:
                        return true;

                    default:
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return false;
            }
        }

        /// <summary>
        /// Returns a bool conversion of the passed in string
        /// </summary>
        /// <param name="stream">string to convert/coerce</param>
        /// <returns>
        /// bool representation of passed-in string
        /// </returns>
        public static bool CoerceToBool(string stream)
        {
            try
            {
                stream = stream.Trim().ToLower();
                switch (stream)
                {
                    case m_0:
                        return true;

                    case m_1:
                        return true;

                    case m_True:
                        return true;

                    case m_False:
                        return false;

                    case m_n:
                        return false;

                    case m_y:
                        return true;

                    case m_No:
                        return false;

                    case m_Yes:
                        return true;

                    default:
                        return false;
                }
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks each character of the string for any character other
        /// than a digit, or a dollar sign or a percentage sign. If any
        /// are found, returns false indicating that the stream is NOT
        /// a number
        /// </summary>
        /// <param name="stream">
        /// The stream of characters (string) to check
        /// </param>
        /// <returns>
        /// True/False value indicating whether the string can be
        /// coerced to a number
        /// </returns>
        public static bool IsNumber(string stream)
        {
            try
            {
                if (stream == null || stream == string.Empty)
                    return false;

                string character = string.Empty;
                //set a string up of all characters that may indicate
                //that the stream is a number, or a formatted number:
                string validCharacters = m_Digits + m_Period +
                m_DollarSign + m_Comma;
                for (int i = 0; i < stream.Length; i++)
                {
                    character = stream.Substring(i, 1);
                    if (!validCharacters.Contains(character))
                        return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks the string to see whether it is a number & if it is,
        /// then it checks whether there is formatting applied to that #
        /// </summary>
        /// <param name="stream">
        /// The stream of characters (string) to check
        /// </param>
        /// <returns>
        /// True/False value indicating whether the string is a number
        /// that is formatted (contains digits and number formatting)
        /// </returns>
        public static bool IsFormattedNumber(string stream)
        {
            try
            {
                if (stream == null || stream == string.Empty)
                    return false;

                string character = string.Empty;
                //set a string up of all characters that may indicate that
                //the stream is a number, or a formatted number:
                string validCharacters = m_Digits + m_Period +
                m_DollarSign + m_PercentSign + m_Comma;

                for (int i = 0; i < stream.Length; i++)
                {
                    character = stream.Substring(i, 1);
                    if (!validCharacters.Contains(character))
                        //the stream contains non-numeric characters:
                        return false;
                }
                //at this point, each single character is a number OR an
                //allowable symbol, but we must see whether those
                //characters contain a formatting character:
                string formattingCharacters = m_DollarSign +
                m_PercentSign + m_Comma;
                for (int i = 0; i < stream.Length; i++)
                {
                    if (formattingCharacters.Contains(character))
                        return true;
                }
                //still here? then the stream is a number, but NOT a
                //formatted number
                return false;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks whether string can be coerced into a DateTime value
        /// </summary>
        /// <param name="stream">The string to check/// </param>
        /// <returns>
        /// bool indicating whether stream can be converted to a date
        /// </returns>
        public static bool IsDate(string stream)
        {
            try
            {
                if (stream == null || stream == string.Empty)
                    return false;
                DateTime checkDate = new DateTime();
                bool dateType = true;
                try
                {
                    checkDate = DateTime.Parse(stream);
                }
                catch
                {
                    dateType = false;
                }
                return dateType;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks the string to see whether it is a number and if it is,
        /// then it checks whether there is a decimal in that number.
        /// </summary>
        /// <param name="stream">
        /// The stream of characters (string) to check
        /// </param>
        /// <returns>
        /// True/False value indicating whether the string is a
        /// double---must be a number, and include a decimal
        /// in order to pass the test
        /// </returns>
        public static bool IsDouble(string stream)
        {
            try
            {
                if (stream == null || stream == string.Empty)
                    return false;

                if (!IsNumber(stream))
                    return false;

                //at this point each character is a number OR an allowable
                //symbol; we must see whether the string holds the decimal:
                if (stream.Contains(m_Period))
                    return true;

                //still here? the stream is a #, but does NOT have a decimal
                return false;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks string to see if it matches a TypeCode string and returns
        /// that TypeCode, or returns TypeCode.Empty if there is no match
        /// </summary>
        /// <param name="dataTypeString">
        /// String representation of a TypeCode (string, int, bool...)
        /// </param>
        /// <returns>TypeCode that maps to the dataTypeString</returns>
        public static TypeCode GetDataType(string dataTypeString)
        {
            try
            {
                switch (dataTypeString.ToLower())
                {
                    // todo: isn't there a better way for guid?
                    case m_GUID:
                        return TypeCode.Object;

                    case m_Boolean1:
                        return TypeCode.Boolean;

                    case m_Boolean2:
                        return TypeCode.Boolean;

                    case m_Byte:
                        return TypeCode.Byte;

                    case m_Char:
                        return TypeCode.Char;

                    case m_DateTime:
                        return TypeCode.DateTime;

                    case m_DBNull:
                        return TypeCode.DBNull;

                    case m_Decimal:
                        return TypeCode.Decimal;

                    case m_Double:
                        return TypeCode.Double;

                    case m_Empty:
                        return TypeCode.Empty;

                    case m_Int16_1:
                        return TypeCode.Int16;

                    case m_Int16_2:
                        return TypeCode.Int16;

                    case m_Int32_1:
                        return TypeCode.Int32;

                    case m_Int32_2:
                        return TypeCode.Int32;

                    case m_Int32_3:
                        return TypeCode.Int32;

                    case m_Int64_1:
                        return TypeCode.Int64;

                    case m_Int64_2:
                        return TypeCode.Int64;

                    case m_Object:
                        return TypeCode.Object;

                    case m_SByte:
                        return TypeCode.SByte;

                    case m_Single:
                        return TypeCode.Single;

                    case m_String:
                        return TypeCode.String;

                    case m_UInt16:
                        return TypeCode.UInt16;

                    case m_UInt32:
                        return TypeCode.UInt32;

                    case m_UInt64:
                        return TypeCode.UInt64;

                    default:
                        return TypeCode.Empty;
                }
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return TypeCode.Empty;
            }
        }

        #endregion MethodsThatCheckDataType

        #region StringConversions

        /// <summary>
        /// Returns a date, as coerced from the string argument. Will raise
        /// an error, if the string cannot be coerced
        /// ----so, use IsDate in this same class FIRST
        /// </summary>
        /// <param name="stream">string to get date value from</param>
        /// <returns>a DateTime object</returns>
        public static DateTime GetDate(string stream)
        {
            DateTime dateValue = new DateTime();
            try
            {
                dateValue = DateTime.Parse(stream);
                return dateValue;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return dateValue;
            }
        }

        /// <summary>
        /// Returns an int, as coerced from the string argument.
        /// Will raise an error, if the string cannot be coerced
        /// ----so, use IsNumber in this same class FIRST
        /// </summary>
        /// <param name="stream">string to get int value from</param>
        /// <returns>an int object</returns>
        public static int GetInteger(string stream)
        {
            try
            {
                int number = 0;
                if (!IsNumber(stream))
                    return number;
                //still here? check to see if it is formatted:
                if (IsFormattedNumber(stream))
                {
                    //it's formatted; replace the format characters
                    //with nothing (retain the decimal so as not to change
                    //the intended value
                    stream = stream.Replace(m_Comma, string.Empty);
                    stream = stream.Replace(m_DollarSign, string.Empty);
                    stream = stream.Replace(m_PercentSign, string.Empty);
                }
                //we've removed superfluous formatting characters, if they
                //did exist, now let's round it/convert it, and return it:
                number = Convert.ToInt32(stream);
                return number;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return 0;
            }
        }

        /// <summary>
        /// Returns a double, as coerced from the string argument.
        /// Will raise an error, if the string cannot be coerced
        /// ----so, use IsNumber in this same class FIRST
        /// </summary>
        /// <param name="stream">string to get double value from</param>
        /// <returns>a double object</returns>
        public static double GetDouble(string stream)
        {
            try
            {
                double number = 0;
                if (!IsNumber(stream))
                    return number;
                //still here? check to see if it is formatted:
                if (IsFormattedNumber(stream))
                {
                    //it's formatted; replace the format characters
                    //with nothing (retain the decimal so as not to change
                    //the intended value)
                    stream = stream.Replace(m_Comma, string.Empty);
                    stream = stream.Replace(m_DollarSign, string.Empty);
                    stream = stream.Replace(m_PercentSign, string.Empty);
                }

                //we've removed superfluous formatting characters, if they
                //did exist, now let's round it/convert it, and return it:
                number = Convert.ToDouble(stream);
                return number;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return 0;
            }
        }

        #endregion StringConversions

        #region StringEdits

        /// <summary>
        /// Iterates thru an entire string, and sets the first letter of
        /// each word to uppercase, and all ensuing letters to lowercase
        /// </summary>
        /// <param name="stream">The string to alter the case of</param>
        /// <returns>
        /// Same string w/uppercase initial letters & others as lowercase
        /// </returns>
        public static string MixCase(string stream)
        {
            try
            {
                string newString = string.Empty;
                string character = string.Empty;
                string preceder = string.Empty;
                for (int i = 0; i < stream.Length; i++)
                {
                    character = stream.Substring(i, 1);
                    if (i > 0)
                    {
                        //look at the character immediately before current
                        preceder = stream.Substring(i - 1, 1);
                        //remove white space character from predecessor
                        if (preceder.Trim() == string.Empty)
                            //the preceding character WAS white space, so
                            //we'll change the current character to UPPER
                            character = character.ToUpper();
                        else
                            //the preceding character was NOT white space,
                            //we'll force the current character to LOWER
                            character = character.ToLower();
                    }
                    else
                        //index is 0, thus we are at the first character
                        character = character.ToUpper();
                    //add the altered character to the new string:
                    newString += character;
                }
                return newString;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        /// <summary>
        /// Iterates thru a string, and removes anything set to clean.
        /// Except---Does NOT remove anything in exceptionsToAllow
        /// </summary>
        /// <param name="stream">
        /// The string to clean</param>
        /// <returns>
        /// The same string, missing all elements that were set to clean
        /// (except when a character was listed in exceptionsToAllow)
        /// </returns>
        public static string Clean(this string stream, bool cleanWhiteSpace,
            bool cleanDigits, bool cleanLetters, string exceptionsToAllow, bool cleanSpecial)
        {
            try
            {
                string newString = string.Empty;
                string character = string.Empty;
                string blessed = string.Empty;
                if (!cleanDigits)
                    blessed += m_Digits;

                if (!cleanLetters)
                    blessed += m_Letters;

                if (!cleanSpecial)
                    blessed += m_special;

                blessed += exceptionsToAllow;
                //we set the comparison string to lower
                //and will compare each character's lower case version
                //against the comparison string, without
                //altering the original case of the character
                blessed = blessed.ToLower();
                for (int i = 0; i < stream.Length; i++)
                {
                    character = stream.Substring(i, 1);
                    if (blessed.Contains(character.ToLower(new CultureInfo("en-US"))))
                        //add the altered character to the new string:
                        newString += character;
                    else if (character.Trim() == string.Empty &&
                    !cleanWhiteSpace)
                        newString += character;
                }
                return newString;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        #endregion StringEdits

        #region StringLocators

        /// <summary>
        /// Parses a file system or url path, and locates the file name
        /// </summary>
        /// <param name="fullPath">
        /// String indicating a file system or url path to a file
        /// </param>
        /// <param name="includeFileExtension">
        /// Whether to return file extension in addition to file name
        /// </param>
        /// <returns>
        /// File name, if found, and extension if requested, and located
        /// </returns>
        public static string GetFileNameFromPath(string fullPath,
            bool includeFileExtension)
        {
            try
            {
                bool url = fullPath.Contains(m_ForwardSlash);
                string search = string.Empty;
                if (url)
                    search = m_ForwardSlash;
                else
                    search = m_BackSlash;
                string portion = string.Empty;

                int decimals = GetKeyCharCount(fullPath, m_Period);
                if (decimals >= 1)
                    //get all text to the RIGHT of the LAST slash:
                    portion = GetExactPartOfString(fullPath, search, false,
                        false, false);
                else
                    return string.Empty;

                if (includeFileExtension)
                    return portion;
                search = m_Period;
                portion = GetExactPartOfString(portion, search, false,
                    true, false);
                return portion;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        /// <summary>
        /// Parses a url or file stream string, to get and return the
        /// path portion (sans the file name and extension)
        /// </summary>
        /// <param name="fullPath">
        /// A string indicating a file system path or a url. Can
        /// contain a file name/extension.
        /// </param>
        /// <returns>
        /// The original path minus the file name and extension,
        /// if it had existed, with no extension will return
        /// the original string, plus an optional slash
        /// </returns>
        public static string GetFolderPath(string fullPath)
        {
            try
            {
                bool url = fullPath.Contains(m_ForwardSlash);
                string slash = string.Empty;
                if (url)
                    slash = m_ForwardSlash;
                else
                    slash = m_BackSlash;

                string fileName = GetFileNameFromPath(fullPath, true);
                //use tool to return all text to the LEFT of the file name
                string portion = GetStringBetween(fullPath, string.Empty,
                    fileName);

                //add the pertinent slash to the end of the string;
                if (portion.Length > 0 && portion.Substring(
                    portion.Length - 1, 1) != slash)
                    portion += slash;
                return portion;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        /// <summary>
        /// Useful to pinpoint exact string between whatever
        /// characters/string you wish to grab text from
        /// </summary>
        /// <param name="stream">
        /// string from which to cull subtext from
        /// </param>
        /// <param name="from">
        /// string that precedes the text you are looking for
        /// </param>
        /// <param name="to">
        /// string that follows the text you are looking for
        /// </param>
        /// <returns>
        /// The string between point x and point y
        /// </returns>
        public static string GetStringBetween(string stream, string from,
            string to)
        {
            try
            {
                string subField = string.Empty;
                subField = RightOf(stream, from);
                subField = LeftOf(subField, to);
                return subField;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        /// <summary>
        /// Will return the text to the LEFT of indicated substring
        /// </summary>
        /// <param name="stream">
        /// string from which to cull a portion of text
        /// </param>
        /// <param name="stringToStopAt">
        /// string that indicates what char or string to stop at
        /// </param>
        /// <returns>
        /// The string to the left of point x (stringToStopAt)
        /// </returns>
        public static string LeftOf(string stream, string stringToStopAt)
        {
            try
            {
                if (stringToStopAt == null || stringToStopAt == string.Empty)
                    return stream;

                int stringLength = stream.Length;
                int findLength = stringToStopAt.Length;

                stringToStopAt = stringToStopAt.ToLower();
                string temp = stream.ToLower();
                int i = temp.IndexOf(stringToStopAt);

                if ((i <= -1) && (stringToStopAt != temp.Substring(0, findLength))
                || (i == -1))
                    return stream;

                string result = stream.Substring(0, i);
                return result;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        /// <summary>
        /// Will return the text to the RIGHT of whatever substring you indicate
        /// </summary>
        /// <param name="stream">
        /// string from which to cull a portion of text
        /// </param>
        /// <param name="stringToStartAfter">
        /// string that indicates what char or string to start after
        /// </param>
        /// <returns>
        /// The string to the right of point x (stringToStartAfter)
        /// </returns>
        public static string RightOf(string stream, string stringToStartAfter)
        {
            try
            {
                if (stringToStartAfter == null || stringToStartAfter == string.Empty)
                    return stream;
                stringToStartAfter = stringToStartAfter.ToLower();
                string temp = stream.ToLower();
                int findLength = stringToStartAfter.Length;
                int i = temp.IndexOf(stringToStartAfter);
                if ((i <= -1) && (stringToStartAfter != temp.Substring(0, findLength))
                || (i == -1))
                    return stream;

                string result =
                stream.Substring(i + findLength, stream.Length - (i + findLength));
                return result;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        /// <summary>
        /// Searches a string for every single instance of the passed-in
        /// field delimiters, and returns all the values between those
        /// delimiters, as a List object
        /// </summary>
        /// <param name="streamToSearch">string to search</param>
        /// <param name="leftFieldDelimiter">string to start at</param>
        /// <param name="rightFieldDelimiter">string to stop at</param>
        /// <returns>A List object of strings</returns>
        public static List<string> GetEachFieldValue(string streamToSearch,
            string leftFieldDelimiter, string rightFieldDelimiter)
        {
            string search = streamToSearch;
            string field = string.Empty;
            List<string> fields = new List<string>();
            while (!string.IsNullOrEmpty(search)
            && search.Contains(leftFieldDelimiter)
            && search.Contains(rightFieldDelimiter))
            {
                //get the val and add to list
                field = GetStringBetween(search, leftFieldDelimiter,
                    rightFieldDelimiter);
                if (!string.IsNullOrEmpty(field))
                    fields.Add(field);
                //shorten the search string and continue
                search = RightOf(search, field + rightFieldDelimiter);
            }
            return fields;
        }

        /// <summary>
        /// Instructions on using arguments:
        /// Set firstInstance = true, to stop at first instance of locateChar
        /// If firstInstance = false, then the LAST instance of locateChar will be used
        /// Set fromLeft = true, to return string from the left of locateChar
        /// If fromLeft = false, then the string from the right of locateChar
        /// will be returned.
        /// Set caseSensitive to true/false for case-sensitivity
        /// EXAMPLES:
        /// GetPartOfString('aunt jemima', 'm', 'true', 'true')
        /// will return 'aunt je'
        /// GetPartOfString('aunt jemima', 'm', 'true', 'false')
        /// </summary>
        /// <param name="stream">
        /// The string from which to cull a portion of text
        /// </param>
        /// <param name="locateChar">
        /// The character or string that is the marker
        /// for which to grab text (from left or right depending
        /// on other argument)
        /// </param>
        /// <param name="firstInstance">
        /// Whether or not to get the substring from the first
        /// encountered instance of the locateChar argument
        /// </param>
        /// <param name="fromLeft">
        /// Whether to search from the left. If set to false,
        /// then the string will be searched from the right.
        /// </param>
        /// <param name="caseSensitive">
        /// Whether to consider case (upper/lower)
        /// </param>
        /// <returns>
        /// A portion of the input string, based on ensuing arguments
        /// </returns>
        public static string GetExactPartOfString(string stream, string locateChar,
                  bool firstInstance, bool fromLeft, bool caseSensitive)
        {
            try
            {
                stream = stream.ToString();
                string tempStream = string.Empty;
                string tempLocateChar = string.Empty;
                if (!caseSensitive)
                { //case doesn't matter, convert to lower:
                    tempStream = stream.ToLower();
                    tempLocateChar = locateChar.ToLower();
                }
                //default charCnt to 1; for first inst of locateChar:
                int charCount = 1;
                if (firstInstance == false)
                    //get number of times char exists in string:
                    if (caseSensitive)
                        charCount = GetKeyCharCount(stream, locateChar);
                    else
                        charCount = GetKeyCharCount(tempStream, tempLocateChar);
                //get position of first/last inst of char in str:
                int position = 0;
                if (caseSensitive)
                    position = GetCharPosition(stream, locateChar, charCount);
                else
                    position = GetCharPosition(tempStream, tempLocateChar, charCount);
                string result = string.Empty;
                //chk that character exists in str:
                if (position == -1)
                    result = string.Empty;
                else
                {
                    //char exists, proceed:
                    int streamLength = stream.Length;
                    if (fromLeft == true)
                        //return string from left:
                        result = stream.Substring(0, position);
                    else
                    {
                        //return string from right:
                        position += 1;
                        result = stream.Substring(position, streamLength - position);
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        /// <summary>
        /// Returns the number of times, that the key character is found
        /// in the stream string
        /// </summary>
        /// <param name="stream">
        /// string in which to locate key character
        /// </param>
        /// <param name="keyChar">
        /// key character: the string or char to count inside the stream
        /// </param>
        /// <returns>
        /// The number of times the string or char was located
        /// </returns>
        public static int GetKeyCharCount(string stream, string keyChar)
        {
            try
            {
                string current;
                int keyCount = 0;
                for (int i = 0; i < stream.Length; i++)
                {
                    current = stream.Substring(i, 1);
                    if (current == keyChar)
                        keyCount += 1;
                }
                if (keyCount <= 0)
                    return -1;
                else
                    return keyCount;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return -1;
            }
        }

        /// <summary>
        /// Is CASE-SENSITIVE
        /// Returns x position of sChar in sstream, where x = iCharInst.
        /// If: getCharPos('pineapple', 'p', 3) Then: 6 is returned
        /// </summary>
        /// <param name="stream">
        /// string in which to pinpoint the character (or string) position
        /// </param>
        /// <param name="charToPinpoint">character or string to locate</param>
        /// <param name="whichCharInstance">
        /// Number indicating WHICH instance of the character/string to locate
        /// </param>
        /// <returns>
        /// The index of the character or string found inside the input string.
        /// Will return -1 if the string/character is not found, or if the
        /// instance number is not found
        /// </returns>
        public static int GetCharPosition(string stream, string charToPinpoint, int whichCharInstance)
        {
            try
            {
                string current;
                int keyCharCount = 0;
                for (int i = 0; i < stream.Length; i++)
                {
                    current = stream.Substring(i, 1);
                    //was BLOCKED SCRIPT sCurr = sstream.charAt(i);
                    if (current == charToPinpoint)
                    {
                        keyCharCount += 1;
                        if (keyCharCount == whichCharInstance)
                            return i;
                    }
                }
                return -1;
            }
            catch (System.Exception ex)
            {
                //ErrorTool.ProcessError(ex);
                return -1;
            }
        }

        #endregion StringLocators

        
    }
}