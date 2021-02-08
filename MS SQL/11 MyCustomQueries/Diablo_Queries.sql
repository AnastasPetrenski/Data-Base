USE DIABLO

Select TOP(5) * FROM Users WHERE Id = 1
Select TOP(5) * FROM UsersGames WHERE UserId = 1
Select TOP(5)* FROM UserGameItems WHERE UserGameId = 56
SELECT * FROM Items

SELECT * FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id
JOIN [Statistics] AS s ON i.StatisticId = s.Id
JOIN Games AS g ON ug.GameId = g.Id
JOIN GameTypes AS gt ON g.GameTypeId = gt.Id
JOIN GameTypeForbiddenItems AS gtfi ON gt.Id = gtfi.GameTypeId
WHERE u.Id = 1 AND ugi.UserGameId = 10
ORDER BY GameId

--Select how many charachters an User has
SELECT * FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN Characters AS c ON ug.CharacterId = c.Id
JOIN [Statistics] AS s ON c.StatisticId = s.Id
JOIN Items AS i ON S.Id = i.Id
WHERE ug.UserId = 1
ORDER BY ug.[Level]

--Select item's stats for each character of the user
SELECT * FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id
JOIN [Statistics] AS s ON i.StatisticId = s.Id
WHERE ug.UserId = 1
ORDER BY ug.[Level]

--not all of the items have stats
SELECT * FROM Items AS i
LEFT JOIN UserGameItems AS ugi ON i.Id = ugi.ItemId
LEFT JOIN UsersGames AS ug ON ugi.UserGameId = ug.Id
LEFT JOIN Users AS u ON ug.UserId = u.Id
LEFT JOIN [Statistics] AS s ON i.Id = s.Id
WHERE u.Id IN (61, 52, 37, 12, 22) AND ug.Id IN (115, 26, 146, 177, 296)
ORDER BY ug.Level

CREATE OR ALTER FUNCTION GetUsersItemsStats()
RETURNS TABLE 
AS
	RETURN(
		--WITH [ItemStats] AS (
		SELECT u.Username, g.Name AS [Game Name], c.Name AS [Char Name], COUNT(*) AS [ItemCount], 
				SUM(s.Strength) AS [Strength], SUM(s.Defence) AS [Defence], SUM(s.Speed) AS [Speed], 
				SUM(s.Mind) AS [Mind], SUM(s.Luck) AS [Luck]
		FROM Users AS u
		JOIN UsersGames AS ug on u.Id = ug.UserId
		JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
		JOIN Items AS i ON ugi.ItemId = i.Id
		JOIN [Statistics] AS s ON i.StatisticId = s.Id
		JOIN Games AS g ON ug.GameId = g.Id
		JOIN Characters AS c ON ug.CharacterId = c.Id
		WHERE u.IsDeleted = 0 AND g.Name = 'London'
		GROUP BY u.Username, g.Name, c.Name
		ORDER BY [Game Name], u.Username--)
		)
		--INSERT INTO @Results SELECT [Game Name], [Char Name] FROM ItemStats
		

	
GO

SELECT * 
FROM Users AS u
JOIN UsersGames AS ug on u.Id = ug.UserId
JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id
JOIN [Statistics] AS s ON i.StatisticId = s.Id
JOIN Games AS g ON ug.GameId = g.Id
JOIN Characters AS c ON ug.CharacterId = c.Id
WHERE u.IsDeleted = 0
ORDER BY ug.GameId, ug.UserId

SELECT u.Username, g.Name AS [Game Name], c.Name AS [Char Name], COUNT(*)
FROM Users AS u
JOIN UsersGames AS ug on u.Id = ug.UserId
JOIN Games AS g ON ug.GameId = g.Id
JOIN Characters AS c ON ug.CharacterId = c.Id
WHERE u.IsDeleted = 0
GROUP BY u.Username, g.Name, c.Name
ORDER BY [Game Name], u.Username

SELECT * FROM Users u
JOIN UsersGames ug ON u.Id = ug.UserId
WHERE FirstName = 'Maria'

SELECT * FROM Users
WHERE ID = 15


--Sort games with users that have more than one CHAR in the Game
SELECT g.Name, u.Username, COUNT(*) ,ROW_NUMBER() OVER(PARTITION BY g.Name ORDER BY u.Username)
FROM Games AS g
JOIN UsersGames ug ON g.Id = ug.GameId
JOIN Users AS u ON ug.UserId = u.Id
GROUP BY g.Name,u.Username
HAVING COUNT(*) > 1


WITH [ItemStats] 
AS (
		SELECT ROW_NUMBER() OVER(ORDER BY u.Username) AS Rows,u.Username, g.Name AS [Game Name], c.Name AS [Char Name], COUNT(*) AS [ItemCount], 
				SUM(s.Strength) AS [Strength], SUM(s.Defence) AS [Defence], SUM(s.Speed) AS [Speed], 
				SUM(s.Mind) AS [Mind], SUM(s.Luck) AS [Luck]
		FROM Users AS u
		JOIN UsersGames AS ug on u.Id = ug.UserId
		JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
		JOIN Items AS i ON ugi.ItemId = i.Id
		JOIN [Statistics] AS s ON i.StatisticId = s.Id
		JOIN Games AS g ON ug.GameId = g.Id
		JOIN Characters AS c ON ug.CharacterId = c.Id
		WHERE u.IsDeleted = 0 AND g.Name = 'London'
		GROUP BY u.Username, g.Name, c.Name
		--ORDER BY [Game Name], u.Username
)

	SELECT 
	CASE 
		WHEN Username = LEAD(Username) OVER(ORDER BY [Rows]) AND 
			 [Game Name] = LEAD([Game Name]) OVER(ORDER BY [Rows]) THEN Username
			 ELSE Username
			 END
	,	s.Username, s.[Game Name], s.[Char Name], s.ItemCount, s.Strength, s.Defence, s.Speed, s.Mind, s.Luck,
		LEAD(s.Username) OVER(ORDER BY [Rows]) AS [SecondUser], 
		LEAD(s.[Game Name]) OVER(ORDER BY [Rows]) AS [SecondName], 
		LEAD(s.[Char Name]) OVER(ORDER BY [Rows]) AS [SecondChar], 
		LEAD(s.Strength) OVER(ORDER BY [Rows]) AS [SecondStrength], 
		LEAD(s.Defence) OVER(ORDER BY [Rows]) AS [SecondDefence], 
		LEAD(s.Speed) OVER(ORDER BY [Rows]) AS [SecondSpeed], 
		LEAD(s.Mind) OVER(ORDER BY [Rows]) AS [SecondMind], 
		LEAD(s.Luck) OVER(ORDER BY [Rows]) AS [SecondLuck]		
	FROM ItemStats AS s


--ORDER BY [Game Name], Username