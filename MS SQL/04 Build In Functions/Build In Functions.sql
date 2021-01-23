USE SoftUni
/****** Script for SelectTopNRows command from SoftUni
		Analytic function ******/
SELECT TOP (1000) [EmployeeID]
      ,[FirstName]
      ,[LastName]
      ,[MiddleName]
      ,[JobTitle]
      ,[DepartmentID]
      ,[ManagerID]
      ,[HireDate]
      ,[Salary]
      ,[AddressID]
	  ,PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY Salary DESC)
OVER (PARTITION BY DepartmentId) AS MedianCont
  FROM [SoftUni].[dbo].[Employees]

USE SoftUni

SELECT * FROM Employees

--Concatenation
--CONCAT replaces NULL values with empty string (null+' '= '')
GO

CREATE VIEW V_EmployeeNameJobTitle AS
SELECT CONCAT(FirstName , (MiddleName + ' '), LastName) AS [Full Name], JobTitle AS [Job Title]
FROM Employees

GO
--null example
SELECT FirstName + ' ' + MiddleName + ' ' + LastName  AS [Full name]
FROM Employees
--double space 
SELECT CONCAT(FirstName, ' ', ISNULL(MiddleName, ''), ' ', LastName) AS [Full name]
FROM Employees

SELECT CONCAT_WS(' ', FirstName, MiddleName, LastName) AS [FullName]
FROM Employees

--Substring
SELECT SUBSTRING(CONCAT(FirstName, LastName) , 1, 2) FROM Employees

SELECT FirstName, LastName, Salary,
SUBSTRING(LastName, 1, 2) + '...' AS Summary
FROM Employees

SELECT FirstName, SUBSTRING(LastName, 1, 2) + '***' AS Name, Salary
FROM Employees

SELECT FirstName, SUBSTRING(LastName, 1, 2) + '***' AS Name, 
'**' + SUBSTRING(CONVERT(VARCHAR, CONVERT(decimal(8,2),Salary)) , 3, 10) AS Salary
FROM Employees

--Replace
USE Demo
SELECT r.LastName, SUBSTRING(r.FirstName, 1, r.Length) AS FirstName FROM  
	(SELECT LEN(FirstName) AS Length, FirstName, LastName
	FROM Customers
	WHERE FirstName LIKE ('%a%')) AS r

SELECT FirstName,
REPLACE(REPLACE(REPLACE(REPLACE(FirstName, 'a','@'), 's', '$'), 'i', '!'), 'o', '0') Result
FROM Customers

SELECT * FROM (
	SELECT FirstName + ' ' + LastName AS FullName,
	CASE 
		WHEN LEN(LastName) > 4 THEN (REPLACE(PaymentNumber, '9', '*'))
		ELSE (REPLACE(PaymentNumber, '5', '$'))
		END AS InnerResult
	FROM Customers 
) AS result

SELECT CONCAT(REPLACE(FirstName, 'Ro', '@@'), REPLACE(LastName, 'ow', '@@')), 
SUBSTRING(PaymentNumber, 1,LEN(PaymentNumber) - 4) + '****'
FROM Customers

--LTRIM & RTRIM OR JUST TRIM
SELECT (LTRIM('			Nasko		'))
SELECT (RTRIM('			Nasko		'))
SELECT (TRIM('			Наско		')) AS Nasko

--Datalength
SELECT DATALENGTH(N'Наско')
SELECT DATALENGTH(N'Nasko')
SELECT LEN(N'Nasko'), DATALENGTH(N'Наско'), 'Наско'

--LEFT & RIGHT
SELECT FirstName, LastName, (LEFT(PaymentNumber, LEN(PaymentNumber) - 10) + '***') + 
(SUBSTRING(PaymentNumber, 10, 4)) + '***', '******' + RIGHT(PaymentNumber, 10), PaymentNumber
FROM CUSTOMERS

SELECT FirstName, (LEFT(PaymentNumber, 6) + REPLICATE('*', LEN(PaymentNumber) - 6)) 
FROM Customers

--Lower & UPPER
SELECT LOWER(FirstName), UPPER(LastName) FROM Customers
SELECT REVERSE(FirstName), FirstName FROM Customers

SELECT REPLICATE(FirstName, 3) FROM Customers 

--FORMAT
USE SoftUni
SELECT * FROM Projects

GO

CREATE VIEW V_PublicData AS 
SELECT [Name], FORMAT(StartDate, 'yyyy, MMMM, dd', 'us-US') AS StartDate 
FROM Projects

GO

SELECT [Description] AS ProjectDescription FROM Projects

GO
--CHARINDEX
SELECT CHARINDEX('Vest',
		(SELECT [Description] 
		FROM Projects
		WHERE ProjectID = 1 )) AS StartPosition

SELECT  LEFT(
				(CONVERT(NVARCHAR(MAX), 
										(SELECT [Description] 
										FROM Projects
										WHERE ProjectID = 1))) 
																, 48)
FROM Projects 
WHERE ProjectID = 1
 
 --STUFF
 SELECT STUFF(
			  CONVERT( NVARCHAR(MAX),
									(SELECT [Description]
									 FROM Projects
									 WHERE ProjectID =1)), 
															45, 4, '(HERE I REPALCE Vest)')

