/*
Syntaxis definition:
CREATE FUNCTION udf_SomeNmae (@StartDate DateTime, @EndDate ) <= (@params1 type, @params2 type...)
RETURNS dateType <= (int, decimal, DateTime2, nvarchar...)
AS
	BEGIN
		.....some CODE here
	END
*/

USE SoftUni

GO
/****************** SCALAR FUNCTION *****************/

CREATE OR ALTER FUNCTION udf_SomeNmae (@StartDate DATETIME, @EndDate DATETIME)
RETURNS INT 
AS
	BEGIN
		DECLARE @projectsWeeks INT;
		IF(@EndDate IS NULL)
		BEGIN
			SET @EndDate = GETDATE()
		END
		SET @projectsWeeks = DATEDIFF(WEEK, @StartDate, @EndDate)
		RETURN @projectsWeeks;
	END

GO

CREATE OR ALTER FUNCTION udf_GetDiffInWeek (@StartDate DATETIME, @EndDate DATETIME)
RETURNS INT 
AS
	BEGIN
		DECLARE @noDate nvarchar(10) = 'In progress' 
		DECLARE @projectsWeeks INT;
		IF(@EndDate IS NULL)
			RETURN 0;
		RETURN DATEDIFF(WEEK, @StartDate, @EndDate)
	END

GO
CREATE FUNCTION utf_Pow (@Base INT, @Exp INT)
RETURNS BIGINT
AS
	BEGIN
		DECLARE @Result BIGINT = 1;

		WHILE(@Exp > 0)
		BEGIN
			SET @Result = @Result * @Base;
			SET @Exp -= 1;
		END
		RETURN @Result
END

GO

CREATE OR ALTER FUNCTION utf_Pow (@Base INT, @Exp INT)
RETURNS BIGINT
AS
	BEGIN
		DECLARE @Result BIGINT = 1;

		WHILE(@Exp > 0)
		BEGIN
			SET @Result = @Result * @Base;
			SET @Exp = @Exp - 1;
		END
		RETURN @Result
END

GO

DROP FUNCTION utf_Pow;

go

CREATE OR ALTER FUNCTION udf_Pow (@Base INT, @Exp INT)
RETURNS DECIMAL (38,2)
AS
	BEGIN
		DECLARE @Result DECIMAL (38,2) = 1;

		WHILE(@Exp > 0)
		BEGIN
			SET @Result = @Result * @Base;
			SET @Exp -= 1;
		END
		RETURN @Result
END

GO

DECLARE @Result INT = dbo.udf_Pow(2,3);

SELECT dbo.udf_Pow(2,3), POWER(2,3);

SELECT dbo.udf_SomeNmae('2000-01-01', null)

