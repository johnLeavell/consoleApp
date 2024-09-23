using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Add a new student");
                Console.WriteLine("2. Add a new course");
                Console.WriteLine("3. Enroll a student in a course");
                Console.WriteLine("4. Exit");
                Console.Write("Option: ");
                var choice = Console.ReadLine();

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
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose again.");
                        break;
                }
            }
        }

        private static async Task AddStudentAsync(StudentRepository studentRepo)
        {
            Console.Write("Enter first name: ");
            var firstName = Console.ReadLine();

            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine();

            Console.Write("Enter email: ");
            var email = Console.ReadLine();

            // Ensure that these values are not null
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Invalid input. All fields are required.");
                return;
            }

            var newStudent = new Student
            {
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Email = email ?? string.Empty,
                RegistrationDate = DateTime.Now
            };

            await studentRepo.AddStudentAsync(newStudent);
            Console.WriteLine("Student added successfully.");
        }

        private static async Task AddCourseAsync(CourseRepository courseRepo)
        {
            Console.Write("Enter course name: ");
            var name = Console.ReadLine();

            Console.Write("Enter course duration (e.g., 2:00:00 for 2 hours): ");
            var durationInput = Console.ReadLine();
            if (!TimeSpan.TryParse(durationInput, out var duration))
            {
                Console.WriteLine("Invalid duration format.");
                return;
            }

            Console.Write("Enter course description: ");
            var description = Console.ReadLine();

            Console.Write("Enter course credits: ");
            if (!int.TryParse(Console.ReadLine(), out var credits))
            {
                Console.WriteLine("Invalid credits format.");
                return;
            }

            var newCourse = new Course
            {
                Name = name ?? string.Empty,
                Duration = duration,
                Description = description ?? string.Empty,
                Credits = credits
            };

            await courseRepo.AddCourseAsync(newCourse);
            Console.WriteLine("Course added successfully.");
        }

        private static async Task EnrollStudentAsync(EnrollmentRepository enrollmentRepo, StudentRepository studentRepo, CourseRepository courseRepo)
        {
            Console.Write("Enter student ID: ");
            if (!int.TryParse(Console.ReadLine(), out var studentId))
            {
                Console.WriteLine("Invalid student ID.");
                return;
            }

            Console.Write("Enter course ID: ");
            if (!int.TryParse(Console.ReadLine(), out var courseId))
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
    }
}
