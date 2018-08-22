create database PwrightSandbox;

use PwrightSandbox;

--drop table LeaderBoard
create table LeaderBoard ([Signature] varchar(10), Score int, Saved datetime, GameId tinyint);

create index Leader1 on LeaderBoard (GameId, Score desc, Saved desc);

insert LeaderBoard ([Signature], Score, Saved, GameId) values ('ABN',100,getdate(),1);

select lb.Signature, lb.Score from LeaderBoard lb order by lb.Score desc, lb.Saved desc;


CREATE USER [foo] FOR LOGIN [dbTest]
GO





CREATE PROCEDURE AddScore

@signature varchar(10)
,@score int
,@gameid tinyint
as
begin

insert
	LeaderBoard
(
	[Signature]
	,Score
	,Saved
	,GameId
)
values
(
	@signature
	,@score
	,getdate()
	,@gameid
);
END

grant execute on [dbo].AddScore to [foo];




go
CREATE PROCEDURE GetScores
@gameid tinyint
as
BEGIN

select
	top 20
	lb.Signature
	,lb.Score
from
	LeaderBoard lb
where
	lb.GameId = gameid
order by
	lb.Score desc
	,lb.Saved desc;
END

grant execute on [dbo].GetScores to [foo];


exec AddScore 'ABN',5,1;
exec GetScores 1;
