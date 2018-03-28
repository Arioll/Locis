using System;

namespace UserMangment.Domain.EmailOperations.Models
{
    public class KeyInfo
    {
        public KeyInfo(string keyValue, DateTime timeOfCreate)
        {
            KeyValue = keyValue;
            TimeOfCreate = timeOfCreate;
        }

        public string KeyValue { get; }
        public DateTime TimeOfCreate { get; }
    }
}
