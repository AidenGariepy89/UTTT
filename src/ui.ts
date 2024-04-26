function backTopBar(): HTMLDivElement {
    const topbar = document.createElement("div");
    topbar.id = "topbar";

    const backButton = document.createElement("a");
    backButton.href = "/";
    backButton.innerHTML = "Back";

    topbar.appendChild(backButton);

    return topbar;
}

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

    const topbar = backTopBar();

    const container = document.createElement("div");
    container.id = "container";
    container.appendChild(topbar);
    container.appendChild(info);

    return container;
}

export function ultimateBoard(): HTMLDivElement {
    const boardContainer = document.createElement("div");
    boardContainer.id = "board-container";

    const infoText = document.createElement("h2");
    infoText.id = "info-text";
    infoText.innerHTML = "[placeholder]";

    const ultimateBoard = document.createElement("div");
    ultimateBoard.id = "ultimate-board";

    boardContainer.appendChild(infoText);
    boardContainer.appendChild(ultimateBoard);

    const topbar = backTopBar();

    const container = document.createElement("div");
    container.id = "container";
    container.appendChild(topbar);
    container.appendChild(boardContainer);

    return container;
}
