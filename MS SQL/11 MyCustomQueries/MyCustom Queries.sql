USE SoftUni

/*Self reference*/
SELECT Tracker.Employee, 
	d1.Name AS [Employee's Department], 
	Tracker.[Employee's Manager],
	Tracker.Department, 
	Tracker.[Department Manager]
FROM
	(SELECT CONCAT(e.FirstName, ' ', e.LastName) AS [Employee],
		e.DepartmentID,
		CONCAT(e1.FirstName, ' ', e1.LastName) AS [Employee's Manager],
		d.Name AS [Department],
		CONCAT(e2.FirstName, ' ', e2.LastName) AS [Department Manager] 
	FROM Employees e
	LEFT JOIN Employees e1 ON e.ManagerID  = e1.EmployeeID
	LEFT JOIN Departments d ON e1.DepartmentID = d.DepartmentID
	LEFT JOIN Employees e2 ON d.ManagerID = e2.EmployeeID) 
	AS [Tracker]
LEFT JOIN Departments d1 ON [Tracker].DepartmentID = d1.DepartmentID
ORDER BY Department

USE RecipesCodeFirst

/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [IngredientID]
      ,[Name]
      ,[Quantity]
      ,[RecipeID]
  FROM [RecipesCodeFirst].[dbo].[Ingredients]

/*LINQ can not generate properly result  => DONE!!! :) */
SELECT * FROM Recipes r
JOIN Ingredients i ON r.RecipeID = i.RecipeID
ORDER BY r.RecipeID

SELECT IngredientID, Name, Quantity,RecipeID
FROM Ingredients

USE SoftUni

SELECT * FROM Employees