--Section 1. DDL (30 pts)
CREATE DATABASE Supermarket

GO

USE Supermarket

GO

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(30) NOT NULL
)

CREATE TABLE Items(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(30) NOT NULL,
	Price DECIMAL (18,2) NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL
)

CREATE TABLE Employees(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	Phone CHAR(12) NOT NULL,
	Salary DECIMAL(18,2) NOT NULL
)

CREATE TABLE Orders(
	Id INT PRIMARY KEY IDENTITY,
	DateTime DATETIME NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL
)

CREATE TABLE OrderItems(
	OrderId INT FOREIGN KEY REFERENCES Orders(Id) NOT NULL,
	ItemId INT FOREIGN KEY REFERENCES Items(Id) NOT NULL,
	Quantity INT NOT NULL CHECK(Quantity > 0),
	PRIMARY KEY (OrderId, ItemId)
)

CREATE TABLE Shifts(
	Id INT IDENTITY NOT NULL,
	EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
	CheckIn	DATETIME NOT NULL,
	CheckOut DATETIME NOT NULL,
	PRIMARY KEY(Id, EmployeeId),
	CONSTRAINT CHK_CheckIn_CheckOut CHECK(CheckIn < CheckOut)
)

--Section 2. DML (10 pts)
INSERT INTO Employees (FirstName, LastName, Phone, Salary) VALUES
  ('Stoyan',	'Petrov',	'888-785-8573',	500.25),
  ('Stamat',	'Nikolov',	'789-613-1122',	999995.25),
  ('Evgeni',	'Petkov',	'645-369-9517',	1234.51),
  ('Krasimir',	'Vidolov',	'321-471-9982',	50.25)

INSERT INTO Items (Name, Price, CategoryId) VALUES
  ('Tesla battery',154.25	,8),
  ('Chess',	30.25,	8),
  ('Juice',	5.32,1),
  ('Glasses',10,	8),
  ('Bottle of water',	1,	1)

--3. Update
UPDATE Items
SET Price = Price * 1.27
WHERE CategoryId IN(1,2,3)

--4. Delete
DELETE FROM OrderItems
WHERE OrderId = 48

--Section 3. Querying (40 pts)
--5. Richest People
SELECT Id, FirstName
FROM Employees
WHERE Salary > 6500
ORDER BY FirstName, Id

--6. Cool Phone Numbers
SELECT CONCAT(FirstName, ' ', LastName) AS [Full Name],
	Phone AS [Phone number]
FROM Employees
WHERE Phone LIKE '3%'
ORDER BY FirstName, [Phone number]

--7. Employee Statistics
SELECT FirstName, LastName, COUNT(o.Id)  AS [Count]
FROM Employees AS e
JOIN Orders AS o ON e.Id = o.EmployeeId
GROUP BY FirstName, LastName
ORDER BY [Count] DESC, FirstName

--8. Hard Workers Club
SELECT FirstName, LastName, AVG([Work Hours]) AS [Work Hours]
FROM (
	SELECT e.Id, FirstName, LastName , DATEDIFF(HOUR, CheckIn, CheckOut) AS [Work Hours] 
	FROM Employees AS e
	JOIN Shifts AS s ON e.Id = s.EmployeeId ) AS [Schedule]
GROUP BY FirstName, LastName, Id
HAVING AVG([Work Hours]) > 7
ORDER BY AVG([Work Hours])  DESC, [Schedule].Id

--Without subquery
SELECT
  FirstName,  LastName, 
  AVG(DATEDIFF(HOUR, s.CheckIn, s.CheckOut)) AS [Work Hours]
FROM Employees AS e
  JOIN Shifts AS s ON s.EmployeeId = e.Id
GROUP BY FirstName, LastName, e.Id
HAVING AVG(DATEDIFF(HOUR, s.CheckIn, s.CheckOut)) > 7
ORDER BY [Work Hours] DESC, e.Id

--9. The Most Expensive Order
SELECT TOP(1) 
	o.Id AS [OrderId], 
	SUM(oi.Quantity * i.Price) AS [TotalPrice] 
