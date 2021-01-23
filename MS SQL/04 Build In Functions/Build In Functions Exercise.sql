USE SoftUni

--Problem 1.	Find Names of All Employees by First Name
SELECT FirstName, LastName
FROM Employees
WHERE FirstName LIKE 'Sa%'

--Problem 2.	  Find Names of All employees by Last Name 
SELECT FirstName, LastName
FROM Employees
WHERE LastName LIKE '%ei%'

--Problem 3.	Find First Names of All Employees
SELECT FirstName
FROM Employees
WHERE (DepartmentID = 3 OR DepartmentID = 10) AND 
	  (DATEPART(YEAR,HireDate) BETWEEN 1995 AND 2005)

--Problem 4.	Find All Employees Except Engineers
SELECT FirstName, LastName, [JobTitle]
FROM Employees
WHERE NOT ([JobTitle] LIKE '%engineer%')

SELECT FirstName, LastName, [JobTitle]
FROM Employees
WHERE CHARINDEX('engineer', [JobTitle]) = 0

--Problem 5.	Find Towns with Name Length
SELECT [Name]
FROM Towns
WHERE LEN([Name]) IN (5, 6)
ORDER BY [Name]

--Problem 6.	 Find Towns Starting With
SELECT TownID, [Name]
FROM Towns
WHERE [Name] LIKE '[MKBE]%'
ORDER BY [Name]

--Problem 7.	 Find Towns Not Starting With
SELECT TownID, [Name]
FROM Towns
WHERE [Name] LIKE '[^RBD]%'
ORDER BY [Name]

--Problem 8.	Create View Employees Hired After 2000 Year
CREATE VIEW V_EmployeesHiredAfter2000 AS
SELECT FirstName, LastName
FROM Employees
WHERE DATEPART(YEAR, HireDate) > 2000

SELECT * FROM V_EmployeesHiredAfter2000

--Problem 9.	Length of Last Name
SELECT FirstName, LastName
FROM Employees
WHERE LEN(LastName) = 5

--Problem 10.	Rank Employees by Salary
SELECT EmployeeID, FirstName, LastName, Salary,
	DENSE_RANK() OVER (
		PARTITION BY Salary
		ORDER BY EmployeeID
) AS ranked
FROM Employees
WHERE Salary BETWEEN 10000 AND 50000
ORDER BY Salary DESC

--Problem 11.	Find All Employees with Rank 2 *
SELECT * FROM (
	SELECT EmployeeID, FirstName, LastName, Salary,
		DENSE_RANK() OVER (
			PARTITION BY Salary
			ORDER BY EmployeeID
		) AS ranked
	FROM Employees
	WHERE Salary BETWEEN 10000 AND 50000) AS t
WHERE ranked = 2
ORDER BY Salary DESC


USE Geography
--Problem 12.	Countries Holding ‘A’ 3 or More Times
SELECT CountryName, IsoCode 
FROM Countries
WHERE LEN(CountryName) - (LEN(REPLACE(CountryName, 'a', ''))) > 2 
ORDER BY IsoCode

--Problem 13.	 Mix of Peak and River Names
SELECT p.PeakName, r.RiverName,
	CONCAT(
			LOWER(p.PeakName), 
			LOWER(SUBSTRING(r.RiverName, 2, LEN(r.RiverName) - 1))
		  ) AS Mix
FROM Peaks AS p, Rivers AS r
WHERE (RIGHT(p.PeakName, 1) = LEFT(r.Rivername, 1))
ORDER BY Mix

USE Diablo
--Problem 14.	Games from 2011 and 2012 year
SELECT TOP(50) [Name], FORMAT(Start, 'yyyy-MM-dd') AS Start
FROM Games
WHERE DATEPART(YEAR, [Start]) BETWEEN 2011 AND 2012
ORDER BY [Start], [Name]

--Problem 15.	 User Email Providers
SELECT Username, SUBSTRING(Email, CHARINDEX('@', Email) + 1, LEN(Email) - CHARINDEX('@', Email)) AS [Email Provider] 
FROM Users
ORDER BY [Email Provider], Username

--Problem 16.	 Get Users with IPAdress Like Pattern
SELECT Username, IpAddress AS [IP Address]
FROM Users
WHERE IpAddress LIKE '___.1%.%.___'
ORDER BY Username

SELECT Username, IpAddress
FROM Users
WHERE IpAddress LIKE '%.%.%.%'
ORDER BY Username

--Problem 17.	 Show All Games with Duration and Part of the Day
SELECT [Name] AS Game,
	CASE
		WHEN (0 <= DATEPART(HOUR, [Start]) AND DATEPART(HOUR, [Start]) < 12) THEN 'Morning'
		WHEN (12 <= DATEPART(HOUR, [Start]) AND DATEPART(HOUR, [Start]) < 18) THEN 'Afternoon'
		WHEN (18 <= DATEPART(HOUR, [Start]) AND DATEPART(HOUR, [Start]) < 24) THEN 'Evening'
		END AS [Part of day],
				CASE
					WHEN Duration <= 3 THEN 'Extra Short'
					WHEN 4 <= Duration AND Duration <= 6 THEN 'Short'
					WHEN Duration > 6 THEN 'Long'
					ELSE 'Extra Long'
					END AS [Duration]
FROM Games
ORDER BY Game, Duration, [Part of day]

--Problem 18.	 Orders Table
USE Demo
SELECT ProductName, OrderDate,
	DATEADD(DAY, 3, OrderDate) AS [Pay Due],
		DATEADD(MONTH, 1, OrderDate) AS [Deliver Due]
FROM Orders

--Problem 19.	 People Table
CREATE TABLE Birthdays(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30) NOT NULL,
	Birthday DATETIME2 NOT NULL
)

SELECT [Name],
	FLOOR(DATEDIFF(YEAR, Birthday, GETDATE())) AS [Age in Years],
		FLOOR(DATEDIFF(MONTH, Birthday, GETDATE())) AS [Age in Months],
			FLOOR(DATEDIFF(DAY, Birthday, GETDATE())) AS [Age in Days],
				DATEDIFF(MINUTE, Birthday, GETDATE()) AS [Age in Minutes]
FROM Birthdays

SELECT [Name],
		(DATEDIFF(DAY, Birthday, GETDATE())/365) AS [Years],
		((DATEDIFF(Day, Birthday, GETDATE())) % 365)/30 AS [Month],
		(DATEDIFF(MONTH, Birthday, GETDATE())) AS [Total Months],
		(DATEDIFF(WEEK, Birthday, GETDATE())) AS [Weeks],
		((DATEDIFF(Day, Birthday, GETDATE())) % 365) % 30 AS [Days],
		FLOOR(DATEDIFF(DAY, Birthday, GETDATE())) AS [Total Days],
		(DATEDIFF(HOUR, Birthday, GETDATE())) AS [Hours],
		DATEDIFF(MINUTE, Birthday, GETDATE()) AS [Minutes]
FROM Birthdays
















