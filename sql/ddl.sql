create database GameData;

use GameData;

create table LeaderBoard (Signature varchar(10), Score int, Saved timestamp, GameId tinyint);

create index Leader1 on LeaderBoard (GameId, Score desc, Saved desc);

insert LeaderBoard (Signature, Score, Saved, GameId) values ('ABN',100,now(),1);

select lb.Signature, lb.Score from LeaderBoard lb order by lb.Score desc, lb.Saved desc;


CREATE USER 'foo'@'%' IDENTIFIED BY '12345';
GRANT ALL PRIVILEGES ON *.* TO 'foo'@'%' WITH GRANT OPTION;



DELIMITER //
CREATE PROCEDURE AddScore
(IN
	signature varchar(10)
	,score int
	,gameid tinyint
)
BEGIN

insert
	LeaderBoard
(
	Signature
	,Score
	,Saved
	,GameId
)
values
(
	signature
	,score
	,now()
	,gameid
);
END //
DELIMITER ;



DELIMITER //
CREATE PROCEDURE GetScores
(IN
	gameid tinyint
)
BEGIN

select
	lb.Signature
	,lb.Score
from
	LeaderBoard lb
where
	lb.GameId = gameid
order by
	lb.Score desc
	,lb.Saved desc
limit 20
;
END //
DELIMITER ;


call AddScore('CAW',5,1);
call GetScores(1);
