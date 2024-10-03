CREATE TABLE [dbo].[Authors]
(
    [Id] INT NOT NULL PRIMARY KEY identity,
    [AzureId] UNIQUEIDENTIFIER NOT NULL, 
    [DisplayName] NVARCHAR(255) NOT NULL, 
    [Email] NVARCHAR(255) NULL
)