FROM Orders AS o
JOIN OrderItems AS oi ON o.Id = oi.OrderId
JOIN Items AS i ON oi.ItemId = i.Id
GROUP BY o.Id
ORDER BY [TotalPrice] DESC

--10. Rich Item, Poor Item
SELECT 
	oi.OrderId, 
	MAX(i.Price) AS [ExpensivePrice], 
	MIN(i.Price) AS [CheapPrice] 
FROM OrderItems AS oi
	JOIN Items AS i ON oi.ItemId = i.Id
GROUP BY oi.OrderId
ORDER BY MAX(i.Price) DESC, oi.OrderId

--11. Cashiers
SELECT e.Id, e.FirstName, e.LastName
FROM Employees AS e
	JOIN Orders AS o ON e.Id = o.EmployeeId
GROUP BY e.FirstName, e.LastName, e.Id
ORDER BY e.Id

--12. Lazy Employees

--14. Tough days
SELECT  e.FirstName + ' ' + e.LastName AS FullName,
CASE
  WHEN DATEPART(WEEKDAY, s.CheckIn) = 2
    THEN 'Monday'
  WHEN DATEPART(WEEKDAY, s.CheckIn) = 3
    THEN 'Tuesday'
  WHEN DATEPART(WEEKDAY, s.CheckIn) = 4
    THEN 'Wednesday'
  WHEN DATEPART(WEEKDAY, s.CheckIn) = 5
    THEN 'Thursday'
  WHEN DATEPART(WEEKDAY, s.CheckIn) = 6
    THEN 'Friday'
  WHEN DATEPART(WEEKDAY, s.CheckIn) = 7
    THEN 'Saturday'
  WHEN DATEPART(WEEKDAY, s.CheckIn) = 1
    THEN 'Sunday'
  END                            as DayOfWeek
FROM Employees AS e
LEFT JOIN Orders AS o ON e.Id = o.EmployeeId
JOIN Shifts AS s ON e.Id = s.EmployeeId
WHERE o.Id IS NULL AND DATEDIFF(HOUR, s.CheckIn, s.CheckOut) > 11
ORDER BY e.Id

--15. Top Order per Employee
SELECT CONCAT(e.FirstName, ' ', e.LastName) AS [Full Name],
		DATEDIFF(HOUR, s.CheckIn, s.CheckOut) AS [WorkHours],
		res.TotalPrice
FROM (
	SELECT o.Id, o.EmployeeId, o.DateTime, SUM(oi.Quantity * i.Price) AS [TotalPrice],
			ROW_NUMBER() OVER(PARTITION BY o.EmployeeId ORDER BY SUM(oi.Quantity * i.Price) DESC) AS Rank
	FROM Orders AS o
	JOIN OrderItems AS oi ON o.Id = oi.OrderId
	JOIN Items AS i ON oi.ItemId = i.Id
	GROUP BY o.EmployeeId, o.Id, o.DateTime
	) AS [res]
JOIN Employees AS e ON res.EmployeeId = e.Id
JOIN Shifts AS s ON e.Id = s.EmployeeId
WHERE res.Rank = 1 AND res.DateTime BETWEEN s.CheckIn AND s.CheckOut
ORDER BY [Full Name], [WorkHours] DESC, [TotalPrice] DESC


--16. Average Profit per Day
SELECT DATEPART(DAY,o.DateTime) AS [Day], FORMAT(AVG(oi.Quantity * i.Price), 'N2')
FROM Orders AS o
JOIN OrderItems AS oi ON o.Id = oi.OrderId
JOIN Items AS i ON oi.ItemId = i.Id
GROUP BY DATEPART(DAY,o.DateTime)
ORDER BY [Day]

--17. Top Products
SELECT i.Name, c.Name, COUNT(oi.Quantity) AS [Count], SUM(oi.Quantity * i.Price) AS [TotalPrice]
FROM Items AS i
JOIN OrderItems AS oi ON i.Id = oi.ItemId
JOIN Categories AS c ON i.CategoryId = c.Id 
GROUP BY i.Name, c.Name
ORDER BY TotalPrice DESC, Count DESC

