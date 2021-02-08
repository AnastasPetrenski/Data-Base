/***** PART I – Queries for Diablo Database ******/

USE Diablo

--Problem 1.	Number of Users for Email Provider
SELECT *, COUNT(*) AS [Nunber of Users] FROM (
SELECT SUBSTRING(u.Email, CHARINDEX('@',u.Email,0) + 1, LEN(u.Email) - CHARINDEX('@',u.Email,0)) AS [Email Provider]
FROM Users AS u) AS O
GROUP BY [Email Provider]
ORDER BY COUNT(*) DESC, [Email Provider]

SELECT CHARINDEX( '@',Email,0) FrOM Users

--Problem 2.	All User in Games
SELECT g.[Name] AS [Game],
	   gt.[Name] AS [Game Type],
	   u.Username,
	   ug.[Level],
	   ug.Cash,
	   c.[Name] AS [Character]
FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN Games AS g ON ug.GameId = g.Id
JOIN GameTypes AS gt ON g.GameTypeId = gt.Id
JOIN Characters AS c ON ug.CharacterId = c.Id
ORDER BY ug.[Level] DESC, u.Username, g.[Name]

--Problem 3.	Users in Games with Their Items
SELECT u.Username, g.Name, COUNT(*) AS [Items Count], SUM(i.Price)
FROM Users AS u
JOIN UsersGames AS ug ON u.Id = ug.UserId
JOIN Games AS g ON ug.GameId = g.Id
JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id
GROUP BY u.Username, g.Name
HAVING COUNT(*) >= 10
ORDER BY COUNT(*) DESC, SUM(i.Price) DESC, u.Username


--Problem 4.	* User in Games with Their Statistics
SELECT  u.Username, 
	    g.Name AS [Game Name],
		MAX(c.Name) AS [Char Name],  
		SUM(s.Strength) + MAX(s1.Strength) + MAX(s2.Strength) AS [Strength], 
		SUM(s.Defence) + MAX(s1.Defence) + MAX(s2.Defence) AS [Defence], 
		SUM(s.Speed) + MAX(s1.Speed) + MAX(s2.Speed) AS [Speed], 
		SUM(s.Mind) + MAX(s1.Mind) + MAX(s2.Mind) AS [Mind], 
		SUM(s.Luck) + MAX(s1.Luck) + MAX(s2.Luck) AS [Luck]
FROM Users AS u
JOIN UsersGames AS ug on u.Id = ug.UserId
JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id
JOIN [Statistics] AS s ON i.StatisticId = s.Id
JOIN Games AS g ON ug.GameId = g.Id
JOIN GameTypes AS gt ON g.GameTypeId = gt.Id
JOIN [Statistics] AS s1 ON gt.BonusStatsId = s1.Id
JOIN Characters AS c ON ug.CharacterId = c.Id
JOIN [Statistics] AS s2 ON c.StatisticId = s2.Id
--WHERE IsDeleted = 0
GROUP BY u.Username, g.Name
ORDER BY Strength DESC, Defence DESC, Speed DESC, Mind DESC, Luck DESC

GO
--Problem 5.	All Items with Greater than Average Statistics
SELECT i.[Name], i.Price, i.MinLevel, s.Strength, s.Defence, s. Speed, s.Luck, s.Mind
FROM Items AS i
JOIN [Statistics] AS s ON i.StatisticId = s.Id
WHERE s.Mind > (SELECT AVG(Mind) FROM [Statistics]) AND 
	  s.Luck > (SELECT AVG(LUCK) FROM [Statistics]) AND
	  s.Speed > (SELECT AVG(Speed) FROM [Statistics])
ORDER BY i.[Name]

GO
--Try with Function
CREATE OR ALTER FUNCTION udn_GetAllItemsWithStatsGreaterThanAverage()
RETURNS @Result TABLE(
	[Name] VARCHAR(50), 
	Price DECIMAL(18,2), 
	MinLevel INT, 
	Strength INT, 
	Defence INT, 
	Speed INT, 
	Luck INT, 
	Mind INT
)
AS
	BEGIN
		DECLARE @AvgSpeed DECIMAL(18,2) = (
											SELECT AVG(s.Speed)
											FROM Items AS i
											JOIN [Statistics] AS s ON i.StatisticId = s.Id)
		DECLARE @AvgMind DECIMAL(18,2) = (
											SELECT AVG(s.Mind), COUNT(*)
											FROM Items AS i
											JOIN [Statistics] AS s ON i.StatisticId = s.Id)
		DECLARE @AvgLuck DECIMAL(18,2) = (
											SELECT AVG(s.Luck), COUNT(*)
											FROM Items AS i
											JOIN [Statistics] AS s ON i.StatisticId = s.Id)
		
		DECLARE @Counter INT = 1;
		WHILE(@Counter <= (SELECT COUNT(*) FROM Items))
			BEGIN
				DECLARE @Speed DECIMAL(18,2) = (SELECT Speed FROM
													(SELECT s. Speed,
															ROW_NUMBER() OVER(ORDER BY i.Name) AS [Rows]
													FROM Items AS i
													JOIN [Statistics] AS s ON i.StatisticId = s.Id) AS [Current]
													WHERE [Rows] = @Counter)

				DECLARE @Mind DECIMAL(18,2) = (SELECT Mind FROM
													(SELECT s. Mind,
															ROW_NUMBER() OVER(ORDER BY i.Name) AS [Rows]
													FROM Items AS i
													JOIN [Statistics] AS s ON i.StatisticId = s.Id) AS [Current]
													WHERE [Rows] = @Counter)
				
				DECLARE @Luck DECIMAL(18,2) = (SELECT Luck FROM
													(SELECT s. Luck,
															ROW_NUMBER() OVER(ORDER BY i.Name) AS [Rows]
													FROM Items AS i
													JOIN [Statistics] AS s ON i.StatisticId = s.Id) AS [Current]
													WHERE [Rows] = @Counter)

				IF((@Speed > @AvgSpeed) AND (@Mind > @AvgMind) AND (@Luck > @AvgLuck))
					BEGIN
						INSERT INTO @Result						
						SELECT i.[Name], i.Price, i.MinLevel, s.Strength, s.Defence, s. Speed, s.Luck, s.Mind,
								ROW_NUMBER() OVER(ORDER BY i.Name) AS [Rows]
						FROM Items AS i
						JOIN [Statistics] AS s ON i.StatisticId = s.Id
					END
				SET @Counter += 1;
			END
		RETURN
	END
