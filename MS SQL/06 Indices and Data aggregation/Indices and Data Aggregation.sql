USE SoftUni

SELECT * FROM Employees

SELECT d.Name,
MIN(e.HireDate) AS FirstHiredEmployee, 
MAX(e.HireDate) AS LastHiredEmployee,
MIN(Salary) AS DepartmentMinSalary,
MAX(Salary) AS DepartmentMaxSalary,
MAX(Salary) - MIN(Salary) AS DiffSalary,
FORMAT(SUM(Salary), 'N', 'en-us') AS TotalDepartmentCost,
ROUND(AVG(Salary), 2) AS AverageDepartmentCost,
COUNT(*) AS CountRowsInDepartment,
COUNT(DISTINCT Salary) AS NoNullRows,
STRING_AGG(CONCAT(FirstName, ' ', LastName), ', ')
FROM Employees e
JOIN Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY d.Name
HAVING COUNT(*) > 5 AND AVG(Salary) < 19000
ORDER BY DiffSalary DESC

