USE SoftUni

--1.	Employee Address
SELECT TOP(5) e.EmployeeID, e.JobTitle, e.AddressID, a.AddressText 
FROM Employees e
JOIN Addresses a ON e.AddressID = a.AddressID
ORDER BY e.AddressID

--2.	Addresses with Towns
SELECT TOP(50) FirstName, LastName, t.Name AS Town, AddressText 
FROM Employees e
JOIN Addresses a ON e.AddressID = a.AddressID
JOIN Towns t ON a.TownID = t.TownID
ORDER BY FirstName, LastName

--3.	Sales Employee
SELECT e.EmployeeID, FirstName, LastName, d.Name
FROM Employees e
JOIN Departments d ON e.DepartmentID = d.DepartmentID
WHERE d.Name = 'Sales'
ORDER BY e.EmployeeID

--4.	Employee Departments
SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.Name AS [Department Name]
FROM Employees e
JOIN Departments d ON e.DepartmentID = d.DepartmentID
WHERE Salary > 15000
ORDER BY d.DepartmentID

--5.	Employees Without Project
SELECT TOP(3) e.EmployeeID, e.FirstName 
FROM Employees e
LEFT JOIN EmployeesProjects ep ON e.EmployeeID = ep.EmployeeID
WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID

SELECT TOP(3) e.EmployeeID, e.FirstName 
FROM EmployeesProjects ep
RIGHT JOIN Employees e ON ep.EmployeeID = e.EmployeeID
WHERE ep.ProjectID IS NULL
ORDER BY e.EmployeeID

--6.	Employees Hired After
SELECT FirstName, LastName, HireDate, d.[Name] AS DeptName 
FROM Employees e
JOIN Departments d ON e.DepartmentID = d.DepartmentID
WHERE e.HireDate > '1999-01-01' AND
d.Name IN ('Sales', 'Finance')
ORDER BY e.HireDate

--7.	Employees with Project
SELECT TOP(5) e.EmployeeID, e.FirstName, p.Name AS ProjectName
FROM Employees e
JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
JOIN Projects p ON ep.ProjectID = p.ProjectID
WHERE p.StartDate > '2002-08-13' AND
		p.EndDate IS NULL
ORDER BY e.EmployeeID

UPDATE Projects SET EndDate = Null
WHERE EndDate >= '2021-01-14'

--8.	Employee 24
SELECT e.EmployeeID, e.FirstName,  
CASE
	WHEN DATEPART(YEAR, p.StartDate) >= 2005 THEN NULL
	ELSE p.[Name]
	END AS ProjectName
FROM Employees e
JOIN EmployeesProjects ep ON e.EmployeeID = ep.EmployeeID
JOIN Projects p ON ep.ProjectID = p.ProjectID
WHERE e.EmployeeID = 24

--9.	Employee Manager
SELECT e.EmployeeID, e.FirstName, e.ManagerID, m.FirstName AS ManagerName
FROM Employees e
JOIN Employees m ON e.ManagerID = m.EmployeeID
WHERE e.ManagerID IN (3, 7)
ORDER BY e.EmployeeID

--10. Employee Summary
SELECT TOP(50) e.EmployeeID, CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName,
		CONCAT(m.FirstName, ' ', m.LastName) AS ManagerName, 
		d.Name AS DepartmentName
FROM Employees e
LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
JOIN Departments d ON e.DepartmentID = d.DepartmentID
ORDER BY e.EmployeeID

SELECT e.EmployeeID, CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName,
		CASE
			WHEN m.FirstName IS NULL THEN 'No manager'
			ELSE CONCAT(m.FirstName, ' ', m.LastName) 
			END AS ManagerName, 
		d.Name AS DepartmentName
FROM Employees e
LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
JOIN Departments d ON e.DepartmentID = d.DepartmentID
WHERE m.FirstName IS NULL
ORDER BY e.EmployeeID

--11. Min Average Salary
SELECT TOP(1) AVG(e.Salary) AS MinAverageSalary
FROM Employees e
JOIN Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY e.DepartmentID
ORDER BY MinAverageSalary

