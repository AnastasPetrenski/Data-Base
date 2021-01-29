USE Gringotts

SELECT * FROM WizzardDeposits

--1. Records’ Count
SELECT COUNT(*) AS [Count]
FROM WizzardDeposits

--2. Longest Magic Wand
SELECT TOP(1) MagicWandSize 
FROM WizzardDeposits
ORDER BY MagicWandSize DESC

SELECT MAX(MagicWandSize) AS [LongestMagicWand]
FROM WizzardDeposits

--3. Longest Magic Wand Per Deposit Groups
SELECT DepositGroup, MAX(MagicWandSize) AS LongestMagicWand 
FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY MagicWandSize DESC

--4. * Smallest Deposit Group Per Magic Wand Size
SELECT TOP(2) DepositGroup
FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY AVG(MagicWandSize) 

SELECT DepositGroup, AVG(MagicWandSize) AS AvgWandSize
FROM WizzardDeposits
GROUP BY DepositGroup
ORDER BY AVG(MagicWandSize)

--5. Deposits Sum
SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
GROUP BY DepositGroup

--6. Deposits Sum for Ollivander Family
SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
WHERE MagicWandCreator LIKE '%Ollivander%'
GROUP BY DepositGroup

SELECT * FROM WizzardDeposits
WHERE MagicWandCreator LIKE '%Ollivander%'
ORDER BY DepositGroup

--7. Deposits Filter
SELECT DepositGroup, SUM(DepositAmount) AS TotalSum
FROM WizzardDeposits
WHERE MagicWandCreator LIKE '%Ollivander%'
GROUP BY DepositGroup
HAVING SUM(DepositAmount) < 150000
ORDER BY TotalSum DESC

--8.  Deposit Charge
SELECT DepositGroup, MagicWandCreator, MIN(DepositCharge) AS [MinDepositCharge]
FROM WizzardDeposits
GROUP BY DepositGroup, MagicWandCreator
ORDER BY MagicWandCreator, DepositGroup

--9. Age Groups
SELECT  
					CASE	
						WHEN AGE <= 10 THEN '[0-10]'
						WHEN AGE BETWEEN 11 AND 20 THEN '[11-20]'
						WHEN AGE BETWEEN 21 AND 30 THEN '[21-30]'
						WHEN AGE BETWEEN 31 AND 40 THEN '[31-40]'
						WHEN AGE BETWEEN 41 AND 50 THEN '[41-50]'
						WHEN AGE BETWEEN 51 AND 60 THEN '[51-60]'
						WHEN AGE > 60 THEN '[61+]'
						END AS [AgeGroup],
						COUNT(*) AS WizardCount
FROM WizzardDeposits
GROUP BY (CASE	
						WHEN AGE <= 10 THEN '[0-10]'
						WHEN AGE BETWEEN 11 AND 20 THEN '[11-20]'
						WHEN AGE BETWEEN 21 AND 30 THEN '[21-30]'
						WHEN AGE BETWEEN 31 AND 40 THEN '[31-40]'
						WHEN AGE BETWEEN 41 AND 50 THEN '[41-50]'
						WHEN AGE BETWEEN 51 AND 60 THEN '[51-60]'
						WHEN AGE > 60 THEN '[61+]'
						END)


--with subquery
SELECT AgeGroup, COUNT(*) AS [WizardCount] 
FROM (
				SELECT  
					CASE	
						WHEN AGE <= 10 THEN '[0-10]'
						WHEN AGE BETWEEN 11 AND 20 THEN '[11-20]'
						WHEN AGE BETWEEN 21 AND 30 THEN '[21-30]'
						WHEN AGE BETWEEN 31 AND 40 THEN '[31-40]'
						WHEN AGE BETWEEN 41 AND 50 THEN '[41-50]'
						WHEN AGE BETWEEN 51 AND 60 THEN '[51-60]'
						WHEN AGE > 60 THEN '[61+]'
						END AS [AgeGroup], *
				FROM WizzardDeposits
	) AS AgeGroupQuery 
GROUP BY AgeGroup

--10. First Letter
SELECT FirstLetter 
FROM 
	(SELECT LEFT(FirstName, 1) AS FirstLetter, * 
	FROM WizzardDeposits) 
	AS GetFirstLetterQuery
WHERE DepositGroup = 'Troll Chest'
GROUP BY FirstLetter

--11. Average Interest 
SELECT DepositGroup, IsDepositExpired, AVG(DepositInterest) AS AverageInterest 
FROM WizzardDeposits
WHERE DepositStartDate > '1985-01-01'
GROUP BY DepositGroup, IsDepositExpired
ORDER BY DepositGroup DESC

