--Section 1. DDL (30 pts)
CREATE DATABASE RentACar

GO

USE RentACar

GO

CREATE TABLE Clients(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(30) NOT NULL,
	LastName NVARCHAR(30) NOT NULL,
	Gender CHAR(1) CHECK(Gender = 'M' OR Gender = 'F'),
	BirthDate DATETIME,
	CreditCard NVARCHAR(30) NOT NULL,
	CardValidity DATETIME,
	Email NVARCHAR(50) NOT NULL
)

CREATE TABLE Towns(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50) NOT NULL,
)

CREATE TABLE Offices(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(50),
	ParkingPlaces INT,
	TownId INT FOREIGN KEY REFERENCES Towns(Id) NOT NULL
)

CREATE TABLE Models(
	Id INT PRIMARY KEY IDENTITY,
	Manufacturer NVARCHAR(50) NOT NULL,
	Model NVARCHAR(50) NOT NULL,
	ProductionYear DATETIME,
	Seats INT,
	Class NVARCHAR(10),
	Consumption DECIMAL(14,2)
)

CREATE TABLE Vehicles(
	Id INT PRIMARY KEY IDENTITY,
	ModelId INT FOREIGN KEY REFERENCES Models(Id) NOT NULL,
	OfficeId INT FOREIGN KEY REFERENCES Offices(Id) NOT NULL,
	Mileage INT
)

CREATE TABLE Orders(
	Id INT PRIMARY KEY IDENTITY,
	ClientId INT FOREIGN KEY REFERENCES Clients(Id) NOT NULL,
	TownId INT FOREIGN KEY REFERENCES Towns(Id) NOT NULL,
	VehicleId INT FOREIGN KEY REFERENCES Vehicles(Id) NOT NULL,
	CollectionDate DATETIME NOT NULL,
	CollectionOfficeId INT FOREIGN KEY REFERENCES Offices(Id) NOT NULL,
	ReturnDate DATETIME,
	ReturnOfficeId INT FOREIGN KEY REFERENCES Offices(Id),
	Bill DECIMAL(14,2),
	TotalMileage INT
)

--Section 2. DML (10 pts)
--2.	Insert
INSERT INTO Models(Manufacturer, Model, ProductionYear, Seats, Class, Consumption) VALUES
('Chevrolet',	'Astro',	'2005-07-27 00:00:00.000',	4,	'Economy',	12.60),
('Toyota',	'Solara',	'2009-10-15 00:00:00.000',	7,	'Family',	13.80),
('Volvo',	'S40',	'2010-10-12 00:00:00.000',	3,	'Average',	11.30),
('Suzuki',	'Swift',	'2000-02-03 00:00:00.000',	7,	'Economy',	16.20)

INSERT INTO Orders(ClientId, TownId, VehicleId, CollectionDate, CollectionOfficeId, 
					ReturnDate, ReturnOfficeId, Bill, TotalMileage) VALUES
(17,	2,	52,	'2017-08-08', 	30,	'2017-09-04', 	42,	2360.00,	7434),
(78,	17,	50,	'2017-04-22', 	10,	'2017-05-09', 	12,	2326.00,	7326),
(27,	13,	28,	'2017-04-25', 	21,	'2017-05-09', 	34,	597.00,	1880)

--3.	Update
UPDATE Models
SET Class = 'Luxury'
WHERE Consumption > 20

--4.	Delete
DELETE FROM Orders
WHERE ReturnDate IS NULL

--Section 3. Querying (40 pts)
--5.	Showroom
SELECT Manufacturer, Model
FROM Models
ORDER BY Manufacturer, Id DESC

--6.	Y Generation
SELECT FirstName, LastName 
FROM Clients
WHERE YEAR(BirthDate) BETWEEN 1977 AND 1994
ORDER BY FirstName,LastName, Id

--7.	Spacious Office
SELECT t.Name AS [TownName],
	o.Name AS [OfficeName],
	o.ParkingPlaces
FROM Offices AS o
JOIN Towns AS t ON o.TownId = t.Id
WHERE ParkingPlaces > 25
ORDER BY t.Name, o.Id

--8.	Available Vehicles
SELECT m.Model, m.Seats, v.Mileage
FROM Vehicles v
JOIN Models m
        ON m.Id = v.ModelId
     WHERE v.Id <> ALL(SELECT o.VehicleId 
                         FROM Orders o 
                        WHERE o.ReturnDate IS NULL)
  ORDER BY v.Mileage,
           m.Seats DESC,
           m.Id

