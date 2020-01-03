using Serilog;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TaskServicer
{
    public class TaskProcessingArguments
    {
        public bool ISTaskAdded { get; set; }
        public int PendingTaskCount { get; set; }
        public string Message { get; set; }
    }

    // BlockingCollection - TaskQueue
    public class TaskQueue
    {
        static Mutex _mut = new Mutex();

        public BlockingCollection<Task> _workTaskQueue;
        
        public TaskQueue(IProducerConsumerCollection<Task> workTaskCollection, int QueueSize, int timeout)
        {
            _workTaskQueue = new BlockingCollection<Task>(workTaskCollection);
        }

        /// Enqueues the task.
        /// Add task to the Queue
        /// <param name="action">The action.</param>
        /// <param name="cancelToken">The cancel token.</param>
        public void EnqueueTask(Action action, CancellationToken cancelToken = default(CancellationToken))
        {
            var task = new Task(action, cancelToken);
            try
            {
                if (_workTaskQueue.TryAdd(task))
                {
                    Log.Debug("TryAdd successful");
                }
                else
                {
                    Log.Error("TryAdd landed in Timedout ");
                }
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("sdf");
            }
        }

        /// Dequeues the task.
        /// Takes the task from the Queue, Service the task 
        public void DequeueTask()
        {

            foreach (var task in _workTaskQueue.GetConsumingEnumerable())
                try
                {
                    _mut.WaitOne(); // Wait until it is safe to enter. 
                    Log.Debug("Running the task - start");
                    if (!task.IsCanceled) task.RunSynchronously();
                    _mut.ReleaseMutex();    // Release the Mutex.                      
                }
                catch (Exception ex)
                {
                    Log.Error("DequeueTask failed with exception" + ex.Message);
                }
        }

        /// CompleteAdding : Will notify Queue that Task Addition is Completed
        public void Close()
        {
            _workTaskQueue.CompleteAdding();
        }

    }
}
