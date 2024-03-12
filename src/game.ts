export const enum PieceType {
    Empty,
    X,
    O,
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
}

export class UltimateBoard {
    gameId: number;
    boardElement: HTMLElement;
    subBoards: Board[];
    listeners: ((m: Move) => void)[];

    readonly BOARD_DIM = 9;

    constructor(
        gameId: number,
        boardElement: HTMLElement,
    ) {
        this.gameId = gameId;
        this.boardElement = boardElement;
        this.subBoards = [];
        this.listeners = [];

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
        const move = new Move();
        move.piece = PieceType.X;
        move.cell = (boardId * this.BOARD_DIM) + cellId;

        for (let i = 0; i < this.listeners.length; i++) {
            let listener = this.listeners[i];

            listener(move);
        }
    }

    playMove(move: Move) {
        this.subBoards[move.board].playMove(move);
    }

    addListener(listener: (m: Move) => void) {
        this.listeners.push(listener);
    }
}

export class Move {
    piece: PieceType;
    board: number;
    cell: number;
}
