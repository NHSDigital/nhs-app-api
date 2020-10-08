export const SET_REGISTRATION = 'SET_REGISTRATION';
export const SET_WAITING = 'SET_WAITING';
export const SET_NOTIFICATION_COOKIE_EXISTS = 'SET_NOTIFICATION_COOKIE_EXISTS';

export const initialState = () => ({
  isWaiting: false,
  registered: false,
  notificationCookieExists: false,
});
