using System;
using System.Threading.Tasks;
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
            AnsiConsole.MarkupLine("7. [green]Exit[/]");
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
    }
}
