/* eslint-disable no-param-reassign */
/* eslint-disable import/extensions */
import AuthorisationService from '@/services/authorisation-service';
import { isAnonymous, BEGINLOGIN, LOGIN } from '@/lib/routes';

export default function ({ app, store, redirect, route }) {
  const isLoggedIn = store.getters['session/isLoggedIn']();

  if (!isAnonymous(route.name) && !isLoggedIn) {
    return redirect('/login');
  }

  if (route.name === LOGIN.name && isLoggedIn) {
    return redirect('/');
  }

  if (route.name === BEGINLOGIN.name) {
    const authorisationService = new AuthorisationService(app.$env);
    const loginValues = authorisationService.generateLoginValues(
      route.query.source,
      store.$cookies,
    );

    return redirect(`${loginValues.authoriseUrl}?scope=${loginValues.scope}&client_id=nhs-online&code_challenge=${loginValues.codeChallenge}&code_challenge_method=${loginValues.codeChallengeMethod}&redirect_uri=${encodeURIComponent(loginValues.redirectUri)}&state=${loginValues.state}&response_type=${loginValues.responseType}`);
  }

  return undefined;
}
