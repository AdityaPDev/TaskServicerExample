using System;

namespace TaskServicer
{
    public class QueuedObject
    {
        public int QueueID { get; set; }
        public int ServicerThreadID { get; set; }
        public int AdderThreadID { get; set; }
        public string RandomString { get; set; }
        public DateTime EnqueueDateTime { get; set; }
        public DateTime DequeueDateTime { get; set; }
     
    }
}
