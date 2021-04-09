using System.Runtime.Serialization;

namespace TTApplication.TT
{
    [DataContract]
    public  class Benefit
    {
        [DataMember( Name = "icon")]
        public Icon Icon { get; set; }

        [DataMember( Name = "text")]
        public string Text { get; set; }
    }
}