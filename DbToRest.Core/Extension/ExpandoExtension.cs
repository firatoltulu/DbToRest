﻿using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DbToRest.Core
{
    public static class ExpandoExtension
    {
        /// <summary>
        /// Provides a ToExpando extension method.
        /// </summary>

        /// <summary>
        /// Makes a ExpandoObject out of the given object.
        /// </summary>
        /// <param name="value">Object to become ExpandoObject</param>
        /// <returns>The ExpandoObject made</returns>
        public static ExpandoObject ToExpando(this object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var expando = value as ExpandoObject;

            if (expando == null)
            {
                expando = new ExpandoObject();
                var dict = (IDictionary<string, object>)expando;

                foreach (var kvp in DictionaryMaker.Make(value))
                    dict.Add(kvp);
            }

            return expando;
        }

        /// <summary>
        /// Removes the property of the given ExpandoObject.
        /// </summary>
        /// <param name="expando">Object to remove property from</param>
        /// <param name="propertyName">Property name to be removed</param>
        public static void RemoveProperty(this ExpandoObject expando, string propertyName)
        {
            if (expando == null)
                throw new ArgumentNullException("expando");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            ((IDictionary<string, object>)expando).Remove(propertyName);
        }

        /// <summary>
        /// Checks if the property exists in the given ExpandoObject.
        /// </summary>
        /// <param name="expando">Object to check if the property exists</param>
        /// <param name="propertyName">Property name to check</param>
        /// <returns>True if the property exists in the given ExpandoObject, false otherwise</returns>
        public static bool HasProperty(this ExpandoObject expando, string propertyName)
        {
            if (expando == null)
                throw new ArgumentNullException("expando");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            return ((IDictionary<string, object>)expando).ContainsKey(propertyName);
        }

        /// <summary>
        /// Checks if the property exists in the given ExpandoObject.
        /// </summary>
        /// <param name="expando">Object to check if the property exists</param>
        /// <param name="propertyName">Property name to check</param>
        /// <returns>True if the property exists in the given ExpandoObject, false otherwise</returns>
        public static void Add(this ExpandoObject expando, string propertyName, object value)
        {
            if (expando == null)
                throw new ArgumentNullException("expando");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            if (expando.HasProperty(propertyName) == false)
                ((IDictionary<string, object>)expando).Add(propertyName, value);
            else
                ((IDictionary<string, object>)expando)[propertyName] = value;
        }

        /// <summary>
        /// Checks if the property exists in the given ExpandoObject.
        /// </summary>
        /// <param name="expando">Object to check if the property exists</param>
        /// <param name="propertyName">Property name to check</param>
        /// <returns>True if the property exists in the given ExpandoObject, false otherwise</returns>
        public static void AddIfExists(this ExpandoObject expando, string propertyName, object value)
        {
            if (expando.HasProperty(propertyName) == false)
                ((IDictionary<string, object>)expando).Add(propertyName, value);
        }

        /// <summary>
        /// Makes a shallow copy of the given object.
        /// </summary>
        /// <param name="expando">Object to be copied</param>
        /// <returns>A shallow copy of the given object</returns>
        public static ExpandoObject ShallowCopy(this ExpandoObject expando)
        {
            return Copy(expando, false);
        }

        /// <summary>
        /// Makes a deep copy of the given object
        /// </summary>
        /// <param name="expando">Object to be copied</param>
        /// <returns>A deep copy of the given object</returns>
        public static ExpandoObject DeepCopy(this ExpandoObject expando)
        {
            return Copy(expando, true);
        }

        private static ExpandoObject Copy(ExpandoObject original, bool deep)
        {
            var clone = new ExpandoObject();

            var _original = (IDictionary<string, object>)original;
            var _clone = (IDictionary<string, object>)clone;

            foreach (var kvp in _original)
                _clone.Add(
                    kvp.Key,
                    deep && kvp.Value is ExpandoObject ? DeepCopy((ExpandoObject)kvp.Value) : kvp.Value
                );

            return clone;
        }
    }
}