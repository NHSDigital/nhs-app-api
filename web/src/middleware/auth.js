import AuthorisationService from '@/services/authorisation-service';
import get from 'lodash/fp/get';
import {
  BEGINLOGIN_NAME,
  REDIRECT_PARAMETER,
  INDEX_NAME,
  INTERSTITIAL_REDIRECTOR_NAME,
  LOGIN_NAME,
} from '@/router/names';
import { EMPTY_PATH, INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';
import { isAnonymous } from '@/router';
import { createRouteByNameObject, createRoutePathObject, pathWithPatientPrefixOrUndefined } from '@/lib/utils';

const trustedReferrers = ['NHS_UK'].map(name => name.toUpperCase());

const logTrustedReferrer = async (store, sanitizedPath, to) => {
  if (sanitizedPath) {
    const destination = sanitizedPath === '/patient/' ? EMPTY_PATH : sanitizedPath;
    await store.dispatch(
      'log/onInfo',
      `Skipping logged out home page - referrer is ${to.query.referrer}, destination is ${destination}`,
    );
  } else {
    await store.dispatch(
      'log/onInfo',
      `Skipping logged out home page - referrer is ${to.query.referrer}, destination is invalid ${to.path}`,
    );
  }
};

export default async ({ router, store, to, next }) => {
  const isLoggedIn = store.getters['session/isLoggedIn']();
  const santizedPath = pathWithPatientPrefixOrUndefined({ path: to.path, store, router });

  if (!isAnonymous(to) && !isLoggedIn) {
    if (store.$env.SKIP_LOGGED_OUT_ENABLED) {
      if (to.query.referrer && trustedReferrers.includes(to.query.referrer.toUpperCase())) {
        await logTrustedReferrer(store, santizedPath, to);
        const authorisationService = new AuthorisationService(store.$env);
        const { loginUrl } = authorisationService.generateLoginUrl({
          redirectTo: santizedPath || EMPTY_PATH,
          cookies: store.$cookies,
          singleSignOnDetails: {
            assertedLoginIdentity: to.query.assertedLoginIdentity,
            prompt: 'none',
          },
        });
        window.location.href = loginUrl;
        return;
      }
    }

    if (to.path === EMPTY_PATH || !santizedPath) {
      next({ name: LOGIN_NAME });
      return;
    }

    let queryParam = to.name;
    let navigateToRedirector = to.name === INTERSTITIAL_REDIRECTOR_NAME;
    if (!to.name) {
      queryParam = santizedPath;
      navigateToRedirector = santizedPath === `/patient/${INTERSTITIAL_REDIRECTOR_PATH}`;
    }

    const query = { [REDIRECT_PARAMETER]: queryParam };
    if (navigateToRedirector) {
      const redirectParam = get(REDIRECT_PARAMETER)(to.query);
      if (redirectParam) {
        query[REDIRECT_PARAMETER] = redirectParam;
      } else {
        next({ name: LOGIN_NAME });
        return;
      }
    }
    next({ name: LOGIN_NAME, query });
    return;
  }

  if (isLoggedIn) {
    if (to.name === LOGIN_NAME || (to.name !== INDEX_NAME && to.path === EMPTY_PATH)) {
      next(createRouteByNameObject({
        name: INDEX_NAME,
        query: to.query,
        params: to.params,
        store,
      }));
      return;
    }

    if (to.matched.length === 0 && santizedPath) {
      next(createRoutePathObject({
        path: to.path,
        query: to.query,
        params: to.params,
        store,
      }));
      return;
    }
  }

  if (to.name === BEGINLOGIN_NAME) {
    const authorisationService = new AuthorisationService(store.$env);
    const { loginUrl } = authorisationService.generateLoginUrl({
      redirectTo: to.query[REDIRECT_PARAMETER],
      cookies: store.$cookies,
    });
    window.location.href = loginUrl;
    next(false);
    return;
  }
  next();
};
