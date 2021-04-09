using System.Runtime.Serialization;

namespace TTApplication.TT
{
    [DataContract]
    public class RequestResult<T>
    {
        [DataMember(Name = "result")]
        public T Result { get; set; }
    }
}