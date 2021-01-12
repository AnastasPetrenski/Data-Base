CREATE TABLE People (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(200) NOT NULL,
	Picture VARBINARY(MAX),
	Height DECIMAL(3,2),
	[Weight] DECIMAL(5,2),
	Gender CHAR(1) NOT NULL,
	Birthdate DATE NOT NULL,
	Biography NVARCHAR(MAX)
)

INSERT INTO People ([Name], Gender, Birthdate)
VALUES ('Anastas Petrenski', 'm', '05.23.1986'),
		('Nasko1 Nasko dsdsad', 'f', '05.23.1985'),
		('Nasko2 Nasko dsdsad', 'f', '05.23.1983'),
		('Nasko3 Nasko dsdsad', 'm', '05.23.1982'),
		('Nasko4 Nasko dsdsad', 'm', '05.23.1982')

-------Extending table People------

CREATE TABLE PeopleExtended (
	Id INT PRIMARY KEY IDENTITY NOT NULL,
	[Name] NVARCHAR(200) NOT NULL,
	Picture VARBINARY(MAX) CHECK 
	(DATALENGTH(Picture) <= 2000 * 1024),
	Height DECIMAL(3,2),
	[Weight] DECIMAL(5,2),
	Gender CHAR(1) CHECK(Gender = 'm' OR Gender = 'f') NOT NULL,
	Birthdate DATE NOT NULL,
	Biography NVARCHAR(MAX),
	CurrentDate DATETIME2,
	ServerName VARCHAR(128),
	HostId CHAR(8)
)

ALTER TABLE People
ADD HostId CHAR(8) 

ALTER TABLE People
ADD CurrentDate DATETIME2

ALTER TABLE People 
ALTER COLUMN HostId CHAR(8)
SELECT HOST_NAME()

INSERT INTO People ([Name], Picture, Height, [Weight], Gender, Birthdate, Biography, CurrentDate, ServerName, HostId)
VALUES ('Nasko1 Nasko dsdsad', 223000, 1.72, 80.222, 's', '05.23.1983', 'nqmam', GETDATE(), HOST_NAME(), HOST_ID())

SELECT * FROM People

DELETE FROM People;

INSERT INTO People ([Name], Picture, Height, [Weight], Gender, Birthdate, Biography, CurrentDate, ServerName, HostId)
VALUES ('Anastas Petrenski', 10000, 1.72, 80.7, 'm', '05.23.1986', 'nqmam', GETDATE() + 2, HOST_NAME(), HOST_ID()),
		('Nasko1 Nasko dsdsad', 20000, 1.32, 80.222, 's', '05.23.1985', 'nqmam', GETDATE(), HOST_NAME(), HOST_ID()),
		('Nasko1 Nasko dsdsad', 5000, 1.62, 99, 'f', '05.23.1983', 'nqmam', GETDATE()+ 3, HOST_NAME(), HOST_ID()),
		('Nasko1 Nasko dsdsad', 700, 1.92, 69.222, 'm', '05.23.1982', 'nqmam', null, HOST_NAME(), HOST_ID())

SELECT * FROM People WHERE Gender = 's'
