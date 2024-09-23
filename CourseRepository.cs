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

        // Additional CRUD methods (Update, Delete, Get, etc.) can be added here
    }
}
