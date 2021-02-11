CREATE FUNCTION udf_CalculateTickets(@origin VARCHAR, @destination VARCHAR, @peopleCount INT)
RETURNS VARCHAR(100)
AS
	BEGIN
		IF(@peopleCount <= 0)
			BEGIN
				RETURN 'Invalid people count!';
			END

		DECLARE @FlightID INT = (SELECT ID FROM Flights
								 WHERE Origin = @origin AND 
								 Destination = @destination);
		IF(@FlightID IS NULL)
			BEGIN
				RETURN 'Invalid flight!';
			END

		DECLARE @TicketPrice DECIMAL(18,2) = (SELECT Price FROM Tickets 
											  WHERE FlightId = @FlightID);

		DECLARE @TotalPrice DECIMAL(18,2) = @peopleCount * @TicketPrice;

		RETURN 'Total price ' + CAST(@TotalPrice AS VARCHAR(30)); 
	END

GO

EXEC udf_CalculateTickets('Kolyshley', 'Rancabolang', 33)