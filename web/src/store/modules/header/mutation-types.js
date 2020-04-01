export const UPDATE_HEADER_TEXT = 'UPDATE_HEADER_TEXT';
export const UPDATE_HEADER_CAPTION = 'UPDATE_HEADER_CAPTION';
export const INIT_HEADER = 'INIT_HEADER';
export const TOGGLE_MINI_MENU = 'TOGGLE_MINI_MENU';
export const CLOSE_MINI_MENU = 'CLOSE_MINI_MENU';
export const initialState = () => ({
  headerText: '',
  headerCaption: '',
  miniMenuExpanded: false,
});
