import {
  SHOULD_BYPASS_ROUTE_GUARD,
  SHOW_LEAVING_PAGE_WARNING,
  SET_ATTEMPTED_REDIRECT_ROUTE,
  RESET,
} from './mutation-types';

export default {
  [SHOW_LEAVING_PAGE_WARNING](state) {
    state.showLeavingWarning = true;
  },
  [SET_ATTEMPTED_REDIRECT_ROUTE](state, route) {
    state.attemptedRedirectRoute = route;
  },
  [RESET](state) {
    state.showLeavingWarning = false;
    state.attemptedRedirectRoute = undefined;
    state.shouldSkipDisplayingLeavingWarning = undefined;
  },
  [SHOULD_BYPASS_ROUTE_GUARD](state, shouldSkipModal) {
    state.shouldSkipDisplayingLeavingWarning = shouldSkipModal;
  },
};
