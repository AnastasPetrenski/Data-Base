--Section 1. DDL 
CREATE DATABASE WMS

GO

USE WMS

Go

CREATE TABLE Clients(
	ClientId INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Phone CHAR(12) NOT NULL
)

CREATE TABLE Mechanics(
	MechanicId INT PRIMARY KEY IDENTITY,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	Address VARCHAR(255) NOT NULL
)

CREATE TABLE Models(
	ModelId INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Jobs(
	JobId INT PRIMARY KEY IDENTITY,
	ModelId INT FOREIGN KEY REFERENCES Models(ModelId) NOT NULL,
	[Status] VARCHAR(11) DEFAULT 'Pending' 
		CHECK([Status] IN ('Pending', 'In Progress', 'Finished')),
	ClientId INT FOREIGN KEY REFERENCES Clients(ClientId) NOT NULL,
	MechanicId INT FOREIGN KEY REFERENCES Mechanics(MechanicId),
	IssueDate Date NOT NULL, --NOT NULL??
	FinishDate Date
)

CREATE TABLE Orders(
	OrderId INT PRIMARY KEY IDENTITY,
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId) NOT NULL,
	IssueDate DATE,
	Delivered BIT DEFAULT 0 NOT NULL
)

CREATE TABLE Vendors(
	VendorId INT PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) UNIQUE NOT NULL
)

CREATE TABLE Parts(
	PartId INT PRIMARY KEY IDENTITY,
	SerialNumber VARCHAR(50) UNIQUE NOT NULL,
	[Description] VARCHAR(255),
	Price DECIMAL(18,2) NOT NULL CHECK(Price > 0),
	VendorId INT FOREIGN KEY REFERENCES Vendors(VendorId) NOT NULL,
	StockQty INT DEFAULT 0 NOT NULL CHECK(StockQty >= 0)
)

CREATE TABLE OrderParts(
	OrderId INT FOREIGN KEY REFERENCES Orders(OrderId) NOT NULL,
	PartId INT FOREIGN KEY REFERENCES Parts(PartId) NOT NULL,
	Quantity INT DEFAULT 1 CHECK(Quantity > 0),
	PRIMARY KEY(OrderId, PartId)
)

CREATE TABLE PartsNeeded(
	JobId INT FOREIGN KEY REFERENCES Jobs(JobId) NOT NULL,
	PartId INT FOREIGN KEY REFERENCES Parts(PartId) NOT NULL,
	Quantity INT DEFAULT 1 CHECK(Quantity > 0),
	PRIMARY KEY(JobId, PartId)
)

--Section 2. DML
--2.	Insert
INSERT INTO Clients(FirstName, LastName, Phone) VALUES
('Teri',	'Ennaco',	'570-889-5187'),
('Merlyn',	'Lawler',	'201-588-7810'),
('Georgene',	'Montezuma',	'925-615-5185'),
('Jettie',	'Mconnell',	'908-802-3564'),
('Lemuel',	'Latzke',	'631-748-6479'),
('Melodie',	'Knipp',	'805-690-1682'),
('Candida',	'Corbley',	'908-275-8357')

INSERT INTO Parts(SerialNumber, Description, Price, VendorId) VALUES
('WP8182119',	'Door Boot Seal',	117.86,	2),
('W10780048',	'Suspension Rod',	42.81,	1),
('W10841140',	'Silicone Adhesive', 	6.77,	4),
('WPY055980',	'High Temperature Adhesive',	13.94,	3)

--3.	Update
UPDATE Jobs 
SET MechanicId = 3, Status = 'In Progress'
WHERE Status = 'Pending'

--4.	Delete
DELETE FROM OrderParts WHERE OrderId = 19

DELETE FROM Orders WHERE OrderId = 19

--Section 3. Querying 
--5.	Mechanic Assignments
SELECT  CONCAT(m.FirstName, ' ', m.LastName) AS [Mechanic],
		j.Status,
		j.IssueDate
FROM Mechanics AS m
JOIN Jobs AS j ON m.MechanicId = j.MechanicId
ORDER BY m.MechanicId, j.IssueDate, j.JobId

--6.	Current Clients
SELECT  CONCAT(c.FirstName, ' ', c.LastName) AS [Client],
		DATEDIFF(DAY, j.IssueDate, '2017-04-24') AS [Days going],
		j.Status
