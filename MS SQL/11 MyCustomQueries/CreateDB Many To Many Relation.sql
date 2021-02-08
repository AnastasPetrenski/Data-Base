CREATE DATABASE Recipes

USE Recipes

CREATE TABLE Ingredient(
	IngredientID INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	[Quantity] INT NOT NULL,
	RecipeID INT
)

CREATE TABLE Recipe(
	RecipeID INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(50) NOT NULL,
	[Description] VARCHAR(MAX),
	IngredientID INT FOREIGN KEY REFERENCES Ingredient(IngredientID)
)


ALTER TABLE Ingredient
ADD FOREIGN KEY (RecipeID) REFERENCES  Recipe(RecipeID)


CREATE TABLE IngredientRecipe(
	IngredientID INT FOREIGN KEY REFERENCES Ingredient(IngredientID),
	RecipeID INT FOREIGN KEY REFERENCES Recipe(RecipeID),
	PRIMARY KEY (IngredientID, RecipeID)
)
