create database PwrightSandbox;

use PwrightSandbox;




-- Model New Model
-- Updated 8/29/2018 2:25:53 PM
-- DDL Generated 8/29/2018 2:25:55 PM
--**********************************************************************
--	Tables
--**********************************************************************
-- Table dbo.Game
create table
	[dbo].[Game]
(
	[GameId] int not null
	, [GameGuid] uniqueidentifier not null
,
constraint [Pk_Game_GameId] primary key clustered
(
	[GameId] asc
)
,
constraint [Ak_Game_GameGuid] unique nonclustered
(
	[GameGuid] asc
)
);
-- Table dbo.Score
create table
	[dbo].[Score]
(
	[GameId] int not null
	, [Signature] char(3) not null
	, [Points] int not null
	, [ScoreDate] smalldatetime not null
);
--**********************************************************************
--	Data
--**********************************************************************
--**********************************************************************
--	Relationships
--**********************************************************************
-- Relationship Fk_Game_Score_GameId
alter table [dbo].[Score]
add constraint [Fk_Game_Score_GameId] foreign key ([GameId]) references [dbo].[Game] ([GameId]);









CREATE USER [foo] FOR LOGIN [dbTest]
GO




--drop procedure dbo.AddScore
CREATE PROCEDURE AddScore

@GameGuid uniqueidentifier
,@Signature char(3)
,@Points int
as
begin

insert
	Score
(
	[GameId]
	,[Signature]
	,[Points]
	,[ScoreDate]
)
select
	g.GameId
	,@Signature
	,@Points
	,getdate()
from
	dbo.Game g
where
	g.GameGuid = @GameGuid;
END

go

grant execute on [dbo].AddScore to [foo];




go
--drop procedure dbo.GetScores
CREATE PROCEDURE GetScores
@GameGuid uniqueidentifier
,@Limit int
as
BEGIN

select
	top (@Limit)
	sc.[Signature]
	,sc.[Points]
from
	Game g
join
	Score sc
	on sc.GameId = g.GameId
where
	g.GameGuid = @GameGuid
order by
	sc.Points desc
	,sc.ScoreDate desc;
END


go

grant execute on [dbo].GetScores to [foo];


insert dbo.Game (GameId,GameGuid) values (1,'A6620930-D791-4A03-8AAC-C2943B40E24D');
exec AddScore 'ABN',5,1;
exec GetScores 1;
