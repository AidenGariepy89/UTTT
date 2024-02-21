-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2024-02-21 18:49:58.928

-- foreign keys
ALTER TABLE game DROP CONSTRAINT fk_o_player;

ALTER TABLE game DROP CONSTRAINT fk_winner;

ALTER TABLE game DROP CONSTRAINT fk_x_player;

ALTER TABLE game_moves DROP CONSTRAINT game_moves_game;

ALTER TABLE game_moves DROP CONSTRAINT game_moves_moves;

-- tables
DROP TABLE game;

DROP TABLE game_moves;

DROP TABLE moves;

DROP TABLE player;

-- End of file.

