using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TTApplication.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// Deserialize json string to specific type object
        /// </summary>
        /// <typeparam name="T">type as result of deserializing</typeparam>
        /// <param name="json">json object's presentation</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json) where T:  class, new ()
        {  
            T deserializedUser = default(T);
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                    
                    var obj= ser.ReadObject(ms) ;
                    deserializedUser = obj as T;
                }
            }
            catch(Exception e)
            {
                //do nothing
            }
            
            return deserializedUser;  
        } 

    }
}
