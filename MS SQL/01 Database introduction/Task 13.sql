CREATE DATABASE Movies

USE Movies

CREATE TABLE Directors(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	DirectorName VARCHAR(50) NOT NULL,
	Notes NVARCHAR(200)
)

CREATE TABLE Genres(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	GenreName VARCHAR(50) NOT NULL,
	Notes NVARCHAR(200)	
)

CREATE TABLE Categories(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	CategoryName VARCHAR(50) NOT NULL,
	Notes NVARCHAR(200)	
)

CREATE TABLE Movies(
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	Title VARCHAR(50) NOT NULL,
	DirectorId INT FOREIGN KEY REFERENCES Directors(Id) NOT NULL,
	CopyrightYear DATE,
	[Length] TIME NOT NULL,
	GenreId INT FOREIGN KEY REFERENCES Genres(Id) NOT NULL,
	CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
	Rating DECIMAL(2,1),
	Notes NVARCHAR(200)
)

INSERT INTO Directors (DirectorName, Notes)
VALUES ('Gay Richie', 'Awsome movies'),
		('Steven Spilberg', 'Best of the Best, all the time director!'),
		('Anastas Petrenski', 'Talant'),
		('Russel Croll', null),
		('Tom Cruse', 'Pretty guy')

INSERT INTO Genres (GenreName, Notes)
VALUES ('Drama', Null),
		('Fantasy', Null),
		('Action', Null),
		('Comedy', Null),
		('Romance', Null)

INSERT INTO Categories (CategoryName, Notes)
VALUES ('Best Movies', 'By Budget'),
		('New style', 'Decorse'),
		('Best song', 'By playing'),
		('Best Actiors', 'Team'),
		('Best support', null)

INSERT INTO Movies (Title, DirectorId, CopyrightYear, [Length], GenreId, CategoryId, Rating, Notes)
VALUES ('King Artur', 1, '05-01-2019', '1:30:33', 1, 1, 7.3, 'Cool Movie'),
		('Star Wars', 2, '05-01-2001', '2:50:33', 2, 2, 9.9, 'Best All the time'),
		('Noob', 3, '05-01-2021', '1:34:01', 3, 3, 6.3, 'Need more experience'),
		('Beatiful mind', 4, '05-01-2009', '3:30:33', 4, 4, 8.3, 'Great players'),
		('Mission Impossible', 5, '05-01-2014', '1:45:33', 5, 5, 4.3, 'Pretty guy')

SELECT * FROM Movies

Select Movies.Id, Movies.Title, Directors.DirectorName, Movies.CopyrightYear, Movies.[Length], Movies.Rating
FROM Movies
LEFT JOIN Directors ON Movies.DirectorId = Directors.Id 



