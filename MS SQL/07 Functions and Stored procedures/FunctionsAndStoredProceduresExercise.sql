/****** 1.	Queries for SoftUni Database *****/

USE SoftUni

GO

--1.	Employees with Salary Above 35000
CREATE PROCEDURE usp_GetEmployeesSalaryAbove35000
AS
	SELECT FirstName AS [First Name], LastName AS [Last Name]
	FROM Employees
	WHERE Salary > 35000
GO

--2.	Employees with Salary Above Number
CREATE PROCEDURE usp_GetEmployeesSalaryAboveNumber(@Salary DECIMAL(18,4))
AS
	SELECT FirstName AS [First Name], LastName AS [Last Name]
	FROM Employees
	WHERE Salary >= @Salary 
GO

--3.	Town Names Starting With
CREATE OR ALTER PROCEDURE usp_GetTownsStartingWith(@Template nvarchar(MAX))
AS
	SELECT [Name] AS [Town]
	FROM Towns
	WHERE [Name] LIKE (@Template + '%') 
GO

--4.	Employees from Town
CREATE OR ALTER PROCEDURE usp_GetEmployeesFromTown(@Town NVARCHAR(100))
AS
	SELECT e.FirstName, e.LastName 
	FROM Employees AS e
	LEFT JOIN Addresses AS a ON e.AddressID = a.AddressID
	LEFT JOIN Towns AS t ON a.TownID = t.TownID
	WHERE t.Name = @Town
GO
--Second solution with variable
CREATE OR ALTER PROCEDURE usp_GetEmployeesFromTown(@Town NVARCHAR(100))
AS
	DECLARE @TownID INT = 
						(SELECT a.AddressID 
						FROM Towns AS t
						LEFT JOIN Addresses AS a ON t.TownID = a.TownID
						WHERE t.[Name] = @Town)

	SELECT * FROM Employees AS e
	WHERE e.AddressID = @TownID
GO

--5.	Salary Level Function
CREATE OR ALTER FUNCTION ufn_GetSalaryLevel(@Salary MONEY)
RETURNS NVARCHAR(7)
AS
BEGIN
	DECLARE @Level NVARCHAR(7)
	IF(@Salary < 30000) 
		SET @Level = 'Low';
	ELSE IF(@Salary BETWEEN 30000 AND 50000 )
		SET @Level = 'Average';
	ELSE IF (@Salary > 50000)
			SET @Level = 'High';
	RETURN @Level;
END

GO
--my modification
CREATE FUNCTION ufn_GetSalaryLevelOfAllEmployees(@salary DECIMAL(18,4))
RETURNS @Results TABLE(
	[Salary] DECIMAL (18,2),
	[Salary Level] NVARCHAR(10))
AS
BEGIN
	WITH Salary_Levels([Salary], [Level])
	AS(
		SELECT Salary,
			CASE
				WHEN Salary < 30000  THEN 'Low'
				WHEN Salary BETWEEN 30000 AND 50000 THEN 'Average'
				WHEN Salary > 50000 THEN 'High'
			END 
		FROM Employees)

	INSERT INTO @Results SELECT [Salary], [Level]
	FROM Salary_Levels
	RETURN
END

GO

--6.	Employees by Salary Level
CREATE PROCEDURE usp_EmployeesBySalaryLevel(@Level NVARCHAR(MAX))
AS
	SELECT FirstName AS [First Name], LastName AS [Last Name] 
	FROM Employees
	WHERE (dbo.ufn_GetSalaryLevel(Salary)) = @Level

GO

--7.	Define Function
CREATE OR ALTER FUNCTION ufn_IsWordComprised(@setOfLetters NVARCHAR(MAX), @word NVARCHAR(MAX))
RETURNS BIT
AS
	BEGIN
		DECLARE @Index INT = 1;
		DECLARE @CurrentLetter NVARCHAR(2);
		DECLARE @isContained BIT;

		WHILE(@Index <= LEN(@word))
			BEGIN
				SET @CurrentLetter = SUBSTRING(@word, @Index, 1);
				SET @isContained = CHARINDEX(@CurrentLetter,@setOfLetters,0)
				IF(@isContained = 0)
					RETURN 'false'
				ELSE
					SET @Index += 1;
			END
		RETURN 'true'
	END
GO

--8.	* Delete Employees and Departments
CREATE PROCEDURE usp_DeleteEmployeesFromDepartment (@departmentId INT)
AS
	--we start from EmployeesProjects table relation
	DELETE FROM EmployeesProjects
	WHERE EmployeeID IN (
						 SELECT EmployeeID FROM Employees
						 WHERE DepartmentID = @departmentId )

	--seting ManagerID of the employee to null, remove self-relation
	UPDATE Employees 
	SET ManagerID = NULL
	WHERE ManagerID IN (
						 SELECT EmployeeID FROM Employees
						 WHERE DepartmentID = @departmentId )

	--Column ManagerID is not allowe NULL, so we alter it
	ALTER TABLE Departments
	ALTER COLUMN ManagerID INT

	--after that set null value to remove relation
	UPDATE Departments
	SET ManagerID = NULL
	WHERE ManagerID IN (
						SELECT EmployeeID FROM Employees
						 WHERE DepartmentID = @departmentId )

	--Delete all employees from Table, to remove relation with Departments
	DELETE FROM Employees
	WHERE DepartmentID = @departmentId

	--Last delete all from Departments
	DELETE FROM Departments
	WHERE DepartmentID = @departmentId

	--return 0 == false
	SELECT COUNT(*) FROM Employees
	WHERE DepartmentID = @departmentId 
	
