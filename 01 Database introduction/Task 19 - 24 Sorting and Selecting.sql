USE SoftUni

INSERT INTO Towns ([Name])
VALUES ('Sofia'),
		('Plovdiv'),
		('Varna'),
		('Burgas')

INSERT INTO Departments([Name])
VALUES ('Engineering'),
		('Sales'),
		('Marketing'),
		('Software Development'),
		('Quality Assurance')

INSERT INTO Addresses(AddressText, TownId)
VALUES ('Marica 7', 1),
		('Dunav 19', 2),
		('Luda Mara', 3),
		('Tunja 8', 4)

INSERT INTO Employees(FirstName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId)
VALUES ('Ivan','Ivanov', '.NET Developer', 4, '02/01/2013', 3500.00, 1),
		('Petar','Petrov', 'Senior Engineer', 1, '03/02/2004', 4000, 2),
		('Maria','Petrova', 'Intern', 5, '08/28/2016', 525.25, 3),
		('Georgi', 'Teziev', 'CEO', 2, '12/09/2007', 3000, 4),
		('Peter', 'Pan', 'Intern', 3, '08/28/2016', 599.98, 1)


--Test
USE SoftUni
Select * FROM EMPLOYEES
UPDATE EMPLOYEES SET HireDate = '08/28/2019' WHERE Id = 5
TRUNCATE TABLE Employees

INSERT INTO Employees(FirstName, LastName, JobTitle, DepartmentId, HireDate, Salary, AddressId)
VALUES ('Test','Testov', '.NET Developer', 
		--Select ID from table Departments
		(Select TOP 1 Id FROM Departments WHERE [Name] = 'Software Development'), 

		'02/01/2013', 33500.00, 1)

--TASK 19
SELECT * FROM Towns
SELECT * FROM Departments
SELECT * FROM Employees

--TASK 20
SELECT * FROM Towns ORDER BY [Name] ASC
SELECT * FROM Departments ORDER BY [Name] ASC
SELECT * FROM Employees ORDER BY Salary DESC

--TASK 21
SELECT [Name] FROM Towns ORDER BY [Name] ASC
SELECT [Name] FROM Departments ORDER BY [Name] ASC
SELECT FirstName, LastName, JobTitle, Salary FROM Employees ORDER BY Salary DESC

--TASK 22
UPDATE Employees SET Salary = Salary * 1.10
SELECT Salary FROM Employees

--TASK 23
USE Hotel
ALTER TABLE Payments
ALTER COLUMN TaxRate DECIMAL(7, 2)
SELECT * FROM Payments
UPDATE Payments SET TaxRate = TaxRate * 0.97 WHERE TaxRate != null
Select TaxRate FROM Payments

--Task 24
TRUNCATE TABLE Occupancies
DELETE FROM Occupancies

