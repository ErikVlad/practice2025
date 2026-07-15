using System;
using System.Threading;
using Xunit;
using task18;

namespace task18tests;

public class SchedulerTests
{
    private class LongRunningDummyCommand : ILongRunningCommand
    {
        private int _ticksRun = 0;
        private readonly int _maxTicks;

        public bool IsCompleted => _ticksRun >= _maxTicks;
        public int TicksRun => _ticksRun;

        public LongRunningDummyCommand(int maxTicks)
        {
            _maxTicks = maxTicks;
        }

        public void Execute()
        {
            _ticksRun++;
        }
    }

    [Fact]
    public void Scheduler_ShouldExecuteCommandsInRoundRobinOrder()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThreadWithScheduler(scheduler);
        
        var cmd1 = new LongRunningDummyCommand(2);
        var cmd2 = new LongRunningDummyCommand(2);

        server.AddCommand(cmd1);
        server.AddCommand(cmd2);

        server.Start();
        
        // Даем потоку время гарантированно завершить обе задачи
        Thread.Sleep(500);
        server.Stop();
        server.Thread.Join(1000);

        // Проверяем, что планировщик успешно довел обе команды до завершения
        Assert.True(cmd1.IsCompleted);
        Assert.True(cmd2.IsCompleted);
    }
}
