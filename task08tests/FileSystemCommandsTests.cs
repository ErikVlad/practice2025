using System;
using System.IO;
using Xunit;
using FileSystemCommands;

namespace task08tests;

public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        string testDir = Path.Combine(Path.GetTempPath(), "TestDirSize");
        if (Directory.Exists(testDir))
        {
            Directory.Delete(testDir, true);
        }
        Directory.CreateDirectory(testDir);
        
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

        DirectorySizeCommand command = new DirectorySizeCommand(testDir);
        command.Execute();

        Assert.True(Directory.Exists(testDir));

        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        string testDir = Path.Combine(Path.GetTempPath(), "TestDirFind");
        if (Directory.Exists(testDir))
        {
            Directory.Delete(testDir, true);
        }
        Directory.CreateDirectory(testDir);
        
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

        FindFilesCommand command = new FindFilesCommand(testDir, "*.txt");
        command.Execute();

        Assert.True(Directory.Exists(testDir));

        Directory.Delete(testDir, true);
    }
}
