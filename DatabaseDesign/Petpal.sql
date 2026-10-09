CREATE DATABASE PetPalDb;
GO

USE PetPalDb;
GO


CREATE TABLE Users
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME NOT NULL 
        DEFAULT GETDATE()
);
GO


CREATE TABLE Species
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO


CREATE TABLE Breeds
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SpeciesId INT NOT NULL,

    CONSTRAINT FK_Breeds_Species
        FOREIGN KEY (SpeciesId) REFERENCES Species(Id),

    CONSTRAINT UQ_Breeds_Name_Species
        UNIQUE (Name, SpeciesId)
);
GO


CREATE TABLE Pets
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Description NVARCHAR(500) NULL,
    BreedId INT NOT NULL,
    OwnerId INT NOT NULL,
    DateOfBirth DATE NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Pets_Breeds
        FOREIGN KEY (BreedId) REFERENCES Breeds(Id),

    CONSTRAINT FK_Pets_Users
        FOREIGN KEY (OwnerId) REFERENCES Users(Id),

    CONSTRAINT CK_Pets_Gender
        CHECK (Gender IN ('Male', 'Female', 'Unknown'))
);
GO


CREATE TABLE AdoptionRequests
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ApplicantId INT NOT NULL,
    PetId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    RequestedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_AdoptionRequests_Users
        FOREIGN KEY (ApplicantId) REFERENCES Users(Id),

    CONSTRAINT FK_AdoptionRequests_Pets
        FOREIGN KEY (PetId) REFERENCES Pets(Id),

    CONSTRAINT CK_AdoptionRequests_Status
        CHECK (Status IN ('Pending', 'Approved', 'Rejected'))
);
GO