USE CarRental

SELECT Row, Col
FROM Rows
CROSS JOIN Cols

USE SoftUni
--Address with Town
Select TOP(50) FirstName AS [First Name], LastName AS [Last Name], t.Name AS Town, a.AddressText 
FROM Employees e
LEFT JOIN Addresses a ON e.AddressID = a.AddressID 
LEFT JOIN Towns t ON a.TownID = t.TownID
ORDER BY FirstName, LastName


--Execution Plan: right click on query, choose Display  Estimated Execution Plan
SELECT CONCAT([Row],',',[Col]) AS [Coordinate], Bool AS Data FROM Matrix

SELECT e.FirstName, e.LastName, e.HireDate, 
		(SELECT COUNT(*) FROM Employees e2 WHERE e.FirstName = e2.FirstName) AS CounterName,
		(SELECT Name FROM Departments WHERE e.DepartmentID = Departments.DepartmentID) AS Department
	FROM Employees e
	WHERE [HireDate] > (SELECT MIN(HireDate) FROM Employees)
		AND e.DepartmentID 
			IN (SELECT DepartmentID FROM Departments WHERE Departments.Name LIKE '%ma%')
	ORDER BY Department

--Min AVG Salary by department
--AVG((SELECT COUNT(Salary) FROM Employees GROUP BY DepartmentID))
--(SELECT DISTINCT Name FROM Departments WHERE e.DepartmentID = Departments.DepartmentID) AS DepName, 
SELECT Name, e.DepartmentID, AVG(Salary) AS AvgSalary, STRING_AGG(FirstName, ' + ') AS FirstName 
FROM Employees AS e
JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
GROUP BY e.DepartmentID, Name
ORDER BY AvgSalary

SELECT *, 
	(SELECT AVG(Salary) FROM Employees e
		WHERE e.DepartmentID = d.DepartmentID) 
			AS AveragySalary
FROM Departments d 
WHERE (SELECT COUNT(*) FROM Employees e
		WHERE e.DepartmentID = d.DepartmentID) > 0
		ORDER BY AveragySalary

--Selfreferences
SELECT e.FirstName, e.LastName, e.EmployeeID, e.ManagerID, m.FirstName, m.ManagerID, m1.FirstName, m1.ManagerID,
	m2.FirstName, m2.ManagerID, m3.FirstName, m3.ManagerID, m4.FirstName, m4.ManagerID
FROM Employees e
JOIN Employees m ON e.ManagerID = m.EmployeeID
JOIN Employees m1 ON m.ManagerID = m1.EmployeeID
JOIN Employees m2 ON m1.ManagerID = m2.EmployeeID
JOIN Employees m3 ON m2.ManagerID = m3.EmployeeID
LEFT JOIN Employees m4 ON m3.ManagerID = m4.EmployeeID
Order By e.EmployeeID