--NOT AVAILABLE CARS
SELECT o.VehicleId FROM Vehicles AS v
JOIN Orders AS o ON v.Id = o.VehicleId
WHERE o.ReturnDate IS NULL

--There is duplicate cars
SELECT m.Model, m.Seats, v.Mileage
FROM Vehicles AS v
JOIN Models AS m ON v.ModelId = m.Id
JOIN Orders AS o ON v.Id = o.VehicleId
WHERE o.ReturnDate IS NOT NULL
ORDER BY v.Mileage, m.Seats DESC, m.Id

--9.	Offices per Town
SELECT t.Name AS [TownName], COUNT(*) AS [OfficesNumber] 
FROM Offices AS o
JOIN Towns AS t ON o.TownId = t.Id
GROUP BY t.Name
ORDER BY [OfficesNumber] DESC, t.Name

--10.	Buyers Best Choice 
SELECT 
	m.Manufacturer, 
	m.Model, 
	COUNT(o.Id) AS [TimesOrdered]
FROM Vehicles AS v
	JOIN Orders AS o ON v.Id = o.VehicleId
	JOIN Models AS m ON v.ModelId = m.Id
GROUP BY 
	m.Manufacturer, 
	m.Model
ORDER BY
	[TimesOrdered] DESC,
	m.Manufacturer DESC,
	m.Model

--11.	Kinda Person
SELECT 
	CONCAT(c.FirstName, ' ', c.LastName) AS [Names], 
	m.Class
FROM Clients AS c
	JOIN Orders AS o ON c.Id = o.ClientId
	JOIN Vehicles AS v ON o.VehicleId = v.Id
	JOIN Models AS m ON v.ModelId = m.Id
GROUP BY c.FirstName, c.LastName, m.Class, c.Id
ORDER BY Names, m.Class, c.Id

--12.	Age Groups Revenue
SELECT  g.AgeGroup, 
		SUM(ISNULL(o.Bill, 0)) AS [Revenue], 
		AVG(v.Mileage) AS [AverageMileage], 
		AVG(o.TotalMileage) AS [AverageMileage]
FROM(
	SELECT Id ,
		CASE
			WHEN YEAR(BirthDate) BETWEEN 1970 AND 1979 THEN '70''s'
			WHEN YEAR(BirthDate) BETWEEN 1980 AND 1989 THEN '80''s'
			WHEN YEAR(BirthDate) BETWEEN 1990 AND 1999 THEN '90''s'
			ELSE 'Others'
			END AS [AgeGroup]
	FROM Clients) AS g
JOIN Orders AS o ON g.Id = o.ClientId
JOIN Vehicles AS v ON o.VehicleId = v.Id
GROUP BY g.AgeGroup

GO

SELECT c.Id, SUM(ISNULL(o.Bill, 0))
FROM Clients AS c
JOIN Orders AS o ON c.Id = o.ClientId
JOIN Vehicles AS v ON o.VehicleId = v.Id
GROUP BY c.Id

GO
--13.	Consumption in Mind
WITH CTE_TopSeven AS
(
	SELECT TOP(7) v.ModelId, COUNT(o.Id) AS [Orders], AVG(m1.Consumption) AS [AverageConsumption]
	FROM Vehicles AS v
	JOIN Orders AS o ON v.Id = o.VehicleId
	JOIN Models AS m1 ON v.ModelId = m1.Id
	GROUP BY v.ModelId
	ORDER BY [Orders] DESC
)
SELECT m.Manufacturer, t7.AverageConsumption 
FROM CTE_TopSeven AS t7
JOIN Models AS m ON t7.ModelId = m.Id
WHERE [AverageConsumption] BETWEEN 5 AND 15

GO

SELECT m.Manufacturer, [AverageConsumption] 
FROM (
		SELECT TOP(7)   v.ModelId, 
						COUNT(o.Id) AS [Orders], 
						AVG(Consumption) AS [AverageConsumption]
		FROM Vehicles AS v
			JOIN Orders AS o ON v.Id = o.VehicleId
			JOIN Models AS m1 ON v.ModelId = m1.Id
		GROUP BY v.ModelId
		ORDER BY [Orders] DESC
	) AS [t7]
JOIN Models AS m ON t7.ModelId = m.Id
WHERE [AverageConsumption] BETWEEN 5 AND 15
GROUP BY m.Manufacturer, [AverageConsumption]

GO

