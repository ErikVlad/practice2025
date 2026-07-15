#nullable disable
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class StudentSerializer
{
    private readonly JsonSerializerOptions _options;

    public StudentSerializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull 
        };
    }

    public string Serialize(Student student)
    {
        return JsonSerializer.Serialize(student, _options);
    }

    public Student Deserialize(string json)
    {
        Student student = JsonSerializer.Deserialize<Student>(json, _options);
        
        if (student == null) throw new ArgumentNullException("JSON не содержит валидного объекта");
        if (string.IsNullOrWhiteSpace(student.FirstName)) throw new Exception("Имя студента не может быть пустым");
        if (string.IsNullOrWhiteSpace(student.LastName)) throw new Exception("Фамилия студента не может быть пустым");
        if (student.BirthDate > DateTime.Now) throw new Exception("Дата рождения не может быть в будущем");

        return student;
    }

    public void SaveToFile(string filePath, Student student)
    {
        string json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public Student LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Файл не найден");
        string json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
}
