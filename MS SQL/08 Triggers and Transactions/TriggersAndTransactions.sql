USE Bank

SELECT * FROM Accounts
SELECT * FROM AccountHolders

DECLARE @AccId INT = (SELECT ID FROM AccountHolders WHERE FirstName = 'Susan' AND LastName = 'Cane')
PRINT @AccId
DECLARE @FakeAccId INT = (SELECT ID FROM AccountHolders WHERE FirstName = 'Fake' AND LastName = 'Fake')
IF(@FakeAccId IS NULL)
	BEGIN
		PRINT 'Null'
	END
GO

/********** Transactions Syntax ********/


CREATE PROCEDURE usp_Withdraw (@Amount Decimal(18,2), @AccountID INT)
AS
	BEGIN TRANSACTION
	--Check AccountID
	IF((SELECT COUNT(*) FROM AccountHolders WHERE ID = @AccountID) <> 1)
		BEGIN
			ROLLBACK;
			THROW 50001, 'Invalid Account!', 1;
		END

	DECLARE @Balance DECIMAL(18,2) = (SELECT Balance FROM Accounts WHERE ID = @AccountID)
	IF(@Balance < @Amount)
		BEGIN
			ROLLBACK;
			DECLARE @Message VARCHAR(MAX) = 'Not enough balance. Your balance is ' + CONVERT(VARCHAR, @Balance);
			THROW 50002, @Message, 1;
		END

	UPDATE Accounts
	SET Balance -= @Amount
	WHERE Id = @AccountID

	IF(@@ROWCOUNT <> 1)
		ROLLBACK;
		THROW 50002, 'Invalid Transaction!', 16;

	COMMIT

EXEC usp_Withdraw 1000000000.82, 1
EXEC usp_Withdraw 20, 10000000

GO

CREATE PROCEDURE usp_BankTransfer(@FromAccountID INT, @ToAccountID INT, @Amount DECIMAL(18,2))
AS
	BEGIN TRANSACTION
		IF((SELECT COUNT(*) FROM AccountHolders WHERE ID = @FromAccountID) <> 1)
			BEGIN
				ROLLBACK;
				THROW 50001, 'Invalid FromAccountID', 1;
			END

		IF((SELECT COUNT(*) FROM AccountHolders WHERE ID = @ToAccountID) <> 1)
			BEGIN
				ROLLBACK;
				THROW 50002, 'Invalid ToAccountID', 1;
			END

		IF (@Amount <= 0)
			BEGIN
				ROLLBACK;
				THROW 50004, 'Invalid Amount to be transfer', 1;
			END


		IF((SELECT Balance FROM Accounts WHERE ID = @FromAccountID) < @Amount)
			BEGIN
				ROLLBACK;
				THROW 50003, 'Insufficient funds', 1;
			END

		UPDATE Accounts
		SET Balance -= @Amount
		WHERE ID = @FromAccountID

		IF(@@ROWCOUNT <> 1)
		ROLLBACK;
		THROW 50002, 'Invalid Transaction!', 16;

		UPDATE Accounts
		SET Balance += @Amount
		WHERE ID = @ToAccountID

		IF(@@ROWCOUNT <> 1)
		ROLLBACK;
		THROW 50002, 'Invalid Transaction!', 16;

	COMMIT

EXEC usp_BankTransfer 1, 1000, 10

GO
/********** Triggers Syntax ********/


--Trigger AFTER Event
-- EXIST == COUNT(*) > 0

CREATE OR ALTER TRIGGER tr_AccountsUpdate ON Accounts FOR UPDATE
AS
	IF (EXISTS(
			SELECT * FROM inserted 
			WHERE Balance >= 1000000))
	BEGIN
		ROLLBACK;
		THROW 50010, 'Amount can be less than 1000000',1;
	END

GO

UPDATE Accounts SET Balance = 1200000 WHERE ID = 2

--Triger INSTEAD OF UPDATE

CREATE TABLE DeletedBalances(
	Id INT,
	AccountHolderedID INT,
	Balance DECIMAL(18,2)
)

GO

CREATE TRIGGER tr_DeleteBalance ON Accounts FOR DELETE
AS
	BEGIN
		INSERT INTO DeletedBalances
		(
			Id,
			AccountHolderedID,
			Balance
		)
		SELECT * FROM [deleted] AS d
	END