FROM Clients AS c
JOIN Jobs AS j ON c.ClientId = j.ClientId
WHERE j.Status NOT LIKE 'Finished'
ORDER BY [Days going] DESC, c.ClientId

--7.	Mechanic Performance
SELECT [Mechanic],
		AVG([Days])
FROM(
SELECT  CONCAT(m.FirstName, ' ', m.LastName) AS [Mechanic],
		DATEDIFF(DAY, j.IssueDate, j.FinishDate) AS [Days],
		m.MechanicId
FROM Mechanics AS m
JOIN Jobs AS j ON m.MechanicId = j.MechanicId) AS [Result]
WHERE [Days] IS NOT NULL
GROUP BY [Mechanic], MechanicId
ORDER BY MechanicId

--8.	Available Mechanics

SELECT CONCAT(m.FirstName, ' ', m.LastName) AS [Available]
FROM Mechanics AS m
LEFT JOIN Jobs AS j ON m.MechanicId = j.MechanicId
WHERE j.JobId IS NULL OR j.Status LIKE 'Finished'
GROUP BY m.MechanicId, m.FirstName, m.LastName
ORDER BY m.MechanicId

SELECT [Available], Status
FROM (
SELECT  CONCAT(m.FirstName, ' ', m.LastName) AS [Available],
		Status, 
		ROW_NUMBER() OVER(PARTITION BY FirstName ORDER BY m.MechanicId) AS [Rows],
		m.MechanicId
FROM Mechanics AS m
LEFT JOIN Jobs AS j ON m.MechanicId = j.MechanicId) AS [Result]
--WHERE Result.Rows = 1
GROUP BY [Available], Status, MechanicId
ORDER BY MechanicId

GO

--9.	Past Expenses

SELECT j.JobId, ISNULL(SUM(op.Quantity * p.Price), 0) AS Total
FROM Jobs AS j
LEFT JOIN Orders AS o ON j.JobId = o.JobId
LEFT JOIN OrderParts AS op ON o.OrderId = op.OrderId
LEFT JOIN Parts AS p ON op.PartId = p.PartId
WHERE j.Status = 'Finished' --AND o.Delivered = 1
GROUP BY j.JobId
ORDER BY Total DESC, j.JobId

--10.	Missing Parts
WITH CTE_JobsNotDoneYet AS(
	SELECT  pn.PartId,
			SUM(pn.Quantity) AS [Required],
			p.StockQty AS [In Stock]
	FROM Jobs AS j
	JOIN PartsNeeded AS pn ON j.JobId = pn.JobId
	JOIN Parts AS p ON pn.PartId = p.PartId
	WHERE Status NOT LIKE 'Finished' 
	GROUP BY pn.PartId, p.StockQty),

CTE_PendingProductsQuantities AS
(
	SELECT p.PartId, SUM(op.Quantity) AS [PendingQty] 
	FROM Parts AS p
	JOIN OrderParts AS op ON p.PartId = op.PartId
	JOIN Orders AS o ON op.OrderId = o.OrderId 
	WHERE Delivered = 0
	GROUP BY p.PartId
)

SELECT p.PartId, p.Description, jndy.Required, jndy.[In Stock], ISNULL(ppq.PendingQty, 0) AS [Ordered]
FROM CTE_JobsNotDoneYet AS jndy			
JOIN Parts AS p ON jndy.PartId = p.PartId
LEFT JOIN CTE_PendingProductsQuantities AS ppq ON jndy.PartId = ppq.PartId
WHERE jndy.Required > jndy.[In Stock] + ISNULL(ppq.PendingQty, 0)
ORDER BY jndy.PartId


--Custome query
SELECT  pn.PartId, 
		p.Description, 
		SUM(pn.Quantity) AS [Required], 
		p.StockQty AS [In Stock]
		--(p.StockQty - SUM(pn.Quantity)) AS Orderred
FROM Jobs AS j
JOIN PartsNeeded AS pn ON j.JobId = pn.JobId
JOIN Parts AS p ON pn.PartId = p.PartId
JOIN OrderParts AS op ON p.PartId = op.PartId
JOIN Orders AS o ON op.OrderId = o.OrderId
WHERE Status NOT LIKE 'Finished' 
GROUP BY pn.PartId, p.Description, p.StockQty 
HAVING (p.StockQty - SUM(pn.Quantity)) < 0
ORDER BY pn.PartId

