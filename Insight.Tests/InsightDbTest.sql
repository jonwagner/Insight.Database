----------------------------------------------------------
-- Beer
----------------------------------------------------------
CREATE TABLE Beer (
	ID int NOT NULL IDENTITY,
	Name [varchar](256),
	Style [varchar](256),
)
GO

----------------------------------------------------------
-- PK_Beer
----------------------------------------------------------
ALTER TABLE Beer ADD CONSTRAINT PK_Beer PRIMARY KEY ([ID])
GO

----------------------------------------------------------
-- SCRIPT [MakeBeer]
----------------------------------------------------------
SET IDENTITY_INSERT Beer ON
IF NOT EXISTS(SELECT * FROM Beer WHERE ID = 1) INSERT INTO Beer (ID, Name, Style) VALUES (1, 'Sly Fox 113', 'IPA')
IF NOT EXISTS(SELECT * FROM Beer WHERE ID = 2) INSERT INTO Beer (ID, Name, Style) VALUES (2, 'Dogfish 60-Minute', 'IPA')
IF NOT EXISTS(SELECT * FROM Beer WHERE ID = 3) INSERT INTO Beer (ID, Name, Style) VALUES (3, 'Sly Fox 90-Minute', 'IPA')
SET IDENTITY_INSERT Beer OFF
GO

----------------------------------------------------------
-- SelectAllBeer
----------------------------------------------------------
CREATE PROC SelectAllBeer
AS
	SELECT * FROM Beer
GO

----------------------------------------------------------
-- SelectAllBeerChildren
----------------------------------------------------------
CREATE PROC SelectAllBeerChildren
AS
	SELECT ParentID=1, b.* FROM Beer b
GO

----------------------------------------------------------
-- AUTOPROC All Beer
----------------------------------------------------------
GO

----------------------------------------------------------
-- ProcWithOutputParameters
----------------------------------------------------------
CREATE PROC ProcWithOutputParameters (
	@in [int],
	@out1 [int] OUTPUT,
	@out2 [int] = 0 OUTPUT
)
AS
	SET @out1 = @in
	SET @out2 = @in
	SELECT @in
	RETURN @in
GO

----------------------------------------------------------
-- BulkCopyData
----------------------------------------------------------
CREATE TABLE BulkCopyData (
	[Int] [int],
	[Computed] AS [Int] + 1,
	[Geometry] [geometry]
)
GO

CREATE TABLE BulkCopyWithComputedData (
	[Int1] [int],
	[Computed] AS [Int1] + 1,
	[Int2] [int]
)
GO

CREATE TABLE [MerchNameTermsTransactions] (
    [Id]     INT IDENTITY (1, 1) NOT NULL,
    [TermId] INT NOT NULL,
    [TranId] INT NOT NULL,
);
GO

CREATE TABLE [CardTransactions] (
    [TranId] INT IDENTITY (1, 1) NOT NULL,
);
GO

ALTER TABLE [CardTransactions] ADD CONSTRAINT [PK_CardTransactions] PRIMARY KEY ([TranId])
GO

ALTER TABLE [MerchNameTermsTransactions] ADD CONSTRAINT [FK_MerchNameTermsTransactions_CardTransactions] FOREIGN KEY ([TranId]) REFERENCES [dbo].[CardTransactions] ([TranID])
GO

----------------------------------------------------------
-- Insert test procs
----------------------------------------------------------
CREATE PROC InsertIdentityReturn (@Value int) AS SELECT Id=1, Id2=@Value
GO
CREATE PROC InsertIdentityReturn2 (@OtherValue int) AS SELECT Id=1, Id2=@OtherValue
GO

CREATE TABLE InsertByTableTable (ID [int] IDENTITY(1,1), ID2 [int] DEFAULT (2), Text [varchar](128), Value int)
GO
CREATE TYPE InsertByTableType AS TABLE (Text [varchar](128), Value int)
GO
CREATE PROC InsertByTable(@OtherValue int, @Items [InsertByTableType] READONLY) AS 
	TRUNCATE TABLE InsertByTableTable
	DBCC CHECKIDENT ('InsertByTableTable', RESEED, 1)
	INSERT INTO InsertByTableTable (Text, Value)
		OUTPUT inserted.ID, inserted.ID2
		SELECT Text, @OtherValue FROM @Items
