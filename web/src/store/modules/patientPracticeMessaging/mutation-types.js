export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const SET_SUMMARIES = 'SET_SUMMARIES';

export const initialState = () => ({
  loaded: false,
  messageSummaries: [],
});
