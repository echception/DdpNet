namespace DdpNet.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    internal class ObjectChanger
    {
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

                    if (property != null)
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

        private PropertyInfo FindProperty(string fieldName, object change)
        {
            Type changeType = change.GetType();

            var properties = changeType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                if (property.Name == fieldName)
                {
                    return property;
                }

                var attribute = property.GetCustomAttribute(typeof(JsonPropertyAttribute));

                if (attribute != null)
                {
                    var propertyAttribute = (JsonPropertyAttribute)attribute;

                    if (propertyAttribute.PropertyName == fieldName)
                    {
                        return property;
                    }
                }
            }

            return null;
        }
    }
}
