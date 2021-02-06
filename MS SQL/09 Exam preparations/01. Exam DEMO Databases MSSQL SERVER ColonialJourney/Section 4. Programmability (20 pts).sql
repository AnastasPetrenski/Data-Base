USE ColonialJourney

GO
--18.	Get Colonists Count
CREATE OR ALTER FUNCTION dbo.udf_GetColonistsCount(@PlanetName VARCHAR (30))
RETURNS INT
AS
	BEGIN
		RETURN 
			(SELECT @PlanetName ,
				COUNT(*) AS [Count]
			 FROM Colonists AS c
			 JOIN TravelCards AS tc ON c.ID = tc.ColonistId
			 JOIN Journeys AS j ON tc.JourneyId = j.Id
			 JOIN Spaceports AS s ON j.DestinationSpaceportId = s.Id
			 JOIN Planets AS p ON s.PlanetId = p.Id
			 WHERE p.Name = @PlanetName
			)
	END
GO

CREATE OR ALTER FUNCTION dbo.udf_GetColonistsCount(@PlanetName VARCHAR (30))
RETURNS TABLE
AS
	
		RETURN 
			(SELECT @PlanetName AS [PlanetName],
				COUNT(*) AS [Count]
			 FROM Colonists AS c
			 JOIN TravelCards AS tc ON c.ID = tc.ColonistId
			 JOIN Journeys AS j ON tc.JourneyId = j.Id
			 JOIN Spaceports AS s ON j.DestinationSpaceportId = s.Id
			 JOIN Planets AS p ON s.PlanetId = p.Id
			 WHERE p.Name = @PlanetName
			)
	
GO

SELECT * FROM dbo.udf_GetColonistsCount('Otroyphus')

DROP FUNCTION dbo.udf_GetColonistsCount

GO

--19.	Change Journey Purpose
CREATE OR ALTER PROCEDURE usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose VARCHAR(11))
AS
	DECLARE @Purpose VARCHAR(11) = (
									SELECT Purpose FROM Journeys
									WHERE Id = @JourneyId)

	IF(@Purpose IS NULL)
		THROW 50001, 'The journey does not exist!', 1;
	ELSE IF(@Purpose = @NewPurpose)
		THROW 50002, 'You cannot change the purpose!', 1;
	ELSE

		UPDATE Journeys
		SET Purpose = @NewPurpose
		WHERE Id = @JourneyId

GO

CREATE OR ALTER PROCEDURE usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose VARCHAR(50))
AS
BEGIN
	DECLARE @targetId INT = (SELECT [j].[Id]
	                           FROM [dbo].[Journeys] AS j
							  WHERE [j].[Id] = @JourneyId)

	IF(@targetId IS NULL)
	BEGIN
		RAISERROR('The journey does not exist!', 16, 1)
		RETURN
	END

	DECLARE @targetPurpose VARCHAR(50) = (SELECT [j].[Purpose]
	                                        FROM [dbo].[Journeys] AS j
							               WHERE [j].[Id] = @JourneyId)

	IF(@targetPurpose = @NewPurpose)
	BEGIN
		RAISERROR('You cannot change the purpose!', 16, 2)
		RETURN
	END

	UPDATE [dbo].[Journeys]
	   SET
	       [dbo].[Journeys].[Purpose] = @NewPurpose
	 WHERE [dbo].[Journeys].[Id] = @JourneyId
END

EXEC usp_ChangeJourneyPurpose 1, 'Technical'
SELECT * FROM Journeys
EXEC usp_ChangeJourneyPurpose 1, 'Educational'
EXEC usp_ChangeJourneyPurpose 2, 'Educational'
EXEC usp_ChangeJourneyPurpose 11111, 'Technical'
EXEC usp_ChangeJourneyPurpose 1, 'Test'

GO
--20.	Deleted Journeys
CREATE TABLE DeletedJourneys(
	Id INT,
	JourneyStart DATETIME2,
	JourneyEnd DATETIME2,
	Purpose VARCHAR(11),
	DestinationSpaceportId INT,
	SpaceshipId INT
);

GO

CREATE TRIGGER tr_DeletedJourneys ON dbo.Journeys FOR DELETE
AS
	BEGIN
			INSERT INTO dbo.DeletedJourneys
			(
				Id,
				JourneyStart,
				JourneyEnd,
				Purpose,
				DestinationSpaceportId,
				SpaceshipId
			)
			SELECT * FROM [DELETED] AS d
	END

DELETE FROM TravelCards
WHERE JourneyId =  1

DELETE FROM Journeys
WHERE Id =  1