GO

SELECT * FROM udn_GetAllItemsWithStatsGreaterThanAverage()

GO

--Problem 6.	Display All Items with Information about Forbidden Game Type
SELECT  i.[Name] AS [Item], 
		i.Price AS [Price], 
		i.MinLevel AS [MinLevel], 
		gt.[Name] AS [Forbidden Game Type]
FROM GameTypes AS gt
RIGHT JOIN GameTypeForbiddenItems AS gtfi ON gt.Id = gtfi.GameTypeId
RIGHT JOIN Items AS i ON gtfi.ItemId = i.Id
ORDER BY [Forbidden Game Type] DESC, [Item]

SELECT  i.[Name] AS [Item], 
		i.Price AS [Price], 
		i.MinLevel AS [MinLevel], 
		gt.[Name] AS [Forbidden Game Type]
FROM Items AS i
LEFT JOIN GameTypeForbiddenItems AS gtfi ON i.Id = gtfi.ItemId
LEFT JOIN GameTypes AS gt ON gtfi.GameTypeId = gt.Id
ORDER BY [Forbidden Game Type] DESC, [Item]

GO
--Problem 7.	Buy Items for User in Game
CREATE PROCEDURE usp_AddItemsToUserAndDisplayUsersItemsInTheGame
AS
	BEGIN TRANSACTION
		DECLARE @UserName VARCHAR(50) = 'Alex';
		DECLARE @GameName VARCHAR(50) = 'Edinburgh';

		DECLARE @UserID INT = (SELECT Id FROM Users WHERE Username = @UserName);
			IF(@UserID IS NULL)
				BEGIN
					ROLLBACK;
					THROW 50001, 'Invalid UserId', 1;
				END

		DECLARE @GameID INT = (SELECT Id FROM Games WHERE [Name] = @GameName );
			IF(@GameID IS NULL)
				BEGIN
					ROLLBACK;
					THROW 50002, 'Invalid UserId', 1;
				END

		DECLARE @UserGameID INT = (SELECT Id FROM UsersGames WHERE GameId = @GameID AND UserId = @UserID)		
			IF(@UserGameID IS NULL)
				BEGIN
					ROLLBACK;
					THROW 50003, 'Invalid GameId', 1;
				END

		DECLARE @Counter INT = 1;
		DECLARE @ItemsCount INT = (SELECT COUNT(*) FROM Items WHERE Name IN ('Blackguard', 'Bottomless Potion of Amplification', 
												'Eye of Etlich (Diablo III)', 'Gem of Efficacious Toxin', 
												'Golden Gorget of Leoric')); 
		SELECT * FROM ITEMS WHere Name Like 'Hellfire Amulet'

		WHILE(@Counter <= @ItemsCount)
			BEGIN
			DECLARE @UserCach MONEY = (SELECT Cash FROM UsersGames WHERE Id = @UserGameID)

			DECLARE @UserLevel INT = (SELECT [Level] FROM UsersGames WHERE ID = @UserGameId)

			DECLARE @ItemID	INT = (SELECT ID FROM (SELECT ID, ROW_NUMBER() OVER(ORDER BY ID) AS [Rows]
												FROM Items WHERE Name IN 
												('Blackguard', 'Bottomless Potion of Amplification', 
												'Eye of Etlich (Diablo III)', 'Gem of Efficacious Toxin', 
												'Golden Gorget of Leoric', 'Hellfire Amulet')) AS R
												WHERE Rows = @Counter);
			DECLARE @ItemPrice MONEY = (SELECT Price FROM Items WHERE ID = @ItemID)
			DECLARE @ItemMinLevel INT = (SELECT MinLevel FROM Items WHERE ID = @ItemID)
				IF(@ItemPrice <= @UserCach AND @UserLevel >=@ItemMinLevel)
					BEGIN
						UPDATE UsersGames
						SET Cash -= @ItemPrice
						WHERE Id = @UserGameId;
						INSERT INTO UserGameItems
						VALUES (@ItemID, @UserGameId);
						--SET @Counter += 1;
					END
				/*ELSE
					BEGIN
						ROLLBACK;
						THROW 50004, 'Not Enough Funds!', 1;
					END*/
				SET @Counter += 1;
			END
	COMMIT

SELECT u.Username, g.Name, ug.Cash, i.Name AS [Item Name]
FROM Games AS g
JOIN UsersGames AS ug ON g.Id = ug.GameId
JOIN Users AS u ON ug.UserId = u.Id
JOIN UserGameItems AS ugi ON ug.Id = ugi.UserGameId
JOIN Items AS i ON ugi.ItemId = i.Id
WHERE g.Name = @GameName	
ORDER BY [Item Name]

GO

EXEC usp_AddItemsToUserAndDisplayUsersItemsInTheGame