--MATH FUNCTION
USE Demo

SELECT * FROM Lines

SELECT Id, (A*H)/2 AS Area
FROM Triangles2

SELECT ABS(X1) FROM Lines
WHERE X1 < 0
--Round
SELECT Id,
	   ROUND(SQRT(SQUARE(X1-X2) + SQUARE(Y1-Y2)), 2) AS Length
FROM Lines
--Floor
SELECT Id,
	   FLOOR(SQRT(SQUARE(X1-X2) + SQUARE(Y1-Y2))) AS Length
FROM Lines
--Ceiling
SELECT Id,
	   CEILING(SQRT(SQUARE(X1-X2) + SQUARE(Y1-Y2))) AS Length
FROM Lines

SELECT * FROM Products

SELECT CEILING((Quantity*1.0/BoxCapacity)/PalletCapacity) AS [Number of Pallets]
FROM Products

--DATE FUNCTION
SELECT * FROM Orders

SELECT ProductName, OrderDate, 
DATEPART(YEAR,OrderDate) AS [Year],
DATEPART(MONTH, OrderDate) AS [Month],
DATEPART(DAY, OrderDate) AS [Day],
DATEPART(quarter, OrderDate) AS [Quarter],
DATENAME(month, GETDATE()) AS [Current],
(SELECT DATENAME(dw,OrderDate)) AS [DayOfWeek],
DATENAME(weekday, OrderDate) AS [Day]
FROM Orders

SELECT FORMAT(OrderDate, 'MMMM') AS Name,
FORMAT(OrderDate, 'Y') AS Date,
FORMAT(OrderDate, 'd'),
FORMAT(OrderDate, 'dddd')
FROM Orders

SELECT ProductName,
		((DATEDIFF(Day, OrderDate, GETDATE())) % 365) % 30 AS [Days],
		((DATEDIFF(Day, OrderDate, GETDATE())) % 365)/30 AS [Month],
		(DATEDIFF(DAY, OrderDate, GETDATE())/365) AS [Years],
		DATEDIFF(DAY, OrderDate, GETDATE()) AS [TotalDays],
		DATEDIFF(MONTH, OrderDate, GETDATE()) AS [TotalMonths],
		DATEDIFF(YEAR, OrderDate, GETDATE()) AS [TotalYears]
FROM Orders

SELECT TOP(1) ProductName, OrderDate, DATEADD(year, 1, OrderDate) AS NewDate FROM Orders
SELECT TOP(1) ProductName, OrderDate, DATEADD(month, 1, OrderDate) AS NewDate FROM Orders
SELECT TOP(1) ProductName, OrderDate, DATEADD(second, 1, OrderDate) AS NewDate FROM Orders

ALTER TABLE Orders
ALTER COLUMN OrderDate DATETIME2

SELECT ProductName, OrderDate,
	CASE
		WHEN OrderDate IS NULL THEN GETDATE()
		ELSE OrderDate
		END AS ResultDate
FROM Orders

SELECT ProductName, ISNULL(CAST(OrderDate AS VARCHAR), 'not ordered')
FROM Orders

SELECT ProductName, OrderDate
FROM Orders
WHERE OrderDate IS NULL

--return third_value
SELECT COALESCE(NULL, NULL, 'third_value','fourth_value') 

--OFFSET & FETCH
SELECT COUNT(*) FROM Orders

SELECT ProductName, OrderDate
FROM Orders
ORDER BY ID
OFFSET 13 ROWS
FETCH NEXT 4 ROWS ONLY

--ROW_NUMBER
SELECT
	ROW_NUMBER() OVER (
		ORDER BY OrderDate
) AS row_num,
ProductName,
OrderDate
FROM Orders

USE SoftUni

SELECT d.Name, AVG(Employees.Salary) AS AVG_Dep_Salary, COUNT(Employees.AddressID) AS People_In_Department FROM Employees
JOIN Departments AS d ON Employees.DepartmentID = d.DepartmentID
--WHERE d.DepartmentID = 1
GROUP BY d.Name
ORDER BY AVG_Dep_Salary DESC

SELECT 
	ROW_NUMBER() OVER (
		ORDER BY Salary DESC
) AS row_num,
FirstName, LastName, Salary
FROM Employees

SELECT FirstName, LastName, [JobTitle],
ROW_NUMBER() OVER (
	PARTITION BY [JobTitle]
	ORDER BY FirstName
) row_num
FROM Employees
ORDER BY [JobTitle]

--WITH
WITH query_one AS (
	SELECT 
		ROW_NUMBER() OVER (
			ORDER BY
				FirstName,
				LastName,
				Salary
		) row_num,
		EmployeeID,
		FirstName + LastName AS FullName,
		Salary
		FROM Employees
) SELECT
	row_num,
	EmployeeID,
	FullName,
	Salary
 FROM query_one
 WHERE row_num BETWEEN 5 AND 10

 --RANK
 SELECT Salary, d.Name AS Department,
	RANK() OVER (
		ORDER BY Salary DESC,
		FirstName,
		e.DepartmentID
		) rank_nomer,
		FirstName
FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID












