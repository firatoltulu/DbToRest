using Fasterflect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DbToRest.Core.Infrastructure.ComponentModel
{
    internal static class SerializationUtils
    {
        /// <summary>
        /// Serializes an object instance to a file.
        /// </summary>
        /// <param name="instance">the object instance to serialize</param>
        /// <param name="fileName"></param>
        /// <param name="binarySerialization">determines whether XML serialization or binary serialization is used</param>
        /// <returns></returns>
        public static bool SerializeObject(object instance, string fileName, bool binarySerialization)
        {
            bool retVal = true;

            if (!binarySerialization)
            {
                XmlTextWriter writer = null;
                try
                {
                    XmlSerializer serializer =
                        new XmlSerializer(instance.GetType());

                    // Create an XmlTextWriter using a FileStream.
                    Stream fs = new FileStream(fileName, FileMode.Create);
                    writer = new XmlTextWriter(fs, new UTF8Encoding());
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = ' ';
                    writer.Indentation = 3;

                    // Serialize using the XmlTextWriter.
                    serializer.Serialize(writer, instance);
                }
                catch (System.Exception ex)
                {
                    Debug.Write("SerializeObject failed with : " + ex.Message, "West Wind");
                    retVal = false;
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
            else
            {
                Stream fs = null;
                try
                {
                    BinaryFormatter serializer = new BinaryFormatter();
                    fs = new FileStream(fileName, FileMode.Create);
                    serializer.Serialize(fs, instance);
                }
                catch
                {
                    retVal = false;
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }

            return retVal;
        }

        /// <summary>
        /// Overload that supports passing in an XML TextWriter. 
        /// </summary>
        /// <remarks>
        /// Note the Writer is not closed when serialization is complete 
        /// so the caller needs to handle closing.
        /// </remarks>
        /// <param name="instance">object to serialize</param>
        /// <param name="writer">XmlTextWriter instance to write output to</param>       
        /// <param name="throwExceptions">Determines whether false is returned on failure or an exception is thrown</param>
        /// <returns></returns>
        public static bool SerializeObject(object instance, XmlTextWriter writer, bool throwExceptions)
        {
            bool retVal = true;

            try
            {
                XmlSerializer serializer =
                    new XmlSerializer(instance.GetType());

                // Create an XmlTextWriter using a FileStream.
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = ' ';
                writer.Indentation = 3;

                // Serialize using the XmlTextWriter.
                serializer.Serialize(writer, instance);
            }
            catch (System.Exception ex)
            {
                Debug.Write("SerializeObject failed with : " + ex.GetBaseException().Message + "\r\n" + (ex.InnerException != null ? ex.InnerException.Message : ""), "West Wind");

                if (throwExceptions)
                    throw;

                retVal = false;
            }

            return retVal;
        }


        /// <summary>
        /// Serializes an object into an XML string variable for easy 'manual' serialization
        /// </summary>
        /// <param name="instance">object to serialize</param>
        /// <param name="xmlResultString">resulting XML string passed as an out parameter</param>
        /// <returns>true or false</returns>
        public static bool SerializeObject(object instance, out string xmlResultString)
        {
            return SerializeObject(instance, out xmlResultString, false);
        }

        /// <summary>
        /// Serializes an object into a string variable for easy 'manual' serialization
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="xmlResultString">Out parm that holds resulting XML string</param>
        /// <param name="throwExceptions">If true causes exceptions rather than returning false</param>
        /// <returns></returns>
        public static bool SerializeObject(object instance, out string xmlResultString, bool throwExceptions)
        {
            xmlResultString = string.Empty;
            MemoryStream ms = new MemoryStream();

            XmlTextWriter writer = new XmlTextWriter(ms, new UTF8Encoding());

            if (!SerializeObject(instance, writer, throwExceptions))
            {
                ms.Close();
                return false;
            }

            xmlResultString = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);

            ms.Close();
            writer.Close();

            return true;
        }


        /// <summary>
        /// Serializes an object instance to a file.
        /// </summary>
        /// <param name="instance">the object instance to serialize</param>
        /// <param name="Filename"></param>
        /// <param name="BinarySerialization">determines whether XML serialization or binary serialization is used</param>
        /// <returns></returns>
        public static bool SerializeObject(object instance, out byte[] resultBuffer, bool throwExceptions = false)
        {
            bool retVal = true;

            MemoryStream ms = null;
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                ms = new MemoryStream();
                serializer.Serialize(ms, instance);
            }
            catch (System.Exception ex)
            {
                Debug.Write("SerializeObject failed with : " + ex.GetBaseException().Message, "West Wind");
                retVal = false;

                if (throwExceptions)
                    throw;
            }
            finally
            {
                if (ms != null)
                    ms.Close();
            }

            resultBuffer = ms.ToArray();

            return retVal;
        }

        /// <summary>
        /// Serializes an object to an XML string. Unlike the other SerializeObject overloads
        /// this methods *returns a string* rather than a bool result!
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="throwExceptions">Determines if a failure throws or returns null</param>
        /// <returns>
        /// null on error otherwise the Xml String.         
        /// </returns>
        /// <remarks>
        /// If null is passed in null is also returned so you might want
        /// to check for null before calling this method.
        /// </remarks>
        public static string SerializeObjectToString(object instance, bool throwExceptions = false)
        {
            string xmlResultString = string.Empty;

            if (!SerializeObject(instance, out xmlResultString, throwExceptions))
                return null;

            return xmlResultString;
        }

        public static byte[] SerializeObjectToByteArray(object instance, bool throwExceptions = false)
        {
            byte[] byteResult = null;

            if (!SerializeObject(instance, out byteResult))
                return null;

            return byteResult;
        }



        /// <summary>
        /// Deserializes an object from file and returns a reference.
        /// </summary>
        /// <param name="fileName">name of the file to serialize to</param>
        /// <param name="objectType">The Type of the object. Use typeof(yourobject class)</param>
        /// <param name="binarySerialization">determines whether we use Xml or Binary serialization</param>
        /// <returns>Instance of the deserialized object or null. Must be cast to your object type</returns>
        public static object DeSerializeObject(string fileName, Type objectType, bool binarySerialization)
        {
            return DeSerializeObject(fileName, objectType, binarySerialization, false);
        }

        /// <summary>
        /// Deserializes an object from file and returns a reference.
        /// </summary>
        /// <param name="fileName">name of the file to serialize to</param>
        /// <param name="objectType">The Type of the object. Use typeof(yourobject class)</param>
        /// <param name="binarySerialization">determines whether we use Xml or Binary serialization</param>
        /// <param name="throwExceptions">determines whether failure will throw rather than return null on failure</param>
        /// <returns>Instance of the deserialized object or null. Must be cast to your object type</returns>
        public static object DeSerializeObject(string fileName, Type objectType, bool binarySerialization, bool throwExceptions)
        {
            object instance = null;

            if (!binarySerialization)
            {

                XmlReader reader = null;
                XmlSerializer serializer = null;
                FileStream fs = null;
                try
                {
                    // Create an instance of the XmlSerializer specifying type and namespace.
                    serializer = new XmlSerializer(objectType);

                    // A FileStream is needed to read the XML document.
                    fs = new FileStream(fileName, FileMode.Open);
                    reader = new XmlTextReader(fs);

                    instance = serializer.Deserialize(reader);
                }
                catch (System.Exception ex)
                {
                    if (throwExceptions)
                        throw;

                    string message = ex.Message;
                    return null;
                }
                finally
                {
                    if (fs != null)
                        fs.Close();

                    if (reader != null)
                        reader.Close();
                }
            }
            else
            {

                BinaryFormatter serializer = null;
                FileStream fs = null;

                try
                {
                    serializer = new BinaryFormatter();
                    fs = new FileStream(fileName, FileMode.Open);
                    instance = serializer.Deserialize(fs);

                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }

            return instance;
        }

        /// <summary>
        /// Deserialize an object from an XmlReader object.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static object DeSerializeObject(XmlReader reader, Type objectType)
        {
            XmlSerializer serializer = new XmlSerializer(objectType);
            object Instance = serializer.Deserialize(reader);
            reader.Close();

            return Instance;
        }

        public static object DeSerializeObject(string xml, Type objectType)
        {
            XmlTextReader reader = new XmlTextReader(xml, XmlNodeType.Document, null);
            return DeSerializeObject(reader, objectType);
        }

        /// <summary>
        /// Deseializes a binary serialized object from  a byte array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="objectType"></param>
        /// <param name="throwExceptions"></param>
        /// <returns></returns>
        public static object DeSerializeObject(byte[] buffer, Type objectType, bool throwExceptions = false)
        {
            BinaryFormatter serializer = null;
            MemoryStream ms = null;
            object Instance = null;

            try
            {
                serializer = new BinaryFormatter();
                ms = new MemoryStream(buffer);
                Instance = serializer.Deserialize(ms);

            }
            catch
            {
                if (throwExceptions)
                    throw;

                return null;
            }
            finally
            {
                if (ms != null)
                    ms.Close();
            }

            return Instance;
        }


        /// <summary>
        /// Returns a string of all the field value pairs of a given object.
        /// Works only on non-statics.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ObjectToString(object instance, string separator, ObjectToStringTypes type)
        {
            var fi = instance.GetType().Fields();

            string output = string.Empty;

            if (type == ObjectToStringTypes.Properties || type == ObjectToStringTypes.PropertiesAndFields)
            {
                foreach (var property in instance.GetType().Properties())
                {
                    try
                    {
                        output += property.Name + ":" + instance.GetPropertyValue(property.Name).ToString() + separator;
                    }
                    catch
                    {
                        output += property.Name + ": n/a" + separator;
                    }
                }
            }

            if (type == ObjectToStringTypes.Fields || type == ObjectToStringTypes.PropertiesAndFields)
            {
                foreach (var field in fi)
                {
                    try
                    {
                        output = output + field.Name + ": " + instance.GetFieldValue(field.Name).ToString() + separator;
                    }
                    catch
                    {
                        output = output + field.Name + ": n/a" + separator;
                    }
                }
            }
            return output;
        }

    }

    public enum ObjectToStringTypes
    {
        Properties,
        PropertiesAndFields,
        Fields
    }
    [XmlRoot("properties")]
    public class ExpondoProperties : ExpondoProperties<object>
    {
        /// <summary>
        /// Creates an instance of a propertybag from an Xml string
        /// </summary>
        /// <param name="xml">Serialize</param>
        /// <returns></returns>
        public new static ExpondoProperties CreateFromXml(string xml)
        {
            var bag = new ExpondoProperties();
            bag.FromXml(xml);
            return bag;
        }
    }


    [XmlRoot("properties")]
    public class ExpondoProperties<TValue> : Dictionary<string, TValue>, IXmlSerializable
    {
        #region Nested TypeUtils class

        private static class TypeUtils
        {
            /// <summary>
            /// Helper routine that looks up a type name and tries to retrieve the
            /// full type reference in the actively executing assemblies.
            /// </summary>
            /// <param name="typeName"></param>
            /// <returns></returns>
            public static Type GetTypeFromName(string typeName)
            {
                Type type = null;

                // try to find manually
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = ass.GetType(typeName, false);

                    if (type != null)
                        break;
                }
                return type;
            }

            /// <summary>
            /// Converts a .NET type into an XML compatible type - roughly
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public static string MapTypeToXmlType(Type type)
            {
                if (type == typeof(string) || type == typeof(char))
                    return "string";
                if (type == typeof(int) || type == typeof(Int32))
                    return "integer";
                if (type == typeof(long) || type == typeof(Int64))
                    return "long";
                if (type == typeof(bool))
                    return "boolean";
                if (type == typeof(DateTime))
                    return "datetime";

                if (type == typeof(float))
                    return "float";
                if (type == typeof(decimal))
                    return "decimal";
                if (type == typeof(double))
                    return "double";
                if (type == typeof(Single))
                    return "single";

                if (type == typeof(byte))
                    return "byte";

                if (type == typeof(byte[]))
                    return "base64Binary";

                return null;

                // *** hope for the best
                //return type.ToString().ToLower();
            }

            public static Type MapXmlTypeToType(string xmlType)
            {
                xmlType = xmlType.ToLower();

                if (xmlType == "string")
                    return typeof(string);
                if (xmlType == "integer")
                    return typeof(int);
                if (xmlType == "long")
                    return typeof(long);
                if (xmlType == "boolean")
                    return typeof(bool);
                if (xmlType == "datetime")
                    return typeof(DateTime);
                if (xmlType == "float")
                    return typeof(float);
                if (xmlType == "decimal")
                    return typeof(decimal);
                if (xmlType == "double")
                    return typeof(Double);
                if (xmlType == "single")
                    return typeof(Single);

                if (xmlType == "byte")
                    return typeof(byte);
                if (xmlType == "base64binary")
                    return typeof(byte[]);

                // return null if no match is found
                // don't throw so the caller can decide more efficiently what to do
                // with this error result
                return null;
            }
        }

        #endregion Nested TypeUtils class

        /// <summary>
        /// Not implemented - this means no schema information is passed
        /// so this won't work with ASMX/WCF services.
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Serializes the dictionary to XML. Keys are
        /// serialized to element names and values as
        /// element values. An xml type attribute is embedded
        /// for each serialized element - a .NET type
        /// element is embedded for each complex type and
        /// prefixed with three underscores.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (string key in this.Keys)
            {
                TValue value = this[key];

                Type type = null;
                if (value != null)
                    type = value.GetType();

                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                writer.WriteString(key as string);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                string xmlType = TypeUtils.MapTypeToXmlType(type);
                bool isCustom = false;

                // Type information attribute if not string
                if (value == null)
                {
                    writer.WriteAttributeString("type", "nil");
                }
                else if (!string.IsNullOrEmpty(xmlType))
                {
                    if (xmlType != "string")
                    {
                        writer.WriteStartAttribute("type");
                        writer.WriteString(xmlType);
                        writer.WriteEndAttribute();
                    }
                }
                else
                {
                    isCustom = true;
                    xmlType = "___" + value.GetType().FullName;
                    writer.WriteStartAttribute("type");
                    writer.WriteString(xmlType);
                    writer.WriteEndAttribute();
                }

                // Serialize simple types with WriteValue
                if (!isCustom)
                {
                    if (value != null)
                        writer.WriteValue(value);
                }
                else
                {
                    // Complex types require custom XmlSerializer
                    XmlSerializer ser = new XmlSerializer(value.GetType());
                    ser.Serialize(writer, value);
                }
                writer.WriteEndElement(); // value

                writer.WriteEndElement(); // item
            }
        }

        /// <summary>
        /// Reads the custom serialized format
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            this.Clear();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "key")
                {
                    string xmlType = null;
                    string name = reader.ReadElementContentAsString();

                    // item element
                    reader.ReadToNextSibling("value");

                    if (reader.MoveToNextAttribute())
                        xmlType = reader.Value;
                    if (string.IsNullOrEmpty(xmlType))
                        xmlType = "string";

                    reader.MoveToContent();

                    TValue value;
                    string strval = String.Empty;
                    if (xmlType == "nil")
                        value = default(TValue);   // null

                    // .NET types that don't map to XML we have to manually
                    // deserialize
                    else if (xmlType.StartsWith("___"))
                    {
                        // skip ahead to serialized value element
                        while (reader.Read() && reader.NodeType != XmlNodeType.Element)
                        { }

                        Type type = TypeUtils.GetTypeFromName(xmlType.Substring(3));
                        XmlSerializer ser = new XmlSerializer(type);
                        value = (TValue)ser.Deserialize(reader);
                    }
                    else
                        value = (TValue)reader.ReadElementContentAs(TypeUtils.MapXmlTypeToType(xmlType), null);

                    this.Add(name, value);
                }
            }
        }

        /// <summary>
        /// Serializes this dictionary to an XML string
        /// </summary>
        /// <returns>XML String or Null if it fails</returns>
        public string ToXml()
        {
            string xml = null;
            //SerializationUtils.SerializeObject(this, out xml);
            return xml;
        }

        /// <summary>
        /// Deserializes from an XML string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>true or false</returns>
        public bool FromXml(string xml)
        {
            this.Clear();

            // if xml string is empty we return an empty dictionary
            if (string.IsNullOrEmpty(xml))
                return true;

            var result = SerializationUtils.DeSerializeObject(xml,
                                               this.GetType()) as ExpondoProperties<TValue>;
            if (result != null)
            {
                foreach (var item in result)
                {
                    this.Add(item.Key, item.Value);
                }
            }
            else
                // null is a failure
                return false;

            return true;
        }

        /// <summary>
        /// Creates an instance of a propertybag from an Xml string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static ExpondoProperties<TValue> CreateFromXml(string xml)
        {
            var bag = new ExpondoProperties<TValue>();
            bag.FromXml(xml);
            return bag;
        }
    }
}