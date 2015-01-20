namespace DdpNet.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// This class uses reflection to change the specified fields or properties on an object.
    /// It is used primarily to handle the 'Changed' message, when an object is modified.
    /// The 'Changed' message contains a diff, so we cannot outright replace the object.
    /// </summary>
    internal class ObjectChanger
    {
        /// <summary>
        /// For the given object, this will set all the fields/properties specified in fields, and clear all fields/properties specified in 'Cleared'
        /// It will match fields/properties with case sensitivity. It will also look at any present JsonProperty attributes to determine property name.
        /// If no matching property is found, the change is skipped.
        /// If there is a matching property, but it doesn't have a public accessor, it is skipped
        /// </summary>
        /// <param name="objectToChange">The object to change</param>
        /// <param name="fields">The fields to set on the object, with their new values</param>
        /// <param name="cleared">The fields/properties to clear</param>
        public void ChangeObject(object objectToChange, Dictionary<string, JToken> fields,
            string[] cleared)
        {
            if (fields != null)
            {
                foreach (var changedField in fields)
                {
                    var field = this.FindField(changedField.Key, objectToChange);

                    if (field != null)
                    {
                        field.SetValue(objectToChange, changedField.Value.ToObject(field.FieldType));
                        continue;
                    }

                    var property = this.FindProperty(changedField.Key, objectToChange);

                    if (property != null && property.GetSetMethod(true) != null)
                    {
                        property.SetValue(objectToChange, changedField.Value.ToObject(property.PropertyType));
                    }
                }
            }

            if (cleared != null)
            {
                foreach (var clearedField in cleared)
                {
                    var field = this.FindField(clearedField, objectToChange);

                    if (field != null)
                    {
                        field.SetValue(objectToChange, null);
                        continue;
                    }

                    var property = this.FindProperty(clearedField, objectToChange);

                    if (property != null)
                    {
                        property.SetValue(objectToChange, null);
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to find a field on the object with the given name. If no field is found, it will look at any
        /// JsonPropertyAttribute, and attempt to match on the PropertyName. 
        /// </summary>
        /// <param name="fieldName">The name of the field to find</param>
        /// <param name="change">The object to find the property on</param>
        /// <returns>The FieldInfo if found, otherwise returns null</returns>
        private FieldInfo FindField(string fieldName, object change)
        {
            Type changeType = change.GetType();

            var fields = changeType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (field.Name == fieldName)
                {
                    return field;
                }

                var attribute = field.GetCustomAttribute(typeof (JsonPropertyAttribute));

                if (attribute != null)
                {
                    var propertyAttribute = (JsonPropertyAttribute) attribute;

                    if (propertyAttribute.PropertyName == fieldName)
                    {
                        return field;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to find a property on the object with the given name. If no property is found, it will look at any
        /// JsonPropertyAttribute, and attempt to match on the PropertyNAme
        /// </summary>
        /// <param name="propertyName">The name of the property to find</param>
        /// <param name="change">The object to find the property on</param>
        /// <returns>The PropertyInfo if found, otherwise returns null</returns>
        private PropertyInfo FindProperty(string propertyName, object change)
        {
            Type changeType = change.GetType();

            var properties = changeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                if (property.Name == propertyName)
                {
                    return property;
                }

                var attribute = property.GetCustomAttribute(typeof(JsonPropertyAttribute));

                if (attribute != null)
                {
                    var propertyAttribute = (JsonPropertyAttribute)attribute;

                    if (propertyAttribute.PropertyName == propertyName)
                    {
                        return property;
                    }
                }
            }

            return null;
        }
    }
}
