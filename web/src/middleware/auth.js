/* eslint-disable no-param-reassign */
import Routes from '../Routes';

export default function ({ store, redirect, route, res }) {
  const excludedRoutes = [
    Routes.LOGIN.name,
    Routes.AUTH_RETURN.name,
    Routes.CHECKYOURSYMPTOMS.name,
  ];

  // debugger;
  const storeCookie = store.state.session.cookie;
  debugger;
  // // add it to axios headers
  if (storeCookie && process.server) {
    debugger;
    res.setHeader('Set-Cookie', [storeCookie]);
  }

  const hasNotLoggedUserAccess = excludedRoutes.indexOf(route.name) !== -1;

  if (!hasNotLoggedUserAccess && !store.state.auth.loggedIn) {
    debugger;
    redirect('/login');
  }
}
