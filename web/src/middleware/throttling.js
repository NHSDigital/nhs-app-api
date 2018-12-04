import {
  LOGIN,
  GP_FINDER,
  INDEX,
} from '@/lib/routes';

import moment from 'moment';

function hasCompletedThrottling(betaCookie) {
  if (betaCookie) {
    return (betaCookie.ODSCode && betaCookie.PracticeParticipating !== undefined &&
        betaCookie.PracticeName && betaCookie.PracticeAddress && betaCookie.Complete) ||
      betaCookie.Skipped;
  }
  return false;
}

export default function ({
  redirect,
  store,
  route,
  env,
  app,
}) {
  const isLoggedIn = store.getters['session/isLoggedIn']();

  if (isLoggedIn) {
    return route.path.startsWith(GP_FINDER.path) ? redirect(INDEX.path) : undefined;
  }

  let betaCookie = app.$cookies.get('BetaCookie');

  if (route.name === GP_FINDER.name) {
    if (route.query.reset) {
      betaCookie = {};
      app.$cookies.set('BetaCookie', betaCookie, {
        path: '/',
        maxAge: -1,
      });
    } else if (route.query.skip) {
      betaCookie = {
        Skipped: true,
      };
      app.$cookies.set('BetaCookie', betaCookie, {
        path: '/',
        maxAge: moment.duration(1, 'y').asSeconds(),
      });
    }
  }

  if (!env.THROTTLING_ENABLED || hasCompletedThrottling(betaCookie)) {
    return route.path.startsWith(GP_FINDER.path) ? redirect(LOGIN.path) : undefined;
  }

  if (betaCookie && betaCookie.PracticeParticipating !== undefined && !isLoggedIn) {
    if (betaCookie.PracticeParticipating === true) {
      return redirect(LOGIN.path);
    }

    return store.app.$http.getV1Odscodelookup({
      odsCode: betaCookie.ODSCode,
    }).then((response) => {
      betaCookie.PracticeParticipating = response && response.isGpSystemSupported;

      app.$cookies.set('BetaCookie', betaCookie, {
        path: '/',
        maxAge: moment.duration(1, 'y').asSeconds(),
      });

      return redirect(LOGIN.path);
    });
  }

  if (route.name === LOGIN.name) {
    return redirect(GP_FINDER.path);
  }

  return undefined;
}
