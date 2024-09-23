using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // Debug output to check connection string
            Console.WriteLine("Connection String from Configuration: " + configuration.GetConnectionString("PostgresConnection"));

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
                Console.WriteLine("Failed to create DatabaseOperations service.");
                return;
            }

            var studentRepo = serviceProvider.GetService<StudentRepository>();
            var courseRepo = serviceProvider.GetService<CourseRepository>();
            var enrollmentRepo = serviceProvider.GetService<EnrollmentRepository>();

            if (studentRepo == null || courseRepo == null || enrollmentRepo == null)
            {
                Console.WriteLine("Failed to create necessary services.");
                return;
            }

            // Command-line interface loop
            while (true)
            {
                ShowMainMenu();

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
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
            }
        }

        private static void ShowMainMenu()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Add a new student");
            Console.WriteLine("2. Add a new course");
            Console.WriteLine("3. Enroll a student in a course");
            Console.WriteLine("4. View all students");
            Console.WriteLine("5. View all courses");
            Console.WriteLine("6. View all enrollments");
            Console.WriteLine("7. Exit");
            Console.Write("Option: ");
        }

        private static async Task AddStudentAsync(StudentRepository studentRepo)
        {
            Console.Write("Enter first name (or type 'menu' to return to main menu): ");
            var firstName = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(firstName))
            {
                return;
            }

            Console.Write("Enter last name (or type 'menu' to return to main menu): ");
            var lastName = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(lastName))
            {
                return;
            }

            Console.Write("Enter email (or type 'menu' to return to main menu): ");
            var email = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(email))
            {
                return;
            }

            // Ensure that these values are not null
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Invalid input. All fields are required.");
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
            Console.WriteLine("Student added successfully.");
        }

        private static async Task AddCourseAsync(CourseRepository courseRepo)
        {
            Console.Write("Enter course name (or type 'menu' to return to main menu): ");
            var name = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(name))
            {
                return;
            }

            Console.Write("Enter course duration (e.g., 2:00:00 for 2 hours, or type 'menu' to return to main menu): ");
            var durationInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(durationInput))
            {
                return;
            }

            if (!TimeSpan.TryParse(durationInput, out var duration))
            {
                Console.WriteLine("Invalid duration format.");
                return;
            }

            Console.Write("Enter course description (or type 'menu' to return to main menu): ");
            var description = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(description))
            {
                return;
            }

            Console.Write("Enter course credits (or type 'menu' to return to main menu): ");
            var creditsInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(creditsInput))
            {
                return;
            }

            if (!int.TryParse(creditsInput, out var credits))
            {
                Console.WriteLine("Invalid credits format.");
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
            Console.WriteLine("Course added successfully.");
        }

        private static async Task EnrollStudentAsync(EnrollmentRepository enrollmentRepo, StudentRepository studentRepo, CourseRepository courseRepo)
        {
            Console.Write("Enter student ID (or type 'menu' to return to main menu): ");
            var studentIdInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(studentIdInput))
            {
                return;
            }

            if (!int.TryParse(studentIdInput, out var studentId))
            {
                Console.WriteLine("Invalid student ID.");
                return;
            }

            Console.Write("Enter course ID (or type 'menu' to return to main menu): ");
            var courseIdInput = Console.ReadLine() ?? string.Empty;
            if (InputHelper.CheckForSpecialCommands(courseIdInput))
            {
                return;
            }

            if (!int.TryParse(courseIdInput, out var courseId))
            {
                Console.WriteLine("Invalid course ID.");
                return;
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledDate = DateTime.Now
            };

            await enrollmentRepo.EnrollStudentAsync(enrollment);
            Console.WriteLine("Student enrolled successfully.");
        }

        private static async Task ViewAllStudentsAsync(StudentRepository studentRepo)
        {
            var students = await studentRepo.GetAllStudentsAsync();
            if(students.Count == 0)
            {
                Console.WriteLine("There are no students to view");
                return;
            }    
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Id}: {student.FirstName} {student.LastName} - {student.Email} - {student.RegistrationDate}");
            }
        }

        private static async Task ViewAllCoursesAsync(CourseRepository courseRepo)
        {
            var courses = await courseRepo.GetAllCoursesAsync();
            if(courses.Count == 0)
            {
                Console.WriteLine("There are no courses to view.");
                return;
            }
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Id}: {course.Name} - {course.Duration} - {course.Description} - {course.Credits} credits");
            }
        }

        private static async Task ViewAllEnrollmentsAsync(EnrollmentRepository enrollmentRepo)
        {
            var enrollments = await enrollmentRepo.GetAllEnrollmentsAsync();
            if(enrollments.Count == 0)
            {
                Console.WriteLine("There are no enrollments to view.");
                return;
            }
            foreach (var enrollment in enrollments)
            {
                Console.WriteLine($"Student ID: {enrollment.StudentId}, Course ID: {enrollment.CourseId}, Enrolled Date: {enrollment.EnrolledDate}");
            }
        }
    }
}
