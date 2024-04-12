import * as signalR from "@microsoft/signalr";
import { Move, UltimateBoard } from "./game";
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
}

function fatalError(err: any) {
    alert(err);
    // window.location.assign("game/gameerror");
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

    data.conn.on("joinGame", (stuff) => { joinGame(stuff, data); });
    data.conn.on("clientError", fatalError);
    data.conn.on("serverMsg", serverMsg);
    data.conn.on("announceMove", recieveMove);
}

function serverMsg(msg: any) {
    console.log("Server: " + msg);
}

function joinGame(player: string, data: GameData) {
    console.log("Player: " + player);

    data.state = GameState.Playing;
    data.conn.send("leaveLobby").catch(fatalError);
    data.display.replaceChildren(ultimateBoard());

    createBoard();
}

function createBoard() {
    const boardElement = document.getElementById("ultimate-board");
    gameData.board = new UltimateBoard(1, boardElement);
    gameData.board.addListener(sendMove);
}

function sendMove(move: Move) {
    const json = JSON.stringify(move);

    gameData.conn.send("submitMove", json)
        // .then(() => console.log("Sent successfully"))
        .catch(err => console.error("Sent unsuccessfully: ", err));
}

function recieveMove(json: string) {
    if (gameData.board == undefined) {
        return;
    }

    const move: Move = JSON.parse(json);
    gameData.board.playMove(move);
}

// const conn = new signalR.HubConnectionBuilder()
//     .withUrl("/uttt")
//     .build();
//
// conn.start()
//     .then(() => console.log("Established connection."))
//     .catch(err => console.error("Connection failed: ", err));
//
//
// const board_element = document.getElementById('ultimate-board');
// const ultimate = new UltimateBoard(1, board_element);
// ultimate.addListener(sendMove);
//
// function recieveMove(json: string) {
//     const move: Move = JSON.parse(json);
//
//     ultimate.playMove(move);
// }
//
// conn.on("announceMove", recieveMove);
//
// ultimate.activate(5, 3);
