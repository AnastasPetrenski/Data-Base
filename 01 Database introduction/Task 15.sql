CREATE DATABASE Hotel

USE Hotel

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	Title VARCHAR(30),
	Notes VARCHAR(200)
)

INSERT INTO Employees (FirstName, LastName)
VALUES ('Bango', 'Vasil'),
		('Bango1', 'Vasil1'),
		('Bango2', 'Vasil2')

CREATE TABLE Customers(
	AccountNumber CHAR(10) PRIMARY KEY NOT NULL,
	FirstName VARCHAR(30) NOT NULL,
	LastName VARCHAR(30) NOT NULL,
	PhoneNumber CHAR(10) NOT NULL,
	EmergencyName VARCHAR(30),
	EmergencyNumber VARCHAR(10),
	Notes VARCHAR(200)
)

INSERT INTO Customers (AccountNumber, FirstName, LastName, PhoneNumber)
VALUES ('1010101010', 'Nas', 'Pet', '0898000333'),
		('1010101011', 'Nas', 'Pet', '0898000333'),
		('1010101012', 'Nas', 'Pet', '0898000333')

CREATE TABLE RoomStatus(
	RoomStatus CHAR(1) PRIMARY KEY NOT NULL,
	Notes VARCHAR(200)
)

INSERT INTO RoomStatus (RoomStatus, Notes)
VALUES ('0', 'Not Reserved'),
		('1', 'Reserved'),
		('2', 'No status')

		
CREATE TABLE RoomTypes(
	RoomType CHAR(1) PRIMARY KEY NOT NULL,
	Notes VARCHAR(200)
)

INSERT INTO RoomTypes (RoomType, Notes)
VALUES ('0','Apartman'),
		('1','Single'),
		('2', 'Double')

CREATE TABLE BedTypes(
	BedType CHAR(1) PRIMARY KEY NOT NULL,
	Notes VARCHAR(200)
)

INSERT INTO BedTypes (BedType)
VALUES ('A'),
		('B'),
		('C')

CREATE TABLE Rooms(
	RoomNumber CHAR(3) PRIMARY KEY NOT NULL,
	RoomType CHAR(1) FOREIGN KEY REFERENCES RoomTypes(RoomType) NOT NULL,
	BedType CHAR(1) FOREIGN KEY REFERENCES BedTypes(BedType) NOT NULL,
	Rate INT,
	RoomStatus CHAR(1) FOREIGN KEY REFERENCES RoomStatus(RoomStatus) NOT NULL,
	Notes VARCHAR(200)
)

INSERT INTO Rooms (RoomNumber, RoomType, BedType, RoomStatus)
VALUES ('101', '0', 'A', '0'),
		('201', '1', 'B', '1'),
		('301', '2', 'C', '2')

CREATE TABLE Payments(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
	PaymentDate DATETIME2 NOT NULL,
	AccountNumber CHAR(10) FOREIGN KEY REFERENCES Customers(AccountNumber) NOT NULL,
	FirstDateOccupied DATE NOT NULL,
	LastDateOccupied DATE NOT NULL,
	TotalDays INT NOT NULL,
	AmountCharged DECIMAL(8,2),
	TaxRate INT,
	TaxAmount INT,
	PaymentTotal MONEY NOT NULL,
	Notes VARCHAR(200)
)

INSERT INTO Payments (EmployeeId, PaymentDate, AccountNumber, FirstDateOccupied, LastDateOccupied, TotalDays,
			PaymentTotal)
VALUES (1, '01.12.2021', '1010101010', '01.01.2021', '01.05.2021', 5, 200200.20),
		(2, '01.12.2021', '1010101011', '01.01.2021', '01.05.2021', 5, 2.20),
		(3, '01.12.2021', '1010101012', '01.01.2021', '01.05.2021', 5, 200.20)

CREATE TABLE Occupancies(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
	DateOccupied DATE NOT NULL,
	AccountNumber CHAR(10) FOREIGN KEY REFERENCES Customers(AccountNumber) NOT NULL,
	RoomNumber CHAR(3) FOREIGN KEY REFERENCES Rooms(RoomNumber) NOT NULL,
	RateApplied BIT,
	PhoneCharge BIT,
	Notes VARCHAR(200)
)

INSERT INTO Occupancies (EmployeeId, DateOccupied, AccountNumber, RoomNumber)
VALUES (1, '01.01.2021', '1010101010', '101'),
		(2, '01.01.2021', '1010101011', '201'),
		(1, '01.01.2021', '1010101012', '301')








