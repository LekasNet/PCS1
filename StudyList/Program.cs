using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

public class Student
{
    public int StudentID { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
}

public class Subject
{
    public int SubjectID { get; set; }
    public string Title { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
}

public class Curriculum
{
    public int StudentID { get; set; }
    public int SubjectID { get; set; }
    public int Grade { get; set; }
}

class Data
{
    public List<Student> Students { get; set; }
    public List<Subject> Subjects { get; set; }
    public List<Curriculum> Curriculum { get; set; }
}

class Program
{
    static List<Student> Students = new List<Student>();
    static List<Subject> Subjects = new List<Subject>();
    static List<Curriculum> Curriculum = new List<Curriculum>();

    static void Main(string[] args)
    {
        LoadDataFromJson("university.json"); 

        while (true) 
        {
            Console.Clear(); 
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1 - Вывести все данные");
            Console.WriteLine("2 - Добавить новую оценку");
            Console.WriteLine("3 - Вывести процент оценок студента");
            Console.WriteLine("4 - Выход");
            Console.WriteLine("0 - Вернуться в начальное меню (этот пункт доступен на любом шаге)");

            string choice = Console.ReadLine(); 

            if (choice == "0") 
            {
                continue; 
            }

            switch (choice)
            {
                case "1":
                    PrintAllData();
                    break;
                case "2":
                    AddGrade();
                    break;
                case "3":
                    PrintGradesPercentageByStudent();
                    break;
                case "4":
                    return; 
                default:
                    Console.WriteLine("Некорректный ввод. Пожалуйста, выберите действие от 1 до 4.");
                    break;
            }

            Console.WriteLine("\nНажмите 0 для возврата в начальное меню или любую другую клавишу для выхода.");
            if (Console.ReadLine() != "0")
            {
                break; 
            }
        }
    }


    static void LoadDataFromJson(string filePath)
    {
        try
        {
            Console.Clear(); 
            string jsonString = File.ReadAllText(filePath);
            Data data = JsonSerializer.Deserialize<Data>(jsonString);

            if (data != null)
            {
                Students = data.Students;
                Subjects = data.Subjects;
                Curriculum = data.Curriculum;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    
    
    static void PrintAllData() {
        Console.Clear(); 
        Console.WriteLine("Students:");
        foreach (var student in Students) {
            Console.WriteLine($"ID: {student.StudentID}, Name: {student.FirstName} {student.MiddleName} {student.LastName}");
        }

        Console.WriteLine("\nSubjects:");
        foreach (var subject in Subjects) {
            Console.WriteLine($"ID: {subject.SubjectID}, Title: {subject.Title}, Lecture Hours: {subject.LectureHours}, Practice Hours: {subject.PracticeHours}");
        }

        Console.WriteLine("\nGrades:");
        var gradesQuery = from grade in Curriculum
                          join student in Students on grade.StudentID equals student.StudentID
                          join subject in Subjects on grade.SubjectID equals subject.SubjectID
                          select new {
                              StudentName = student.FirstName + " " + student.LastName,
                              SubjectTitle = subject.Title,
                              Grade = grade.Grade
                          };

        foreach (var item in gradesQuery) {
            Console.WriteLine($"Student: {item.StudentName}, Subject: {item.SubjectTitle}, Grade: {item.Grade}");
        }
    }

static void AddGrade()
{
    Console.Clear();
    Console.WriteLine("Выберите предмет:");
    foreach (var subject in Subjects.Select((value, index) => new { value, index }))
    {
        Console.WriteLine($"{subject.index + 1}. {subject.value.Title}");
    }
    int subjectIndex = Convert.ToInt32(Console.ReadLine()) - 1;

    Console.WriteLine("Выберите ученика:");
    foreach (var student in Students.Select((value, index) => new { value, index }))
    {
        Console.WriteLine($"{student.index + 1}. {student.value.LastName} {student.value.FirstName}");
    }
    int studentIndex = Convert.ToInt32(Console.ReadLine()) - 1;

    Console.WriteLine("Введите оценку:");
    int grade = Convert.ToInt32(Console.ReadLine());

    Curriculum.Add(new Curriculum
    {
        StudentID = Students[studentIndex].StudentID,
        SubjectID = Subjects[subjectIndex].SubjectID,
        Grade = grade
    });

    SaveDataToJson("university.json"); // Сохраняем изменения в файл

    Console.WriteLine("Оценка добавлена. Нажмите любую клавишу для возврата в меню.");
    Console.ReadKey();
}

static void SaveDataToJson(string filePath)
{
    var data = new Data
    {
        Students = Students,
        Subjects = Subjects,
        Curriculum = Curriculum
    };

    string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(filePath, jsonString);
}


    static void PrintGradesPercentageByStudent() {
        Console.Clear(); 
        Console.WriteLine("Введите номер студента:");
        int studentID = Convert.ToInt32(Console.ReadLine());

        var studentGrades = Curriculum.Where(c => c.StudentID == studentID).ToList();
        if (studentGrades.Count > 0) {
            var gradesGroup = studentGrades
                .GroupBy(g => g.Grade)
                .Select(group => new {
                    Grade = group.Key,
                    Percentage = (double)group.Count() / studentGrades.Count * 100
                });

            foreach (var item in gradesGroup) {
                Console.WriteLine($"Оценка: {item.Grade}, Процент: {item.Percentage:F2}%");
            }
        }
        else {
            Console.WriteLine("Оценки не найдены.");
        }
    }
}
