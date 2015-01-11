using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdpNet.Collections
{
    using Newtonsoft.Json.Linq;

    internal interface IDdpCollection
    {
        void Added(string id, JObject jObject);
        void Changed(string id, Dictionary<string, JToken> fields, string[] cleared);

        void Removed(string id);
    }
}
