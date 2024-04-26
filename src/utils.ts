export function fatalError(err: any) {
    window.location.assign("game/gameerror?msg=" + err);
}
