CREATE PROCEDURE [dbo].[spCharacters_GetPaged]
	@SortBy nvarchar(50),
	@SortOrder nvarchar(50),
	@Start int,
	@PageSize int,
	@Search nvarchar(150) = null,
	@Begin date = null,
	@End date = null,
	@Total int output
AS
BEGIN
	--Drop table is hanging around
	DROP TABLE IF EXISTS #TempResults;
	--Create table with expected results
	Create Table #TempResults 
	(
		Id int, 
		[FirstName] varchar(150),
		[LastName] varchar(150),
		[PlanetName] varchar(150),
		DateCreated datetime, 
		DateUpdated datetime
	)

	--Populate table with content
	INSERT INTO #TempResults ( Id, [FirstName], [LastName], [PlanetName], DateCreated, DateUpdated	)
		Select c.Id, c.[FirstName], c.[LastName], p.[Name] as 'PlanetName', c.CreatedOn, c.UpdatedOn From Characters as c
		inner join Planets as p on c.PlanetId = p.Id;

	SELECT @Total = Count(Id) FROM #TempResults;

	SELECT Id, [FirstName], [LastName], [PlanetName], DateCreated as 'CreatedOn', DateUpdated as 'UpdatedOn'
		FROM #TempResults 
		WHERE 
		(@Search IS NULL or ([FirstName] LIKE '%' + @Search +'%' OR [LastName] LIKE '%' + @Search +'%'))
		AND 
		((@Begin IS NULL AND @End IS NULL) or DateCreated BETWEEN @Begin AND @End)
		ORDER BY 
			CASE WHEN @SortBy = 'FirstName' AND @SortOrder = 'Asc' Then [FirstName] END Asc,
			CASE WHEN @SortBy = 'FirstName' AND @SortOrder = 'Desc' Then [FirstName] END Desc,
			CASE WHEN @SortBy = 'LastName' AND @SortOrder = 'Asc' Then [LastName] END Asc,
			CASE WHEN @SortBy = 'LastName' AND @SortOrder = 'Desc' Then [LastName] END Desc,
			CASE WHEN @SortBy = 'PlanetName' AND @SortOrder = 'Asc' Then [PlanetName] END Asc,
			CASE WHEN @SortBy = 'PlanetName' AND @SortOrder = 'Desc' Then [PlanetName] END Desc,
			CASE WHEN @SortBy = 'Created' AND @SortOrder = 'Asc' Then 'CreatedOn' END Asc,
			CASE WHEN @SortBy = 'Created' AND @SortOrder = 'Desc' Then 'CreatedOn' END Desc,
			CASE WHEN @SortBy = 'Modified' AND @SortOrder = 'Asc' Then 'UpdatedOn' END Asc,
			CASE WHEN @SortBy = 'Modified' AND @SortOrder = 'Desc' Then 'UpdatedOn' END Desc
		OFFSET @Start ROWS
		FETCH NEXT @PageSize ROWS ONLY;

END