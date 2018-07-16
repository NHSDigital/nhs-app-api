/* eslint-disable no-param-reassign */
import Routes from '../Routes';

// eslint-disable-next-line object-curly-newline
export default function ({ store, redirect, route, res }) {
  const excludedRoutes = [
    Routes.LOGIN.name,
    Routes.AUTH_RETURN.name,
    Routes.CHECKYOURSYMPTOMS.name,
  ];

  const storeCookie = store.state.session.cookie;
  // add it to axios headers
  if (storeCookie && process.server) {
    res.setHeader('' +
      'Set-Cookie', [storeCookie]);
  }

  const hasNotLoggedUserAccess = excludedRoutes.indexOf(route.name) !== -1;

  if (!hasNotLoggedUserAccess && !store.state.auth.loggedIn) {
    redirect('/login');
  }
}
