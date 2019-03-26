/* eslint-disable no-param-reassign */
/* eslint-disable import/extensions */
import AuthorisationService from '@/services/authorisation-service';
import { isAnonymous, BEGINLOGIN, LOGIN, INDEX } from '@/lib/routes';

export default function ({ app, store, redirect, route }) {
  const isLoggedIn = store.getters['session/isLoggedIn']();

  if (!isAnonymous(route.name) && !isLoggedIn) {
    return redirect(LOGIN.path);
  }

  if (route.name === LOGIN.name && isLoggedIn) {
    return redirect(INDEX.path);
  }

  if (route.name === BEGINLOGIN.name) {
    const authorisationService = new AuthorisationService(app.$env);
    const { loginUrl } = authorisationService.generateLoginUrl({
      source: route.query.source,
      cookies: store.$cookies,
    });

    return redirect(loginUrl);
  }

  return undefined;
}
