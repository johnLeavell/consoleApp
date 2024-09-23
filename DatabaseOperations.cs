using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace sqltest
{
    public class DatabaseOperations
    {
        private string connectionString = "Host=localhost;Username=postgres;Password=password;Database=postgres";

        public async Task CreateTablesAsync()
        {
            try
            {
                // List of CREATE TABLE statements
                var statements = new List<string>
                {
                    @"
                    CREATE TABLE IF NOT EXISTS courses (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(255) NOT NULL UNIQUE,
                        duration INTERVAL NOT NULL,
                        description TEXT,
                        credits INT
                    )",
                    @"
                    CREATE TABLE IF NOT EXISTS students (
                        id SERIAL PRIMARY KEY,
                        first_name VARCHAR(255) NOT NULL,
                        last_name VARCHAR(255) NOT NULL,
                        email VARCHAR(400) NOT NULL UNIQUE,
                        registration_date DATE NOT NULL
                    )",
                    @"
                    CREATE TABLE IF NOT EXISTS enrollments (
                        student_id INT NOT NULL,
                        course_id INT NOT NULL,
                        enrolled_date DATE NOT NULL,
                        PRIMARY KEY(student_id, course_id)
                    )"
                };

                await using var dataSource = NpgsqlDataSource.Create(connectionString);

                foreach (var statement in statements)
                {
                    await using var cmd = dataSource.CreateCommand(statement);
                    await cmd.ExecuteNonQueryAsync();
                }

                Console.WriteLine("The tables have been created successfully.");
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

        // Additional methods for CRUD operations can be added here
    }
}
