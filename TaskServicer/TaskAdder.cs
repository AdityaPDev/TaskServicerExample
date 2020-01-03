using System;
using System.Linq;
using System.Threading;
using Serilog;

namespace TaskServicer
{
    public class TaskAdder
    {
        public TaskQueue _taskQueue;

        public TaskAdder(TaskQueue taskQueue) 
        { 
            _taskQueue = taskQueue; 
        }

        /// <summary>
        /// Adds the tasks.
        /// </summary>
        public void AddTasks(int numTasks)
        {
            Random random = new Random();
            //for (int i = 1; i <= 180; i++)
            for (int i = 1; i <= numTasks; i++)
            {
                //if (i == 100)
                //{
                //    Thread.Sleep(5000);
                //    Console.WriteLine("pausing the Task addition for some 5 seconds ");
                //    Log.Debug("Pausing the Task addition for some 5 seconds ... ");
                //    //Console.WriteLine("Please hit ENTER to add some more tasks");
                //    //string str = Console.ReadLine();

                //}

                // Prepare Queue Object (Hold the Test Data)
                var queue = new QueuedObject
                {
                    QueueID = i,
                    AdderThreadID = Thread.CurrentThread.ManagedThreadId,
                    EnqueueDateTime = DateTime.Now,
                    // Used to Generate Random String
                    RandomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray())
                };

                // Add Task to Queue with Action
                _taskQueue.EnqueueTask(() => { ReverseString(queue); });
                Console.WriteLine
                    (
                    "Enqueued: " + queue.QueueID +
                    "\t" + "Adder ThreadID :" + queue.AdderThreadID +
                    "\t" + queue.EnqueueDateTime.ToLongTimeString() +
                    "\t" + "RandomString   :" + queue.RandomString
                    );
                Log.Debug(
                    "Enqueued: " + queue.QueueID +
                    "Adder ThreadID :" + queue.AdderThreadID
                    );
            }
        }
        /// <summary>
        /// Reverses the string. (This is an Associated Action with Task)
        /// </summary>
        /// <param name="queue">The queue.</param>
        public void ReverseString(QueuedObject queue)
        {
            Log.Debug(
                    $"\t INSIDE::ReverseString - start, for Task#{queue.QueueID} " +
                    $"input string is {queue.RandomString}"
                    );
            
            string reversedString = new string(queue.RandomString.Reverse().ToArray());
          
                Console.WriteLine
                (
                "Dequeued: " + queue.QueueID +
                "\t" + "Servicer ThreadID :" + Thread.CurrentThread.ManagedThreadId +
                "\t" + DateTime.Now.ToLongTimeString() +
                "\t" + "ReversedString :" + reversedString
                );
            Log.Debug(
                "Dequeued-After: " + queue.QueueID +
                "Servicer ThreadID :" + Thread.CurrentThread.ManagedThreadId
                );

            // Add some delay at servicer, slow down servicer just a little 
            Thread.Sleep(100);
            Log.Debug(
                    $"\t INSIDE::ReverseString - end, for Task#{queue.QueueID} "
                    );

        }

    }
}
