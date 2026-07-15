#nullable disable 
using System; 
using System.IO; 
using System.Reflection; 
using System.Collections.Generic; 
 
namespace PluginSystem; 
 
public interface ICommand 
{ 
    void Execute(); 
} 
 
[AttributeUsage(AttributeTargets.Class)] 
public class PluginLoadAttribute : Attribute 
{ 
    private string _pluginName; 
    private string _dependsOn; 
 
    public PluginLoadAttribute(string pluginName) 
    { 
        _pluginName = pluginName; 
        _dependsOn = ""; 
    } 
 
    public PluginLoadAttribute(string pluginName, string dependsOn) 
    { 
        _pluginName = pluginName; 
        _dependsOn = dependsOn; 
    } 
 
    public string PluginName 
    { 
        get { return _pluginName; } 
    } 
 
    public string DependsOn 
    { 
        get { return _dependsOn; } 
    } 
} 
 
class Program 
{ 
    static void Main(string[] args) 
    { 
        string pluginsFolder = AppContext.BaseDirectory; 
         
        if (!Directory.Exists(pluginsFolder)) 
        { 
            Console.WriteLine("Папка с плагинами не найдена."); 
            return; 
        } 
 
        string[] dllFiles = Directory.GetFiles(pluginsFolder, "*.dll"); 
        List<Type> foundPlugins = new List<Type>(); 
 
        foreach (string dllPath in dllFiles) 
        { 
            try 
            { 
                Assembly assembly = Assembly.LoadFrom(dllPath); 
                Type[] types = assembly.GetTypes(); 
 
                foreach (Type type in types) 
                { 
                    if (type.IsClass && typeof(ICommand).IsAssignableFrom(type)) 
                    { 
                        PluginLoadAttribute attr = type.GetCustomAttribute<PluginLoadAttribute>(); 
                        if (attr != null) 
                        { 
                            foundPlugins.Add(type); 
                        } 
                    } 
                } 
            } 
            catch 
            { 
            } 
        } 
 
        List<string> loadedPluginNames = new List<string>(); 
        int previousCount = -1; 
 
        while (foundPlugins.Count > 0 && foundPlugins.Count != previousCount) 
        { 
            previousCount = foundPlugins.Count; 
            List<Type> postponedPlugins = new List<Type>(); 
 
            foreach (Type pluginType in foundPlugins) 
            { 
                PluginLoadAttribute attr = pluginType.GetCustomAttribute<PluginLoadAttribute>(); 
                string dependency = attr.DependsOn; 
 
                if (string.IsNullOrEmpty(dependency) || loadedPluginNames.Contains(dependency)) 
                { 
                    try 
                    { 
                        object instance = Activator.CreateInstance(pluginType); 
                        ICommand command = instance as ICommand; 
 
                        if (command != null) 
                        { 
                            Console.WriteLine("Запуск плагина: " + attr.PluginName); 
                            command.Execute(); 
                            loadedPluginNames.Add(attr.PluginName); 
                        } 
                    } 
                    catch (Exception ex) 
                    { 
                        Console.WriteLine("Ошибка запуска плагина " + attr.PluginName + ": " + ex.Message); 
                    } 
                } 
                else 
                { 
                    postponedPlugins.Add(pluginType); 
                } 
            } 
 
            foundPlugins = postponedPlugins; 
        } 
 
        if (foundPlugins.Count > 0) 
        { 
            Console.WriteLine("Предупреждение: Некоторые плагины не удалось загрузить из-за неразрешимых зависимостей."); 
        } 
    } 
} 
