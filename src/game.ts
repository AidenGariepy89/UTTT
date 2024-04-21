const WinStates: number[] = [
    0b111000000,
    0b000111000,
    0b000000111,
    0b100100100,
    0b010010010,
    0b001001001,
    0b100010001,
    0b001010100,
];

export const enum PieceType {
    Empty = "",
    X = "X",
    O = "O",
}

function pieceTypeToString(piece: PieceType): string {
    switch (piece) {
        case PieceType.Empty:
            return "";
        case PieceType.X:
            return "X";
        case PieceType.O:
            return "O";
    }
}

export function stringToPieceType(piece: string): PieceType | undefined {
    switch (piece) {
        case "":
            return PieceType.Empty;
        case "X":
            return PieceType.X;
        case "O":
            return PieceType.O;
        default:
            return undefined;
    }
}

class Cell {
    cellId: number;
    value: PieceType;
    cellElement: HTMLElement;

    constructor(id: number, cellElement: HTMLElement) {
        this.cellId = id;
        this.cellElement = cellElement;

        this.setCell(PieceType.Empty);
    }

    setCell(value: PieceType) {
        this.cellElement.dataset.value = value.toString();
        this.cellElement.innerHTML = pieceTypeToString(value);
        this.value = value;
    }

    activate() {
        this.cellElement.classList.add("activated");
    }

    disable() {
        this.cellElement.classList.add("disabled");
    }

    highlight() {
        this.cellElement.classList.add("highlight");
    }

    xTaken() {
        this.cellElement.classList.add("x-taken");
    }

    oTaken() {
        this.cellElement.classList.add("o-taken");
    }

    clear() {
        this.cellElement.classList.remove("activated");
        this.cellElement.classList.remove("disabled");
        this.cellElement.classList.remove("highlight");
        this.cellElement.classList.remove("x-taken");
        this.cellElement.classList.remove("o-taken");
    }
}

class Board {
    boardId: number;
    boardElement: HTMLElement;
    cells: Cell[];
    parentBoard: UltimateBoard;
    winner: PieceType;

    readonly BOARD_DIM = 9;

    constructor(
        id: number,
        boardElement: HTMLElement,
        parentBoard: UltimateBoard,
    ) {
        this.boardId = id;
        this.boardElement = boardElement;
        this.cells = [];
        this.parentBoard = parentBoard;
        this.winner = PieceType.Empty;

        for (let i = 0; i < this.BOARD_DIM; i++) {
            this.cells.push(this.createCell());
        }
    }

    createCell(): Cell {
        const cellId = this.cells.length;
        const element = document.createElement("div");
        element.id = `cell-${this.boardId}-${cellId}`;
        element.classList.add("cell");

        this.boardElement.appendChild(element);
        element.addEventListener('click', () => {
            this.cellClicked(cellId);
        });

        const cell = new Cell(cellId, element);
        return cell;
    }

    cellClicked(cellId: number) {
        if (this.cells[cellId].value != PieceType.Empty) {
            return;
        }
        this.parentBoard.cellClicked(this.boardId, cellId);
    }

    playMove(move: Move) {
        this.cells[move.cell].setCell(move.piece);
    }

    winCheck(): PieceType {
        let xState = 0;
        let oState = 0;
        for (let i = 0; i < this.BOARD_DIM; i++) {
            xState <<= 1;
            oState <<= 1;

            if (this.cells[i].value === PieceType.X) {
                xState ^= 1;
            } else if (this.cells[i].value === PieceType.O) {
                oState ^= 1;
            }
        }

        console.log("X: " + xState.toString(2));
        console.log("O: " + oState.toString(2));

        let winner = PieceType.Empty;

        for (let i = 0; i < WinStates.length; i++) {
            if ((xState & WinStates[i]) === WinStates[i]) {
                winner = PieceType.X;
                break;
            }
            if ((oState & WinStates[i]) === WinStates[i]) {
                winner = PieceType.O;
                break;
            }
        }

        this.winner = winner;

        return winner;
    }
}

const enum PlayState {
    YourTurn,
    OtherTurn,
    GameOver,
}

export class UltimateBoard {
    gameId: number;
    player: PieceType;

    boardElement: HTMLElement;
    infoTextElement: HTMLElement;

    subBoards: Board[];
    currentSubBoard: number;
    
    moveListeners: ((gameId: number, m: Move) => void)[];
    winListeners: ((gameId: number, winner: PieceType) => void)[];

    state: PlayState;

    readonly BOARD_DIM = 9;

    constructor(
        gameId: number,
        boardElement: HTMLElement,
        player: PieceType
    ) {
        this.gameId = gameId;
        this.player = player;

        this.boardElement = boardElement;
        this.infoTextElement = document.getElementById("info-text");

        this.subBoards = [];
        this.currentSubBoard = -1;

        this.moveListeners = [];
        this.winListeners = [];

        for (let i = 0; i < this.BOARD_DIM; i++) {
            this.subBoards.push(this.createSubBoard());
        }

        if (this.player == PieceType.X) {
            this.yourTurn(undefined);
        } else {
            this.otherTurn();
        }
    }

