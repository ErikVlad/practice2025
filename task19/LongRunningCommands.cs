using System;
using task18;

namespace task19;

public class TestCommand : ILongRunningCommand
{
    private readonly int _id;
    private int _counter = 0;
    private readonly int _maxTicks = 3;

    public bool IsCompleted => _counter >= _maxTicks;
    public int Counter => _counter;

    public TestCommand(int id)
    {
        _id = id;
    }

    public void Execute()
    {
        if (IsCompleted) return;
        _counter++;
        Console.WriteLine($"Поток {_id} вызов {_counter}");
    }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThreadWithScheduler _serverThread;

    public HardStopCommand(ServerThreadWithScheduler serverThread)
    {
        _serverThread = serverThread;
    }

    public void Execute()
    {
        _serverThread.Stop();
    }
}
