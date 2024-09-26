# SQLTest Console Application

## Overview

SQLTest is a C# console application designed to manage a simple academic database. It allows users to add students, courses, and enrollments, and view and manage these entities through a command-line interface (CLI). The application uses PostgreSQL as its database and various .NET libraries for configuration, dependency injection, and console interface enhancements.

## Features

- **Crud functionality for student, Course, Enrollments**
- **Search for a student by ID**
- **Search for students by name**
- **Export students to CSV**
- **Export students to JSON**

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- A PostgreSQL database with the following tables:
  - `students`
  - `courses`
  - `enrollments`

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/yourusername/sqltest.git
cd sqltest
```

## Configure the Database
Ensure your PostgreSQL database is set up with the following tables:

- `students`
- `courses`
- `enrollments`

## Install Dependencies
Run the following command to restore the necessary NuGet packages:

```bash
dotnet restore
```
### Build and Run

## Build the project:

```
dotnet build
```
## Run the project:
```
dotnet run
```

## Project Structure
- Program.cs: The main entry point of the application.
- DatabaseOperations.cs: Handles the creation of database tables.
- EnrollmentRepository.cs: Manages database operations related to enrollments.
- StudentRepository.cs: Manages database operations related to students.
 - CourseRepository.cs: Manages database operations related to courses.
- Helpers: Contains helper classes for input validation and data export.

## Dependencies
The project uses the following NuGet packages:

- CsvHelper
- Microsoft.Data.SqlClient
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.EnvironmentVariables
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.DependencyInjection
- Newtonsoft.Json
- Npgsql
- Spectre.Console
  
## Usage
```
Main Menu

1. Add a new student
2. Add a new course
3. Enroll a student in a course
4. View all students
5. View all courses
6. View all enrollments
7. Search for a student by ID
8. Search for students by name
9. Update a student’s details
10. Delete a student
11. Export students to CSV
12. Export students to JSON
13. Exit
```
## Example Commands
```
Add a Student
Enter first name: John
Enter last name: Doe
Enter email: john.doe@example.com
```
```
Add a Course
Enter course name: Introduction to Programming
Enter course duration (e.g., 2:00:00 for 2 hours): 3:00:00
Enter course description: Basic programming concepts
Enter course credits: 3
```
```
Enroll a Student
Enter student ID: 1
Enter course ID: 2
```
## View All Students
- Displays a table of all students with their ID, first name, last name, email, and registration date.

## View All Courses
- Displays a table of all courses with their ID, name, duration, description, and credits.

## View All Enrollments
- Displays a table of all enrollments with student ID, student name, course ID, course name, and enrolled date.

## Contributing
- Contributions are welcome! Please fork the repository and submit a pull request with your changes.

## License
- This project is licensed under the MIT License.

## Acknowledgements
- **Spectre.Console**
- **Npgsql**
- **CsvHelper**
- **Newtonsoft.Json**

