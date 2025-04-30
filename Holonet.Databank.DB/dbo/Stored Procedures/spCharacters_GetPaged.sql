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
		[GivenName] varchar(150) NOT NULL,
		[FamilyName] varchar(150) NULL,
		[PlanetId] int,
		[PlanetName] varchar(150),
		[Shard] nvarchar(500),
		[UpdatedOn] datetime
	);

	--Populate table with content
	WITH LatestRecords AS (
		SELECT 
			CharacterId,
			Shard,
			ROW_NUMBER() OVER (PARTITION BY CharacterId ORDER BY UpdatedOn DESC) as rn
		FROM DataRecords
		WHERE Shard IS NOT NULL
	)

	INSERT INTO #TempResults ( Id, [GivenName], [FamilyName], [PlanetId], [PlanetName], [Shard], [UpdatedOn] )
		Select c.Id, c.[GivenName], c.[FamilyName], p.[Id] as 'PlanetId', p.[Name] as 'PlanetName', lr.[Shard], c.[UpdatedOn] 
			From Characters as c 
			inner join Planets as p on c.PlanetId = p.Id
			LEFT JOIN LatestRecords lr ON lr.CharacterId = c.Id AND lr.rn = 1
				WHERE c.[Active]=1;

	SELECT @Total = Count(Id) 
		FROM #TempResults
		WHERE 
		(@Search IS NULL or ([GivenName] LIKE '%' + @Search +'%' OR [FamilyName] LIKE '%' + @Search +'%' OR [PlanetName] LIKE '%' + @Search +'%'))
		AND 
		((@Begin IS NULL AND @End IS NULL) or [UpdatedOn] BETWEEN @Begin AND @End);

	SELECT Id, [GivenName], [FamilyName], [PlanetId], [PlanetName], [Shard], [UpdatedOn]
		FROM #TempResults 
		WHERE 
		(@Search IS NULL or ([GivenName] LIKE '%' + @Search +'%' OR [FamilyName] LIKE '%' + @Search +'%' OR [PlanetName] LIKE '%' + @Search +'%'))
		AND 
		((@Begin IS NULL AND @End IS NULL) or [UpdatedOn] BETWEEN @Begin AND @End)
		ORDER BY 
			CASE WHEN @SortBy = 'GivenName' AND @SortOrder = 'Asc' Then [GivenName] END Asc,
			CASE WHEN @SortBy = 'GivenName' AND @SortOrder = 'Desc' Then [GivenName] END Desc,
			CASE WHEN @SortBy = 'FamilyName' AND @SortOrder = 'Asc' Then [FamilyName] END Asc,
			CASE WHEN @SortBy = 'FamilyName' AND @SortOrder = 'Desc' Then [FamilyName] END Desc,
			CASE WHEN @SortBy = 'PlanetName' AND @SortOrder = 'Asc' Then [PlanetName] END Asc,
			CASE WHEN @SortBy = 'PlanetName' AND @SortOrder = 'Desc' Then [PlanetName] END Desc,			
			CASE WHEN @SortBy = 'UpdatedOn' AND @SortOrder = 'Asc' Then [UpdatedOn] END Asc,
			CASE WHEN @SortBy = 'UpdatedOn' AND @SortOrder = 'Desc' Then [UpdatedOn] END Desc
		OFFSET @Start ROWS
		FETCH NEXT @PageSize ROWS ONLY;

END