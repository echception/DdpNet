namespace DdpNet.Collections
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Similiar to the ObjectChanger, but works on the JToken objects from an untyped collection.
    /// The ObjectChanger uses reflection to change the objects, which does not work on the JTokens.
    /// This uses the JToken specific methods to apply the changes
    /// </summary>
    internal class UntypedObjectChanger
    {
        /// <summary>
        /// Applies changes to a JToken
        /// </summary>
        /// <param name="objectToChange">The object to change</param>
        /// <param name="fields">The fields that changed</param>
        /// <param name="cleared">The fields that were cleared</param>
        public void ChangeObject(JToken objectToChange, Dictionary<string, JToken> fields, string[] cleared)
        {
            if (fields != null)
            {
                foreach (var changedField in fields)
                {
                    var fieldToChange =
                        objectToChange.Children<JProperty>().SingleOrDefault(x => x.Name == changedField.Key);

                    if (fieldToChange != null)
                    {
                        fieldToChange.Value = changedField.Value;
                    }
                }
            }

            if (cleared != null)
            {
                foreach (var clearedField in cleared)
                {
                    var fieldToRemove =
                        objectToChange.Children<JProperty>().SingleOrDefault(x => x.Name == clearedField);

                    if (fieldToRemove != null)
                    {
                        fieldToRemove.Remove();
                    }
                }
            }
        }
    }
}
