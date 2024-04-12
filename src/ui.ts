export function spinnerElement(): HTMLDivElement {
    const spinner = document.createElement("div");
    spinner.className = "lds-spinner";

    for (let i = 0; i < 12; i++) {
        spinner.appendChild(document.createElement("div"));
    }

    return spinner;
}

export function infoLoading(msg: string): HTMLDivElement {
    const infoMsg = document.createElement("p");
    infoMsg.id = "info-msg";
    infoMsg.innerHTML = msg;
    
    const info = document.createElement("div");
    info.id = "info";

    info.appendChild(infoMsg);
    info.appendChild(spinnerElement());

    return info;
}

export function ultimateBoard(): HTMLDivElement {
    const boardContainer = document.createElement("div");
    boardContainer.id = "board-container";

    const ultimateBoard = document.createElement("div");
    ultimateBoard.id = "ultimate-board";

    boardContainer.appendChild(ultimateBoard);

    return boardContainer;
}
