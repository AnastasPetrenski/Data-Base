USE Bank

GO

--Create Table Logs
CREATE TABLE Logs(
	LogID INT NOT NULL PRIMARY KEY IDENTITY,
	AccountID INT REFERENCES Accounts(Id),
	OldSum DECIMAL(18,2),
	NewSum DECIMAL (18,2)
)

GO

CREATE TRIGGER tr_AccountChangeTracher ON Accounts FOR UPDATE
AS
	BEGIN
		INSERT INTO Logs(AccountId, OldSum, NewSum)
		SELECT i.Id, d.Balance, i.Balance FROM INSERTED AS i
		JOIN DELETED AS d ON i.Id = d.Id
		WHERE i.Balance != d.Balance
	END
GO

SELECT * FROM Logs;

--2.	Create Table Emails
CREATE TABLE NotificationEmails(
	ID INT NOT NULL PRIMARY KEY IDENTITY,
	Recipient INT REFERENCES Accounts(ID),
	[Subject] VARCHAR(100),
	Body VARCHAR(MAX)
)

GO

CREATE OR ALTER TRIGGER tr_SendNotificationEmail ON Logs FOR INSERT
AS
	BEGIN
		INSERT INTO NotificationEmails(Recipient, [Subject], Body)
		SELECT  i.AccountID, 
				CONCAT('Balance change for account: ' , i.AccountID),  
				CONCAT('On ', 
					  CONVERT(VARCHAR, GETDATE(), 0), 
					  ' your balance was changed from ',
					  i.OldSum,
					  ' to ',
					  i.NewSum,
					  '.')
		FROM INSERTED AS i
	END
GO

SELECT * FROM NotificationEmails
SELECT * FROM Logs

UPDATE Accounts 
SET Balance = 333333
WHERE AccountHolderId = 7 AND ID = 19

