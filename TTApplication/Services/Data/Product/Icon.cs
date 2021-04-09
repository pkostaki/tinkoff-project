using System.Runtime.Serialization;

namespace TTApplication.TT
{
    [DataContract]
    public  class Icon
    {
        [DataMember( Name = "text")]
        public string Text { get; set; }

        [DataMember( Name = "name")]
        public string Name { get; set; }
    }
}