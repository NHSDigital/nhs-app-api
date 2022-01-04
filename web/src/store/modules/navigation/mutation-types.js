export const CLEAR_SELECTED_MENUITEM = 'CLEAR_SELECTED_MENUITEM';
export const SET_NEWMENUITEM = 'SET_NEWMENUITEM';
export const INIT_NAVIGATION = 'INIT_NAVIGATION';
export const SET_BACK_LINK_OVERRIDE = 'SET_BACK_LINK_OVERRIDE';
export const SET_ROUTE_CRUMB = 'SET_ROUTE_CRUMB';
export const initialState = () => ({
  menuItemStatusAt: [],
  crumbSetName: 'defaultCrumb',
});