GO

EXEC usp_GetEmployeesSalaryAbove35000
EXEC usp_GetEmployeesSalaryAboveNumber 35000
EXEC usp_GetTownsStartingWith 'Bo'
EXEC usp_GetEmployeesFromTown 'Sofia'
SELECT * FROM dbo.ufn_GetSalaryLevelOfAllEmployees(35000)
EXEC usp_EmployeesBySalaryLevel 'High'
SELECT dbo.ufn_IsWordComprised('oistmiahf', 'Sofiaz')

GO

/********* 2.Queries for Bank Database *********/

USE Bank

SELECT * FROM AccountHolders ah
JOIN Accounts a ON ah.Id = a.AccountHolderId
ORDER BY FirstName

GO

--9.	Find Full Name
CREATE PROCEDURE usp_GetHoldersFullName
AS
	SELECT CONCAT(FirstName, ' ', LastName) AS [Full Name]
	FROM AccountHolders
GO

--10.	People with Balance Higher Than
CREATE OR ALTER PROCEDURE usp_GetHoldersWithBalanceHigherThan(@Amount DECIMAL(18,2))
AS
	SELECT ah.FirstName AS [First Name], ah.LastName AS [Last Name]
	FROM AccountHolders AS ah
	JOIN Accounts AS a ON ah.Id = a.AccountHolderId
	GROUP BY ah.FirstName, ah.LastName
	HAVING SUM(a.Balance) > @Amount
	ORDER BY ah.FirstName
GO

--11.	Future Value Function
CREATE OR ALTER FUNCTION ufn_CalculateFutureValue(@sum DECIMAL(18,4), @rate FLOAT, @years INT)
RETURNS DECIMAL(20,4)
AS
	BEGIN
		DECLARE @Amount DECIMAL (20,4);
		SET @Amount = @SUM * (POWER((1 + @rate), @years))
		RETURN @Amount
	END
GO

--12.	Calculating Interest
CREATE PROCEDURE usp_CalculateFutureValueForAccount(@accountID INT, @rate FLOAT)
AS
	SELECT  a.Id AS [Account Id],
			ah.FirstName AS [First Name],
			ah.LastName AS [Last Name],
			a.Balance AS [Current Balance],
			dbo.ufn_CalculateFutureValue(a.Balance, @rate, 5) AS [Balance in 5 years]
	FROM AccountHolders	ah
	JOIN Accounts a ON ah.Id = a.AccountHolderId
	WHERE a.Id = @accountID
GO

EXEC usp_GetHoldersWithBalanceHigherThan 100
SELECT dbo.ufn_CalculateFutureValue(1000, 0.1, 5)
EXEC usp_CalculateFutureValueForAccount 1, 0.1

/******** 3.	Queries for Diablo Database ******/

USE Diablo



GO
--13.	*Scalar Function: Cash in User Games Odd Rows

CREATE OR ALTER FUNCTION ufn_CashInUsersGames(@gameName NVARCHAR(50))
RETURNS TABLE
AS
	RETURN SELECT(
					SELECT SUM(Cash) AS B
					FROM (
						SELECT ug.Cash, ROW_NUMBER() OVER(PARTITION BY g.Name ORDER BY ug.Cash DESC) AS [RowNumber]
						FROM Games AS g
						JOIN UsersGames AS ug ON ug.GameId = g.Id
						WHERE g.Name = @gameName) AS [Query]
					WHERE RowNumber % 2 = 1
				) AS [SumCash]
	
GO

SELECT * FROM dbo.ufn_CashInUsersGames('Love in a mist')

CREATE OR ALTER FUNCTION ufn_CashInUsersGamesAlternative(@gameName NVARCHAR(50))
RETURNS DECIMAL(18,2)
AS
	BEGIN
		DECLARE @TotalSum DECIMAL(18,2);

		SET @TotalSum = (SELECT SUM(Cash) AS B
					FROM (
						SELECT ug.Cash, ROW_NUMBER() OVER(ORDER BY ug.Cash DESC) AS [RowNumber]
						FROM Games AS g
						JOIN UsersGames AS ug ON ug.GameId = g.Id
						WHERE g.Name = @gameName) AS [Query]
					WHERE RowNumber % 2 = 1)
		RETURN @TotalSum 
	END
GO

SELECT dbo.ufn_CashInUsersGamesAlternative('Love in a mist')