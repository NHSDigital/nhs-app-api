/* eslint-disable no-param-reassign */
/* eslint-disable import/extensions */
import Routes from '@/Routes';

export default function ({ store, redirect, route }) {
  const excludedRoutes = [
    Routes.LOGIN.name,
    Routes.AUTH_RETURN.name,
    Routes.CHECKYOURSYMPTOMS.name,
    Routes.TERMSANDCONDITIONS.name,
  ];

  const hasNotLoggedUserAccess = excludedRoutes.indexOf(route.name) !== -1;
  if (!hasNotLoggedUserAccess && !store.getters['session/isLoggedIn']()) {
    redirect('/login');
  }
}
