using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public interface ICommand
{
    void Execute();
}

public class ServerThread
{
    private readonly BlockingCollection<ICommand> _queue = new();
    private readonly Thread _thread;
    private bool _running = true;
    private bool _softStopRequested = false;

    public ServerThread()
    {
        _thread = new Thread(ProcessQueue);
    }

    public void Start()
    {
        _thread.Start();
    }

    public void Add(ICommand command)
    {
        if (!_queue.IsAddingCompleted)
        {
            _queue.Add(command);
        }
    }

    public void HardStop()
    {
        if (Thread.CurrentThread != _thread)
        {
            throw new InvalidOperationException();
        }
        _running = false;
        _queue.CompleteAdding();
    }

    public void SoftStop()
    {
        if (Thread.CurrentThread != _thread)
        {
            throw new InvalidOperationException();
        }
        _softStopRequested = true;
        _queue.CompleteAdding();
    }

    public void Join()
    {
        _thread.Join();
    }

    private void ProcessQueue()
    {
        try
        {
            foreach (var command in _queue.GetConsumingEnumerable())
            {
                if (!_running) break;

                try
                {
                    command.Execute();
                }
                catch (Exception)
                {
                }

                if (_softStopRequested && _queue.Count == 0)
                {
                    break;
                }
            }
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
