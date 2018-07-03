/* eslint-disable no-param-reassign */
import Routes from '../Routes';

export default function ({ store, redirect, route }) {
  const excludedRoutes = [
    Routes.LOGIN.name,
    Routes.AUTH_RETURN.name,
  ];

  const hasNotLoggedUserAccess = excludedRoutes.indexOf(route.name) !== -1;

  if (!hasNotLoggedUserAccess && !store.state.auth.loggedIn) {
    redirect('/login');
  }
}
