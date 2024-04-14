import * as signalR from "@microsoft/signalr";
import { Move, PieceType, UltimateBoard, stringToPieceType } from "./game";
import { infoLoading, ultimateBoard } from "./ui";

const enum GameState {
    None,
    Lobby,
    Connecting,
    Playing,
    GameOver,
}

type GameData = {
    state: GameState;
    conn: signalR.HubConnection;
    display: HTMLElement;
    username: string;
    board: UltimateBoard | undefined;
};

type JoinGamePacket = {
    gameId: number;
    playerPiece: string;
};

type MovePacket = {
    gameId: number;
    board: number;
    cell: number;
    playerPiece: string;
    // move: Move;
};

function fatalError(err: any) {
    // alert(err);
    window.location.assign("game/gameerror?msg=" + err);
}

let username = prompt("Username: ");
if (username == "") {
    username = "aiden";
}

const gameData: GameData = {
    state: GameState.None,
    conn: undefined,
    display: document.getElementById("display"),
    username: username,
    board: undefined,
};

if (gameData.username == "") {
    fatalError("No username provided");
}

setupGame(gameData);

function setupGame(data: GameData) {
    data.display.replaceChildren(infoLoading("Connecting..."));

    const conn = new signalR.HubConnectionBuilder()
        .withUrl("/uttt")
        .build();

    data.conn = conn;

    data.conn.start()
        .then(() => {
            data.state = GameState.Lobby;

            console.log("In lobby");
            data.display.replaceChildren(infoLoading("Finding match..."));

            data.conn.send("registerUsername", data.username).catch(fatalError);
        })
        .catch(fatalError);

    data.conn.on("joinGame", (joinGameJson) => { joinGame(joinGameJson); });
    data.conn.on("clientError", fatalError);
    data.conn.on("serverMsg", serverMsg);
    data.conn.on("announceMove", recieveMove);
}

function serverMsg(msg: any) {
    console.log("Server: " + msg);
}

function joinGame(joinGameJson: string) {
    let packet: JoinGamePacket = JSON.parse(joinGameJson);

    let player = stringToPieceType(packet.playerPiece);
    if (player == undefined) {
        fatalError("Undefined piece type from server");
    }
    console.log("Player: " + player);

    gameData.state = GameState.Playing;
    gameData.conn.send("leaveLobby").catch(fatalError);
    gameData.display.replaceChildren(ultimateBoard());

    createBoard(packet.gameId, player);
}

function createBoard(gameId: number, player: PieceType) {
    const boardElement = document.getElementById("ultimate-board");
    gameData.board = new UltimateBoard(gameId, boardElement, player);
    gameData.board.addListener(sendMove);
}

function sendMove(gameId: number, move: Move) {
    const packet: MovePacket = {
        gameId: gameId,
        board: move.board,
        cell: move.cell,
        playerPiece: move.piece,
    };

    const json = JSON.stringify(packet);

    gameData.conn.send("submitMove", json)
        // .then(() => console.log("Sent successfully"))
        .catch(err => console.error("Sent unsuccessfully: ", err));
}

function recieveMove(json: string) {
    if (gameData.board == undefined) {
        return;
    }

    const packet: MovePacket = JSON.parse(json);

    const move: Move = new Move;
    move.board = packet.board;
    move.cell = packet.cell;
    move.piece = stringToPieceType(packet.playerPiece);

    gameData.board.playMove(move);
}