SELECT *, 
	CASE
		WHEN dbo.udf_GetDiffInWeek(StartDate, EndDate) = 0 THEN 'In Progress'
		ELSE CONVERT(NVARCHAR, dbo.udf_GetDiffInWeek(StartDate, EndDate))
		END AS [Project's week Duration]
FROM Projects

GO

/****************** TABLE FUNCTION *****************/

CREATE OR ALTER FUNCTION udf_GetEmployeesCountByYear(@year int)
RETURNS TABLE
AS
RETURN
(
	SELECT *
	/*SELECT COUNT(*) AS [Number of employees]*/
	FROM [Employees]
	WHERE DATEPART(YEAR, [HireDate]) = @year
)

GO

SELECT COUNT(*) FROM dbo.udf_GetEmployeesCountByYear(2001)
SELECT * FROM dbo.udf_GetEmployeesCountByYear(2001)


/****************** MULTI-STATEMENT TABLE VALUE FUNCTION (TVF) *****************/
GO

CREATE OR ALTER FUNCTION udf_EmployeeListByDepartment(@DepName nvarchar(20))
RETURNS @result TABLE(
	FirstName nvarchar(50) NOT NULL,
	LastName nvarchar(50) NOT NULL,
	DepartmentName nvarchar(20) NOT NULL) 
AS
BEGIN
	WITH Emplaoyees_CTE (FirstName, LastName, DepartmentName)
	AS(
		SELECT e.FirstName, e.LastName, d.[Name] 
		FROM Employees AS e
		LEFT JOIN Departments AS d ON e.DepartmentID = d.DepartmentID)

	INSERT INTO @result SELECT FirstName, LastName, DepartmentName
	FROM Emplaoyees_CTE WHERE DepartmentName = @DepName
	RETURN
END

GO

CREATE OR ALTER FUNCTION udf_SquareTableOfNumber(@number INT)
RETURNS @SquareTable TABLE(
	Id INT NOT NULL IDENTITY,
	Number INT NOT NULL,
	SquareOf INT NOT NULL,
	Result INT NOT NULL)
AS
BEGIN
	DECLARE @Count INT = 1;
	
	WHILE(@Count <= 10)
	BEGIN
		INSERT INTO @SquareTable (Number, SquareOf, Result)
		VALUES (@number, @Count, @number * @Count)
		SET @Count += 1;
	END
	RETURN
END
GO

SELECT * FROM udf_EmployeeListByDepartment('Sales')

SELECT * FROM udf_SquareTableOfNumber(100)

INSERT INTO Grouped(Salary) SELECT Result FROM udf_SquareTableOfNumber(9)

GO
/******************* TASK ********************/
--Write a function ufn_GetSalaryLevel(@Salary MONEY)
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

SELECT e.FirstName, e.LastName, e.Salary,
	dbo.ufn_GetSalaryLevel(e.Salary) AS SalaryLevel 
FROM Employees e

/******************* STORED PRECEDURES ********************/
EXEC sp_databases

SELECT * FROM sys.master_files

GO

CREATE OR ALTER PROCEDURE sp_GetEmployeesByExperience(@Year int = 10)
AS
	SELECT *, DATEDIFF(YEAR, [HireDate], GETDATE()) AS Experience
	FROM Employees
	WHERE DATEDIFF(YEAR, [HireDate], GETDATE()) > @Year
	ORDER BY Experience DESC
GO

CREATE OR ALTER PROCEDURE sp_GetEmployeesByExperienceAndSalary(@Year INT = 10, @MinSalary MONEY = 10000)
AS
	SELECT *, DATEDIFF(YEAR, [HireDate], GETDATE()) AS Experience
	FROM Employees
	WHERE DATEDIFF(YEAR, [HireDate], GETDATE()) > @Year AND Salary > @MinSalary
	ORDER BY Salary DESC, Experience DESC
GO

CREATE OR ALTER PROCEDURE sp_GetEmployeesCountByExperienceAndSalary
		(@Counter INT OUTPUT, @Year INT = 10, @MinSalary MONEY = 10000)
AS
	SET @Counter = (SELECT COUNT(*)
	FROM Employees
	WHERE DATEDIFF(YEAR, [HireDate], GETDATE()) > @Year AND Salary > @MinSalary)

	SELECT COUNT(*) FROM Projects;
	SELECT COUNT(*) FROM Towns;
GO

CREATE OR ALTER PROCEDURE sp_GetMultipleOutputByExperienceAndSalary
		(@Counter INT OUTPUT, @ProjectsCount INT OUTPUT, @Year INT = 10, @MinSalary MONEY = 10000)
AS
	SET @Counter = (SELECT COUNT(*)
	FROM Employees
	WHERE DATEDIFF(YEAR, [HireDate], GETDATE()) > @Year AND Salary > @MinSalary)

	SET @ProjectsCount = (SELECT COUNT(*) FROM Projects);
	SELECT COUNT(*) FROM Towns;
GO

EXEC sp_GetEmployeesByExperience 20

EXEC sp_GetEmployeesByExperienceAndSalary
EXEC sp_GetEmployeesByExperienceAndSalary 19, 25000
EXEC sp_GetEmployeesByExperienceAndSalary @MinSalary = 35000, @Year = 15

DECLARE @Count INT;
DECLARE @CountProjects INT;
EXEC sp_GetEmployeesCountByExperienceAndSalary @Count OUTPUT
SELECT @Count AS EmployeesCount

EXEC sp_GetMultipleOutputByExperienceAndSalary @Count OUTPUT, @CountProjects OUTPUT;
SELECT @Count AS Employees, @CountProjects AS Projects

/******************* EXEPTION HANDLING ********************/
GO

CREATE PROCEDURE sp_AddEmployeeToProject(@EmployeeID INT, @ProjectID INT)
AS
	DECLARE @EmployeeProjects INT = 
		(SELECT COUNT(*) FROM EmployeesProjects
			WHERE EmployeeID = @EmployeeID AND ProjectID = @ProjectID);

	IF(@EmployeeProjects >= 3)
		THROW 50001, 'This employee is already exist!', 1;

	DECLARE @CountEmployee INT =
		(SELECT COUNT(*) FROM EmployeesProjects
			WHERE EmployeeID >= @EmployeeID AND ProjectID = @ProjectID)

	IF(@CountEmployee >= 1)
		THROW 50002, 'Employees', 1;

	INSERT INTO EmployeesProjects (EmployeeID, ProjectID)
	VALUES (@EmployeeID , @ProjectID )
	
GO

EXECUTE sp_AddEmployeeToProject 7, 3

DECLARE @CurrentError INT = 0;

BEGIN TRY
	SELECT 0/0
END TRY 
BEGIN CATCH
	SELECT @@ERROR AS SameAs_ErrorNumber,
		ERROR_NUMBER() AS ErrorNumber,
		ERROR_SEVERITY() AS ErrorSevirity,
		ERROR_MESSAGE() AS ErrorMesage,
		ERROR_STATE() AS ErrorState,
		ERROR_PROCEDURE() AS ErrorProcedure,
		ERROR_LINE() AS ErrorLine,
		@@PACKET_ERRORS
END CATCH


