# RaceDay - Event Management System

## System Description
RaceDay is a full-stack event management platform designed for the South African road running, walking, and cycling community. It allows Organisers to create and manage events, while Participants can browse events, enrol, and track their results.

This repository contains the complete system:
- **Part 1:** System Planning (ERD, Endpoint Plan, SQL Script)
- **Part 2:** RESTful API (ASP.NET Core Web API)
- **Part 3:** MVC Web Application (ASP.NET Core MVC)

## Roles
### Organiser
- Create, edit, and delete events
- Manage event categories
- Capture participant results
- View all event enrolments for their events
- Upload event banner images

### Participant
- Create an account and log in
- Browse upcoming events
- Enrol in events by selecting a category
- View their own enrolments and results
- Upload a profile picture
  
## CI/CD
![CI Build]<img width="428" height="65" alt="green light" src="https://github.com/user-attachments/assets/fa3f87ec-b2f8-496f-924a-9e49f1859284" />
![CI Build] <img width="182" height="61" alt="green mark" src="https://github.com/user-attachments/assets/8bc53725-9f3c-4295-8394-02fc046889d6" />
![CI Build]<img width="152" height="23" alt="green" src="https://github.com/user-attachments/assets/120c664b-f40f-4abc-9bd4-220d81f9f3bd" />


## Video Presentation
Part one: [YouTube Video Link](https://youtu.be/SjGEt7v-eVg)
Part two: [YouTube Video Link](https://youtu.be/7EWQPeVoZVw)
Part three: [YouTube Video Link](https://youtu.be/o0tymYDNuQo)
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


## Part 3 Setup Instructions 

### Prerequisites
- Visual Studio 2022 or 2026
- .NET 8 SDK
- SQL Server (Express or Developer Edition)
- SQL Server Management Studio (SSMS)

### Steps
1. **Clone the repository:**

2. **Open the solution:**
Open `Part2/RaceDay/RaceDay.slnx` in Visual Studio.

3. **Update Connection Strings:**
- In **RaceDay.Api**, open `appsettings.json` and update the `DefaultConnection` to point to your SQL Server.
- In **RaceDay.Api**, update the `AzureBlobStorage` connection string with your Azure Storage account details.

4. **Apply Migrations:**
- Open the Package Manager Console.
- Set the Default Project to `RaceDay.Api`.
- Run: `Update-Database`

5. **Run the Applications:**
- Configure Visual Studio to start both `RaceDay.Api` and `RaceDay.Mvc` at the same time.
- Press **F5**. Swagger will open for the API, and the MVC app will open in another tab.
- Ensure the `BaseAddress` in `RaceDay.Mvc/Program.cs` matches the HTTPS port of your running API.

## Docker Setup

The MVC application is containerized using Docker.

1. **Build the Docker image:**
Navigate to the `Part3/RaceDayMvc/RaceDay.Mvc` folder in your terminal and run:
```bash
docker build -t raceday-mvc .

Run the conatiner:
docker run -d -p 8080:8080 -p 8081:8081 --name raceday-mvc-app raceday-mvc

Access the app:
Open your browser and go to http://localhost:8080.













## Tech Stack
- **Backend API:** ASP.NET Core 8 Web API, Entity Framework Core 8, SQL Server
- **Frontend App:** ASP.NET Core 8 MVC, Bootstrap 5
- **Authentication:** Session-based (server-side cookies)
- **Storage:** Azure Blob Storage (for images)
- **Testing:** xUnit
- **CI/CD:** GitHub Actions
- **Containerization:** Docker