GO

CREATE PROCEDURE [dbo].[InsertBeer_ScopeIdentity]
(
	@Name varchar(256) = NULL,
	@Style varchar(256) = NULL
)
AS

	INSERT INTO [dbo].[Beer] ([Name], [Style])
		VALUES 	(@Name,	@Style)

	SELECT SCOPE_IDENTITY() AS [Id]

GO


----------------------------------------------------------
-- General test types
----------------------------------------------------------
CREATE TYPE [Int32Table] AS TABLE ([Value] [int])
GO
CREATE PROC ReflectInt (@Value int = 5) AS SELECT Value=@Value
GO
CREATE PROC ReflectInt32Table (@Value Int32Table READONLY) AS SELECT * FROM @Value
GO
CREATE PROC RaiseAnError (@Value varchar(128)) AS raiserror ('test', 18, 1)
GO

----------------------------------------------------------
-- XML test types
----------------------------------------------------------
CREATE PROC ReflectXml (@Xml xml) AS SELECT Xml=@Xml
GO
CREATE PROC ReflectXmlAsData (@Xml xml) AS SELECT Data=@Xml
GO
CREATE TYPE [XmlDataTable] AS TABLE ([id] int, [Data] [Xml])
GO
CREATE PROCEDURE ReflectXmlTable @p [XmlDataTable] READONLY AS SELECT * FROM @p
GO
CREATE PROCEDURE ReflectXmlTableAsVarChar @p [XmlDataTable] READONLY AS SELECT CONVERT(varchar(MAX),Data) FROM @p
GO
CREATE PROC ReflectString (@Value varchar(128)) AS SELECT Value=@Value
GO
CREATE PROC ReflectTwoRecordsets (@Value varchar(128), @Value2 varchar(128)) AS SELECT Value=@Value SELECT Value=@Value2
GO

----------------------------------------------------------
-- Output parameter procs
----------------------------------------------------------
CREATE PROCEDURE TestOutputParameters @p int = 1 OUTPUT AS SET @p = 9
GO
CREATE PROC ReturnAValue AS RETURN 11
GO
CREATE PROCEDURE TestXmlOutputParameters @data [xml] = NULL OUTPUT AS SET @data = @data
GO

----------------------------------------------------------
-- Mapping test procs
----------------------------------------------------------
CREATE PROCEDURE OutputParameterParentMappingTest @parent int = NULL OUTPUT, @foo int = NULL OUTPUT
AS
	SELECT @parent = 1
	SELECT @foo = 2
GO
CREATE PROCEDURE OutputParameterMappingTest @out_foo int = NULL OUTPUT AS SELECT @out_foo = 5
GO
CREATE PROC MappingAsJson1 @SubClass [varchar](max) AS SELECT SubClass=@SubClass
GO
CREATE PROC MappingAsJson2 @SubClass [varchar](max) AS SELECT SubClass=@SubClass
GO
CREATE PROC MappingAsJson3 @SubClass [varchar](max) AS SELECT SubClass=@SubClass
GO
CREATE PROC MappingAsJson4 @SubClass [varchar](max) AS SELECT SubClass=@SubClass
GO

CREATE TYPE MappingTestTable AS TABLE ([IntParentX] [int], [IntX][int])
GO
CREATE PROCEDURE MappingTestProc @p MappingTestTable READONLY AS SELECT * FROM @p
GO
CREATE PROCEDURE MappingTestProc2 @intParentX [int] AS SELECT @intParentX
GO
CREATE PROCEDURE MappingTestProcGeography @geo [geography] AS SELECT GEO=@geo
GO
CREATE TABLE MappingBulkCopyTestTable ([IntParentX] [int], [IntX][int])
GO