--14.	Debt Hunter
SELECT [CategoryName], [Email], [Bill], [Town]
FROM (
SELECT  CONCAT(c.FirstName, ' ', c.LastName) AS [CategoryName], 
		c.Email AS [Email], 
		o.Bill AS [Bill], 
		t.Name AS [Town], 
		ROW_NUMBER() OVER(PARTITION BY o.TownId ORDER BY o.Bill DESC) AS [Rows],
		c.Id
FROM Orders AS o
JOIN Clients AS c ON o.ClientId = c.Id
JOIN Towns AS t ON o.TownId = t.Id
WHERE c.CardValidity < o.CollectionDate AND o.Bill IS NOT NULL)  AS [ClientsInfo]
WHERE Rows IN (1,2)
Order BY Town, Id 


--CTE Example
WITH cte_OrderPeople
AS
(
        SELECT c.Id,
               c.FirstName,
               c.LastName,
               c.Email,
               o.Bill,
               t.Name AS [Town],
               RANK() OVER(PARTITION BY t.Name ORDER BY o.Bill DESC) AS [Rank]
          FROM Clients c
    INNER JOIN Orders o
            ON o.ClientId = c.Id
    INNER JOIN Towns t
            ON t.Id = o.TownId
         WHERE c.CardValidity < o.CollectionDate AND o.Bill IS NOT NULL
    
)

  SELECT cte.FirstName + ' ' + cte.LastName AS [Category Name],
         cte.Email,
         cte.Bill,
         cte.[Town]
    FROM cte_OrderPeople cte
   WHERE cte.[Rank] IN (1, 2)
ORDER BY cte.[Town],
         cte.Bill,
         cte.Id

--15.	Town Statistics
WITH CTE_TownsMensOrders AS
(
	SELECT o.TownId, COUNT(c.Id) AS [MenCount]
	FROM Orders AS o
		JOIN Clients AS c ON o.ClientId = c.Id
		WHERE c.Gender = 'M'
		GROUP BY o.TownId
),

CTE_TownsWomensOrders AS
(
	SELECT o.TownId, COUNT(c.Id) AS [WomenCount]
	FROM Orders AS o
		JOIN Clients AS c ON o.ClientId = c.Id
		WHERE c.Gender = 'F'
		GROUP BY o.TownId
)

SELECT t.Name AS [TownName], 
		m.MenCount * 100 / (ISNULL(MenCount, 0) + ISNULL(WomenCount, 0)) AS [MalePercent], 
		w.WomenCount * 100 / (ISNULL(MenCount, 0) + ISNULL(WomenCount, 0)) AS [FemalePercent]
FROM Towns AS t
LEFT JOIN CTE_TownsMensOrders AS m ON t.Id = m.TownId
LEFT JOIN CTE_TownsWomensOrders AS w ON t.Id = w.TownId
ORDER BY t.Name, t.Id

--Null miss
SELECT o.TownId, c.Gender, COUNT(o.TownId), RANK() OVER(PARTITION BY c.Gender ORDER BY o.TownId)
FROM Orders AS o
JOIN Clients AS c ON o.ClientId = c.Id
GROUP BY o.TownId, c.Gender
ORDER BY TownId

--16.	Home Sweet Home
SELECT Id, CONCAT(Manufacturer, ' - ',  Model) AS [Vehicle], [Location], [Rank] FROM (
SELECT m.Manufacturer, m.Model, v.Id,
	CASE
		WHEN o.Id IS NULL OR v.OfficeId = o.ReturnOfficeId THEN 'home' 
		WHEN o.ReturnOfficeId <> v.OfficeId THEN CONCAT(tReturn.Name, ' - ', OfReturn.Name)
		WHEN o.ReturnDate IS NULL THEN 'on a rent'
		END AS [Location],
		DENSE_RANK() OVER (PARTITION BY v.Id ORDER BY o.CollectionDate DESC) AS [Rank]
FROM Vehicles AS v
LEFT JOIN Orders AS o ON v.Id = o.VehicleId
LEFT JOIN Towns AS t ON o.TownId = t.Id
JOIN Offices AS OrderOf ON v.OfficeId = OrderOf.Id
JOIN Towns AS tOrder ON OrderOf.TownId = tOrder.Id
LEFT JOIN Offices AS OfReturn ON o.ReturnOfficeId = OfReturn.Id
LEFT JOIN Towns AS tReturn ON OfReturn.TownId = tReturn.Id
JOIN Models AS m ON v.ModelId = m.Id) AS Result
WHERE [Rank] = 1
ORDER BY [Vehicle], Id

