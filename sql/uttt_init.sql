drop table if exists lobby;
drop table if exists conn;
drop table if exists game_moves;
drop table if exists moves;
drop table if exists game;
drop table if exists player;

create table player (
    id int primary key,
    username varchar(32) unique not null
);

create table conn (
    conn_id varchar(32) primary key,
    player_id int not null,
    foreign key(player_id) references player(id)
);

create table lobby (
    conn_id varchar(32) primary key,
    foreign key(conn_id) references conn(conn_id)
);

create table game (
    id int primary key,
    x_player int not null,
    o_player int not null,
    winner int null,
    foreign key(x_player) references player(id),
    foreign key(o_player) references player(id),
    foreign key(winner) references player(id)
);

create table moves (
    id int primary key,
    board int not null,
    cell int not null,
    piece char(1) not null
);

create table game_moves (
    id int primary key,
    game_id int not null,
    move_id int not null,
    turn int not null,
    foreign key(game_id) references game(id),
    foreign key(move_id) references moves(id)
);

------------------------------ Sample Data ------------------------------------

insert into player values (1, 'aiden');
insert into player values (2, 'jack');

insert into game values (1, 1, 2, 1);

insert into moves values (1, 4, 0, 'X');
insert into moves values (2, 0, 4, 'O');
insert into moves values (3, 4, 1, 'X');
insert into moves values (4, 1, 4, 'O');
insert into moves values (5, 4, 2, 'X');
insert into moves values (6, 2, 4, 'O');
insert into moves values (7, 3, 0, 'X');
insert into moves values (8, 0, 3, 'O');
insert into moves values (9, 3, 1, 'X');
insert into moves values (10, 1, 3, 'O');
insert into moves values (11, 3, 2, 'X');
insert into moves values (12, 2, 3, 'O');
insert into moves values (13, 5, 0, 'X');
insert into moves values (14, 0, 5, 'O');
insert into moves values (15, 5, 1, 'X');
insert into moves values (16, 1, 5, 'O');
insert into moves values (17, 5, 2, 'X');

insert into game_moves values (1, 1, 1, 1);
insert into game_moves values (2, 1, 2, 2);
insert into game_moves values (3, 1, 3, 3);
insert into game_moves values (4, 1, 4, 4);
insert into game_moves values (5, 1, 5, 5);
insert into game_moves values (6, 1, 6, 6);
insert into game_moves values (7, 1, 7, 7);
insert into game_moves values (8, 1, 8, 8);
insert into game_moves values (9, 1, 9, 9);
insert into game_moves values (10, 1, 10, 10);
insert into game_moves values (11, 1, 11, 11);
insert into game_moves values (12, 1, 12, 12);
insert into game_moves values (13, 1, 13, 13);
insert into game_moves values (14, 1, 14, 14);
insert into game_moves values (15, 1, 15, 15);
insert into game_moves values (16, 1, 16, 16);
insert into game_moves values (17, 1, 17, 17);