SELECT MIN([Average Salary]) AS [MinAverageSalary]
FROM (
		SELECT e.DepartmentID, AVG(Salary) AS [Average Salary]
		FROM Employees e
		GROUP BY e.DepartmentID) AS [AverageSalaryQuery]

USE Geography
--12. Highest Peaks in Bulgaria
SELECT mc.CountryCode, m.MountainRange, p.PeakName, p.Elevation
FROM Peaks p
JOIN Mountains m ON p.MountainId = m.Id
JOIN MountainsCountries mc ON m.Id = mc.MountainId
JOIN Countries c ON mc.CountryCode = c.CountryCode
WHERE c.CountryName = 'Bulgaria' AND p.Elevation > 2835
ORDER BY p.Elevation DESC

SELECT mc.CountryCode, m.MountainRange, p.PeakName, p.Elevation
FROM Peaks p
JOIN Mountains m ON p.MountainId = m.Id
JOIN MountainsCountries mc ON m.Id = mc.MountainId
WHERE mc.CountryCode = 'BG' AND p.Elevation > 2835
ORDER BY p.Elevation DESC

--13. Count Mountain Ranges
SELECT CountryCode, COUNT(m.MountainRange)
FROM Mountains m
JOIN MountainsCountries mc ON m.Id = mc.MountainId
WHERE mc.CountryCode IN ('BG', 'RU', 'US')
GROUP BY CountryCode

--14. Countries with Rivers
SELECT TOP(5) c.CountryName, r.RiverName
FROM Countries c
LEFT JOIN CountriesRivers cr ON c.CountryCode = cr.CountryCode
LEFT JOIN Rivers r ON cr.RiverId = r.Id
WHERE c.ContinentCode = 'AF'
ORDER BY c.CountryName 

--15. *Continents and Currencies
SELECT ContinentCode, CurrencyCode, CurrencyCount AS CurrencyUsage 
FROM (SELECT    ContinentCode, 
				CurrencyCode, 
				CurrencyCount,
				DENSE_RANK() OVER(PARTITION BY ContinentCode ORDER BY CurrencyCount DESC) AS [CurrencyRank]
				FROM (
						SELECT  ContinentCode, 
								CurrencyCode, 
								COUNT(*) AS [CurrencyCount]
						FROM Countries
						GROUP BY ContinentCode, CurrencyCode) AS [Usage]
		WHERE CurrencyCount > 1	) AS [CurrencyRanking]
WHERE CurrencyRank = 1
ORDER BY ContinentCode

GO

SELECT ContinentCode, CurrencyCode, CurrencyUsage
	  FROM 
				(SELECT ContinentCode, CurrencyCode, 
					COUNT(*) AS CurrencyUsage,
					DENSE_RANK() OVER 
					(PARTITION BY ContinentCode ORDER BY COUNT(*) DESC) AS [Result] 
				FROM Countries
				GROUP BY ContinentCode, CurrencyCode) AS Usage
	   WHERE Usage.Result = 1 AND Usage.CurrencyUsage > 1
ORDER BY ContinentCode


GO

WITH [test1] AS (
	SELECT ContinentCode, CurrencyCode, 
		(SELECT COUNT(CountryCode) AS c WHERE COUNT(CountryCode) > 1) AS Usage 
	FROM Countries
	GROUP BY CurrencyCode, ContinentCode
	)
SELECT ContinentCode, CurrencyCode, Usage
FROM test1
WHERE Usage IS NOT NULL 
ORDER BY ContinentCode DESC

--16. Countries Without Any Mountains
--Find all the count of all countries, which don’t have a mountain.
SELECT COUNT(*) AS [Count]
FROM Countries c
LEFT JOIN MountainsCountries mc ON c.CountryCode = mc.CountryCode
WHERE MountainId IS NULL

