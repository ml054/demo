import { deepCopy } from "../../utils/miscUtils";
import { RouterState } from "connected-react-router";
import { ErrorState } from "./error";
import { DemoState } from "./demo";
import { ParametersState } from "./parameters";
import { SettingsState } from "./settings";

export interface AppState {
    router: RouterState;
    error: ErrorState;
    demos: DemoState;
    params: ParametersState;
    settings: SettingsState;
}

export function modifyState<S>(state: S, modify: (state: S) => void): S {
    const stateCopy = deepCopy(state);
    modify(stateCopy);
    return stateCopy;
}
