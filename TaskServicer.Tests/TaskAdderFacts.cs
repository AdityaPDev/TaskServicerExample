using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace TaskServicer.Tests
{
    [TestClass]
    public class TaskAdderFacts
    {
        [TestMethod]
        public void AddTasks_Always_EnqueueTask()
        {
            // Arrange
            var sut = this.CeateSystemUnderTest();

            // Act
            sut.AddTasks(4);

            // Assert
            sut._taskQueue._workTaskQueue.Count.ShouldBe(4);
        }

        [TestMethod]
        public void ReverseString_sdf_sdf()
        {
            // Arrange
            //var sut = this.CeateSystemUnderTest();
            //var queue = new QueuedObject
            //{
            //    QueueID = 1,
            //    AdderThreadID = Thread.CurrentThread.ManagedThreadId,
            //    EnqueueDateTime = DateTime.Now,
            //    // Used to Generate Random String
            //    RandomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray())
            //};

            //// Act
            //sut.ReverseString(queue);

            //// Assert
            //sut.
        }

        private TaskAdder CeateSystemUnderTest()
        {
            var g = new TaskQueue(new ConcurrentQueue<Task>(), 100, 10);
            return new TaskAdder(g);
        }
    }
}
