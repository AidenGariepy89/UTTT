drop table if exists game_moves;
drop table if exists move;
drop table if exists game;
drop table if exists player;

create table player (
    id integer not null primary key autoincrement,
    username text not null,
    pwd_hash text
);

create table game (
    id integer not null primary key autoincrement,
    x_player integer not null,
    o_player integer not null,
    winner integer,
    foreign key(x_player) references player(id),
    foreign key(o_player) references player(id),
    foreign key(winner) references player(id)
);

create table move (
    id integer not null primary key autoincrement,
    board integer not null,
    cell integer not null,
    piece text not null,
    check (board > 0 and board <= 9),
    check (cell > 0 and cell <= 9),
    check (piece in ("X", "O"))
);

create table game_moves (
    game_id integer not null primary key,
    move integer not null,
    turn integer not null,
    foreign key(game_id) references game(id),
    foreign key(move) references move(id),
    check (turn > 0)
);
