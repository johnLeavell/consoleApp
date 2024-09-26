console.log("Hello, TypeScript!");
async function fetchStudents() {
    const response = await fetch('http://localhost:5000/api/students');
    const students = await response.json();
    console.log(students);
}

fetchStudents();