SELECT pn.PartId, SUM(pn.Quantity) AS InStock
FROM Jobs AS j
JOIN PartsNeeded AS pn ON j.JobId = pn.JobId
WHERE Status NOT LIKE 'Finished' 
GROUP BY pn.PartId


GO

--Section 4. Programmability
--11.	Place Order
CREATE OR ALTER PROCEDURE usp_PlaceOrder(@jobID INT, @partSerialNumber VARCHAR(50), @quantity INT)
AS
	BEGIN
		IF(@quantity <= 0)
			THROW 50012, 'Part quantity must be more than zero!', 1;

		IF((SELECT COUNT(*) FROM Jobs WHERE JobId = @jobId) <> 1)
			THROW 50013, 'Job not found!', 1;

		/*IF EXISTS (SELECT * FROM Jobs WHERE JobId = @jobID AND FinishDate IS NOT NULL)
			ROLLBACK;
			THROW 50011, 'This job is not active!', 1;*/

		IF((SELECT FinishDate FROM Jobs WHERE JobId = @jobID) IS NOT NULL)
			THROW 50011, 'This job is not active!', 1;

		DECLARE @partId INT = (SELECT PartId FROM Parts WHERE SerialNumber = @partSerialNumber)

		IF(@partId IS NULL)
			THROW 50014, 'Part not found!', 1;

		DECLARE @isOrderExist INT = (SELECT COUNT(*) FROM Orders WHERE JobId = @jobID AND IssueDate IS NULL)
		IF (@isOrderExist <> 1)
			BEGIN
				INSERT INTO Orders(JobId, IssueDate, Delivered)
                VALUES (@JobId, NULL, 0)
			END

		DECLARE @orderID INT = (SELECT OrderId FROM Orders WHERE JobId = @jobID AND IssueDate IS NULL)

		IF((SELECT COUNT(*) FROM OrderParts WHERE PartId = @partId AND OrderId = @orderID) = 0)
					INSERT INTO OrderParts(OrderId, PartId, Quantity) 
					VALUES (@orderID, @partId, @quantity);
		ELSE
					UPDATE OrderParts
					SET Quantity += @quantity
					WHERE OrderId = @orderID AND PartId = @partId
			
	END
GO
--'285811' '80040' '285805'
DECLARE @err_msg AS NVARCHAR(MAX);
BEGIN TRY
  EXEC usp_PlaceOrder 46, '285805', 1
END TRY

BEGIN CATCH
  SET @err_msg = ERROR_MESSAGE();
  SELECT @err_msg
END CATCH

SELECT * FROM JOBS AS j
JOIN Orders AS o ON j.JobId = o.JobId
JOIN OrderParts AS op ON o.OrderId = op.OrderId
WHERE j.JobId = 46

SELECT o.OrderId
FROM Orders AS o
WHERE o.JobId = 46
AND o.IssueDate IS NULL

UPDATE Orders
SET IssueDate = GETDATE()
WHERE JobId = 46

GO
--12.	Cost Of Order
CREATE FUNCTION udf_GetCost(@jobId INT)
RETURNS DECIMAL(18,2)
AS
	BEGIN
	IF((SELECT COUNT(*) FROM Jobs WHERE JobId = @jobId) <> 1)
		BEGIN
			RETURN 0.00;
		END

	DECLARE @Orders INT = (SELECT COUNT(*) FROM Orders WHERE JobId = @jobId)
	IF(@Orders <= 0)
		BEGIN
			RETURN 0.00;
		END

	DECLARE @TotalCost DECIMAL(18,2) = (SELECT SUM(op.Quantity * p.Price) AS [Result]
										FROM Jobs AS j
										JOIN Orders AS o ON j.JobId = o.JobId
										JOIN OrderParts AS op ON o.OrderId = op.OrderId
										JOIN Parts AS p ON op.PartId = p.PartId
										WHERE j.JobId = @jobId
										GROUP BY j.JobId)
	
	RETURN @TotalCost;
	END
GO