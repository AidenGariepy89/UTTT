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
        this.cellElement.classList.toggle("activate");
    }
}

class Board {
    boardId: number;
    boardElement: HTMLElement;
    cells: Cell[];
    parentBoard: UltimateBoard;

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

    activate(cell: number) {
        this.cells[cell].activate();
    }
}

const enum PlayState {
    YourTurn,
    OtherTurn,
    GameOver,
}

export class UltimateBoard {
    gameId: number;
    boardElement: HTMLElement;
    infoTextElement: HTMLElement;
    subBoards: Board[];
    listeners: ((gameId: number, m: Move) => void)[];
    player: PieceType;

    state: PlayState;

    readonly BOARD_DIM = 9;

    constructor(
        gameId: number,
        boardElement: HTMLElement,
        player: PieceType
    ) {
        this.gameId = gameId;
        this.boardElement = boardElement;
        this.infoTextElement = document.getElementById("info-text");
        this.subBoards = [];
        this.listeners = [];
        this.player = player;

        if (this.player == PieceType.X) {
            this.state = PlayState.YourTurn;
            this.setInfo("Your turn!");
        } else {
            this.state = PlayState.OtherTurn;
            this.setInfo("Their turn!");
        }

        for (let i = 0; i < this.BOARD_DIM; i++) {
            this.subBoards.push(this.createSubBoard());
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

        const move = new Move();
        move.piece = this.player;
        move.board = boardId;
        move.cell = cellId;

        for (let i = 0; i < this.listeners.length; i++) {
            // let listener = this.listeners[i];
            // listener(move);
            this.listeners[i](this.gameId, move);
        }
    }

    playMove(move: Move) {
        if (move.piece == this.player) {
            this.state = PlayState.OtherTurn;
            this.setInfo("Their turn!");
        } else {
            this.state = PlayState.YourTurn;
            this.setInfo("Your turn!");
        }

        this.subBoards[move.board].playMove(move);
    }

    addListener(listener: (gameId: number, m: Move) => void) {
        this.listeners.push(listener);
    }

    activate(board: number, cell: number) {
        this.subBoards[board].activate(cell);
    }

    setInfo(msg: string) {
        this.infoTextElement.innerHTML = msg;
    }
}

export class Move {
    piece: PieceType;
    board: number;
    cell: number;
}
