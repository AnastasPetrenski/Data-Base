--5.	The "Tr" Planes
SELECT * FROM Planes
WHERE Name Like '%tr%'
ORDER BY Id, Name, Seats, Range

--6.	Flight Profits
SELECT f.Id, SUM(t.Price) AS [Price] 
FROM Flights f
JOIN Tickets AS t ON f.Id = t.FlightId
GROUP BY f.Id
ORDER BY SUM(t.Price) DESC, f.Id

--7.	Passenger Trips
SELECT CONCAT(FirstName, ' ', LastName) AS [FullName],
		f.Origin,
		f.Destination
FROM Passengers AS p
JOIN Tickets AS t ON p.Id = t.PassengerId
JOIN Flights AS f ON t.FlightId = f.Id
ORDER BY [FullName], Origin, Destination

--8.	Non Adventures People
SELECT p.FirstName, p.LastName, p.Age 
FROM Passengers AS p
FULL JOIN Tickets AS t ON p.Id = t.PassengerId
WHERE t.Id IS NULL
ORDER BY p.Age DESC, FirstName, LastName

--9.	Full Info
SELECT  CONCAT(FirstName, ' ', LastName) AS [Full Name],
		pl.Name AS [Plane Name],
		CONCAT(f.Origin, ' - ', f.Destination) AS [Trip],
		lt.Type
FROM Passengers AS p
JOIN Tickets AS t ON p.Id = t.PassengerId
JOIN Flights AS f ON t.FlightId = f.Id
JOIN Planes AS pl ON f.PlaneId = pl.Id
JOIN Luggages AS l ON p.Id = l.PassengerId
JOIN LuggageTypes AS lt ON l.LuggageTypeId = lt.Id
ORDER BY [Full Name], p.FirstName, f.Origin, f.Destination, lt.Type

--10.	PSP
SELECT p.Name, p.Seats, COUNT(t.Id) AS [Passengers Count]
FROM Planes AS p
LEFT JOIN Flights AS f ON p.Id = f.PlaneId
LEFT JOIN Tickets AS t ON f.Id = t.FlightId
GROUP BY p.Name, p.Seats
ORDER BY [Passengers Count] DESC, p.Seats


SELECT p.Name, p.Seats, COUNT(*) AS [Passengers Count]
FROM Planes AS p
LEFT JOIN Flights AS f ON p.Id = f.PlaneId
LEFT JOIN Tickets AS t ON f.Id = t.FlightId
GROUP BY p.Name, p.Seats
ORDER BY [Passengers Count] DESC, p.Seats

SELECT *
FROM Planes AS p
LEFT JOIN Flights AS f ON p.Id = f.PlaneId
LEFT JOIN Tickets AS t ON f.Id = t.FlightId