GO
--3.	Deposit Money
CREATE OR ALTER PROCEDURE usp_DepositMoney (@AccountId INT, @MoneyAmount DECIMAL(15, 4))
AS
BEGIN
	DECLARE @targetAccountId INT = (SELECT a.Id FROM dbo.Accounts a WHERE a.Id = @AccountId)

	IF(@MoneyAmount < 0 OR @MoneyAmount IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid amount of money', 16, 1)
		RETURN
	END
	
	IF(@targetAccountId IS NULL)
	BEGIN
		ROLLBACK
		RAISERROR('Invalid Id Parameter', 16, 2)
		RETURN
	END

	UPDATE [dbo].[Accounts]
	   SET
	       [dbo].[Accounts].[Balance] += @MoneyAmount
	 WHERE [dbo].[Accounts].[Id] = @AccountId
END
GO

CREATE OR ALTER PROCEDURE usp_DepositMoney (@AccountId INT, @MoneyAmount DECIMAL(20, 4))
AS
	BEGIN TRANSACTION
		DECLARE @ID INT;

		IF((SELECT COUNT(*) FROM Accounts WHERE ID = @AccountId) < 1)
			BEGIN
				ROLLBACK;
				THROW 50001, 'Invalid Account ID @AccountID', 1; 
			END

		IF(@MoneyAmount < 0 OR @MoneyAmount IS NULL)
			BEGIN
				ROLLBACK;
				THROW 50002, 'The amount must be positive number', 1;
			END

		UPDATE Accounts
		SET Balance = Balance + @MoneyAmount
		WHERE ID = @AccountId
		
	COMMIT

GO

SELECT * FROM Accounts
SELECT * FROM AccountHolders

EXEC usp_DepositMoney 1, 10

SELECT *
FROM Logs;

SELECT *
FROM NotificationEmails;

GO
--4.	Withdraw Money
CREATE PROCEDURE usp_WithdrawMoney (@AccountId INT, @MoneyAmount DECIMAL(20,4)) 
AS
	BEGIN TRANSACTION
		IF((SELECT COUNT(*) FROM Accounts WHERE Id = @AccountId) < 1)
			BEGIN
				ROLLBACK;
				THROW 50001, 'Invalid Account ID @AccountID', 1; 
			END

		IF((SELECT Balance FROM Accounts WHERE Id = @AccountId) < @MoneyAmount)
			BEGIN
				ROLLBACK;
				THROW 50002, 'Insufficiently funds!', 1;
			END

		IF((SELECT Balance FROM Accounts WHERE Id = @AccountId) <= 0)
			BEGIN
				ROLLBACK;
				THROW 50003, 'Empty card!', 1;
			END

		IF(@MoneyAmount < 0 OR @MoneyAmount IS NULL)
			BEGIN
				ROLLBACK;
				THROW 50004, 'Intended amount must be positive number', 1;
			END

		UPDATE Accounts
		SET Balance -= @MoneyAmount
		WHERE Id = @AccountId

	COMMIT

GO

SELECT AccountHolderId ,SUM(Balance) AS TotalBalance, COUNT(*) AS Accounts FROM (
SELECT *, DENSE_RANK() OVER(Order BY AccountHolderId) AS RowOrder
FROM Accounts) AS O
GROUP BY RowOrder, AccountHolderId
ORDER BY TotalBalance DESC 

SELECT *, DENSE_RANK() OVER(PARTITION BY AccountHolderId Order BY Balance) AS PartitionByAccountID,
ROW_NUMBER() OVER(PARTITION BY AccountHolderID ORDER BY Balance) AS RowOrderByAccountID
FROM Accounts

GO
--5.	Money Transfer	
CREATE PROCEDURE usp_TransferMoney(@SenderId INT, @ReceiverId INT, @Amount DECIMAL(20,4))
AS
	BEGIN TRANSACTION
			EXEC usp_WithdrawMoney @SenderId, @Amount;
			EXEC usp_DepositMoney @ReceiverId, @Amount;
	COMMIT

GO

/************** Queries for Diablo Database ************/

USE Diablo

GO
--6.	Trigger
--1.TASK
CREATE TRIGGER tr_PurshedItemLevelShouldBeLessThanCharLevelRestriction 
ON UserGameItems INSTEAD OF INSERT
AS
	BEGIN
		INSERT INTO UserGameItems 
		SELECT i.ItemId, i.UserGameId FROM inserted AS i
		JOIN UsersGames AS ug ON i.UserGameId = ug.Id
		JOIN Items AS it ON i.ItemId = it.Id
		WHERE it.MinLevel < ug.Level 
	END
GO

INSERT INTO UserGameItems (ItemId, UserGameId)
VALUES (2,94)

SELECT * FROM UserGameItems
WHERE UserGameID = 94
ORDER BY UserGameId, ItemId

SELECT * FROM Items


--2.TASK
--with Subqueries
UPDATE UsersGames
SET Cash += 50000
--SELECT * FROM UsersGames
WHERE UserID IN 
				(SELECT Id FROM Users WHERE Username IN ('baleremuda', 'loosenoise', 
														 'inguinalself', 'buildingdeltoid',
														 'monoxidecos ')) 
			 AND GameId =
	            (SELECT ID FROM Games WHERE [Name] = 'Bali')

--with Joins
UPDATE UsersGames
SET Cash += 67
FROM UsersGames AS ug
JOIN Users AS u ON ug.UserId = u.Id
JOIN Games AS g ON ug.GameId = g.Id
WHERE g.[Name] LIKE 'Safflower' AND
	  u.Username IN ('Stamat')

GO
--3.TASK
CREATE PROCEDURE usp_StoreItems(@Username VARCHAR(50))
AS
	DECLARE	@GameID INT =  (SELECT GameId 
							FROM UsersGames AS ug
							JOIN Games AS g ON ug.GameId = g.Id
							WHERE g.[Name] = 'Bali');

	DECLARE @UserID INT =  (SELECT UserId
							FROM UsersGames AS ug
							JOIN Users AS u ON ug.UserId = u.Id
							WHERE u.Username = @Username);

	DECLARE @UserGameId INT = (SELECT Id
							   FROM UsersGames AS ug
							   WHERE ug.GameId = @GameID AND
									 ug.UserId = @UserID);

	DECLARE @UserCash MONEY = (SELECT CASH FROM UsersGames WHERE ID = @UserGameId)

	DECLARE @UserLevel INT = (SELECT [Level] FROM UsersGames WHERE ID = @UserGameId)

	DECLARE @Counter INT = 251;
	WHILE(@Counter <= 539)
		BEGIN
			DECLARE @ItemCost MONEY = (SELECT Price FROM Items WHERE Id = @Counter);
			DECLARE @ItemLevel INT = (SELECT MinLevel FROM Items WHERE Id = @Counter);

			IF(@UserLevel >= @ItemLevel AND @UserCash >= @ItemCost)
				BEGIN
					UPDATE UsersGames
					SET Cash -= @ItemCost
					WHERE Id = @UserGameId
					INSERT INTO UserGameItems
					VALUES (@Counter, @UserGameId)
				END

			IF(@Counter < 299)
				SET @Counter += 1;
			ELSE
				SET @Counter = 501
		END

GO


SELECT * FROM UserGameItems
WHERE ItemId BETWEEN 251 AND 539




--4.TASK
SELECT  u.Username, 
		g.[Name], 
		ug.Cash, 
		i.[Name] AS [Item Name], 
		i.Id
FROM Items AS i
LEFT JOIN UserGameItems AS ugi ON i.Id = ugi.ItemId
LEFT JOIN UsersGames AS ug ON ugi.UserGameId = ug.Id
LEFT JOIN Users AS u ON ug.UserId = u.Id
LEFT JOIN Games AS g ON ug.GameId = g.Id
WHERE g.[Name] = 'Bali' 
ORDER BY u.Username, i.[Name]


GO
--7.	*Massive Shopping
CREATE PROCEDURE usp_ChangeTracker(@ItemLevelStart INT, @ItemLevelFinish INT, 
					@UserLevel INT, @UserCash MONEY, @UserGameID INT)
AS
	WHILE(@ItemLevelStart <= @ItemLevelFinish)
	BEGIN TRANSACTION
		DECLARE @ItemLevel INT = (SELECT MinLevel FROM Items WHERE Id = @ItemLevelStart);
		DECLARE @ItemCost MONEY = (SELECT Price FROM Items WHERE Id = @ItemLevelStart);

		IF(@UserLevel >= @ItemLevel AND @UserCash >= @ItemCost)
			BEGIN
				UPDATE UsersGames
				SET Cash -= @ItemCost
				WHERE ID = @UserGameID;
				INSERT INTO UserGameItems
				VALUES(@ItemLevelStart, @UserGameID)
				SET @ItemLevelStart += 1;
			END
		ELSE
			ROLLBACK;
	
	COMMIT
GO

CREATE PROCEDURE usp_MassiveShopping
AS
	BEGIN TRANSACTION
		DECLARE @Username VARCHAR(50) = 'Stamat';
		DECLARE @GameName VARCHAR(100) = 'Safflower';
		DECLARE @GameID INT =  (SELECT GameId 
								FROM UsersGames ug
								JOIN Games AS g ON ug.GameId = g.Id
								WHERE g.Name = @GameName);

		DECLARE @UserID INT =  (SELECT UserId 
								FROM UsersGames AS ug
								JOIN Users AS u ON ug.UserId = u.Id);

		DECLARE @UserGameID INT = (SELECT Id FROM UsersGames
									 WHERE GameId = @GameID AND UserId = @UserID);

		DECLARE @UserLevel INT = (SELECT [Level] FROM UsersGames WHERE ID = @UserGameID);
		DECLARE @UserCash MONEY = (SELECT Cash FROM UsersGames WHERE ID = @UserGameID);

		EXEC usp_ChangeTracker 11,12,@UserLevel,@UserCash,@UserGameID;

		EXEC usp_ChangeTracker 19,21,@UserLevel,@UserCash,@UserGameID;

	COMMIT

SELECT	i.[Name] AS [Item Name]
FROM Items AS i
LEFT JOIN UserGameItems AS ugi ON i.Id = ugi.ItemId
LEFT JOIN UsersGames AS ug ON ugi.UserGameId = ug.Id
LEFT JOIN Users AS u ON ug.UserId = u.Id
LEFT JOIN Games AS g ON ug.GameId = g.Id
WHERE g.[Name] = @GameName AND u.Username = @Username 
ORDER BY u.Username, i.[Name]

GO


/******** Queries for SoftUni Database **********/
USE SoftUni

GO

--8.	Employees with Three Projects
CREATE PROCEDURE usp_AssignProject(@emloyeeId INT, @projectID INT)
AS
	BEGIN TRANSACTION
		DECLARE @EmployeeProjectsCount INT = (SELECT COUNT(*) FROM Employees AS e
											JOIN EmployeesProjects AS ep ON e.EmployeeID = ep.EmployeeID
											WHERE e.EmployeeID = @emloyeeId)
		IF((@EmployeeProjectsCount >= 3))
			BEGIN
				ROLLBACK;
				RAISERROR ('The employee has too many projects!',16, 1)
			END

		INSERT INTO EmployeesProjects
		VALUES (@emloyeeId, @projectID)
	COMMIT
GO

--9.	Delete Employees
CREATE TABLE Deleted_Employees(
	EmployeeId INT PRIMARY KEY IDENTITY, 
	FirstName VARCHAR(50), 
	LastName VARCHAR(50), 
	MiddleName VARCHAR(50), 
	JobTitle VARCHAR(50), 
	DepartmentId INT, 
	Salary MONEY)

GO

CREATE TRIGGER tr_FiredEmployees ON Employees FOR DELETE
AS
	BEGIN
		INSERT INTO Deleted_Employees (FirstName, LastName, MiddleName, JobTitle, DepartmentId, Salary)
		SELECT d.FirstName, d.LastName, d.MiddleName, d.JobTitle, d.DepartmentID, d.Salary
		FROM deleted AS d
	END
GO