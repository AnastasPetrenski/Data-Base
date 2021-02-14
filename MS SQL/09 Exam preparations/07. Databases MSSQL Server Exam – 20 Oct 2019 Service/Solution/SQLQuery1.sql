--Section 1. DDL (30 pts)
CREATE DATABASE Service

GO

USE Service

GO

CREATE TABLE Users(
	Id INT PRIMARY KEY IDENTITY,
	Username VARCHAR(30) UNIQUE NOT NULL,
	Password VARCHAR(50) NOT NULL,
	Name VARCHAR(50),
	Birthdate DATETIME,
	Age INT CHECK(Age BETWEEN 14 AND 110),
	Email VARCHAR(50) NOT NULL
)

CREATE TABLE Status(
	Id INT PRIMARY KEY IDENTITY,
	Label VARCHAR(30) NOT NULL
)

CREATE TABLE Departments(
	Id INT PRIMARY KEY IDENTITY,
	Name VARCHAR(50) NOT NULL
)

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY,
	Name VARCHAR(50) NOT NULL,
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id) NOT NULL
)

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(25),
	LastName VARCHAR(25),
	Birthdate DATETIME,
	Age INT CHECK(Age >=18 AND Age <= 110),
	DepartmentId INT FOREIGN KEY REFERENCES Departments(Id)
)

CREATE TABLE Reports(
	Id INT PRIMARY KEY IDENTITY,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	StatusId INT FOREIGN KEY REFERENCES Status(Id) NOT NULL,
	OpenDate DATETIME NOT NULL,
	CloseDate DATETIME,
	Description VARCHAR(200) NOT NULL,
	UserId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id)
)

--Section 2. DML (10 pts)
--2.	Insert
INSERT INTO Employees(FirstName, LastName, Birthdate, DepartmentId) VALUES
('Marlo',	'O''Malley',	'1958-9-21',	1),
('Niki',	'Stanaghan',	'1969-11-26',	4),
('Ayrton',	'Senna',	'1960-03-21',	9),
('Ronnie',	'Peterson',	'1944-02-14',	9),
('Giovanna',	'Amati',	'1959-07-20',	5);

INSERT INTO Reports(CategoryId, StatusId, OpenDate, CloseDate, Description, UserId, EmployeeId) VALUES
(1,	1,	'2017-04-13', NULL		,'Stuck Road on Str.133',	6,	2),
(6,	3,	'2015-09-05',	'2015-12-06',	'Charity trail running',	3,	5),
(14,	2,	'2015-09-07',	NULL,	'Falling bricks on Str.58',	5,	2),
(4,	3,	'2017-07-03',	'2017-07-06',	'Cut off streetlight on Str.11',	1,	1)

--3.	Update
UPDATE Reports
SET CloseDate = GETDATE()
WHERE CloseDate IS NULL

--4.	Delete
DELETE FROM Reports
WHERE StatusId = 4

--Section 3. Querying (40 pts)
--5.	Unassigned Reports
SELECT Description, FORMAT(OpenDate, 'dd-MM-yyyy') AS [OpenDate] 
FROM Reports AS r
LEFT JOIN Employees AS e ON r.EmployeeId = e.Id
WHERE e.Id IS NULL
ORDER BY r.OpenDate, r.Description

--Order by return wrong result
SELECT Description, FORMAT(OpenDate, 'dd-MM-yyyy') AS [OpenDate]
FROM Reports
WHERE EmployeeId IS NULL
ORDER BY OpenDate, Description

--Return more results
SELECT * 
FROM Reports AS r
FULL JOIN Employees AS e ON r.EmployeeId = e.Id
WHERE r.EmployeeId IS NULL

--6.	Reports & Categories
SELECT r.Description, c.Name AS [CategoryName] 
FROM Reports AS r
JOIN Categories AS c ON r.CategoryId = c.Id
ORDER BY r.Description, c.Name

