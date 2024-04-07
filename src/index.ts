import * as signalR from "@microsoft/signalr";
import { Move, UltimateBoard } from "./game";

const enum GameState {
    None,
    Lobby,
    Connecting,
    Playing,
    GameOver,
}

class GameData {
    state: GameState;
    conn: signalR.HubConnection;
    username: string;

    constructor() {
        this.state = GameState.None;
        this.conn = undefined;
        this.username = "";
    }
}

const gameData: GameData = new GameData();
gameData.username = prompt("hey, whats ur name?");

setupGame(gameData);

// const conn = new signalR.HubConnectionBuilder()
//     .withUrl("/uttt")
//     .build();
//
// conn.start()
//     .then(() => console.log("Established connection."))
//     .catch(err => console.error("Connection failed: ", err));
//
// function sendMove(move: Move) {
//     const json = JSON.stringify(move);
//
//     conn.send("submitMove", json)
//         // .then(() => console.log("Sent successfully"))
//         .catch(err => console.error("Sent unsuccessfully: ", err));
// }
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

function fatalError(err: any) {
    console.error(err);
    window.location.assign("game/gameerror");
}

function setupGame(data: GameData) {
    const conn = new signalR.HubConnectionBuilder()
        .withUrl("/uttt")
        .build();

    conn.start()
        .then(() => {
            data.state = GameState.Lobby;
            console.log("In lobby");

            conn.send("registerUsername", data.username).catch(fatalError);
        })
        .catch(fatalError);

    conn.on("joinGame", (stuff) => { joinGame(stuff, data); });

    data.conn = conn;
}

function joinGame(stuff: any, data: GameData) {
}