--12. * Rich Wizard, Poor Wizard
SELECT SUM([Diff]) AS [SumDifference] FROM (
		SELECT FirstName AS [Host Wizzard], 
			   DepositAmount AS [Host Wiazzard Deposit],
			   LEAD(FirstName) OVER(ORDER BY Id ASC) AS [Guest Wizzard],
			   LEAD(DepositAmount) OVER(ORDER BY Id ASC) AS [Guest Wizzard Deposit],
			   DepositAmount - LEAD(DepositAmount) OVER(ORDER BY Id ASC) AS [Diff]
		FROM WizzardDeposits) AS [LeadQuery]
WHERE [Guest Wizzard] IS NOT NULL

--MyWay using variables and while loop syntaxis
DECLARE @DepositOne DECIMAL(18,2)
DECLARE @DepositTwo DECIMAL(18,2)
DECLARE @TotalDiff DECIMAL(18,2)
DECLARE @Counter INT
SET @TotalDiff = 0
SET @Counter = 1
WHILE (@Counter < (SELECT COUNT(*) FROM WizzardDeposits))
BEGIN
	SELECT @DepositOne = DepositAmount
	FROM WizzardDeposits WHERE Id = @Counter

	SELECT @DepositTwo = DepositAmount
	FROM WizzardDeposits WHERE Id = (@Counter + 1)

	--PRINT CONVERT(VARCHAR,@TotalDiff) + 'Test' + Convert(VARCHAR,(@DepositOne - @DepositTwo)) + 
	--'Counter' + Convert(VARCHAR,(@Counter))
	SET @TotalDiff = @TotalDiff + (@DepositOne - @DepositTwo)
	SET @COUNTER = @Counter + 1 
END
SELECT @TotalDiff AS [SumDifference]

USE SoftUni

--13. Departments Total Salaries
SELECT DepartmentID, 
	   ROUND(SUM(Salary), 2) AS [TotalSalary]
FROM Employees
GROUP BY DepartmentID
ORDER BY DepartmentID

--14. Employees Minimum Salaries
SELECT DepartmentID, MIN(Salary) AS MinumumSalary 
FROM Employees
WHERE DepartmentID IN (2,5,7) AND 
		HireDate > '2000-01-01'
GROUP BY DepartmentID
ORDER BY DepartmentID

SELECT DepartmentID, Salary AS [MinimumSalary] FROM
(SELECT DepartmentID, Salary, HireDate,
		DENSE_RANK() OVER(PARTITION BY DepartmentID ORDER BY Salary) AS [RankSalary] 
FROM Employees) AS [SalaryRankingQuery]
WHERE DepartmentID IN (2,5,7) AND
	  HireDate > '2000-01-01' AND RankSalary = 1
GROUP BY DepartmentID, Salary
ORDER BY DepartmentID

--15. Employees Average Salaries
SELECT * INTO [FilterSalary]
FROM Employees 
WHERE Salary > 30000 

DELETE FROM [FilterSalary]
WHERE ManagerID = 42

UPDATE [FilterSalary]
SET Salary += 5000
WHERE DepartmentID = 1

SELECT DepartmentID, AVG(Salary) AS AverageSalary
FROM [FilterSalary]
GROUP BY DepartmentID

--https://www.w3resource.com/sql/delete-statement/delete-with-subqueries.php
DELETE FROM Employees
WHERE ManagerID = ANY
					(SELECT *
					FROM Employees 
					WHERE Salary > 30000 AND ManagerID = 42)

--16. Employees Maximum Salaries
SELECT DepartmentID, MAX(Salary) AS [MaxSalary]
FROM Employees
GROUP BY DepartmentID
HAVING MAX(Salary) NOT BETWEEN 30000 AND 70000

--17. Employees Count Salaries
SELECT COUNT(Salary) AS [Count] FROM Employees
WHERE ManagerID IS NULL

--18. *3rd Highest Salary
SELECT DepartmentID, ROUND(Salary, 2) AS ThirdHighestSalary FROM (
SELECT *, DENSE_RANK() OVER(PARTITION BY DepartmentID ORDER BY Salary DESC) AS [SalaryRanking]
FROM Employees
) AS [RankingQuery]
WHERE [SalaryRanking] = 3
GROUP BY DepartmentID, Salary

--19. **Salary Challenge
SELECT TOP(10) FirstName,
		LastName,
		DepartmentID
FROM Employees AS e1
WHERE e1.Salary > (
					SELECT AVG(Salary) AS [AverageSalary]
					FROM Employees AS e2
					WHERE e2.DepartmentID = e1.DepartmentID
					GROUP BY DepartmentID
					) 
ORDER BY DepartmentID 

GO
--return the same result but in different order
--SELECT DepartmentID, AVG(Salary) AS [Average] INTO [Filter]
--FROM Employees
--GROUP BY DepartmentID

SELECT TOP(10) FirstName, LastName, e.DepartmentID 
FROM Employees e
JOIN (  SELECT DepartmentID, AVG(Salary) AS [Average]
		FROM Employees
		GROUP BY DepartmentID ) 
AS f ON e.DepartmentID = f.DepartmentID
WHERE e.Salary > f.Average
ORDER BY e.DepartmentID
