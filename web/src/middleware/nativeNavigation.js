export const CLEAR_SELECTED_MENU_ITEM = 'CLEAR_SELECTED_MENU_ITEM';
export const ADVICE_MENU_ITEM = 0;
export const APPOINTMENTS_MENU_ITEM = 1;
export const PRESCRIPTIONS_MENU_ITEM = 2;
export const YOUR_RECORD_MENU_ITEM = 3;
export const MESSAGES_MENU_ITEM = 4;

export default ({ to, store, next }) => {
  const { meta } = to;
  if (meta.nativeNavigation) {
    const { nativeNavigation } = meta;

    if (nativeNavigation === CLEAR_SELECTED_MENU_ITEM) {
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
    } else {
      store.dispatch('navigation/setNewMenuItem', nativeNavigation);
    }
  }

  return next();
};