----------------------------------------------------------
-- JSON test procs
----------------------------------------------------------
CREATE PROC ClassAsJsonParameter @SubClass [varchar](max) AS SELECT SubClass=@SubClass
GO
CREATE PROC StructAsJsonParameter @SubStruct [varchar](max) AS SELECT SubStruct=@SubStruct
GO
CREATE PROC FieldAsJsonParameter @DateTimeField [varchar](max) AS SELECT DateTimeField=@DateTimeField
GO
CREATE PROC ListAsJsonParameter @ListOfClass [varchar](max) AS SELECT ListOfClass=@ListOfClass
GO

----------------------------------------------------------
-- Type test procs
----------------------------------------------------------
CREATE PROC ConvertClassToString (@ID int, @Name varchar(128)) AS SELECT ID=@ID, Name=@Name
GO
CREATE PROC TimeInput @t [time] AS SELECT @t
GO
CREATE PROC DateTimeInput @t [datetime] AS SELECT @t
GO
CREATE PROC TimeAdd @t [datetime], @add [datetime] AS SELECT @t + @add
GO
CREATE PROC TimeAdd2 @t [datetime], @add [time] AS SELECT @t + CAST(@add as [datetime])
GO
CREATE PROC TestDateTime2 @date datetime2 AS SELECT @date
GO
CREATE PROC TestDateTimeConvert @p datetime2 AS SELECT @p
GO
CREATE PROC TestGuidFromStringParam @p uniqueidentifier AS SELECT @p
GO
CREATE PROC TestGuidToStringParam @p varchar(300) AS SELECT convert(uniqueidentifier, @p)
GO
CREATE PROC TestStringToGuidValue @p varchar(300) AS SELECT convert(uniqueidentifier, @p)
GO
CREATE PROC GeometryProc (@geo [geometry]) AS SELECT @geo
GO

----------------------------------------------------------
-- Type test procs
----------------------------------------------------------
CREATE PROC GetParentTestData AS
	SELECT ParentX=2, X=5 
GO
CREATE PROC GetParentAndChildTestData AS
	SELECT ParentX=2, X=5
	SELECT Y=7 
GO

----------------------------------------------------------
-- Dynamic connection procs
----------------------------------------------------------
CREATE PROC DynamicProcWithTable (@i int = 0, @table Int32Table READONLY, @j int = 0) AS SELECT * FROM @table
GO

----------------------------------------------------------
-- Interface Test procs
----------------------------------------------------------
CREATE TYPE ObjectTable AS TABLE (ParentX [int])
GO
CREATE PROC ExecuteSomething AS SELECT NULL
GO
CREATE PROC ExecuteSomethingWithParameters @p int, @q [varchar](128) AS SELECT @p, @q
GO
CREATE PROC ExecuteSomethingScalar @p int AS SELECT @p
GO
CREATE PROC QueryValue @p int AS SELECT @p UNION ALL SELECT @p
GO
CREATE PROC QueryObject AS SELECT ParentX=2, X=5 
GO
CREATE PROC SingleObject AS SELECT ParentX=2, X=5
GO
CREATE PROC SingleObjectWithNoData AS SELECT ParentX=2, X=5 WHERE 0=1
GO
CREATE PROC ObjectAsParameter @ParentX [int] AS SELECT @ParentX
GO
CREATE PROC ObjectListAsParameter (@objects [ObjectTable] READONLY) AS SELECT ParentX FROM @objects
GO
CREATE PROC QueryResults @p int AS
	SELECT ParentX=2, X=5 
	SELECT @p
GO

CREATE TYPE InsertTestDataTVP AS TABLE (X [int], Z [int])
GO
CREATE TABLE InsertTestDataTable (X [int] identity (5, 1), Z [int])
GO
CREATE PROC ResetTestDataTable AS
	INSERT INTO InsertTestDataTable (Z) VALUES (0)
	TRUNCATE TABLE InsertTestDataTable
	DBCC CHECKIDENT ('InsertTestDataTable', RESEED, 1)
GO
CREATE PROC InsertTestData @Z [int] AS 
	INSERT INTO InsertTestDataTable (Z) OUTPUT inserted.X VALUES (@Z)
