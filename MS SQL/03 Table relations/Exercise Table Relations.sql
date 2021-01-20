USE Example

--Problem 1.	One-To-One Relationship
CREATE TABLE Passports(
	PassportId INT NOT NULL PRIMARY KEY IDENTITY(101, 1),
	PassportNumber VARCHAR(8) NOT NULL UNIQUE
)

INSERT INTO Passports (PassportNumber) VALUES
('N34FG21B'), ('K65LO4R7'), ('ZE657QP2')

CREATE TABLE Persons(
	PersonlId INT PRIMARY KEY NOT NULL  IDENTITY(1,1),
	FirstName NVARCHAR(30) NOT NULL,
	Salary DECIMAL(8,2) NOT NULL,
	PassportID INT FOREIGN KEY REFERENCES Passports(PassportId) UNIQUE NOT NULL
)

INSERT INTO Persons (FirstName, Salary, PassportID) VALUES
('Roberto', 43300, 102),
('Tom', 56100, 103),
('Yana', 60200, 101)

--Second way One-TO-One
CREATE TABLE PersonsPassports(
	PassportID INT FOREIGN KEY REFERENCES Passports(PassportId) NOT NULL UNIQUE,
	PersonID INT FOREIGN KEY REFERENCES Persons(PassportID) NOT NULL UNIQUE
)

INSERT INTO PersonsPassports(PassportID, PersonID) VALUES
(101, 101), (102, 102), (103, 103)

SELECT FirstName, Salary, PassportNumber
FROM Persons AS p
JOIN PersonsPassports AS pp ON p.PassportID = pp.PassportID
JOIN Passports AS s ON pp.PassportID = s.PassportId

DROP TABLE PersonsPassports
DROP TABLE Passports
DROP TABLE Persons

--Problem 2.	One-To-Many Relationship
CREATE TABLE Manufacturers(
	ManufacturerID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] NVARCHAR(30) NOT NULL,
	EstablishedOn DATE NOT NULL
)

INSERT INTO Manufacturers ([Name], EstablishedOn) VALUES
('BMW','03-07-1916'), ('Tesla','01-01-2003'), ('Lada','05-01-1966')

CREATE TABLE Models(
	ModelID INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] NVARCHAR(30) NOT NULL,
	ManufacturerID INT NOT NULL FOREIGN KEY REFERENCES Manufacturers(ManufacturerID)
)

INSERT INTO Models ([Name], ManufacturerID) VALUES
('X1',1), ('i6',2), ('Model S',2), ('Model X',2), ('Model 3',2), ('Nova',3)

DROP TABLE Manufacturers
DROP TABLE Models

--Problem 3.	Many-To-Many Relationship
CREATE TABLE Students(
	StudentID INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(30) NOT NULL
)

CREATE TABLE Exams(
	ExamID INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(30) NOT NULL
)

CREATE TABLE StudentsExams(
	StudentID INT NOT NULL FOREIGN KEY REFERENCES Students(StudentID),
	ExamID INT NOT NULL FOREIGN KEY REFERENCES Exams(ExamID),
	PRIMARY KEY (StudentID, ExamID)
)

INSERT INTO Students([StudentID], [Name]) VALUES
(1, 'Mila'), (2, 'Toni'), (3, 'Ron')

INSERT INTO Exams(ExamID, [Name]) VALUES
('101', 'SpringMVC'), ('102', 'Neo4j'), ('103', 'Oracle 11g')

INSERT INTO StudentsExams (StudentID, ExamID) VALUES
(1,101), (1,102), (2,101), (3,103), (2,102), (2,103)

ALTER TABLE StudentsExams
ADD CONSTRAINT PK_StEx_CompositeKey
PRIMARY KEY(StudentID, ExamID)

ALTER TABLE StudentsExams
ADD FOREIGN KEY (StudentID) REFERENCES Students(StudentID)

ALTER TABLE StudentsExams
ADD FOREIGN KEY (ExamID) REFERENCES Exams(ExamID)

--Problem 4.	Self-Referencing
CREATE TABLE Teachers(
	TeacherID INT NOT NULL PRIMARY KEY,
	[Name] NVARCHAR(30) NOT NULL,
	ManagerID INT FOREIGN KEY REFERENCES Teachers(TeacherID)
)

INSERT INTO Teachers (TeacherID, [Name], ManagerID) VALUES
(101, 'John', null), 
(102, 'Maya', 106), 
(103, 'Silvia', 106), 
(104, 'Ted', 105), 
(105, 'Mark', 101), 
(106, 'Greta', 101)

UPDATE Teachers
SET ManagerID = 101
WHERE [Name] IN ('Mark', 'Greta')

UPDATE Teachers
SET ManagerID = 106
WHERE [Name] = 'Maya' OR [Name] = 'Silvia'

UPDATE Teachers
SET ManagerID = 105
WHERE [Name] = 'Ted'

--Problem 5.	Online Store Database
CREATE TABLE Cities(
	CityID INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL
)

CREATE TABLE Customers(
	CustomerID INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	Birthday DATE,
	CityID INT FOREIGN KEY REFERENCES Cities(CityID) NOT NULL
)

