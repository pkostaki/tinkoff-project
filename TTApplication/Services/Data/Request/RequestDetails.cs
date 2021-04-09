
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TTApplication.TT
{
    [DataContract]
    public class RequestDetails<T>
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "value")]
        public List<T> Value { get; set; }
    }
}
