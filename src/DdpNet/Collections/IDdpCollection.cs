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
        void Add(string id, JObject jObject);
    }
}
