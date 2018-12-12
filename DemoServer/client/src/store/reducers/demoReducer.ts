import { modifyState } from "../state";
import { DemoAction } from "../actions/demoActions";
import { LocationChangeAction } from "connected-react-router";
import { matchDemoPath, matchDemoWithWalkthroughPath } from "../../utils/paths";
import { DemoState } from "../state/DemoState";
import { FilesCache } from "../../utils/FilesCache";

const initialState: DemoState = {
    language: "csharp",
    categorySlug: "",
    demoSlug: "",
    demo: null,
    finishedLoadingDemo: false,
    finishedSettingPrerequisites: false,
    showResultsPanel: false,
    loadingRunResults: false,
    parameters: [],
    attachmentNamesToUpload: [],
    runResults: null,
    showShareMessage: false
};

export function demoReducer(state: DemoState = initialState, action: DemoAction | LocationChangeAction): DemoState {
    switch (action.type) {
        case "DEMO_GET_METADATA_REQUEST":
            return modifyState(state, s => s.finishedLoadingDemo = false);

        case "DEMO_GET_METADATA_SUCCESS":
            return modifyState(state, s => {
                s.finishedLoadingDemo = true;
                s.demo = action.result
            });

        case "DEMO_GET_METADATA_FAILURE":
            return modifyState(state, s => {
                s.finishedLoadingDemo = true;
            });

        case "@@router/LOCATION_CHANGE":
            return modifyState(state, s => {
                const params = matchDemoWithWalkthroughPath(action) || matchDemoPath(action);
                if (params) {
                    s.categorySlug = params.category;
                    s.demoSlug = params.demo;
                    s.currentWalkthroughSlug = params.wtSlug;
                }
            });

        case "DEMO_SET_PREREQUISITES_REQUEST":
            return modifyState(state, s => s.finishedSettingPrerequisites = false);

        case "DEMO_SET_PREREQUISITES_FAILURE":
        case "DEMO_SET_PREREQUISITES_SUCCESS":
            return modifyState(state, s => s.finishedSettingPrerequisites = true);

        case "DEMO_RUN_REQUEST":
            return modifyState(state, s => {
                s.showResultsPanel = true;
                s.loadingRunResults = true;
            });

        case "DEMO_RUN_SUCCESS":
            return modifyState(state, s => {
                s.loadingRunResults = false;
                s.runResults = action.results;
            });

        case "DEMO_RUN_FAILURE":
            return modifyState(state, s => {
                s.loadingRunResults = false;
            });

        case "DEMO_HIDE_RESULTS":
            return modifyState(state, s => {
                s.showResultsPanel = false
            });

        case "DEMO_PARAMS_INIT":
            FilesCache.clear();
            return modifyState(state, s => {
                s.parameters = action.parameters;
                s.attachmentNamesToUpload = [];
            });

        case "DEMO_PARAMS_CHANGE":
            return modifyState(state, s => {
                s.parameters = s.parameters.map(x => x.name === action.name
                    ? { ...x, value: action.value }
                    : x);
            });

        case "DEMO_PARAMS_CHANGE_FILE":
            return modifyState(state, s => {
                const { name, file } = action;
                FilesCache.addOrUpdate(name, file);

                const alreadyStored = s.attachmentNamesToUpload.find(x => x === name);
                if (!alreadyStored) {
                    s.attachmentNamesToUpload.push(name);
                }
            });

        case "DEMO_TOGGLE_SHARE_MESSAGE":
            return modifyState(state, s => {
                s.showShareMessage = action.show;
            });
    }

    return state;
}