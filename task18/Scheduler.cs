using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace task18;

public interface ICommand
{
    void Execute();
}

public interface ILongRunningCommand : ICommand
{
    bool IsCompleted { get; }
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly List<ILongRunningCommand> _commands = new();
    private int _index = 0;

    public bool HasCommand() => _commands.Count > 0;

    public void Add(ICommand cmd)
    {
        if (cmd is ILongRunningCommand longCmd && !_commands.Contains(longCmd))
        {
            if (!longCmd.IsCompleted)
            {
                _commands.Add(longCmd);
            }
        }
    }

    public ICommand Select()
    {
        if (_commands.Count == 0) return null!;

        if (_index >= _commands.Count)
        {
            _index = 0;
        }

        var cmd = _commands[_index];

        if (cmd.IsCompleted)
        {
            _commands.RemoveAt(_index);
            return cmd;
        }

        _index = (_index + 1) % _commands.Count;
        return cmd;
    }
}

public class ServerThreadWithScheduler
{
    private readonly ConcurrentQueue<ICommand> _queue = new();
    private readonly IScheduler _scheduler;
    private readonly Thread _thread;
    private readonly AutoResetEvent _waitHandle = new(false);
    private bool _running = true;

    public Thread Thread => _thread;

    public ServerThreadWithScheduler(IScheduler scheduler)
    {
        _scheduler = scheduler;
        _thread = new Thread(ProcessLoop);
    }

    public void Start() => _thread.Start();

    public void Stop()
    {
        _running = false;
        _waitHandle.Set();
    }

    public void AddCommand(ICommand command)
    {
        _queue.Enqueue(command);
        _waitHandle.Set();
    }

    private void ProcessLoop()
    {
        while (_running)
        {
            bool worked = false;

            if (_queue.TryDequeue(out var command))
            {
                if (command is ILongRunningCommand)
                {
                    _scheduler.Add(command);
                }
                else
                {
                    ExecuteSingle(command);
                }
                worked = true;
            }

            if (!worked && _scheduler.HasCommand())
            {
                var longCommand = _scheduler.Select();
                if (longCommand != null)
                {
                    ExecuteSingle(longCommand);
                    worked = true;
                }
            }

            if (!worked)
            {
                _waitHandle.WaitOne();
            }
        }
    }

    private void ExecuteSingle(ICommand command)
    {
        try
        {
            command.Execute();
        }
        catch (Exception)
        {
        }
    }
}
