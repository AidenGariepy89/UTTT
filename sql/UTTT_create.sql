-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2024-02-21 18:49:58.928

-- tables
-- Table: game
CREATE TABLE game
(
    id int identity NOT NULL,
    x_player int NOT NULL,
    o_player int NOT NULL,
    winner int NULL,
    CONSTRAINT game_pk PRIMARY KEY  (id)
);

-- Table: game_moves
CREATE TABLE game_moves
(
    game_id int NOT NULL,
    move_id int NOT NULL,
    turn int NOT NULL,
    CONSTRAINT chk_turn CHECK (turn > 0),
    CONSTRAINT game_moves_pk PRIMARY KEY  (move_id,game_id)
);

-- Table: moves
CREATE TABLE moves
(
    id int identity NOT NULL,
    board int NOT NULL,
    cell int NOT NULL,
    piece char(1) NOT NULL,
    CONSTRAINT chk_board CHECK (board > 0 and board <= 9),
    CONSTRAINT chk_cell CHECK (cell > 0 and cell <= 9),
    CONSTRAINT chk_piece CHECK (piece in ('X', 'O')),
    CONSTRAINT moves_pk PRIMARY KEY  (id)
);

-- Table: player
CREATE TABLE player
(
    id int identity NOT NULL,
    username varchar(32) NOT NULL,
    pwd_hash varchar(32) NOT NULL,
    CONSTRAINT player_pk PRIMARY KEY  (id)
);

-- foreign keys
-- Reference: fk_o_player (table: game)
ALTER TABLE game ADD CONSTRAINT fk_o_player
    FOREIGN KEY (o_player)
    REFERENCES player (id);

-- Reference: fk_winner (table: game)
ALTER TABLE game ADD CONSTRAINT fk_winner
    FOREIGN KEY (winner)
    REFERENCES player (id);

-- Reference: fk_x_player (table: game)
ALTER TABLE game ADD CONSTRAINT fk_x_player
    FOREIGN KEY (x_player)
    REFERENCES player (id);

-- Reference: game_moves_game (table: game_moves)
ALTER TABLE game_moves ADD CONSTRAINT game_moves_game
    FOREIGN KEY (game_id)
    REFERENCES game (id);

-- Reference: game_moves_moves (table: game_moves)
ALTER TABLE game_moves ADD CONSTRAINT game_moves_moves
    FOREIGN KEY (move_id)
    REFERENCES moves (id);

-- End of file.

