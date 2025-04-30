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
		[UpdatedOn] datetime
	);

	--Populate table with content
	WITH LatestRecords AS (
		SELECT 
			SpeciesId,
			Shard,
			ROW_NUMBER() OVER (PARTITION BY SpeciesId ORDER BY UpdatedOn DESC) as rn
		FROM DataRecords
		WHERE Shard IS NOT NULL
	)
	INSERT INTO #TempResults (Id, [Name], [Shard], [UpdatedOn])
		Select S.Id, S.[Name] as 'Name', lr.[Shard], S.[UpdatedOn] 
			From Species as S
			LEFT JOIN LatestRecords lr ON lr.SpeciesId = s.Id AND lr.rn = 1
			WHERE S.[Active]=1

	SELECT @Total = Count(Id) 
		FROM #TempResults
		WHERE 
		(@Search IS NULL or ([Name] LIKE '%' + @Search +'%' OR [Name] LIKE '%' + @Search +'%'))
		AND 
		((@Begin IS NULL AND @End IS NULL) or [UpdatedOn] BETWEEN @Begin AND @End);

	SELECT Id, [Name], [Shard], [UpdatedOn]
		FROM #TempResults 
		WHERE 
		(@Search IS NULL or ([Name] LIKE '%' + @Search +'%' OR [Name] LIKE '%' + @Search +'%'))
		AND 
		((@Begin IS NULL AND @End IS NULL) or [UpdatedOn] BETWEEN @Begin AND @End)
		ORDER BY 
			CASE WHEN @SortBy = 'Name' AND @SortOrder = 'Asc' Then [Name] END Asc,
			CASE WHEN @SortBy = 'Name' AND @SortOrder = 'Desc' Then [Name] END Desc,
			CASE WHEN @SortBy = 'UpdatedOn' AND @SortOrder = 'Asc' Then [UpdatedOn] END Asc,
			CASE WHEN @SortBy = 'UpdatedOn' AND @SortOrder = 'Desc' Then [UpdatedOn] END Desc
		OFFSET @Start ROWS
		FETCH NEXT @PageSize ROWS ONLY;

END