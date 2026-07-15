#nullable disable
using System;
using System.IO;
using System.Reflection;

namespace MetadataAnalyzer;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Укажите путь до файла DLL в параметрах командной строки.");
            return;
        }

        string dllPath = args[0];

        if (!File.Exists(dllPath))
        {
            Console.WriteLine("Файл не найден по указанному пути: " + dllPath);
            return;
        }

        Assembly assembly = Assembly.LoadFrom(dllPath);
        Console.WriteLine("Анализ библиотеки: " + assembly.GetName().Name);
        Console.WriteLine("========================================");

        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            if (type.IsClass)
            {
                Console.WriteLine("Класс: " + type.FullName);

                object[] attributes = type.GetCustomAttributes(true);
                if (attributes.Length > 0)
                {
                    Console.WriteLine("  Атрибуты класса:");
                    foreach (object attr in attributes)
                    {
                        Console.WriteLine("    - " + attr.GetType().Name);
                    }
                }

                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (constructors.Length > 0)
                {
                    Console.WriteLine("  Конструкторы:");
                    foreach (ConstructorInfo ctor in constructors)
                    {
                        Console.Write("    - Конструктор: (");
                        ParameterInfo[] parameters = ctor.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write(parameters[i].ParameterType.Name + " " + parameters[i].Name);
                            if (i < parameters.Length - 1)
                            {
                                Console.Write(", ");
                            }
                        }
                        Console.WriteLine(")");
                    }
                }

                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (methods.Length > 0)
                {
                    Console.WriteLine("  Методы:");
                    foreach (MethodInfo method in methods)
                    {
                        Console.Write("    - Метод: " + method.Name + " (");
                        ParameterInfo[] parameters = method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write(parameters[i].ParameterType.Name + " " + parameters[i].Name);
                            if (i < parameters.Length - 1)
                            {
                                Console.Write(", ");
                            }
                        }
                        Console.WriteLine(") -> Возвращает: " + method.ReturnType.Name);

                        object[] methodAttrs = method.GetCustomAttributes(true);
                        if (methodAttrs.Length > 0)
                        {
                            foreach (object attr in methodAttrs)
                            {
                                Console.WriteLine("        Атрибут метода: " + attr.GetType().Name);
                            }
                        }
                    }
                }

                Console.WriteLine("----------------------------------------");
            }
        }
    }
}
 
