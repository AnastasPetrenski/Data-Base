CREATE DATABASE Education

USE Education

CREATE TABLE Students(
	Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[User] NVARCHAR(30) NOT NULL,
	[Full Name] NVARCHAR(100)

)

CREATE TABLE Towns(
	Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] NVARCHAR(50)
	
)

CREATE TABLE Courses(
	Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] NVARCHAR(50) NOT NULL,
	TownId INT NOT NULL FOREIGN KEY REFERENCES Towns(Id),
	NextCourseId INT FOREIGN KEY REFERENCES Courses(Id)
)

CREATE TABLE StudentsCourses(
	StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(Id),
	CourseId INT NOT NULL FOREIGN KEY REFERENCES Courses(Id),
	NextCourseId INT FOREIGN KEY REFERENCES Courses(Id)
)

ALTER TABLE Courses
ADD NextCoursesId INT FOREIGN KEY REFERENCES Courses(Id)

ALTER TABLE Courses
ADD UNIQUE (Id)

ALTER TABLE Courses
ADD CONSTRAINT [UQ_Test] UNIQUE (TownId)

ALTER TABLE StudentsCourses
ADD NextCourseId INT FOREIGN KEY REFERENCES Courses(Id)

ALTER TABLE Students
ADD StudentFK INT

ALTER TABLE Students
ADD CONSTRAINT [UQ_StudentFK] UNIQUE (StudentFK)

INSERT INTO Towns ([Name])
VALUES ('Sofia'), ('Plovdiv'), ('Pleven'), ('Petrich'), ('Varna')

INSERT INTO Courses ([Name], TownId)
VALUES ('C# Basic', 1), ('C# Fundamental', 2), ('C# Advance', 3), ('C# OOP', 4), ('C# DB', 5)

INSERT INTO Students([User], [Full Name]) VALUES 
('LandCruiser', 'Anastas Petrenski'),
('WonderBoy', 'Stoycho Stoychev'),
('PernikBoy', 'Orlin Aleksiev'),
('Kenly', 'Georgi Jekov'),
('Frodo', 'Rosen Zheliaskov')

INSERT INTO StudentsCourses (StudentId, CourseId) VALUES
(1, 1), (1, 2), (2, 1), (3, 1), (4, 1), (5, 1), (3, 2), (3, 3), (3, 4), (3, 5)

SELECT * FROM StudentsCourses
SELECT * FROM Students
SELECT * FROM Courses

--Students in Course
SELECT c.Name, (COUNT([User])) AS Students
FROM Courses AS c
JOIN Towns AS t ON c.Id = t.Id
JOIN StudentsCourses AS st ON st.CourseId = c.Id
JOIN Students AS s ON s.Id = st.StudentId
GROUP BY c.Name

--Select distinct number of courses and students
SELECT COUNT(DISTINCT c.Name) AS Courses, (COUNT([User])) AS Students
FROM Courses AS c
JOIN Towns AS t ON c.Id = t.Id
JOIN StudentsCourses AS st ON st.CourseId = c.Id
JOIN Students AS s ON s.Id = st.StudentId

--Select student's courses and cours's town
SELECT  [Full Name] , c.Name AS Course , t.Name AS Town
FROM Students AS s
JOIN StudentsCourses AS st ON st.StudentId = s.Id
JOIN Courses AS c ON c.Id = st.CourseId
JOIN Towns AS t ON c.TownId = t.ID
ORDER BY [Full Name], Course

-- Select student's next course 
SELECT [Full Name], c.Name AS [Next Course]
FROM Students AS s
JOIN StudentsCourses AS sc ON sc.StudentId = s.Id
JOIN Courses AS c ON c.Id = sc.NextCourseId

--select distinct students and how many courses each of them have
SELECT [Full Name], COUNT(c.Name) AS Course, COUNT(t.Name) AS Town
FROM Students AS s
JOIN StudentsCourses AS st ON st.StudentId = s.Id
JOIN Courses AS c ON c.Id = st.CourseId
JOIN Towns AS t ON c.TownId = t.ID
GROUP BY [Full Name]

--Select current student and his courses and towns count 
SELECT [Full Name], COUNT(c.Name) AS Course, COUNT(t.Name) AS Town
FROM Students AS s
JOIN StudentsCourses AS sc ON sc.StudentId = s.Id
JOIN Courses AS c ON c.Id = sc.CourseId
JOIN Towns AS t ON c.TownId = t.ID
WHERE [USER] = 'PernikBoy'
GROUP BY [Full Name]


