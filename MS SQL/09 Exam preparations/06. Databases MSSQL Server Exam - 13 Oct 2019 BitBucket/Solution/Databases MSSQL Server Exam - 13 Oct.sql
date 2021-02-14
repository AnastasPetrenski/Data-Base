--Section 1. DDL (30 pts)
CREATE DATABASE Bitbucket

GO

USE Bitbucket

Go

CREATE TABLE Users(
	Id INT PRIMARY KEY IDENTITY,
	Username VARCHAR(30) NOT NULL,
	Password VARCHAR(30) NOT NULL,
	Email VARCHAR(50) NOT NULL
)

CREATE TABLE Repositories(
	Id INT PRIMARY KEY IDENTITY,
	Name VARCHAR(50) NOT NULL
)

CREATE TABLE RepositoriesContributors(
	RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
	ContributorId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL,
	PRIMARY KEY (RepositoryId, ContributorId)
)

CREATE TABLE Issues(
	Id INT PRIMARY KEY IDENTITY,
	Title VARCHAR(255) NOT NULL,
	IssueStatus CHAR(6) NOT NULL,
	RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
	AssigneeId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL
)

CREATE TABLE Commits(
	Id INT PRIMARY KEY IDENTITY,
	Message VARCHAR(255) NOT NULL,
	IssueId INT FOREIGN KEY REFERENCES Issues(Id),
	RepositoryId INT FOREIGN KEY REFERENCES Repositories(Id) NOT NULL,
	ContributorId INT FOREIGN KEY REFERENCES Users(Id) NOT NULL
)

CREATE TABLE Files(
	Id INT PRIMARY KEY IDENTITY,
	Name VARCHAR(100) NOT NULL,
	Size DECIMAL(18,2) NOT NULL,
	ParentId INT FOREIGN KEY REFERENCES Files(Id),
	CommitId INT FOREIGN KEY REFERENCES Commits(Id) NOT NULL
)

--Section 2. DML (10 pts)
--2.	Insert
INSERT INTO Files(Name, Size, ParentId, CommitId) VALUES
('Trade.idk',	2598.0,	1,	1),
('menu.net',	9238.31,	2,	2),
('Administrate.soshy',	1246.93,	3,	3),
('Controller.php',	7353.15,	4,	4),
('Find.java',	9957.86,	5,	5),
('Controller.json',	14034.87,	3,	6),
('Operate.xix',	7662.92,	7,	7)

INSERT INTO Issues(Title, IssueStatus, RepositoryId, AssigneeId) VALUES 
('Critical Problem with HomeController.cs file',	'open',	1,	4),
('Typo fix in Judge.html',	'open',	4,	3),
('Implement documentation for UsersService.cs',	'closed',	8,	2),
('Unreachable code in Index.cs',	'open',	9,	8)

--3.	Update
UPDATE Issues
SET IssueStatus = 'closed'
WHERE AssigneeId = 6

--4.	Delete
DELETE FROM RepositoriesContributors
WHERE RepositoryId = (SELECT Id 
						FROM Repositories
							WHERE Name = 'Softuni-Teamwork')

DELETE FROM Issues
WHERE RepositoryId = (SELECT Id 
						FROM Repositories
							WHERE Name = 'Softuni-Teamwork')

--Section 3. Querying (40 pts)
--5.	Commits
SELECT Id, Message, RepositoryId, ContributorId
FROM Commits
ORDER BY Id, Message, RepositoryId, ContributorId

--6.	Heavy HTML
SELECT Id, Name, Size
FROM Files
WHERE Size > 1000 AND Name LIKE '%html%'
ORDER BY Size DESC, Id, Name

--7.	Issues and Users
SELECT i.Id, CONCAT(u.Username, ' : ', i.Title) AS [IssueAssignee] 
FROM Issues AS i
JOIN Users AS u ON i.AssigneeId = u.Id
ORDER BY i.Id DESC, IssueAssignee

--8.	Non-Directory Files
SELECT f2.ID, f2.Name, CONCAT(f2.Size,'KB') AS [Size]
FROM Files AS f
RIGHT JOIN Files AS f2 ON f.ParentId = f2.Id
WHERE f2.ParentId IS NULL
ORDER BY ID, Name, Size DESC

--My Solution
SELECT ID, Name, CONCAT(Size,'KB') AS [Size] 
FROM Files
WHERE ParentId IS NULL

--9.	Most Contributed Repositories
SELECT TOP(5) r.Id, R.Name, COUNT(c.Id) Commits
FROM Repositories AS r
JOIN Commits AS c ON r.Id = c.RepositoryId
JOIN RepositoriesContributors AS rc ON r.Id =rc.RepositoryId 
GROUP BY r.Id, R.Name
ORDER BY COUNT(c.Id) DESC, r.Id, r.Name

--10.	User and Files
SELECT u.Username, AVG(f.Size)
FROM Users AS u
JOIN Commits AS c ON u.Id = c.ContributorId
JOIN Files AS f ON c.Id = f.CommitId
GROUP BY u.Username
Order BY AVG(f.Size) DESC, u.Username

GO
--Section 4. Programmability (20 pts)
--11.	 User Total Commits
CREATE FUNCTION udf_UserTotalCommits(@username VARCHAR(30))
RETURNS	INT
AS
	BEGIN
		RETURN (
				SELECT COUNT(*) 
					FROM Users AS u
					JOIN Commits AS c ON u.Id = c.ContributorId
						WHERE u.Username = @username);

	END

GO

SELECT dbo.udf_UserTotalCommits('UnderSinduxrein')

GO

--12.	 Find by Extensions
CREATE PROCEDURE usp_FindByExtension(@extension VARCHAR(MAX))
AS
	SELECT Id, Name, CONCAT(Size, 'KB') AS [Size]
		FROM Files
			WHERE (RIGHT(Name, LEN(@extension))) LIKE @extension
				ORDER BY Id, Name, Size DESC
GO

CREATE OR ALTER PROCEDURE usp_FindByExtension(@extension VARCHAR(50))
AS
BEGIN
	SELECT [f].[Id],
	       [f].[Name],
		   CONCAT([f].[Size], 'KB') AS [Size]
	  FROM [dbo].[Files] AS f
	 WHERE CHARINDEX(@extension, [f].[Name]) > 0
  ORDER BY [f].[Id], [f].[Name], [f].[Size] DESC
END

GO

EXEC usp_FindByExtension 'txt'


--Custom quries
SELECT CHARINDEX('txt', Name) AS M
FROM Files
WHERE CHARINDEX('txt', Name) > 0

SELECT u.Username, COUNT(r.Id) [Contributing]
FROM Users AS u
JOIN RepositoriesContributors AS rc ON u.Id = rc.ContributorId
JOIN Repositories AS r ON rc.RepositoryId = r.Id
JOIN Commits AS c ON u.Id = c.ContributorId
GROUP BY u.Username
ORDER BY COUNT(r.Id) DESC

--Custom 
SELECT u.Id, u.Username, r.Name AS [Repo], COUNT(r.Id) [Contributing]
FROM Users AS u
JOIN RepositoriesContributors AS rc ON u.Id = rc.ContributorId
JOIN Repositories AS r ON rc.RepositoryId = r.Id
JOIN Commits AS c ON u.Id = c.ContributorId
GROUP BY u.Id, u.Username, r.Name
ORDER BY u.Username




