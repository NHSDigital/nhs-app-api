export const ADD = 'ADD';
export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const VALIDATE = 'VALIDATE';
export const HAS_BEEN_SHOWN = 'HAS_BEEN_SHOWN';
export const initialState = () => ({
  message: '',
  hasBeenShown: false,
  show: false,
  type: 'success',
  key: '',
});
