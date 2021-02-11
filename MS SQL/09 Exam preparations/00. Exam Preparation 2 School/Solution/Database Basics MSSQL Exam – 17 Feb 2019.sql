--Section 1. DDL (30 pts)
CREATE DATABASE School

GO

USE School

GO

CREATE TABLE Students(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	MiddleName NVARCHAR(25),
	LastName NVARCHAR(30) NOT NULL,
	Age INT NOT NULL CHECK(Age > 0),
	Address NVARCHAR(50),
	Phone NVARCHAR(10)
)

CREATE TABLE Subjects(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(20) NOT NULL,
	Lessons INT NOT NULL CHECK(Lessons > 0)
)

CREATE TABLE StudentsSubjects(
	Id INT PRIMARY KEY IDENTITY,
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL,
	Grade DECIMAL(18,2) NOT NULL CHECK(Grade >= 2 AND Grade <= 6)
)

CREATE TABLE Exams(
	Id INT PRIMARY KEY IDENTITY,
	Date DATE,
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL
)

CREATE TABLE StudentsExams(
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	ExamId INT FOREIGN KEY REFERENCES Exams(Id) NOT NULL,
	Grade DECIMAL(18,2) NOT NULL CHECK(Grade >= 2 AND Grade <=6)
	PRIMARY KEY(StudentId, ExamId)
)

CREATE TABLE Teachers(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(20) NOT NULL,
	LastName NVARCHAR(20) NOT NULL,
	[Address] NVARCHAR(20) NOT NULL,
	Phone CHAR(10),
	SubjectId INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL
)

CREATE TABLE StudentsTeachers(
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	TeacherId INT FOREIGN KEY REFERENCES Teachers(Id) NOT NULL,
	PRIMARY KEY (StudentId, TeacherId)
)

--2. Insert
INSERT INTO Teachers(FirstName, LastName, Address, Phone, SubjectId) VALUES
('Ruthanne',	'Bamb',	'84948 Mesta Junction',	'3105500146',	6),
('Gerrard',	'Lowin',	'370 Talisman Plaza',	'3324874824',	2),
('Merrile',	'Lambdin',	'81 Dahle Plaza',	'4373065154',	5),
('Bert',	'Ivie',	'2 Gateway Circle',	'4409584510',	4)

INSERT INTO Subjects(Name, Lessons) VALUES
('Geometry', 12),
('Health', 10),
('Drama', 7),
('Sports',9)

--3. Update
UPDATE StudentsSubjects
SET Grade = 6.00
WHERE SubjectId IN (1,2) AND Grade >= 5.50

--4. Delete
DELETE FROM StudentsTeachers
WHERE TeacherId IN (SELECT ID FROM Teachers
						WHERE Phone LIKE '%72%')

DELETE FROM Teachers
WHERE Phone LIKE '%72%'

--Custom query
SELECT * FROM Teachers
WHERE RIGHT(Phone,2) LIKE '%78'

SELECT * FROM Teachers
WHERE Phone LIKE '%78%'

(SELECT RIGHT(Phone,2) FROM Teachers)

--Section 3. Querying (40 pts)
--5. Teen Students
SELECT FirstName, LastName, Age 
FROM Students
WHERE Age >= 12
ORDER BY FirstName, LastName

--6. Students Teachers
SELECT FirstName, LastName, COUNT(st.TeacherId) 
FROM Students AS s
JOIN StudentsTeachers AS st ON s.Id = st.StudentId
GROUP BY FirstName, LastName

--7. Students to Go
SELECT CONCAT(FirstName, ' ', LastName) AS [Full Name]
FROM Students AS s
LEFT JOIN StudentsExams AS se ON s.Id = se.StudentId
WHERE se.ExamId IS NULL
GROUP BY FirstName, LastName

--8. Top Students
SELECT TOP(10) FirstName, LastName, FORMAT(AVG(se.Grade), 'N2') AS [Grade]
FROM Students AS s
JOIN StudentsExams AS se ON s.Id = se.StudentId
GROUP BY FirstName, LastName 
ORDER BY [Grade] DESC, FirstName, LastName

--9. Not So In The Studying
SELECT CONCAT(FirstName, ' ', ISNULL(MiddleName + ' ', ''), LastName) AS [Full Name]
FROM Students AS s
LEFT JOIN StudentsSubjects AS ss ON s.Id = ss.StudentId
WHERE ss.StudentId IS NULL
ORDER BY [Full Name]

--10. Average Grade per Subject
SELECT s.Name, AVG(ss.Grade) AS [AverageGrade]  
FROM Subjects AS s
JOIN StudentsSubjects AS ss ON s.Id = ss.SubjectId
GROUP BY s.Name, s.Id
ORDER BY s.Id

--second query left part by Exams
SELECT g.Name, g.[AverageGrade] FROM(
SELECT s.Name, s.Id, AVG(se.Grade) AS [AverageGrade]  
FROM Subjects AS s
JOIN Exams AS e ON s.Id = e.SubjectId
JOIN StudentsExams AS se ON e.Id = se.ExamId
GROUP BY s.Name, s.Id) AS g
ORDER BY g.Id

GO
--Section 4. Programmability (20 pts)
--11. Exam Grades
CREATE OR ALTER FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(18,2))
RETURNS VARCHAR(MAX)
AS
	BEGIN
		DECLARE @CheckId INT = (SELECT Id FROM Students WHERE Id = @studentId)

		IF(@CheckId IS NULL)
			RETURN 'The student with provided id does not exist in the school!';

		DECLARE @StudentFirstName VARCHAR(50) = (SELECT FirstName FROM Students WHERE Id = @studentId) 
		
		IF(@Grade > 6.00)
			RETURN 'Grade cannot be above 6.00!';

		DECLARE @Result INT = (SELECT COUNT(*)
	                        FROM Students AS s
							JOIN StudentsExams AS se ON s.Id = se.StudentId
						   WHERE s.Id = @studentId AND se.Grade BETWEEN @grade AND @grade + 0.50)

		RETURN CONCAT('You have to update ',@Result,' grades for the student ',@StudentFirstName)
	END

GO
SELECT dbo.udf_ExamGradesToUpdate(12, 6.20)
SELECT dbo.udf_ExamGradesToUpdate(12, 5.50)
SELECT dbo.udf_ExamGradesToUpdate(121, 5.50)

GO
--12. Exclude from school
CREATE PROCEDURE usp_ExcludeFromSchool(@StudentId INT) 
AS
	BEGIN 
		DECLARE @CheckStudentId INT = (SELECT Id FROM Students WHERE Id = @StudentId)
	
		IF(@CheckStudentId IS NULL)
			THROW 50001, 'This school has no student with the provided id!', 1;

		DELETE FROM StudentsExams
		WHERE StudentId = @StudentId;

		DELETE FROM StudentsTeachers
		WHERE StudentId = @StudentId;

		
		DELETE FROM StudentsSubjects
		WHERE StudentId = @StudentId;

		
		DELETE FROM Students
		WHERE Id = @StudentId


	END
GO







