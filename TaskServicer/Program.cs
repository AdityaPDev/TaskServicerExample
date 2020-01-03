using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TaskServicer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console(outputTemplate:
                //    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .Enrich.WithThreadId()
                .WriteTo.File(new CompactJsonFormatter(), "logs\\Log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Initialize Task Queue and Specify Capacity and timeout
            //TaskQueue taskQueue = new TaskQueue(new ConcurrentQueue<Task>(), 1000, 10);
            TaskQueue taskQueue = new TaskQueue(new ConcurrentQueue<Task>(), 100, 10);

            // Subscribe to Queue Processing Events
            //taskQueue.TaskStatus += WorkQueue_TaskStatus;

            //Setup TaskAdder - To add Tasks and corresponding Action
            TaskAdder adderOne = new TaskAdder(taskQueue);
            
            //Start task servicers 
            int numOfServicers = 0;
            // Console.WriteLine("Enter number of task servicers to execute the tasks \n");
            numOfServicers = 1;// Int32.Parse(Console.ReadLine());

            Task[] taskSrvcsArray = new Task[numOfServicers];
            for (int i = 0; i < numOfServicers; i++)
            {
                taskSrvcsArray[i] = Task.Factory.StartNew(() => taskQueue.DequeueTask());
                Log.Debug($"Creating servicer task {i}");
            }
            int numTasks = 100;

            // Now start the processing
            // First, Start Adding Tasks 
            Task adderTaskOne = Task.Run(() => adderOne.AddTasks(numTasks));

            //Wait for task Adder to Complete adding of Tasks
            Task.WaitAll(adderTaskOne);

            //Console.WriteLine("Enter number of tasks to add additionally ...\n");
            numTasks = Int32.Parse(Console.ReadLine());
            adderTaskOne = Task.Run(() => adderOne.AddTasks(numTasks));
            Task.WaitAll(adderTaskOne);
            
            //Task.Wait(adderTaskOne);

            //Call Queue Close Method (Indicate TaskAdder have stopped adding tasks)
            taskQueue.Close();
            
            Task.WaitAll(taskSrvcsArray);
            
            Console.WriteLine("Tasks Processed");
            Log.CloseAndFlush();
            Console.ReadLine();

        }

        /// Trigged when attempt is made to Add Task to Queue(Either Success or Timeout)
        /// See TaskProcessingArguments
        /// <param name = "e" > The e.</param>
        //private static void WorkQueue_TaskStatus(TaskProcessingArguments e)
        //{
        //    if (!e.ISTaskAdded)
        //    {
        //        Console.WriteLine("In WorkQueue_TaskStatus" + e.ISTaskAdded);
        //        Log.Error($"Addition of task#{e.PendingTaskCount} is FAILED");
        //    }
        //    else
        //        Log.Debug($"Addition of task#{e.PendingTaskCount} is {e.ISTaskAdded}");
        //}
    }
}
