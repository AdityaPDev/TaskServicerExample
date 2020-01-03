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

        

        private TaskAdder CeateSystemUnderTest()
        {
            var g = new TaskQueue(new ConcurrentQueue<Task>(), 100, 10);
            return new TaskAdder(g);
        }
    }
}
