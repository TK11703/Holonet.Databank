CREATE PROCEDURE [dbo].[spSpecies_GetPaged]
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
		[Name] varchar(150),
		[Shard] nvarchar(500),
		DateUpdated datetime
	)

	--Populate table with content
	INSERT INTO #TempResults (Id, [Name], [Shard], DateUpdated)
		Select Id, [Name] as 'Name', [Shard], UpdatedOn	From Species

	SELECT @Total = Count(Id) FROM #TempResults;

	SELECT Id, [Name], [Shard], DateUpdated as 'UpdatedOn'
		FROM #TempResults 
		WHERE 
		(@Search IS NULL or ([Name] LIKE '%' + @Search +'%' OR [Name] LIKE '%' + @Search +'%'))
		AND 
		((@Begin IS NULL AND @End IS NULL) or DateUpdated BETWEEN @Begin AND @End)
		ORDER BY 
			CASE WHEN @SortBy = 'Name' AND @SortOrder = 'Asc' Then [Name] END Asc,
			CASE WHEN @SortBy = 'Name' AND @SortOrder = 'Desc' Then [Name] END Desc,
			CASE WHEN @SortBy = 'Modified' AND @SortOrder = 'Asc' Then 'UpdatedOn' END Asc,
			CASE WHEN @SortBy = 'Modified' AND @SortOrder = 'Desc' Then 'UpdatedOn' END Desc
		OFFSET @Start ROWS
		FETCH NEXT @PageSize ROWS ONLY;

END