GO

 SELECT v.Id,
               o.ReturnOfficeId,
               v.OfficeId,
               m.Manufacturer,
               m.Model,
               DENSE_RANK() OVER (PARTITION BY v.Id ORDER BY o.CollectionDate DESC) AS [Rank],
			   o.CollectionDate,
			   o.ReturnDate
          FROM Vehicles AS v
    INNER JOIN Models m 
            ON m.Id = v.ModelId
     LEFT JOIN Orders o 
            ON o.VehicleId = v.Id
			ORDER BY v.Id
GO

WITH cte_AllCars 
AS
(
        SELECT v.Id,
               o.ReturnOfficeId,
               v.OfficeId,
               m.Manufacturer,
               m.Model,
               DENSE_RANK() OVER (PARTITION BY v.Id ORDER BY o.CollectionDate DESC) AS [Rank],
			   o.CollectionDate,
			   o.ReturnDate
          FROM Vehicles AS v
    INNER JOIN Models m 
            ON m.Id = v.ModelId
     LEFT JOIN Orders o 
            ON o.VehicleId = v.Id
			ORDER BY v.Id
)

  SELECT CONCAT(cte.Manufacturer, ' - ',  cte.Model) AS [Vehicle],
         CASE
             WHEN (SELECT COUNT(*) FROM Orders WHERE VehicleId = cte.Id) = 0 
					OR cte.OfficeId = cte.ReturnOfficeId THEN 'home'
             WHEN cte.ReturnOfficeId IS NULL THEN 'on a rent'
             WHEN cte.OfficeId <> cte.ReturnOfficeId THEN
                  (    SELECT CONCAT(t.[Name], ' - ', o.[Name])
                         FROM Offices AS o
                   INNER JOIN Towns AS t ON t.Id = o.TownId
                        WHERE cte.ReturnOfficeId = o.Id)
         END AS [Location]
    FROM cte_AllCars cte
   WHERE cte.[Rank] = 1
ORDER BY Vehicle, 
         cte.Id

GO
--Section 4. Programmability (14 pts)
--17.	Find My Ride
CREATE OR ALTER FUNCTION udf_CheckForVehicle(@townName NVARCHAR(50), @seatsNumber INT)
RETURNS NVARCHAR(MAX)
AS
	BEGIN
	DECLARE @FindCar NVARCHAR(50);
	SET @FindCar =(	SELECT TOP(1) CONCAT(o.Name, ' - ', m.Model)
					FROM Towns AS t
					JOIN Offices AS o ON t.Id = o.TownId
					JOIN Vehicles AS v ON o.Id = v.OfficeId
					JOIN Models AS m ON v.ModelId = m.Id
					WHERE t.Name = 'La Escondida' AND m.Seats = 9  
					ORDER BY o.Name)
	IF(@FindCar IS NULL)
		RETURN 'NO SUCH VEHICLE FOUND'

	RETURN @FindCar

	END

GO

SELECT dbo.udf_CheckForVehicle ('La Escondida', 9) 

GO
--18.	Move a Vehicle
CREATE PROCEDURE usp_MoveVehicle(@vehicleId INT, @officeId INT)
AS
	DECLARE @ParkingPlaces INT = (
									SELECT ParkingPlaces FROM Offices
									WHERE Id = 7)

	DECLARE @OfficeCarsCount INT = (
									SELECT COUNT(Id) FROM Vehicles
									WHERE OfficeId = 7)
	BEGIN TRANSACTION
		UPDATE Vehicles
		SET OfficeId = @officeId
		WHERE Id = @vehicleId

		IF(@OfficeCarsCount >= @ParkingPlaces)
			ROLLBACK;
			THROW 50001, 'Not enough room in this office!', 1;

		COMMIT
GO

--19.	Move the Tally
CREATE TRIGGER tr_AddTotalMileage
ON Orders
AFTER UPDATE
AS
BEGIN
    DECLARE @NewMileage INT 
    SET @NewMileage = (SELECT TotalMileage FROM INSERTED)

    DECLARE @VehicleId INT
    SET @VehicleId = (SELECT VehicleId FROM INSERTED)

    DECLARE @OldMileage INT
    SET @OldMileage = (SELECT TotalMileage FROM DELETED)

    IF (@OldMileage IS NULL)
    BEGIN
        UPDATE Vehicles
           SET Mileage += @NewMileage
         WHERE Id = @VehicleId
    END
END



