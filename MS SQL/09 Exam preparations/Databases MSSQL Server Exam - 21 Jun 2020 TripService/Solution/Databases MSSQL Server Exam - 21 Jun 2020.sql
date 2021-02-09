--Section 1. DDL (30 pts)

CREATE DATABASE TripService

USE TripService

CREATE TABLE Cities(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(20) NOT NULL,
	CountryCode CHAR(2) NOT NULL
)

CREATE TABLE Hotels(
	Id INT PRIMARY KEY IDENTITY,
	Name NVARCHAR(30) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	EmployeeCount INT NOT NULL,
	BaseRate DECIMAL(18,2)
)

CREATE TABLE Rooms(
	Id INT PRIMARY KEY IDENTITY,
	Price DECIMAL(18,2) NOT NULL,
	Type NVARCHAR(20) NOT NULL,
	Beds INT NOT NULL,
	HotelId INT FOREIGN KEY REFERENCES Hotels(Id) NOT NULL
)

CREATE TABLE Trips(
	Id INT PRIMARY KEY IDENTITY,
	RoomId INT FOREIGN KEY REFERENCES Rooms(Id) NOT NULL,
	BookDate DATETIME NOT NULL,
	ArrivalDate DATETIME NOT NULL, 
	ReturnDate DATETIME NOT NULL,
	CancelDate DATETIME,

	CONSTRAINT CHK_BookDate CHECK (BookDate < ArrivalDate),
	CONSTRAINT CHK_ArrivalDate CHECK(ArrivalDate < ReturnDate)
)

CREATE TABLE Accounts(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(50) NOT NULL,
	MiddleName NVARCHAR(20),
	LastName NVARCHAR(50) NOT NULL,
	CityId INT FOREIGN KEY REFERENCES Cities(Id) NOT NULL,
	BirthDate DATE NOT NULL,
	Email VARCHAR(100) UNIQUE NOT NULL
)

CREATE TABLE AccountsTrips(
	AccountId INT FOREIGN KEY REFERENCES Accounts(Id) NOT NULL,
	TripId INT FOREIGN KEY REFERENCES Trips(Id) NOT NULL,
	Luggage INT NOT NULL CHECK(Luggage >= 0),
	PRIMARY KEY (AccountId, TripId)
)

--Section 2. DML (10 pts)
--2. Insert
INSERT INTO Accounts(FirstName, MiddleName, LastName, CityId, BirthDate, Email) VALUES
('John', 'Smith', 'Smith', 34, '1975-07-21', 'j_smith@gmail.com'),
('Gosho', NULL, 'Petrov', 11, '1978-05-16', 'g_petrov@gmail.com'),
('Ivan', 'Petrovich', 'Pavlov', 59, '1849-09-26', 'i_pavlov@softuni.bg'),
('Friedrich', 'Wilhelm', 'Nietzsche', 2, '1844-10-15', 'f_nietzsche@softuni.bg')

INSERT INTO Trips(RoomId, BookDate, ArrivalDate, ReturnDate, CancelDate) VALUES
(101, '2015-04-12', '2015-04-14', '2015-04-20', '2015-02-02'),
(102, '2015-07-07', '2015-07-15', '2015-07-22', '2015-04-29'),
(103, '2013-07-17', '2013-07-23', '2013-07-24', NULL),
(104, '2012-03-17', '2012-03-31', '2012-04-01', '2012-01-10'),
(109, '2017-08-07', '2017-08-28', '2017-08-29', NULL)

--3. Update
UPDATE Rooms
SET Price = Price * 1.14
WHERE HotelId IN (5, 7, 9)

--4. Delete
DELETE FROM AccountsTrips
WHERE AccountId = 47

DELETE FROM Accounts
WHERE ID = 47

--Section 3. Querying (40 pts)
SELECT FirstName, LastName, FORMAT(BirthDate, 'MM-dd-yyyy'), c.Name AS [Hometown], Email 
FROM Accounts AS a
JOIN Cities AS c ON a.CityId = c.Id
WHERE Email LIKE 'e%'
ORDER BY Hometown

