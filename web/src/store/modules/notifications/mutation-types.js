export const SET_REGISTRATION = 'SET_REGISTRATION';
export const SET_WAITING = 'SET_WAITING';
export const SET_SETTINGS_ENABLED = 'SET_SETTINGS_ENABLED';

export const initialState = () => ({
  isWaiting: false,
  isSettingsEnabled: false,
  registered: false,
});