--17. Highest Peak and Longest River by Country
SELECT TOP(5) CountryName, Elevation AS HighestPeakElevation, [Length] AS LongestRiverLength FROM (
				SELECT c.CountryName, 
					   p.Elevation,
					   r.[Length],
					   DENSE_RANK() OVER(PARTITION BY CountryName ORDER BY p.Elevation DESC) AS HighestPeakDesc,
					   DENSE_RANK() OVER(PARTITION BY CountryName ORDER BY r.[Length] DESC) AS LongestRiverDesc
				FROM Countries c
				LEFT JOIN MountainsCountries mc ON c.CountryCode = mc.CountryCode
				LEFT JOIN Mountains m ON mc.MountainId = m.Id
				LEFT JOIN Peaks p ON m.Id = p.MountainId
				LEFT JOIN CountriesRivers cr ON c.CountryCode = cr.CountryCode
				LEFT JOIN Rivers r ON cr.RiverId = r.Id
				) AS PeaksOrdered
WHERE PeaksOrdered.HighestPeakDesc = 1 AND PeaksOrdered.LongestRiverDesc = 1
ORDER BY Elevation DESC, Length DESC, CountryName

SELECT DISTINCT CountryName,  Elevation, PeakName FROM (
				SELECT c.CountryName, 
					   p.Elevation,
					   p.PeakName,
					   r.[Length],
					   r.RiverName,
					   DENSE_RANK() OVER(PARTITION BY CountryName ORDER BY p.Elevation DESC) AS HighestPeakDesc,
					   DENSE_RANK() OVER(PARTITION BY CountryName ORDER BY r.[Length] DESC) AS LongestRiverDesc
				FROM Countries c
				LEFT JOIN MountainsCountries mc ON c.CountryCode = mc.CountryCode
				LEFT JOIN Mountains m ON mc.MountainId = m.Id
				LEFT JOIN Peaks p ON m.Id = p.MountainId
				LEFT JOIN CountriesRivers cr ON c.CountryCode = cr.CountryCode
				LEFT JOIN Rivers r ON cr.RiverId = r.Id
				) AS PeaksOrdered
--WHERE PeaksOrdered.HighestPeakDesc = 1 AND PeaksOrdered.LongestRiverDesc = 1
WHERE CountryName = 'China'
ORDER BY Elevation DESC, CountryName

SELECT c.CountryName, 
	MAX(p.Elevation) AS [Max peak],
	MAX(r.[Length]) AS [Max river]
FROM Countries c
LEFT JOIN MountainsCountries mc ON c.CountryCode = mc.CountryCode
LEFT JOIN Mountains m ON mc.MountainId = m.Id
LEFT JOIN Peaks p ON m.Id = p.MountainId
LEFT JOIN CountriesRivers cr ON c.CountryCode = cr.CountryCode
LEFT JOIN Rivers r ON cr.RiverId = r.Id
GROUP BY c.CountryName
ORDER BY [Max peak] DESC, [Max river] DESC


--18. Highest Peak Name and Elevation by Country
SELECT TOP(5) CountryName,
		CASE
			WHEN testtable.PeakName IS NULL THEN '(no highest peak)'
			ELSE testtable.PeakName
			END AS [Highest Peak Name],
		CASE
			WHEN testtable.Elevation IS NULL THEN 0
			ELSE testtable.Elevation
			END AS [Highest Peak Elevation],
		CASE
			WHEN testtable.MountainRange IS NULL THEN '(no mountain)'
			ELSE testtable.MountainRange
			END AS [Mountain]
FROM 
		(SELECT CountryName, p.PeakName, p.Elevation, m.MountainRange,
		 DENSE_RANK() OVER(PARTITION BY CountryName ORDER BY p.Elevation DESC) AS HighestPeakDesc
		 FROM Countries c
		 LEFT JOIN MountainsCountries mc ON c.CountryCode = mc.CountryCode
		 LEFT JOIN Mountains m ON mc.MountainId = m.Id
		 LEFT JOIN Peaks p ON m.Id = p.MountainId
		 ) AS [testtable]
WHERE testtable.HighestPeakDesc = 1

