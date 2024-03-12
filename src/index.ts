import * as signalR from "@microsoft/signalr";
import { Move, UltimateBoard } from "./game";

const conn = new signalR.HubConnectionBuilder()
    .withUrl("/uttt")
    .build();

conn.start()
    .then(() => console.log("Established connection."))
    .catch(err => console.error("Connection failed: ", err));

function sendMove(move: Move) {
    const json = JSON.stringify(move);

    conn.send("submitMove", json)
        .then(() => console.log("Sent successfully"))
        .catch(err => console.error("Sent unsuccessfully: ", err));
}

const board_element = document.getElementById('ultimate-board');
const ultimate = new UltimateBoard(1, board_element);
ultimate.addListener(sendMove);

function recieveMove(json: string) {
    const move: Move = JSON.parse(json);

    ultimate.playMove(move);
}

conn.on("announceMove", recieveMove);
