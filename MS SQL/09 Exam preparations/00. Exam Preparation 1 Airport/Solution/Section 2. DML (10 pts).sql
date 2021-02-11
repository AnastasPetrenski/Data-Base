USE Airport

GO

--2.	Insert
INSERT INTO Planes (Name, Seats, Range) VALUES
('Airbus 336', 112, 5132),
('Airbus 330', 432, 5325),
('Boeing 369', 231, 2355),
('Stelt 297', 254, 2143),
('Boeing 338', 165, 5111),
('Airbus 558', 387, 1342),
('Boeing 128', 345, 5541)

INSERT INTO LuggageTypes (Type) VALUES 
('Crossbody Bag'),
('School Backpack'),
('Shoulder Bag')


--3.	Update
UPDATE Tickets
SET Price = Price * 1.13
WHERE FlightId = (SELECT Id FROM Flights
				  WHERE Destination Like 'Carlsbad')

--4.	Delete
--CHECKS
SELECT * FROM Flights
WHERE Destination = 'Ayn Halagim'

SELECT * FROM Planes
WHERE ID = 28

SELECT * FROM Tickets
WHERE FlightId = (SELECT ID FROM Flights
					WHERE Destination = 'Ayn Halagim')
-- SOLUTION
DELETE FROM Tickets
WHERE FlightId = (SELECT ID FROM Flights
					WHERE Destination = 'Ayn Halagim')

DELETE FROM Flights
WHERE Destination = 'Ayn Halagim'