GO
CREATE PROC UpdateTestData @X [int], @Z [int] AS
	UPDATE InsertTestDataTable SET Z=@Z WHERE X=@X SELECT X=0
GO
CREATE PROC UpsertTestData (@X [int], @Z [int]) AS
	UPDATE InsertTestDataTable SET Z=@Z WHERE X=@X SELECT X=0
GO
CREATE PROC InsertMultipleTestData @data [InsertTestDataTVP] READONLY AS
	INSERT INTO InsertTestDataTable (Z) OUTPUT inserted.X SELECT Z FROM @data
GO
CREATE PROC UpsertMultipleTestData @data [InsertTestDataTVP] READONLY AS
	UPDATE InsertTestDataTable SET Z=data.Z FROM @data data WHERE data.X = InsertTestDataTable.X SELECT X=0 FROM @data
GO
CREATE PROC ReflectMultipleTestData @data [InsertTestDataTVP] READONLY AS
	SELECT * FROM @data
GO

CREATE PROC ExecuteWithOutputParameter (@p int = NULL OUTPUT) AS
	SELECT @p = @p + 1
GO
CREATE PROC ExecuteScalarWithOutputParameter (@p int = NULL OUTPUT) AS
	SELECT @p = @p + 1
	SELECT 7
GO
CREATE PROC QueryWithOutputParameter (@p int = NULL OUTPUT) AS
	SELECT @p = @p + 1
	SELECT 5
GO
CREATE PROC QueryResultsWithOutputParameter (@p int = NULL OUTPUT) AS
	SELECT @p = @p + 1
	SELECT 1
	SELECT 2
GO
CREATE PROC InsertWithOutputParameter @data [InsertTestDataTVP] READONLY, @p [int] OUTPUT AS
	INSERT INTO InsertTestDataTable (Z) OUTPUT inserted.X SELECT z FROM @data OUTPUT SELECT @p=@p+1
GO

----------------------------------------------------------
-- Interface Test procs
----------------------------------------------------------
CREATE TYPE [InsightTestDataTable] AS TABLE ([Int] [int] NOT NULL, [IntNull][int])
GO
CREATE TYPE [InsightTestDataTable2] AS TABLE ([Int] [decimal] NOT NULL, [IntNull][decimal])
GO
CREATE TYPE [InsightTestDataStringTable] AS TABLE ([String] [varchar](128) NOT NULL)
GO
CREATE PROCEDURE [Int32TestProc] @p [Int32Table] READONLY AS SELECT * FROM @p
GO
CREATE PROCEDURE [InsightTestDataTestProc] @p [InsightTestDataTable] READONLY AS SELECT * FROM @p
GO
CREATE PROCEDURE [InsightTestDataTestProc2] @p [InsightTestDataTable2] READONLY AS SELECT * FROM @p
GO
CREATE PROCEDURE [InsightTestDataStringTestProc] @p [InsightTestDataStringTable] READONLY AS SELECT * FROM @p
GO
CREATE TYPE [EvilTypes] AS TABLE (a [money], b [smallmoney], c [date])
GO
CREATE PROCEDURE [EvilProc] @p [EvilTypes] READONLY AS SELECT * FROM @p
GO
CREATE TYPE [StringMapTable] AS TABLE
(
	[Key] [varchar](1024),
	[Value] [varchar](1024)
)
GO
GRANT EXEC ON TYPE::StringMapTable TO public
GO
CREATE PROC [TestMap]
	@Map [StringMapTable] READONLY
AS
	SELECT * FROM @Map
GO
CREATE TYPE [VarcharIDTableType] AS TABLE ([ID] [varchar](300))
GO
CREATE PROCEDURE [VarCharProc] (@p [VarcharIDTableType] READONLY) AS SELECT * FROM @p
GO
CREATE FUNCTION [ReflectTableFunction](@p [BeerTable] READONLY)
RETURNS TABLE AS
	RETURN SELECT ID FROM @p
GO
CREATE FUNCTION [ReflectTableFunction2](@p [BeerTable] READONLY, @q int)
RETURNS TABLE AS
	RETURN SELECT ID FROM @p
GO
