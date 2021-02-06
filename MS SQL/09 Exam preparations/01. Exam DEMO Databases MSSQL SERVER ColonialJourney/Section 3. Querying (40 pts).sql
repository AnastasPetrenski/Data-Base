USE ColonialJourney

--5.	Select all travel cards
SELECT CardNumber, JobDuringJourney
FROM TravelCards
ORDER BY CardNumber


--6.	Select all colonists
SELECT Id, 
	CONCAT(FirstName, ' ', LastName) AS [FullName], 
	Ucn
FROM Colonists
ORDER BY FirstName, LastName, Id


--7.	Select all military journeys
SELECT Id, JourneyStart, JourneyEnd 
FROM Journeys
WHERE Purpose = 'Military'
ORDER BY JourneyStart


--8.	Select all pilots
SELECT c.Id, CONCAT(FirstName, ' ', LastName) AS [full_name] 
FROM Colonists AS c
JOIN TravelCards AS tc ON c.Id = tc.ColonistId
WHERE tc.JobDuringJourney = 'Pilot'
ORDER BY c.Id


--9.	Count colonists
SELECT COUNT(*) AS [count]
FROM Colonists AS c
JOIN TravelCards AS tc ON c.Id = tc.ColonistId
Join Journeys AS j ON tc.JourneyId = j.Id
WHERE j.Purpose = 'Technical'

--Subquery
SELECT COUNT(*) AS [count]
FROM TravelCards
WHERE JourneyId IN (
					SELECT Id 
					FROM Journeys
					WHERE Purpose = 'Technical')


--10.	Select the fastest spaceship
SELECT TOP(1) s.[Name] AS [SpaceshipName],
	p.[Name] AS [SpaceportName]
FROM Spaceships AS [s]
JOIN Journeys AS [j] ON s.Id = j.SpaceshipId
JOIN Spaceports AS [p] ON j.DestinationSpaceportId = p.Id
ORDER BY s.LightSpeedRate DESC


--11.	Select spaceships with pilots younger than 30 years
SELECT s.Name, s.Manufacturer
FROM Spaceships AS [s]
JOIN Journeys AS [j] ON s.Id = j.SpaceshipId
JOIN TravelCards AS [tc] ON j.Id = tc.JourneyId
JOIN Colonists AS [c] ON tc.ColonistId = c.Id
WHERE tc.JobDuringJourney = 'Pilot' AND DATEDIFF(YEAR, c.BirthDate, '2019-01-01') < 30
GROUP BY s.Name, s.Manufacturer
ORDER BY s.[Name]


--12.	Select all educational mission planets and spaceports
--Start from Spaceports is the right solution
  SELECT [p].[Name] AS [PlanetName],
         [s].[Name] AS [SpaceportName]
    FROM [dbo].[Spaceports] AS s
    JOIN [dbo].[Planets] AS [p] ON [s].[PlanetId] = [p].[Id]
    JOIN [dbo].[Journeys] AS [j] ON [s].[Id] = [j].[DestinationSpaceportId]
   WHERE [j].[Purpose] = 'Educational'
ORDER BY [s].[Name] DESC;

--Starting from planets retrive different information
  SELECT p.[Name] AS [PlanetName],
	     s.[Name] AS [SpaceportName]
    FROM Planets AS [p]
    JOIN Spaceports AS [s] ON p.Id = s.PlanetId
    JOIN Journeys AS [j] ON p.Id = j.DestinationSpaceportId
   WHERE j.Purpose = 'Educational'
ORDER BY s.[Name] DESC


--13.	Select all planets and their journey count
  SELECT p.[Name] AS [PlanetName],
	     COUNT(*) AS [JourneysCount]
    FROM Journeys AS [j]
    JOIN Spaceports AS [s] ON j.DestinationSpaceportId = s.Id
    JOIN Planets AS [p] ON s.PlanetId = p.Id
GROUP BY p.[Name]
ORDER BY COUNT(*) DESC, p.[Name]

--SAME
 SELECT [p].[Name] AS [PlanetName],
         COUNT([j].[Id]) AS [JourneysCount]
    FROM [dbo].[Planets] AS p
    JOIN [dbo].[Spaceports] AS [s] ON [p].[Id] = [s].[PlanetId]
    JOIN [dbo].[Journeys] AS [j] ON [s].[Id] = [j].[DestinationSpaceportId]
GROUP BY [p].[Name]
ORDER BY [JourneysCount] DESC, [PlanetName];

--14.	Select the shortest journey
SELECT TOP(1) p.Id,
			  p.[Name] AS [PlanetName],
			  s.[Name] AS [SpaceportName],
			  j.Purpose AS [JourneyPurpose]
			  --DATEDIFF(SECOND, JourneyStart, JourneyEnd) AS Duration
FROM Journeys AS [j]
JOIN Spaceports AS [s] ON j.DestinationSpaceportId = s.Id
JOIN Planets AS [p] ON s.PlanetId = p.Id
ORDER BY DATEDIFF(SECOND, JourneyStart, JourneyEnd)


--15.	Select the less popular job
SELECT TOP(1) JourneyId,
	          JobDuringJourney AS [JobName]
FROM TravelCards
WHERE JourneyId =  (
					SELECT TOP(1) Id FROM Journeys
					ORDER BY DATEDIFF(SECOND, JourneyStart, JourneyEnd) DESC
				   )
GROUP BY JourneyId, JobDuringJourney
ORDER BY COUNT(JobDuringJourney)


--16.	Select Second Oldest Important Colonist
SELECT * FROM(
SELECT tc.JobDuringJourney, 
	   CONCAT(c.FirstName, ' ', c.LastName) AS [FullName], 
	   ROW_NUMBER() OVER(PARTITION BY tc.JobDuringJourney ORDER BY c.BirthDate) AS [RankBy]
FROM Colonists AS c
JOIN TravelCards AS tc ON c.Id = tc.ColonistId) AS [K]
WHERE k.RankBy = 2

--Custom Query
SELECT tc.JobDuringJourney, 
	   CONCAT(c.FirstName, ' ', c.LastName) AS [FullName], 
	   RANK() OVER(ORDER BY j.ID) AS RankJourneys,
	   DENSE_RANK() OVER(PARTITION BY tc.JourneyId ORDER BY tc.JobDuringJourney) AS [RankByJob],
	   ROW_NUMBER() OVER(PARTITION BY tc.JobDuringJourney ORDER BY c.BirthDate) AS [RankBy]
FROM Journeys AS [j]
JOIN TravelCards AS [tc] ON j.Id = tc.JourneyId
JOIN Colonists AS [c] ON tc.ColonistId = c.Id
ORDER BY RankJourneys


--17.	Planets and Spaceports
SELECT p.[Name], COUNT(*) AS [Count] 
FROM Planets AS p
JOIN Spaceports AS s ON p.Id = s.PlanetId
GROUP BY p.[Name]
ORDER BY [Count] DESC, p.[Name]