GO


--Logs
CREATE TABLE LoggerData(
	Id INT NOT NULL PRIMARY KEY IDENTITY,
	AccountId INT REFERENCES Accounts(Id),
	OldAmount MONEY NOT NULL,
	NewAmount MONEY NOT NULL,
	UpdateOn DATETIME DEFAULT GETDATE(),
	UpdateBy NVARCHAR(MAX),
	Active BIT NOT NULL DEFAULT 0
)

GO

CREATE OR ALTER TRIGGER tr_AllLogsOnAccountUpdate
ON Accounts FOR UPDATE
AS
	INSERT INTO LoggerData(AccountId, OldAmount, NewAmount, UpdateOn, UpdateBy)
	--replace VALUES() with SELECT 
	SELECT i.ID, d.Balance, i.Balance, GETDATE(), CURRENT_USER
	FROM INSERTED AS i
	JOIN DELETED AS d ON i.ID = d.ID
	WHERE i.Balance != d.Balance
GO

CREATE OR ALTER TRIGGER tr_AccountsDelete ON AccountHolders
INSTEAD OF DELETE
AS
	BEGIN
	UPDATE Accounts SET IsDeleted = 1
		WHERE ID IN (SELECT Id FROM deleted)

	UPDATE ah SET ah.IsDeleted = 1
		FROM AccountHolders AS ah 
		JOIN deleted d ON d.Id = ah.Id
		WHERE ah.IsDeleted = 0;
	END
GO

SELECT * FROM Accounts WHERE AccountHolderId = 1

DELETE FROM Accounts
WHERE AccountHolderId = 7;

DELETE FROM AccountHolders
WHERE ID = 1;

SELECT * FROM DeletedBalances

UPDATE a
SET a.Balance = 1000
FROM Accounts AS a
JOIN AccountHolders AS ah ON a.AccountHolderId = ah.Id
WHERE a.ID =2

SELECT * FROM Accounts
SELECT * FROM AccountHolders

SELECT * FROM DeletedBalances

GO

CREATE OR ALTER PROCEDURE sp_RestoreRemovedAccounts
AS
	IF((SELECT COUNT(*) FROM DeletedBalances) > 0)
		BEGIN
		DECLARE @Count INT;
		SET @Count = 1;
		WHILE(@Count <= (SELECT COUNT(*) FROM DeletedBalances))
			BEGIN
				DECLARE @ID INT = 
									(SELECT Id FROM (
										SELECT *,
											(ROW_NUMBER() OVER( ORDER BY Id)) AS [Rows] 
										FROM DeletedBalances) AS Result
										WHERE Result.Rows = @Count);
				DECLARE @AccountHolderId INT =
									(SELECT AccountHolderedID FROM (
										SELECT *,
											(ROW_NUMBER() OVER( ORDER BY Id)) AS [Rows] 
									FROM DeletedBalances) AS Result
									WHERE Result.Rows = @Count);
				DECLARE @Balance DECIMAL(18,2) =									
									(SELECT Balance FROM (
											SELECT *,
												(ROW_NUMBER() OVER( ORDER BY Id)) AS [Rows] 
									FROM DeletedBalances) AS Result
									WHERE Result.Rows = @Count);

				INSERT INTO Accounts (Id, AccountHolderId, Balance)
				VALUES(@Id, @AccountHolderId, @Balance)

				UPDATE AccountHolders
				SET IsDeleted = 0
				WHERE Id = @Count

				SET @Count += 1;
			END
		END
	
EXEC sp_RestoreRemovedAccounts
GO


DECLARE @Count INT = 1;
INSERT INTO AccountHolders (Id, FirstName, LastName, SSN)
				VALUES (
					(SELECT Id FROM (
						SELECT *,
							(ROW_NUMBER() OVER( ORDER BY Id)) AS [Rows] 
					FROM DeletedBalances) AS Result
					WHERE Result.Rows = @Count), 
					'Anastas',
					'Petrenski',
					'1234567890'
					)

INSERT INTO Accounts (Id, AccountHolderId, Balance) 
VALUES (20, 1, 523000),
(1,1,1000),
(18,1,23000)
Truncate TABLE Deleted