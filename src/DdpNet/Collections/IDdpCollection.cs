namespace DdpNet.Collections
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    internal interface IDdpCollection
    {
        void Added(string id, JObject jObject);
        void Changed(string id, Dictionary<string, JToken> fields, string[] cleared);

        void Removed(string id);
    }
}
