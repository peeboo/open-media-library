sp_configure 'show advanced options', 1
RECONFIGURE
GO

sp_configure 'max server memory', 300
RECONFIGURE
GO

sp_configure 'show advanced options', 0
RECONFIGURE
GO
		