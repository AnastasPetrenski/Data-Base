--Section 1. DDL 
CREATE DATABASE Bakery

USE Bakery

CREATE TABLE Products(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(25) UNIQUE,
	[Description] NVARCHAR(250),
	Recipe NVARCHAR(MAX),
	Price DECIMAL(18,2) CHECK(Price >= 0) --decimal can be with 4 precision
)

CREATE TABLE Countries(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) UNIQUE
)

CREATE TABLE Customers(
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(25),
	LastName NVARCHAR(25),
	Gender CHAR(1) CHECK(Gender = 'M' OR Gender = 'F'),
	Age INT,
	PhoneNumber CHAR(10),
	CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
)

CREATE TABLE Feedbacks(
	Id INT PRIMARY KEY IDENTITY,
	[Description] NVARCHAR(255),
	Rate DECIMAL(18,2) CHECK(Rate >=0 AND Rate <= 10),
	ProductId INT FOREIGN KEY REFERENCES Products(Id) NOT NULL,
	CustomerId INT FOREIGN KEY REFERENCES Customers(Id) NOT NULL
)

CREATE TABLE Distributors(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(25) UNIQUE,
	AddressText NVARCHAR(30),
	Summary NVARCHAR(200),
	CountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL
)

CREATE TABLE Ingredients(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(30),
	[Description] NVARCHAR(200),
	OriginCountryId INT FOREIGN KEY REFERENCES Countries(Id) NOT NULL,
	DistributorId INT FOREIGN KEY REFERENCES Distributors(Id) NOT NULL
)

CREATE TABLE ProductsIngredients(
	ProductId INT FOREIGN KEY REFERENCES Products(Id) NOT NULL,
	IngredientId INT FOREIGN KEY REFERENCES Ingredients(Id) NOT NULL,
	PRIMARY KEY (ProductId, IngredientId)
)

--Section 2. DML 
--2.	Insert
INSERT INTO Distributors(Name, CountryId, AddressText, Summary) VALUES
('Deloitte & Touche', 2, '6 Arch St #9757', 'Customizable neutral traveling'),
('Congress Title',	13,	'58 Hancock St', 'Customer loyalty'),
('Kitchen People',1,'3 E 31st St #77',	'Triple-buffered stable delivery'),
('General Color Co Inc',21,	'6185 Bohn St #72',	'Focus group'),
('Beck Corporation', 23, '21 E 64th Ave', 'Quality-focused 4th generation hardware')

INSERT INTO Customers(FirstName, LastName, Age, Gender, PhoneNumber, CountryId) VALUES
('Francoise',	'Rautenstrauch',	15,	'M',	'0195698399',	5),
('Kendra',	'Loud',	22,	'F',	'0063631526',	11),
('Lourdes',	'Bauswell',	50,	'M',	'0139037043',	8),
('Hannah',	'Edmison',	18,	'F',	'0043343686',	1),
('Tom',	'Loeza',	31,	'M',	'0144876096',	23),
('Queenie',	'Kramarczyk',	30,	'F',	'0064215793',	29),
('Hiu',	'Portaro',	25,	'M',	'0068277755',	16),
('Josefa',	'Opitz',	43,	'F',	'0197887645',	17)

--3.	Update
UPDATE Ingredients
SET DistributorId = 35
WHERE NAME IN ('Paprika', 'Poppy', 'Bay Leaf')

UPDATE Ingredients
SET OriginCountryId = 14
WHERE OriginCountryId = 8

--4.	Delete
DELETE FROM Feedbacks
WHERE CustomerId = 14 OR ProductId = 5

--Section 3. Querying 
--5.	Products by Price
SELECT Name, Price, Description FROM Products
ORDER BY Price DESC, Name

--6.	Negative Feedback
SELECT f.ProductId, f.Rate, f.Description, f.CustomerId, c.Age, c.Gender 
FROM Feedbacks AS f
JOIN Customers AS c ON c.Id = f.CustomerId 
WHERE f.Rate < 5
ORDER BY f.ProductId DESC, f.Rate

--7.	Customers without Feedback
SELECT CONCAT(c.FirstName, ' ', c.LastName) AS [CustomerName],
		c.PhoneNumber,
		c.Gender
FROM Customers AS c
LEFT JOIN Feedbacks AS f ON c.Id = f.CustomerId
WHERE f.CustomerId IS NULL
ORDER BY c.Id

--8.	Customers by Criteria
SELECT c.FirstName, c.Age, c.PhoneNumber
FROM Customers AS c
JOIN Countries AS c1 ON c.CountryId = c1.Id
WHERE (c.Age > 20 AND c.FirstName LIKE '%an%') OR 
		((SELECT RIGHT(c.PhoneNumber, '2')) LIKE '38' AND 
		c.CountryId <> (SELECT ID FROM Countries WHERE Name = 'Greece'))
ORDER BY c.FirstName, c.Age DESC

--9.	Middle Range Distributors

SELECT * FROM (
SELECT d.Name AS [DistributorName],
		i.Name AS [IngredientName],
		p.Name AS [ProducrName],
		AVG(f.Rate) AS AverageRate
FROM Distributors AS d
JOIN Ingredients AS i ON d.Id = i.DistributorId
JOIN ProductsIngredients AS ip ON i.Id = ip.IngredientId
JOIN Products AS p ON ip.ProductId = p.Id
JOIN Feedbacks AS f ON p.Id = f.ProductId
GROUP BY d.Name, i. Name, p.Name
) AS [Result]
WHERE AverageRate BETWEEN 5 AND 8
ORDER BY [DistributorName], [IngredientName], [ProducrName]

--10.	Country Representative
SELECT [CountryName], [DisributorName]
FROM (
	SELECT c.Name AS [CountryName], 
		   d.Name AS [DisributorName], 
		   DENSE_RANK() OVER(PARTITION BY  c.Name ORDER BY COUNT(i.Id) DESC) AS [Rows]
	FROM Countries AS c
	JOIN Distributors AS d ON c.Id = d.CountryId
	JOIN Ingredients AS i ON d.Id = i.DistributorId 
	GROUP BY c.Name, d.Name) AS [Ordered]
WHERE Ordered.Rows = 1
ORDER BY [CountryName], [DisributorName]


--Section 4. Programmability 
--11.	Customers with Countries
GO

CREATE VIEW v_UserWithCountries AS
SELECT  CONCAT(c.FirstName, ' ', c.LastName) AS [CustomerName],
			c.Age,
			c.Gender,
			c1.Name AS [CountryName]
	FROM Customers AS c
	JOIN Countries AS c1 ON c.CountryId = c1.Id

GO

--12.	Delete Products
CREATE TRIGGER tr_DeleteProducts ON Products 
INSTEAD OF DELETE
AS
	BEGIN
		DECLARE @ProductIdToBeDeleted INT = (SELECT p.Id
											 FROM Products as p
											 JOIN deleted as d on d.Id = p.Id);

		DELETE FROM Feedbacks WHERE ProductId = @ProductIdToBeDeleted;
		DELETE FROM ProductsIngredients WHERE ProductId = @ProductIdToBeDeleted;
		DELETE FROM Products WHERE Id = @ProductIdToBeDeleted;

	END
GO
--SELECT how many ingredient used to be made the product
SELECT p.Name, COUNT(*)
FROM Products AS p
JOIN ProductsIngredients ip ON p.Id = ip.ProductId
JOIN Ingredients AS i ON ip.IngredientId = i.Id
GROUP BY p.Name




