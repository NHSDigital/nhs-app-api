export const SHOULD_BYPASS_ROUTE_GUARD = 'SHOULD_BYPASS_ROUTE_GUARD';
export const SHOW_LEAVING_PAGE_WARNING = 'SHOW_LEAVING_PAGE_WARNING';
export const SET_ATTEMPTED_REDIRECT_ROUTE = 'SET_ATTEMPTED_REDIRECT_ROUTE';
export const RESET = 'RESET';

export const initialState = () => ({
  showLeavingWarning: false,
  attemptedRedirectRoute: undefined,
  shouldSkipDisplayingLeavingWarning: undefined,
});
