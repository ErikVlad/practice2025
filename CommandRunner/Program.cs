#nullable disable
using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner;

class Program
{
    static void Main(string[] args)
    {
        string dllPath = Path.Combine(AppContext.BaseDirectory, "FileSystemCommands.dll");

        if (File.Exists(dllPath))
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);

            Type sizeCommandType = assembly.GetType("FileSystemCommands.DirectorySizeCommand");
            if (sizeCommandType != null)
            {
                object sizeCmd = Activator.CreateInstance(sizeCommandType, AppContext.BaseDirectory);
                ICommand command1 = sizeCmd as ICommand;
                if (command1 != null)
                {
                    command1.Execute();
                }
            }

            Type findCommandType = assembly.GetType("FileSystemCommands.FindFilesCommand");
            if (findCommandType != null)
            {
                object findCmd = Activator.CreateInstance(findCommandType, AppContext.BaseDirectory, "*.dll");
                ICommand command2 = findCmd as ICommand;
                if (command2 != null)
                {
                    command2.Execute();
                }
            }
        }
    }
}
