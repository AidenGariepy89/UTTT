-- The actual DB Script

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
    id int identity primary key,
    board int not null,
    cell int not null,
    piece char(1) not null
);

create table game_moves (
    id int identity primary key,
    game_id int not null,
    move_id int not null,
    turn int not null,
    foreign key(game_id) references game(id),
    foreign key(move_id) references moves(id)
);