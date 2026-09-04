CREATE DATABASE RaceDay; 

CREATE TABLE [User] ( 

    UserID INT IDENTITY(1,1) PRIMARY KEY, 
    Email NVARCHAR(255) NOT NULL UNIQUE, 
    PasswordHash NVARCHAR(255) NOT NULL, 
    FullName NVARCHAR(255) NOT NULL, 
    DateOfBirth DATE NOT NULL, 
    EmergencyContact NVARCHAR(50) NOT NULL, 
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')), 
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE() 
);
SELECT*FROM [User]

CREATE TABLE EventType ( 
    EventTypeID INT IDENTITY(1,1) PRIMARY KEY, 
    Name NVARCHAR(50) NOT NULL UNIQUE, 
    Description NVARCHAR(255) NULL 
);
SELECT* FROM EventType;

CREATE TABLE Event ( 
    EventID INT IDENTITY(1,1) PRIMARY KEY, 
    OrganiserID INT NOT NULL, 
    EventTypeID INT NOT NULL, 
    Name NVARCHAR(255) NOT NULL, 
    Description NVARCHAR(MAX) NULL, 
    Date DATETIME NOT NULL, 
    Location NVARCHAR(255) NOT NULL, 
    Distance NVARCHAR(20) NOT NULL, 
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(), 

    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserID) REFERENCES [User](UserID), 
    CONSTRAINT FK_Event_EventType FOREIGN KEY (EventTypeID) REFERENCES EventType(EventTypeID) 
);
SELECT* FROM Event;

CREATE TABLE Category ( 
    CategoryID INT IDENTITY(1,1) PRIMARY KEY, 
    EventID INT NOT NULL, 
    Name NVARCHAR(100) NOT NULL, 
    CategoryType NVARCHAR(20) NOT NULL CHECK (CategoryType IN ('Age', 'Distance')), 
    MinValue INT NULL, 
    MaxValue INT NULL, 
   
    CONSTRAINT FK_Category_Event FOREIGN KEY (EventID) REFERENCES Event(EventID), 
    CONSTRAINT CHK_Category_Values CHECK ( 
        (CategoryType = 'Age' AND MinValue IS NOT NULL AND MaxValue IS NOT NULL) OR 
        (CategoryType = 'Distance' AND MinValue IS NOT NULL AND MaxValue IS NOT NULL) 
    )); 
SELECT* FROM Category;

CREATE TABLE Enrolment ( 
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY, 
    ParticipantID INT NOT NULL, 
    EventID INT NOT NULL, 
    CategoryID INT NOT NULL, 
    EnrolmentDate DATETIME NOT NULL DEFAULT GETDATE(), 
    Withdrawn BIT NOT NULL DEFAULT 0, 
  
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (ParticipantID) REFERENCES [User](UserID), 
    CONSTRAINT FK_Enrolment_Event FOREIGN KEY (EventID) REFERENCES Event(EventID), 
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID), 
    CONSTRAINT EventParticipant UNIQUE (ParticipantID, EventID) 
); 
SELECT* FROM Enrolment;

CREATE TABLE Result ( 
    ResultID INT IDENTITY(1,1) PRIMARY KEY, 
    EnrolmentID INT NOT NULL UNIQUE, 
    FinishTime TIME NULL, 
    Position INT NULL, 
    Status NVARCHAR(20) NOT NULL DEFAULT 'Completed'  
        CHECK (Status IN ('Completed', 'DNF', 'DQ', 'DNS')), 
  
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (EnrolmentID) REFERENCES Enrolment(EnrolmentID) 
);
SELECT* FROM Result;

INSERT INTO EventType (Name, Description) VALUES 
('Run', 'Road running events'), 
('Walk', 'Walking and hiking events'), 
('Cycle', 'Cycling events');

SELECT* FROM EventType;

INSERT INTO [User] (Email, PasswordHash, FullName, DateOfBirth, EmergencyContact, Role) 
VALUES ('thabo.mokoena@raceday.co.za', 'hashed_password_1', 'Thabo Mokoena', '1985-06-15', '+27 82 123 4567', 'Organiser'), 
('sarah.smith@raceday.co.za', 'hashed_password_2', 'Sarah Smith', '1990-11-22', '+27 73 987 6543', 'Organiser'), 
('linda.mthembu@gmail.com', 'hashed_password_3', 'Linda Mthembu', '1995-03-10', '+27 64 555 7890', 'Participant'), 
('john.Buthelezi@gmail.com', 'hashed_password_4', 'John Buthelezi', '1988-07-30', '+27 76 333 4444', 'Participant');

SELECT* FROM [User];

INSERT INTO Event (OrganiserID, EventTypeID, Name, Description, [Date], Location, Distance) 
VALUES (1, 1, 'Comrades Marathon', 'The ultimate human race - 90km ultra marathon between Pietermaritzburg and Durban.', '2026-06-16 05:30:00', 'Pietermaritzburg to Durban, KZN', '90km'), 
(2, 3, 'Cape Town Cycle Tour', 'The world''s largest timed cycle race around the Cape Peninsula.', '2026-03-08 06:00:00', 'Cape Town, Western Cape', '109km'), 
(1, 1, 'Soweto Marathon', 'Iconic marathon through the streets of Soweto.', '2026-11-01 06:00:00', 'Soweto, Gauteng', '42.2km');

SELECT* FROM Event;

INSERT INTO Category (EventID, Name, CategoryType, MinValue, MaxValue) 
VALUES (1, 'Junior (Under 20)', 'Age', 0, 19), 
(1, 'Senior (20-39)', 'Age', 20, 39), 
(1, '40-49', 'Age', 40, 49), 
(1, '50-59', 'Age', 50, 59), 
(1, 'Veteran (60+)', 'Age', 60, 999), 
(2, '10km', 'Distance', 10, 10), 
(2, '21km', 'Distance', 21, 21), 
(2, '42.2km', 'Distance', 42, 42), 
(2, '109km', 'Distance', 109, 109), 
(3, 'Under 20', 'Age', 0, 19), 
(3, '20-34', 'Age', 20, 34), 
(3, '35-49', 'Age', 35, 49), 
(3, '50+', 'Age', 50, 999), 
(3, '10km', 'Distance', 10, 10), 
(3, '21km', 'Distance', 21, 21), 
(3, '42.2km', 'Distance', 42, 42); 

SELECT* FROM Category;


INSERT INTO Enrolment (ParticipantID, EventID, CategoryID) 
VALUES (3, 1, 2), 
(3, 2, 5), 
(4, 1, 3), 
(4, 3, 6); 

SELECT* FROM Enrolment;

INSERT INTO Result (EnrolmentID, FinishTime, Position, Status) 
VALUES (1, '07:45:23', 456, 'Completed'), 
(3, NULL, NULL, 'DNF');

SELECT* FROM Result;