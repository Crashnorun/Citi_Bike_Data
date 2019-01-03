CREATE TABLE Trips
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Date] DATE NULL, 
    [StartTime] DATETIME NULL, 
    [StopTime] DATETIME NULL, 
    [TripDuration] INT NULL, 
    [StartStationID] INT NULL, 
    [EndStationID] INT NULL, 
    [StartStationName] TEXT NULL, 
    [EndStationName] TEXT NULL, 
    [StartStationLatitude] FLOAT NULL, 
    [EndStationLatitude] FLOAT NULL, 
    [StartStationLongitude] FLOAT NULL, 
    [EndStationLongitude] FLOAT NULL, 
    [BikeID] INT NULL, 
    [UserType] TEXT NULL, 
    [BirthYear] TEXT NULL, 
    [Gender] INT NULL
)
