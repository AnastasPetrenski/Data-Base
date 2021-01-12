CREATE DATABASE CarRental

USE CarRental

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	CategoryName VARCHAR(50) NOT NULL,
	DailyRate INT,
	WeeklyRate INT,
	MonthlyRate INT,
	WeekendRate INT
)

INSERT INTO Categories (CategoryName)
VALUES ('Extreme'),
		('OffRoad'),
		('Race')

CREATE TABLE Cars(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	PlateNumber VARCHAR(8) NOT NULL,
	Manufacturer NVARCHAR (50),
	Model NVARCHAR(50),
	CarYear SMALLINT,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	Doors TINYINT,
	Picture VARBINARY(MAX) CHECK
	(DATALENGTH(Picture) <= 2000 * 1024),
	Condition BIT,
	Available BIT
)

INSERT INTO Cars (PlateNumber, CategoryId)
VALUES ('AA1122WW', 1),
		('AA0000WW', 2),
		('AA9999WW', 3)

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	Title NVARCHAR(50) NOT NULL,
	Notes NVARCHAR(200)
)

INSERT INTO Employees (FirstName, LastName, Title)
VALUES ('Anastas', 'Petrenski', 'Driver'),
		('Joko', 'Peshich', 'Driver'),
		('Bango', 'Vasil', 'Driver')

CREATE TABLE Customers(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	DriverLicenceNumber CHAR(10) NOT NULL,
	FullName NVARCHAR(30),
	[Address] NVARCHAR(50) NOT NULL,
	City NVARCHAR(20) NOT NULL,
	ZIPCode NVARCHAR(10),
	Notes NVARCHAR(200)
)

INSERT INTO Customers (DriverLicenceNumber, [Address], City)
VALUES ('1234567890', 'Fondovi', 'Sofia'),
		('1111111111', 'Miladinov 7', 'Petrich'),
		('2222222222', 'Ohrid 19', 'Varna')

CREATE TABLE RentalOrders(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id) NOT NULL,
	CarId INT FOREIGN KEY REFERENCES Cars(Id) NOT NULL,
	TankLevel INT NOT NULL,
	KilometrageStart INT NOT NULL,
	KilometrageEnd INT NOT NULL,
	TotalKilometrage INT,
	StartDate DATE NOT NULL,
	EndDate DATE NOT NULL,
	TotalDays INT,
	RateApplied BIT,
	TaxRate INT,
	OrderStatus BIT,
	Notes NVARCHAR(200)
)

INSERT INTO RentalOrders (EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, StartDate, EndDate)
VALUES (1, 1, 1, 100, 0.0, 55.00, '04.09.2021', '01.11.2021'),
		(2, 2, 2, 100, 0.0, 155.00, '05.09.2021', '01.11.2021'),
		(1, 1, 1, 100, 0.0, 225.00, '06.09.2021', '01.11.2021')


SELECT * FROM Categories

DECLARE @startdate1 date = '2011/1/1'
DECLARE @enddate1 date = '2011/3/1'
SELECT DATEDIFF(day, @startdate1, @enddate1)

ALTER TABLE RentalOrders
ADD CONSTRAINT DF_Sum_Total_Kilometrage
DEFAULT (KilometrageEnd - KilometrageStart)

DROP DATABASE CarRental

USE Minions