GO
--Section 4. Programmability (20 pts)
--18. Promotion days
CREATE OR ALTER FUNCTION udf_GetPromotedProducts(@CurrentDate DATETIME, @StartDate DATETIME, @EndDate DATETIME, @Discount INT, 
				@FirstItemId INT, @SecondItemId INT, @ThirdItemId INT)
RETURNS VARCHAR(MAX)
AS
	BEGIN
		DECLARE @FirstItemPrice DECIMAL(18,2) = (SELECT Price FROM Items WHERE Id = @FirstItemId);
		DECLARE @SecondItemPrice DECIMAL(18,2) = (SELECT Price FROM Items WHERE Id = @SecondItemId);
		DECLARE @ThirdItemPrice DECIMAL(18,2) = (SELECT Price FROM Items WHERE Id = @ThirdItemId);

			IF(@FirstItemPrice IS NULL OR @SecondItemPrice IS NULL OR @ThirdItemPrice IS NULL)
				RETURN 'One of the items does not exists!';
			
		SET @FirstItemPrice = @FirstItemPrice - (@FirstItemPrice * @Discount / 100);
		SET @SecondItemPrice = @SecondItemPrice - (@SecondItemPrice * @Discount / 100)
		SET @ThirdItemPrice = @ThirdItemPrice - (@ThirdItemPrice * @Discount / 100)

		DECLARE @FirstItemName VARCHAR(100) = (SELECT Name FROM Items WHERE Id = @FirstItemId);
		DECLARE @SecondItemName VARCHAR(100) = (SELECT Name FROM Items WHERE Id = @SecondItemId);
		DECLARE @ThirdItemName VARCHAR(100) = (SELECT Name FROM Items WHERE Id = @ThirdItemId);
						
		IF(@CurrentDate NOT BETWEEN @StartDate AND @EndDate)
			RETURN 'The current date is not within the promotion dates!';

		RETURN CONCAT(@FirstItemName, ' price: ', @FirstItemPrice, ' <-> ',	@SecondItemName, ' price: ', 
						@SecondItemPrice, ' <-> ', @ThirdItemName, ' price: ', @ThirdItemPrice)
	END
GO

SELECT dbo.udf_GetPromotedProducts('2018-08-02', '2018-08-01', '2018-08-03',13, 3,4,5)
SELECT Price FROM Items WHERE Id IN (3,4,5)
GO

CREATE FUNCTION udf_Test_MyTest(@CurrentDate DATETIME, @StartDate DATETIME, @EndDate DATETIME)
RETURNS VARCHAR(MAX)
AS
	BEGIN
	IF(@CurrentDate NOT BETWEEN @StartDate AND @EndDate)
			RETURN 'The current date is not within the promotion dates!';

	RETURN 'Pass';
	END
GO

SELECT dbo.udf_Test_MyTest ('2018-08-02', '2018-08-01', '2018-08-03')

GO
--19. Cancel order
CREATE PROCEDURE usp_CancelOrder(@OrderId INT, @CancelDate DATETIME)
AS
	BEGIN
		DECLARE @OrderDate DATETIME = (SELECT [DateTime] FROM Orders WHERE Id = @OrderId)
			IF(@OrderDate IS NULL)
				THROW 50001, 'The order does not exist!', 1;

		IF((DATEDIFF(Day, @OrderDate, @CancelDate)) > 3)
			THROW 50002, 'You cannot cancel the order!', 1;

		DELETE FROM OrderItems
		WHERE OrderId = @OrderId;

		DELETE FROM Orders
		WHERE Id = @OrderId;
	END

GO

EXEC usp_CancelOrder 1000, '2021-02-12'

GO--20 Cancel order
CREATE TABLE DeletedOrders
(
	OrderId INT,
	ItemId INT,
	ItemQuantity INT
)

CREATE TRIGGER t_DeleteOrders
    ON OrderItems AFTER DELETE
    AS
    BEGIN
	  INSERT INTO DeletedOrders (OrderId, ItemId, ItemQuantity)
	  SELECT d.OrderId, d.ItemId, d.Quantity
	    FROM deleted AS d
    END

