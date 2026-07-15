#nullable disable
using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private string _dirPath;

    public DirectorySizeCommand(string dirPath)
    {
        _dirPath = dirPath;
    }

    public void Execute()
    {
        if (Directory.Exists(_dirPath))
        {
            string[] files = Directory.GetFiles(_dirPath, "*", SearchOption.AllDirectories);
            long totalSize = 0;

            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                totalSize += info.Length;
            }

            Console.WriteLine("Общий размер каталога: " + totalSize + " байт");
        }
    }
}

public class FindFilesCommand : ICommand
{
    private string _dirPath;
    private string _searchPattern;

    public FindFilesCommand(string dirPath, string searchPattern)
    {
        _dirPath = dirPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (Directory.Exists(_dirPath))
        {
            string[] foundFiles = Directory.GetFiles(_dirPath, _searchPattern);
            
            foreach (string file in foundFiles)
            {
                Console.WriteLine("Найден файл: " + Path.GetFileName(file));
            }
        }
    }
}
 
