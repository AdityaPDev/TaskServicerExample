using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace TaskServicer.Tests
{
    [TestClass]
    public class TaskQueueFacts
    {
        Random random = new Random();

        [TestMethod]
        public void EnqueueTask_AlwaysAddOneTask_ToQueue()
        {
            // Arrange
            var sut = this.CreateSystemUnderTest();
            var queue = new QueuedObject
            {
                QueueID = 1,
                AdderThreadID = Thread.CurrentThread.ManagedThreadId,
                EnqueueDateTime = DateTime.Now,
                // Used to Generate Random String
                RandomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray())
            };
            var h = new Action(DoSomeAction);

            // Act
            sut.EnqueueTask(h);

            // Assert
            sut._workTaskQueue.Count.ShouldBe(1);
        }

        [TestMethod]
        public void DequeueTask_AlwaysReleaseOneTask_FromQueue()
        {
            // Arrange
            var sut = this.CreateSystemUnderTest();
            var queue = new QueuedObject
            {
                QueueID = 1,
                AdderThreadID = Thread.CurrentThread.ManagedThreadId,
                EnqueueDateTime = DateTime.Now,
                // Used to Generate Random String
                RandomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray())
            };
            var h = new Action(DoSomeAction);
            sut.EnqueueTask(h);


            // Act
            sut.DequeueTask();

            // Assert
            sut._workTaskQueue.Count.ShouldBe(0);
        }

        

        private void DoSomeAction()
        {

        }

        private TaskQueue CreateSystemUnderTest()
        {
            return new TaskQueue(new ConcurrentQueue<Task>(), 100, 10);
        }
    }
}