--6. City Statistics
SELECT c.Name, COUNT(h.Id) AS [Hotels] 
FROM Cities AS c
JOIN Hotels AS h ON c.Id = h.CityId
GROUP BY c.Name
ORDER BY COUNT(*) DESC, c.Name

--Including cities without hotels
SELECT c.Name, COUNT(h.Id) AS [Hotels] 
FROM Cities AS c
LEFT JOIN Hotels AS h ON c.Id = h.CityId
GROUP BY c.Name
ORDER BY COUNT(h.Id) DESC, c.Name

--7. Longest and Shortest Trips
SELECT     a.Id AS AccountId, 
	CONCAT(FirstName, ' ', LastName) AS [FullName],
	MAX (DATEDIFF(DAY, ArrivalDate, ReturnDate)) AS [LongestTrip],
	MIN (DATEDIFF(DAY, ArrivalDate, ReturnDate)) AS [ShortestTrip]
FROM Accounts AS a
JOIN AccountsTrips AS at ON a.Id = at.AccountId
JOIN Trips AS t ON at.TripId = t.Id
WHERE MiddleName IS NULL AND CancelDate IS NULL
GROUP BY a.Id, FirstName, LastName
ORDER BY LongestTrip DESC, ShortestTrip

--8. Metropolis
--All towns
SELECT TOP(10) c.Id, 
			   c.Name AS [City], 
			   c.CountryCode AS [Country], 
			   COUNT(a.Id) AS [Accounts]
FROM Cities AS c
LEFT JOIN Accounts AS a ON c.Id = a.CityId
GROUP BY c.Id, c.Name, c.CountryCode
ORDER BY Accounts DESC

--Towns that have registrated accounts
SELECT TOP(10) c.Id, 
			   c.Name AS [City], 
			   c.CountryCode AS [Country], 
			   COUNT(a.Id) AS [Accounts]
FROM Cities AS c
JOIN Accounts AS a ON c.Id = a.CityId
GROUP BY c.Id, c.Name, c.CountryCode
ORDER BY Accounts DESC

--9. Romantic Getaways
SELECT  a.Id, 
		a.Email, 
		c.Name AS [City], 
		COUNT(*) AS [Trips] 
FROM Accounts AS a
	JOIN Cities AS c ON a.CityId = c.Id
	JOIN AccountsTrips AS at ON a.Id = at.AccountId
	JOIN Trips AS t ON at.TripId = t.Id
	JOIN Rooms AS r ON t.RoomId = r.Id
	JOIN Hotels AS h ON r.HotelId = h.Id
	JOIN Cities AS c2 ON h.CityId = c2.Id
WHERE a.CityId = h.CityId
GROUP BY a.Id, a.Email, c.Name
ORDER BY [Trips] DESC, a.Id

--10. GDPR Violation
SELECT  t.Id, 
		CONCAT(FirstName, ' ', ISNULL(MiddleName + ' ', ''), LastName) AS [Full Name],
		c.Name AS [From],
		c2.Name AS [To],
		CASE
			WHEN t.CancelDate IS NOT NULL THEN 'Canceled'
			ELSE CONCAT(CAST(DATEDIFF(DAY, t.ArrivalDate, t.ReturnDate) AS VARCHAR), ' days')
			END AS [Duration]
FROM Trips AS t
JOIN AccountsTrips AS at ON t.Id = at.TripId
JOIN Accounts AS a ON at.AccountId = a.Id
JOIN Cities AS c ON a.CityId = c.Id
JOIN Rooms AS r ON t.RoomId = r.Id
JOIN Hotels AS h ON r.HotelId = h.Id
JOIN Cities AS c2 ON h.CityId = c2.Id
ORDER BY [Full Name], t.Id