--7.	Most Reported Category
SELECT TOP(5) c.Name AS [CategoryName], 
			  COUNT(r.CategoryId) AS [ReportsNumber] 
FROM Categories AS c
JOIN Reports AS r ON c.Id = r.CategoryId
GROUP BY c.Name
ORDER BY [ReportsNumber] DESC, [CategoryName]

--8.	Birthday Report
SELECT u.Username, c.Name AS [CategoryName]
FROM Users AS u
JOIN Reports AS r ON u.Id = r.UserId
JOIN Categories AS c ON r.CategoryId = c.Id
WHERE (DAY(u.Birthdate) = DAY(r.OpenDate)) AND (MONTH(u.Birthdate) = MONTH(r.OpenDate))
ORDER BY u.Username, c.Name

--9.	Users per Employee 
SELECT CONCAT(e.FirstName, ' ', e.LastName) AS [FullName], 
		COUNT(u.Username) AS [UsersCount] 
FROM Employees AS e
JOIN Reports AS r ON e.Id = r.EmployeeId
JOIN Users AS u ON r.UserId = u.Id
GROUP BY e.FirstName, e.LastName
ORDER BY COUNT(u.Username) DESC, [FullName]

--10.	Full Info
SELECT  ISNULL(CONCAT(e.FirstName, ' ', e.LastName), 'None') AS [Employee],
		ISNULL(d.Name, 'None') AS [Department],
		ISNULL(c.Name, 'None') AS [Category],
		ISNULL(r.Description, 'None') AS [Description],
		ISNULL(FORMAT(r.OpenDate, 'dd.MM.yyyy'), 'None') AS [OpenDate],
		ISNULL(s.Label, 'None') AS [Status],
		ISNULL(u.Name, 'None') AS [User]
FROM Reports AS r
LEFT JOIN Employees AS e ON r.EmployeeId = e.Id
JOIN Categories AS c ON r.CategoryId = c.Id
JOIN Departments AS d ON c.DepartmentId = d.Id
JOIN Users AS u ON r.UserId = u.Id
JOIN Status AS s ON r.StatusId = s.Id 
ORDER BY e.FirstName DESC, e. LastName DESC, [Department], [Category], r.Description, [OpenDate],
			[Status], [User]

GO
--Section 4. Programmability (20 pts)
--11.	Hours to Complete
CREATE FUNCTION udf_HoursToComplete(@StartDate DATETIME, @EndDate DATETIME)
RETURNS INT
AS
	BEGIN
		IF(@StartDate IS NULL OR @EndDate IS NULL)
			RETURN 0;
	
		RETURN DATEDIFF(DAY, @StartDate, @EndDate);
	END

GO

SELECT dbo.udf_HoursToComplete(OpenDate, CloseDate) AS TotalHours
   FROM Reports

GO
--12.	Assign Employee
CREATE PROCEDURE usp_AssignEmployeeToReport(@EmployeeId INT, @ReportId INT)
AS
	DECLARE @EmployeeDepartmentId INT = (SELECT DepartmentId FROM Employees WHERE Id = @EmployeeId)
	
	DECLARE @ReportDepartmentId INT = (SELECT e.DepartmentId 
										FROM Reports AS r 
										 JOIN Employees AS e ON r.EmployeeId = e.Id
										 WHERE r.Id = @ReportId)

	IF(@EmployeeDepartmentId <> @ReportDepartmentId)
		THROW 50001, 'Employee doesn''t belong to the appropriate department!',1;

	UPDATE Reports
	SET EmployeeId = @EmployeeId
	WHERE Id = @ReportId
GO


EXEC usp_AssignEmployeeToReport 12,1

DECLARE @err_msg AS NVARCHAR(MAX);
BEGIN TRY
  EXEC usp_AssignEmployeeToReport 12,1
END TRY

BEGIN CATCH
  SET @err_msg = ERROR_MESSAGE();
  SELECT @err_msg
END CATCH











