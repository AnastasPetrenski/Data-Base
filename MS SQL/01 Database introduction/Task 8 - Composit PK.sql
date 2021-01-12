CREATE TABLE Users(
	Id BIGINT PRIMARY KEY IDENTITY NOT NULL,
	Username VARCHAR(30) UNIQUE NOT NULL,
	[Password] VARCHAR(26) NOT NULL,
	ProfilePicture VARBINARY(MAX) CHECK
	(DATALENGTH(ProfilePicture) <= 900 * 1024),
	LastLoginTime DATETIME2 NOT NULL,
	IsDeleted BIT NOT NULL
)

INSERT INTO Users (Username, [Password], LastLoginTime, IsDeleted)
VALUES ('Bango', '123456', '01.11.2021', 0),
		('Bango1', '123456', '01.11.2021', 1),
		('Bango2', '123456', '01.11.2021', 0),
		('Bango3', '123456', '01.11.2021', 1),
		('Bango4', '123456', '01.11.2021', 0)

DELETE FROM Users WHERE Id = 5

-- Activate manual insertion on Id
SET IDENTITY_INSERT Users ON

--Restore Bango4
INSERT INTO Users (Id, Username, [Password], ProfilePicture, LastLoginTime, IsDeleted)
VALUES (5, 'Bango4', '123456', 1000,'01.11.2021', 0)

-- Deactivate manual insertion and autoincrement working again
SET IDENTITY_INSERT Users OFF

-- TASK 09
ALTER TABLE Users
DROP CONSTRAINT [PK__Users__3214EC07FA54CAA2]

-- ADD new CONSTRAINT: PK - COMPOSITE
ALTER TABLE Users
ADD CONSTRAINT PK_Users_CompositeKey
PRIMARY KEY (Id, Username)

-- TASK 10
ALTER TABLE Users
ADD CONSTRAINT CK_PasswordLength
CHECK(Len([Password]) >= 5)

-- TASK 11
ALTER TABLE Users
ADD CONSTRAINT DF_Last_Login_Time
DEFAULT GETDATE() FOR LastLoginTime -- we can use CURRENT_TIMESTAMP also

--We cannot ALTER CONSTRAINT, we can only DROP and RECREATE it
ALTER TABLE Users
DROP CONSTRAINT [DF_Last_Login_Time]


-- TASK 12
ALTER TABLE Users
DROP CONSTRAINT PK_Users_CompositeKey

ALTER TABLE Users
ADD CONSTRAINT PK_Users_Id
PRIMARY KEY(Id)

ALTER TABLE Users
ADD CONSTRAINT CK_Username_Length
CHECK(Len([Username]) >= 3)

INSERT INTO Users (Username, [Password], IsDeleted)
VALUES ('Ban', '000000', 0)
SELECT * FROM Users