GO
--Section 4. Programmability (14 pts)
--11. Available Room
CREATE FUNCTION udf_GetAvailableRoom(@HotelId INT, @Date DATE, @People INT)
RETURNS VARCHAR(MAX)
AS
	BEGIN
	DECLARE @room_Id INT =  (SELECT TOP(1) r.Id
							FROM Hotels AS h
							JOIN Rooms AS r ON h.Id = r.HotelId
							JOIN Trips AS t ON r.Id = t.RoomId
							WHERE h.Id = @HotelId AND
								  r.Beds >= @People AND
						          @Date NOT BETWEEN t.ArrivalDate AND t.ReturnDate AND
								  t.CancelDate IS NULL AND 
								  YEAR(@Date) = YEAR(t.ArrivalDate)
								  ORDER BY r.Price DESC);
	
	IF (@room_Id IS NULL)
		BEGIN
			RETURN 'No rooms available';
		END

	DECLARE @roomType NVARCHAR(20) = (SELECT Type FROM Rooms WHERE Id =	@room_Id);
	DECLARE @roomBeds INT = (SELECT Beds FROM Rooms WHERE Id =	@room_Id);
	DECLARE @roomPrice DECIMAL(18,2) = (SELECT Price FROM Rooms WHERE Id = @room_Id);
	DECLARE @hotelBaseRate DECIMAL(18,2) = (SELECT BaseRate FROM Hotels WHERE Id = 
													(SELECT HotelId FROM Rooms WHERE id = @room_Id));
	DECLARE @totalPrice DECIMAL(18,2) = (@hotelBaseRate + @roomPrice) * @People;
	
	DECLARE @output NVARCHAR(MAX) = 
			CONCAT('Room ', @room_Id, ': ', @roomType, ' (', @roomBeds, ' beds) - $', @totalPrice);

	RETURN @output;
END

GO

--12. Switch Room
CREATE PROCEDURE usp_SwitchRoom(@TripId INT, @TargetRoomId INT)
AS
	BEGIN
	/*IF((SELECT COUNT(*) FROM Trips WHERE Id = @TripId) <> 1)
		THROW 50001, 'Invalid Id', 1;
	
	IF((SELECT COUNT(*) FROM Rooms WHERE Id = @TargetRoomId) <> 1)
		THROW 50001, 'Invalid Id', 1;*/

	DECLARE @TripHotelId INT =  (SELECT r.HotelId FROM Trips AS t
								JOIN Rooms AS r ON t.RoomId = r.Id
								WHERE t.Id = @TripId)

	DECLARE @TargetRoomHotelId INT =	(SELECT HotelId FROM Rooms 
										WHERE Id = @TargetRoomId)

	IF(@TripHotelId <> @TargetRoomHotelId)
		THROW 50002, 'Target room is in another hotel!', 1;

	DECLARE @TripRoomBeds INT = (SELECT COUNT(AccountId) FROM AccountsTrips WHERE TripId=@TripId)

	DECLARE @TargetRoomBeds INT = (SELECT Beds FROM Rooms WHERE Id = @TargetRoomId)

	IF(@TargetRoomBeds < @TripRoomBeds)
		THROW 50003, 'Not enough beds in target room!', 1;
		

	UPDATE Trips
	SET RoomId = @TargetRoomId
	WHERE Id = @TripId

	END

EXEC usp_SwitchRoom 10, 11
SELECT RoomId FROM Trips WHERE Id = 10
EXEC usp_SwitchRoom 10, 7
EXEC usp_SwitchRoom 10, 8


	-----------------------------
	SELECT * FROM Trips AS t
	JOIN Rooms AS r ON t.RoomId = r.Id
	JOIN Hotels AS h ON r.HotelId = h.Id
	WHERE t.Id = 10

	SELECT * FROM Rooms AS r
	JOIN Hotels AS h ON r.HotelId = h.Id
	JOIN Trips AS T ON r.Id = t.RoomId
	WHERE r.Id = 1
	
	SELECT * FROM AccountsTrips
	Where TripId = 154
	ORDER BY TripId