CREATE TABLE Orders(
	OrderID INT NOT NULL PRIMARY KEY IDENTITY,
	CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID) NOT NULL
)

CREATE TABLE ItemTypes(
	ItemTypeID INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL
)

CREATE TABLE Items(
	ItemID INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	ItemTypeID INT FOREIGN KEY REFERENCES ItemTypes(ItemTypeID) NOT NULL
)

CREATE TABLE OrderItems(
	OrderID INT FOREIGN KEY REFERENCES Orders(OrderID) NOT NULL,
	ItemID INT FOREIGN KEY REFERENCES Items(ItemID) NOT NULL,
	PRIMARY KEY (OrderID, ItemID)
)

--Problem 6.	University Database
CREATE TABLE Majors(
	MajorID INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL
)

CREATE TABLE Students(
	StudentID INT NOT NULL PRIMARY KEY IDENTITY,
	StudentNumber CHAR(10) NOT NULL,
	StudentName VARCHAR(50) NOT NULL,
	MajorID INT FOREIGN KEY REFERENCES Majors(MajorID)
)

CREATE TABLE Payments(
	PaymentID INT NOT NULL PRIMARY KEY IDENTITY,
	PaymentDate DATETIME2 NOT NULL,
	PaymentAmount DECIMAL(15,2) NOT NULL,
	StudentID INT NOT NULL FOREIGN KEY REFERENCES Students(StudentID)
)

CREATE TABLE Subjects(
	SubjectID INT NOT NULL PRIMARY KEY IDENTITY,
	SubjectName VARCHAR(50) NOT NULL
)

CREATE TABLE Agenda(
	StudentID INT NOT NULL FOREIGN KEY REFERENCES Students(StudentID),
	SubjectID INT NOT NULL FOREIGN KEY REFERENCES Subjects(SubjectID),
	PRIMARY KEY (StudentID, SubjectID)
)

--Problem 7.	SoftUni Design
--Create an E/R Diagram of the SoftUni Database. 
--There are some special relations you should check out: Employees are self-referenced (ManagerID) 
--and Departments have One-to-One with the Employees (ManagerID) while 
--the Employees have One-to-Many (DepartmentID). You might find it interesting how it looks on the diagram. 

--Problem 8.	Geography Design
--Create an E/R Diagram of the Geography Database.

--Problem 9.	*Peaks in Rila
USE Geography

SELECT MountainRange, PeakName, Elevation
FROM Peaks AS p
JOIN Mountains AS m ON p.MountainID = m.ID
WHERE MountainRange = 'Rila'
ORDER BY Elevation DESC


--*****************CUSTOM SELECTION********************--
--If we start from the middle, we will loose some information about the peaks
SELECT c.CountryCode, IsoCode, CountryName, c.CurrencyCode, c.ContinentCode, [Population], AreaInSqKm,
		Capital, MountainRange, PeakName, Elevation, [Description], RiverName, [Length], DrainageArea,
		AverageDischarge, Outflow
FROM Countries AS c
JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
JOIN Mountains AS m ON m.Id = mc.MountainId
JOIN Peaks AS p ON p.MountainId = m.Id
JOIN Currencies AS cur ON c.CurrencyCode = cur.CurrencyCode
JOIN Continents AS con ON c.ContinentCode = con.ContinentCode
JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
JOIN Rivers AS r ON cr.RiverId = r.Id
Order By CountryCode

SELECT *
FROM Countries AS c
JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
JOIN Mountains AS m ON m.Id = mc.MountainId
JOIN Peaks AS p ON p.MountainId = m.Id
JOIN Currencies AS cur ON c.CurrencyCode = cur.CurrencyCode
JOIN Continents AS con ON c.ContinentCode = con.ContinentCode
JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
ORDER BY mc.MountainId

--Start from the peaks
SELECT * FROM Peaks AS p
JOIN Mountains AS m ON p.MountainId = m.Id
JOIN MountainsCountries AS mc ON mc.MountainId = m.Id
JOIN Countries AS c ON mc.CountryCode = c.CountryCode
JOIN Currencies AS cur ON c.CurrencyCode = cur.CurrencyCode
JOIN Continents AS con ON c.ContinentCode = con.ContinentCode
FULL JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
FULL JOIN Rivers AS r ON cr.RiverId = r.Id
--WHERE mc.CountryCode = 'BG'
ORDER BY p.MountainId

SELECT * FROM Rivers AS r
JOIN Mountains AS m ON r.MountainID = m.Id
JOIN CountriesRivers AS cr ON r.Id = cr.RiverId
JOIN Countries AS c ON cr.CountryCode = c.CountryCode
JOIN Currencies AS cur ON c.CurrencyCode = cur.CurrencyCode
JOIN Continents AS con ON c.ContinentCode = con.ContinentCode
FULL JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode

SELECT * FROM Mountains
SELECT * FROM Countries
SELECT * FROM Rivers
SELECT * FROM MountainsCountries

ALTER TABLE Rivers
ADD Spring INT FOREIGN KEY REFERENCES Mountains(Id)

EXEC sp_RENAME 'Rivers.Spring' , 'MountainID', 'COLUMN'
