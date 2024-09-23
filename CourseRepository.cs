using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace sqltest
{
    public class CourseRepository
    {
        private readonly string connectionString;

        public CourseRepository(IConfiguration configuration)
        {
            connectionString = "Host=localhost;Username=postgres;Password=password;Database=postgres";
                //?? throw new InvalidOperationException("Connection string not found in configuration.");
        }

        public async Task AddCourseAsync(Course course)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand(@"
                    INSERT INTO courses (name, duration, description, credits) 
                    VALUES (@Name, @Duration, @Description, @Credits)");
                cmd.Parameters.AddWithValue("Name", course.Name);
                cmd.Parameters.AddWithValue("Duration", course.Duration);
                cmd.Parameters.AddWithValue("Description", course.Description);
                cmd.Parameters.AddWithValue("Credits", course.Credits);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Course added successfully.");
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            var courses = new List<Course>();

            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand("SELECT * FROM courses");
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    courses.Add(new Course
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Duration = reader.GetTimeSpan(2),
                        Description = reader.GetString(3),
                        Credits = reader.GetInt32(4)
                    });
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return courses;
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand("SELECT * FROM courses WHERE id = @Id");
                cmd.Parameters.AddWithValue("Id", id);
                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Course
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Duration = reader.GetTimeSpan(2),
                        Description = reader.GetString(3),
                        Credits = reader.GetInt32(4)
                    };
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return null;
        }
    }
}