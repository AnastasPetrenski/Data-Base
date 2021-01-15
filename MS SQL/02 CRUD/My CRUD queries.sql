USE Geography

CREATE VIEW [V_HighestPeak] AS
SELECT TOP 1 * FROM Peaks
ORDER BY Elevation DESC

SELECT * FROM V_HighestPeak

ALTER VIEW [V_HighestPeak] AS
SELECT TOP 3 * FROM Peaks
ORDER BY Elevation DESC

SELECT Id, PeakName, Elevation, MountainId
INTO Info
FROM Peaks

CREATE SEQUENCE seq_Numbers
AS INT 
START WITH 0
INCREMENT BY 101

SELECT NEXT VALUE FOR seq_Numbers

-- Delete all records one by one and keep Id counter
-- Delete provedi option for restrictions clause WHERE.
DELETE FROM Info
	
-- Delete provedi option for restrictions clause WHERE.
DELETE FROM Info
	WHERE PeakName Like 'm%'

-- Restart new table with 0 base Id counter - Faster than Delete
TRUNCATE TABLE Info;

UPDATE Info
SET PeakName = '-->' + PeakName
WHERE Elevation >= 8000

SELECT COUNT(Elevation)
FROM Info
WHERE PeakName LIKE '-->%'

SELECT MountainId, COUNT(Elevation) AS [Count_*8000m_Peaks]
FROM Info
WHERE PeakName LIKE '-->%'
GROUP BY MountainId

CREATE VIEW [V_Peaks_Between_5000_AND_6000_meters] AS
SELECT MountainId, COUNT(Elevation) AS [Count between 5000 - 6000]
FROM Peaks
Where (Elevation BETWEEN 5000 AND 6000)
GROUP BY MountainId

CREATE VIEW [V_Peaks_Between_6000_And_7000_meters] AS
SELECT MountainId, COUNT(Elevation) AS [Count between 6000 - 7000]
FROM Peaks
WHERE Elevation BETWEEN 6000 AND 7000
GROUP BY MountainId

CREATE VIEW [V_Peaks_Between_7000_And_8000_meters] AS
SELECT MountainId, COUNT(Elevation) AS [Count between 7000 - 8000]
FROM Peaks
WHERE Elevation BETWEEN 7000 AND 8000
GROUP BY MountainId

CREATE VIEW [V_Above_8000_meters] AS
SELECT MountainId, COUNT(Elevation) AS [Count_Above_8000_meters]
FROM Peaks
WHERE Elevation >= 8000
GROUP BY MountainId


SELECT * FROM [dbo].[V_Peaks_Between_5000_AND_6000_meters]
UNION All
SELECT * FROM [dbo].[V_Above_8000_meters]
UNION ALL
SELECT * FROM [dbo].[V_Peaks_Between_6000_And_7000_meters]
UNION ALL 
SELECT * FROM [dbo].[V_Peaks_Between_7000_And_8000_meters]
ORDER BY MountainId

SELECT * 
INTO FinalTable
FROM (SELECT MountainId, [Count between 5000 - 6000]
			FROM [dbo].[V_Peaks_Between_5000_AND_6000_meters]
			UNION ALL 
				SELECT MountainId, [Count between 6000 - 7000]
					FROM [dbo].[V_Peaks_Between_6000_And_7000_meters]) n
				
SELECT * FROM Peaks
ORDER BY Elevation DESC

USE SoftUni

SELECT * FROM [SoftUni].[dbo].[Projects]
WHERE [EndDate] IS NULL

UPDATE [SoftUni].[dbo].[Projects]
SET [EndDate] = GETDATE()
WHERE [EndDAte] IS NULL

SELECT * FROM [SoftUni].[dbo].[Projects]
WHERE [EndDate] >= '2021-01-14' AND [EndDate] < '2021-01-15'
-- WHERE EndDate = '2021-01-14 12:00:00'  <-- Exact Date and Time

SELECT (FirstName + ' ' + LastName) AS [FullName], 
	Departments.[Name] AS Departments, Projects.[Name] AS Projects, Projects.StartDate
FROM [SoftUni].[dbo].[Employees]
LEFT JOIN [SoftUni].[dbo].[Departments] ON Employees.EmployeeID = Departments.DepartmentID
LEFT JOIN [SoftUni].[dbo].[Projects] ON Employees.EmployeeID = Projects.ProjectId
--WHERE SoftUni.dbo.Projects.Name IS NOT NULL AND SoftUni.dbo.Departments.Name IS NOT NULL
ORDER BY Projects.Name 

SELECT FirstName, LastName, Departments.Name AS Department, EmployeesProjects.ProjectID, 
	Projects.Name AS Projects, Salary
--INTO tableName : Create new table and populate the result from the query
--INTO Grouped
FROM Employees
LEFT JOIN Departments ON Employees.DepartmentID = Departments.DepartmentID
LEFT JOIN EmployeesProjects ON Employees.EmployeeID = EmployeesProjects.EmployeeID
LEFT JOIN Projects ON EmployeesProjects.ProjectID = Projects.ProjectID
ORDER BY FirstName

--DROP TABLE Grouped

SELECT FirstName, LastName, Departments.Name, 
--COUNT(EmployeesProjects.ProjectID) AS Projects, COUNT(Projects.Name) , AVG(Salary)
	COUNT(EmployeesProjects.ProjectID) AS Projects, AVG(Salary) AS [AVG Salary], SUM(Salary) AS TotalReward
FROM Employees
LEFT JOIN Departments ON Employees.DepartmentID = Departments.DepartmentID
LEFT JOIN EmployeesProjects ON Employees.EmployeeID = EmployeesProjects.EmployeeID
LEFT JOIN Projects ON EmployeesProjects.ProjectID = Projects.ProjectID
GROUP BY FirstName, LastName, Departments.Name
ORDER BY FirstName

SELECT COUNT(FirstName) AS People, Projects, SUM(Salary) AS TotalInvestment, ROUND(AVG(Salary), 0) AS [AVG_Salary]
FROM Grouped
--WHERE Projects IS NOT NULL
GROUP BY Projects

SELECT FirstName, LastName, Department, Projects
FROM Grouped
WHERE Projects = 'Bike Wash'

SELECT * FROM SoftUni.dbo.EmployeesProjects

SELECT * FROM SoftUni.dbo.Employees
WHERE FirstName = 'Dan' AND LastName = 'Wilson'