    createSubBoard(): Board {
        const boardId = this.subBoards.length;
        const element = document.createElement("div");
        element.classList.add("board");
        element.id = `board-${boardId}`;

        this.boardElement.appendChild(element);

        const board = new Board(boardId, element, this);
        return board;
    }

    cellClicked(boardId: number, cellId: number) {
        if (this.state != PlayState.YourTurn) {
            return;
        }

        if (this.currentSubBoard != -1 && this.currentSubBoard != boardId) {
            return;
        }

        if (this.subBoards[boardId].winner !== PieceType.Empty) {
            return;
        }

        const move = new Move();
        move.piece = this.player;
        move.board = boardId;
        move.cell = cellId;

        for (let i = 0; i < this.moveListeners.length; i++) {
            this.moveListeners[i](this.gameId, move);
        }
    }

    playMove(move: Move) {
        this.subBoards[move.board].playMove(move);

        let result = this.winCheck();

        this.clearBoard();
        this.subBoards[move.board].cells[move.cell].highlight();

        if (move.piece == this.player) {
            this.otherTurn();
        } else {
            this.yourTurn(move);
        }

        if (result !== PieceType.Empty) {
            this.win(result);
        }
    }

    addMoveListener(listener: (gameId: number, m: Move) => void) {
        this.moveListeners.push(listener);
    }

    addWinListener(listener: (gameId: number, winner: PieceType) => void) {
        this.winListeners.push(listener);
    }

    activate(board: number, cell: number) {
        this.subBoards[board].cells[cell].activate();
    }

    setInfo(msg: string) {
        this.infoTextElement.innerHTML = msg;
    }

    otherTurn() {
        this.state = PlayState.OtherTurn;
        this.setInfo("Their turn!");

        for (let i = 0; i < this.BOARD_DIM; i++) {
            for (let j = 0; j < this.BOARD_DIM; j++) {
                this.subBoards[i].cells[j].disable();
            }
        }
    }

    yourTurn(move: Move | undefined) {
        this.state = PlayState.YourTurn;
        this.setInfo("Your turn!");

        if (move === undefined) {
            return;
        }

        this.currentSubBoard = move.cell;
        if (this.subBoards[this.currentSubBoard].winner === PieceType.Empty) {
            for (let i = 0; i < this.BOARD_DIM; i++) {
                for (let j = 0; j < this.BOARD_DIM; j++) {
                    if (i == this.currentSubBoard) {
                        continue;
                    }

                    this.subBoards[i].cells[j].disable();
                }
            }
        } else {
            this.currentSubBoard = -1;
        }
    }

    clearBoard() {
        for (let i = 0; i < this.BOARD_DIM; i++) {
            for (let j = 0; j < this.BOARD_DIM; j++) {
                this.subBoards[i].cells[j].clear();
            }
        }

        for (let i = 0; i < this.BOARD_DIM; i++) {
            if (this.subBoards[i].winner === PieceType.Empty) {
                continue;
            }

            for (let j = 0; j < this.BOARD_DIM; j++) {
                if (this.subBoards[i].winner === PieceType.X) {
                    this.subBoards[i].cells[j].xTaken();
                } else if (this.subBoards[i].winner === PieceType.O) {
                    this.subBoards[i].cells[j].oTaken();
                }
            }
        }
    }

    winCheck(): PieceType {
        let xWins = 0;
        let oWins = 0;

        for (let i = 0; i < this.BOARD_DIM; i++) {
            xWins <<= 1;
            oWins <<= 1;

            let result = this.subBoards[i].winCheck();
            if (result === PieceType.X) {
                xWins ^= 1;
            } else if (result === PieceType.O) {
                oWins ^= 1;
            }
        }

        for (let i = 0; i < WinStates.length; i++) {
            if ((xWins & WinStates[i]) === WinStates[i]) {
                return PieceType.X;
            }
            if ((oWins & WinStates[i]) === WinStates[i]) {
                return PieceType.O;
            }
        }

        return PieceType.Empty;
    }

    win(winner: PieceType) {
        if (winner === PieceType.X) {
            this.state = PlayState.GameOver;
        } else if (winner === PieceType.O) {
            this.state = PlayState.GameOver;
        }

        if (winner === this.player) {
            this.setInfo("You win!!!");
        } else {
            this.setInfo("You lose!!!");
        }
        
        for (let i = 0; i < this.winListeners.length; i++) {
            this.winListeners[i](this.gameId, winner);
        }
    }
}

export class Move {
    piece: PieceType;
    board: number;
    cell: number;
}
