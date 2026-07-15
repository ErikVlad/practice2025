using System;
using System.Threading;
using Xunit;
using task18;
using task19;

namespace task19tests;

public class LongRunningTests
{
    [Fact]
    public void TestCommand_ShouldRunFiveInstancesThreeTimes_ThenStop()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThreadWithScheduler(scheduler);

        var commands = new TestCommand[5];
        for (int i = 0; i < 5; i++)
        {
            commands[i] = new TestCommand(i + 1);
            server.AddCommand(commands[i]);
        }

        server.Start();

        Thread.Sleep(500);

        var hardStop = new HardStopCommand(server);
        server.AddCommand(hardStop);

        server.Thread.Join(1000);

        for (int i = 0; i < 5; i++)
        {
            Assert.True(commands[i].IsCompleted);
            Assert.Equal(3, commands[i].Counter);
        }
    }
}
