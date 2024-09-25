using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using sqltest.Helpers;

namespace sqltest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Setup dependency injection
            var services = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddTransient<DatabaseOperations>()
                .AddTransient<StudentRepository>()
                .AddTransient<CourseRepository>()
                .AddTransient<EnrollmentRepository>();

            // Create service provider
            var serviceProvider = services.BuildServiceProvider();

            // Use the services
            var dbOps = serviceProvider.GetService<DatabaseOperations>();
            if (dbOps != null)
            {
                await dbOps.CreateTablesAsync();
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Failed to create DatabaseOperations service.[/]");
                return;
            }

            var studentRepo = serviceProvider.GetService<StudentRepository>();
            var courseRepo = serviceProvider.GetService<CourseRepository>();
            var enrollmentRepo = serviceProvider.GetService<EnrollmentRepository>();

            if (studentRepo == null || courseRepo == null || enrollmentRepo == null)
            {
                AnsiConsole.MarkupLine("[red]Failed to create necessary services.[/]");
                return;
            }

            // Command-line interface loop
            while (true)
            {
                ShowMainMenuHeading();
                ShowMainMenuOptions();

                var choice = Console.ReadLine() ?? string.Empty;
                if (InputHelper.CheckForSpecialCommands(choice))
                {
                    continue;
                }

                switch (choice)
                {
                    case "1":
                        await AddStudentAsync(studentRepo);
                        break;
                    case "2":
                        await AddCourseAsync(courseRepo);
                        break;
                    case "3":
                        await EnrollStudentAsync(enrollmentRepo, studentRepo, courseRepo);
                        break;
                    case "4":
                        await ViewAllStudentsAsync(studentRepo);
                        break;
                    case "5":
                        await ViewAllCoursesAsync(courseRepo);
                        break;
                    case "6":
                        await ViewAllEnrollmentsAsync(enrollmentRepo);
                        break;
                    case "7":
                        await SearchStudentByIdAsync(studentRepo);
                        break;
                    case "8":
                        await SearchStudentsByNameAsync(studentRepo);
                        break;
                    case "9":
                        await UpdateStudentAsync(studentRepo);
                        break;
                    case "10":
                        await DeleteStudentAsync(studentRepo);
                        break;
                    case "11":
                        await ExportStudentsToCsvAsync(studentRepo);
                        break;
                    case "12":
                        await ExportStudentsToJsonAsync(studentRepo);
                        break;
                    case "13":
                        return;
                    default:
                        AnsiConsole.MarkupLine("[red]Invalid option. Please choose again.[/]");
                        break;
                }

                // Pause before showing the main menu again
                AnsiConsole.MarkupLine("[grey]Press any key to return to the main menu...[/]");
                Console.ReadKey();
            }
        }

        private static void ShowMainMenuHeading()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("Main Menu")
                    .Centered()
                    .Color(Color.Green));
        }

        private static void ShowMainMenuOptions()
        {
            AnsiConsole.MarkupLine("Choose an option:");
            AnsiConsole.MarkupLine("1. [green]Add a new student[/]");
            AnsiConsole.MarkupLine("2. [green]Add a new course[/]");
            AnsiConsole.MarkupLine("3. [green]Enroll a student in a course[/]");
            AnsiConsole.MarkupLine("4. [green]View all students[/]");
            AnsiConsole.MarkupLine("5. [green]View all courses[/]");
            AnsiConsole.MarkupLine("6. [green]View all enrollments[/]");
            AnsiConsole.MarkupLine("7. [green]Search for a student by ID[/]");
            AnsiConsole.MarkupLine("8. [green]Search for students by name[/]");
            AnsiConsole.MarkupLine("9. [green]Update a student[/]");
            AnsiConsole.MarkupLine("10. [green]Delete a student[/]");
            AnsiConsole.MarkupLine("11. [green]Export students to CSV[/]");
            AnsiConsole.MarkupLine("12. [green]Export students to JSON[/]");
            AnsiConsole.MarkupLine("13. [green]Exit[/]");
            AnsiConsole.Markup("Option: ");
        }

        private static async Task AddStudentAsync(StudentRepository studentRepo)
        {
            AnsiConsole.Markup("Enter first name (or type 'menu' to return to main menu): ");
            var firstName = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(firstName))
            {
                return;
            }

            AnsiConsole.Markup("Enter last name (or type 'menu' to return to main menu): ");
            var lastName = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(lastName))
            {
                return;
            }

            AnsiConsole.Markup("Enter email (or type 'menu' to return to main menu): ");
            var email = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(email))
            {
                return;
            }

            // Ensure that these values are not null
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
            {
                AnsiConsole.MarkupLine("[red]Invalid input. All fields are required.[/]");
                return;
            }

            var newStudent = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                RegistrationDate = DateTime.Now
            };

            await studentRepo.AddStudentAsync(newStudent);
            AnsiConsole.MarkupLine("[green]Student added successfully.[/]");
        }

        private static async Task AddCourseAsync(CourseRepository courseRepo)
        {
            AnsiConsole.Markup("Enter course name (or type 'menu' to return to main menu): ");
            var name = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(name))
            {
                return;
            }

            AnsiConsole.Markup("Enter course duration (e.g., 2:00:00 for 2 hours, or type 'menu' to return to main menu): ");
            var durationInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(durationInput))
            {
                return;
            }

            if (!TimeSpan.TryParse(durationInput, out var duration))
            {
                AnsiConsole.MarkupLine("[red]Invalid duration format.[/]");
                return;
            }

            AnsiConsole.Markup("Enter course description (or type 'menu' to return to main menu): ");
            var description = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(description))
            {
                return;
            }

            AnsiConsole.Markup("Enter course credits (or type 'menu' to return to main menu): ");
            var creditsInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(creditsInput))
            {
                return;
            }

            if (!int.TryParse(creditsInput, out var credits))
            {
                AnsiConsole.MarkupLine("[red]Invalid credits format.[/]");
                return;
            }

            var newCourse = new Course
            {
                Name = name,
                Duration = duration,
                Description = description,
                Credits = credits
            };

            await courseRepo.AddCourseAsync(newCourse);
            AnsiConsole.MarkupLine("[green]Course added successfully.[/]");
        }

        private static async Task EnrollStudentAsync(EnrollmentRepository enrollmentRepo, StudentRepository studentRepo, CourseRepository courseRepo)
        {
            // Display the list of students
            var students = await studentRepo.GetAllStudentsAsync();
            if (students.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no students to enroll.[/]");
                return;
            }

            var studentTable = new Table();
            studentTable.AddColumn("ID");
            studentTable.AddColumn("First Name");
            studentTable.AddColumn("Last Name");
            studentTable.AddColumn("Email");

            foreach (var stud in students)
            {
                studentTable.AddRow(stud.Id.ToString(), stud.FirstName, stud.LastName, stud.Email);
            }

            AnsiConsole.Write(studentTable);

            // Display the list of courses
            var courses = await courseRepo.GetAllCoursesAsync();
            if (courses.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no courses to enroll in.[/]");
                return;
            }

            var courseTable = new Table();
            courseTable.AddColumn("ID");
            courseTable.AddColumn("Name");
            courseTable.AddColumn("Duration");
            courseTable.AddColumn("Description");
            courseTable.AddColumn("Credits");

            foreach (var crs in courses)
            {
                courseTable.AddRow(crs.Id.ToString(), crs.Name, crs.Duration.ToString(), crs.Description, crs.Credits.ToString());
            }

            AnsiConsole.Write(courseTable);

            // Enroll the student in the course
            AnsiConsole.Markup("Enter student ID (or type 'menu' to return to main menu): ");
            var studentIdInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(studentIdInput))
            {
                return;
            }

            if (!int.TryParse(studentIdInput, out var studentId))
            {
                AnsiConsole.MarkupLine("[red]Invalid student ID.[/]");
                return;
            }

            AnsiConsole.Markup("Enter course ID (or type 'menu' to return to main menu): ");
            var courseIdInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(courseIdInput))
            {
                return;
            }

            if (!int.TryParse(courseIdInput, out var courseId))
            {
                AnsiConsole.MarkupLine("[red]Invalid course ID.[/]");
                return;
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledDate = DateTime.Now
            };

            await enrollmentRepo.EnrollStudentAsync(enrollment);
            AnsiConsole.MarkupLine("[green]Student enrolled successfully.[/]");

            // Fetch and display student and course details
            var enrolledStudent = await studentRepo.GetStudentByIdAsync(studentId);
            var enrolledCourse = await courseRepo.GetCourseByIdAsync(courseId);

            if (enrolledStudent != null && enrolledCourse != null)
            {
                var table = new Table();
                table.AddColumn("Student ID");
                table.AddColumn("Student Name");
                table.AddColumn("Course ID");
                table.AddColumn("Course Name");

                table.AddRow(enrolledStudent.Id.ToString(), $"{enrolledStudent.FirstName} {enrolledStudent.LastName}", enrolledCourse.Id.ToString(), enrolledCourse.Name);

                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Failed to fetch student or course details.[/]");
            }
        }

        private static async Task ViewAllStudentsAsync(StudentRepository studentRepo)
        {
            var students = await studentRepo.GetAllStudentsAsync();
            if (students.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no students to view.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("First Name");
            table.AddColumn("Last Name");
            table.AddColumn("Email");
            table.AddColumn("Registration Date");

            foreach (var student in students)
            {
                table.AddRow(student.Id.ToString(), student.FirstName, student.LastName, student.Email, student.RegistrationDate.ToString());
            }

            AnsiConsole.Write(table);
        }

        private static async Task ViewAllCoursesAsync(CourseRepository courseRepo)
        {
            var courses = await courseRepo.GetAllCoursesAsync();
            if (courses.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no courses to view.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Duration");
            table.AddColumn("Description");
            table.AddColumn("Credits");

            foreach (var course in courses)
            {
                table.AddRow(course.Id.ToString(), course.Name, course.Duration.ToString(), course.Description, course.Credits.ToString());
            }

            AnsiConsole.Write(table);
        }

        private static async Task ViewAllEnrollmentsAsync(EnrollmentRepository enrollmentRepo)
        {
            var enrollments = await enrollmentRepo.GetAllEnrollmentsAsync();
            if (enrollments.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no enrollments to view.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("Student ID");
            table.AddColumn("Course ID");
            table.AddColumn("Enrolled Date");

            foreach (var enrollment in enrollments)
            {
                table.AddRow(enrollment.StudentId.ToString(), enrollment.CourseId.ToString(), enrollment.EnrolledDate.ToString());
            }

            AnsiConsole.Write(table);
        }

        private static async Task SearchStudentByIdAsync(StudentRepository studentRepo)
        {
            AnsiConsole.Markup("Enter student ID: ");
            var input = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(input))
            {
                return;
            }

            if (!int.TryParse(input, out var studentId))
            {
                AnsiConsole.MarkupLine("[red]Invalid student ID.[/]");
                return;
            }

            var student = await studentRepo.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                AnsiConsole.MarkupLine("[yellow]Student not found.[/]");
            }
            else
            {
                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("First Name");
                table.AddColumn("Last Name");
                table.AddColumn("Email");
                table.AddColumn("Registration Date");
                table.AddRow(student.Id.ToString(), student.FirstName, student.LastName, student.Email, student.RegistrationDate.ToString());

                AnsiConsole.Write(table);
            }
        }

        private static async Task SearchStudentsByNameAsync(StudentRepository studentRepo)
        {
            AnsiConsole.Markup("Enter student name: ");
            var name = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(name))
            {
                return;
            }

            var students = await studentRepo.SearchStudentsByNameAsync(name);
            if (students.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No students found with the given name.[/]");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("First Name");
            table.AddColumn("Last Name");
            table.AddColumn("Email");
            table.AddColumn("Registration Date");

            foreach (var student in students)
            {
                table.AddRow(student.Id.ToString(), student.FirstName, student.LastName, student.Email, student.RegistrationDate.ToString());
            }

            AnsiConsole.Write(table);
        }

        private static async Task UpdateStudentAsync(StudentRepository studentRepo)
        {
            AnsiConsole.Markup("Enter student ID to update: ");
            var input = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(input))
            {
                return;
            }

            if (!int.TryParse(input, out var studentId))
            {
                AnsiConsole.MarkupLine("[red]Invalid student ID.[/]");
                return;
            }

            var student = await studentRepo.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                AnsiConsole.MarkupLine("[yellow]Student not found.[/]");
                return;
            }

            AnsiConsole.Markup($"Enter new first name (current: {student.FirstName}): ");
            var firstName = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(firstName))
            {
                firstName = student.FirstName;
            }

            AnsiConsole.Markup($"Enter new last name (current: {student.LastName}): ");
            var lastName = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(lastName))
            {
                lastName = student.LastName;
            }

            AnsiConsole.Markup($"Enter new email (current: {student.Email}): ");
            var email = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(email))
            {
                email = student.Email;
            }

            student.FirstName = firstName;
            student.LastName = lastName;
            student.Email = email;
            student.RegistrationDate = DateTime.Now;

            await studentRepo.UpdateStudentAsync(student);
            AnsiConsole.MarkupLine("[green]Student updated successfully.[/]");
        }

        private static async Task DeleteStudentAsync(StudentRepository studentRepo)
        {
            AnsiConsole.Markup("Enter student ID to delete: ");
            var input = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(input))
            {
                return;
            }

            if (!int.TryParse(input, out var studentId))
            {
                AnsiConsole.MarkupLine("[red]Invalid student ID.[/]");
                return;
            }

            await studentRepo.DeleteStudentAsync(studentId);
            AnsiConsole.MarkupLine("[green]Student deleted successfully.[/]");
        }

        private static async Task ExportStudentsToCsvAsync(StudentRepository studentRepo)
        {
            var students = await studentRepo.GetAllStudentsAsync();
            if (students.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no students to export.[/]");
                return;
            }

            var filePath = "students.csv";
            ExportHelper.ExportToCsv(students, filePath);
            AnsiConsole.MarkupLine($"[green]Students exported to {filePath}[/]");
        }

        private static async Task ExportStudentsToJsonAsync(StudentRepository studentRepo)
        {
            var students = await studentRepo.GetAllStudentsAsync();
            if (students.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]There are no students to export.[/]");
                return;
            }

            var filePath = "students.json";
            ExportHelper.ExportToJson(students, filePath);
            AnsiConsole.MarkupLine($"[green]Students exported to {filePath}[/]");
        }
    }
}
