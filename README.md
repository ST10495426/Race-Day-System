# RaceDay - Event Management System

## System Description
RaceDay is a full-stack event management platform designed for the South African road running, walking, and cycling community. It allows Organisers to create and manage events, while Participants can browse events, enrol, and track their results.

## Roles
### Organiser
- Create, edit, and delete events
- Manage event categories
- Capture participant results
- View all event enrolments

### Participant
- Create an account and log in
- Browse events
- Enrol in events by selecting a category
- View their own enrolments and results

## CI/CD
![CI Build]<img width="428" height="65" alt="green light" src="https://github.com/user-attachments/assets/fa3f87ec-b2f8-496f-924a-9e49f1859284" />
![CI Build] <img width="182" height="61" alt="green mark" src="https://github.com/user-attachments/assets/8bc53725-9f3c-4295-8394-02fc046889d6" />


## Video Presentation
Part one: [YouTube Video Link](https://youtu.be/SjGEt7v-eVg)
Part two: [YouTube Video Link]
## Part 1 Setup Instructions
1. Clone the repository
2. Open the SQL script in SSMS and run it on a fresh database
3. Review the ERD and API endpoint plan in the /docs folder

## Part 2 Setup Instructions
### Prerequisites
- Visual Studio 2022 or 2026
- .NET 8 SDK
- SQL Server (Express or Developer Edition)
- SQL Server Management Studio (SSMS)

### Steps to Run the API
1. **Clone the repository:**
2. **Check out the Part 2 branch:**
3. **Open the solution in Visual Studio:**
4. **Update the connection string:**
5. **Apply EF Core migrations**
6. **Run the API**


## Tech Stack
- **ASP.NET Core 8 Web API** (C#)
- **Entity Framework Core 8** (Code-First)
- **SQL Server** (LocalDB / Express)
- **Session-based authentication** (server-side sessions)
- **BCrypt** for password hashing
- **Swagger / OpenAPI** for API documentation
- **xUnit** for unit testing
- **GitHub Actions** for CI/CD

## Project Structure
RaceDay/
├── .github/workflows
│ ├── ci.yml
│ └── part2-ci.yml
├── docs/
│ ├── ERD.png
│ ├── API_Endpoint_Plan.pdf
│ └── RaceDay_Schema.sql
├── Part2/
│ └── RaceDay/
│ ├── RaceDay.slnx
│ ├── RaceDay.Api/ # Main API project
│ └── RaceDay.Api.Tests/ # Unit test project
├── README.md
└── .gitignore
