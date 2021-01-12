CREATE DATABASE Minions

USE Minions

CREATE TABLE Minions(
	Id INT PRIMARY KEY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	Age TINYINT
)
-- Error:
--String or binary data would be truncated in table 'Minions.dbo.Minions', column 'Name'. Truncated value: 'S'.
ALTER TABLE Minions
ALTER COLUMN [Name] NVARCHAR(50)

CREATE TABLE Towns(
	Id INT PRIMARY KEY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL
)

ALTER TABLE Minions
ADD TownId INT FOREIGN KEY REFERENCES Towns(Id)


INSERT INTO Towns(Id, [Name])
VALUES (1, 'Sofia'),
		(2, 'Plovdiv'),
		(3, 'Varna')

INSERT INTO Minions (Id, [Name], Age, TownId)
VALUES (1, 'Kevin', 22, 1),
		(2, 'Bob', 15, 3),
		(3, 'Steward', NULL,2)

Select Minions.Id, Minions.[Name], Minions.Age, Towns.[Name]
From Minions
Left Join Towns
ON Minions.TownId = Towns.Id

DELETE FROM Minions WHERE Minions.[Name]='Bob'
DELETE FROM Minions

TRUNCATE TABLE Minions

Select * FROM Minions

DROP TABLE Minions
DROP TABLE Towns

DROP DATABASE Minions

