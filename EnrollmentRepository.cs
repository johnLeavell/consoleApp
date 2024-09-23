using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace sqltest
{
    public class EnrollmentRepository
    {
        private readonly string connectionString;

        public EnrollmentRepository(IConfiguration configuration)
        {
            connectionString = "Host=localhost;Username=postgres;Password=password;Database=postgres";
                //?? throw new InvalidOperationException("Connection string not found in configuration.");
        }

        public async Task EnrollStudentAsync(Enrollment enrollment)
        {
            try
            {
                await using var dataSource = NpgsqlDataSource.Create(connectionString);
                await using var cmd = dataSource.CreateCommand(@"
                    INSERT INTO enrollments (student_id, course_id, enrolled_date) 
                    VALUES (@StudentId, @CourseId, @EnrolledDate)");
                cmd.Parameters.AddWithValue("StudentId", enrollment.StudentId);
                cmd.Parameters.AddWithValue("CourseId", enrollment.CourseId);
                cmd.Parameters.AddWithValue("EnrolledDate", enrollment.EnrolledDate);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Student enrolled successfully.");
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
