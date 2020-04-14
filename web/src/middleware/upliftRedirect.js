import { conditionalRedirector } from '@/middleware/conditionalRedirect';
import { findByName } from '@/lib/routes';

export default ({ redirect, route, store }) => {
  const routeDetail = findByName(route.name);

  if (routeDetail && routeDetail.upliftPath) {
    conditionalRedirector({
      redirect,
      path: routeDetail.path,
      redirectRules: [{
        condition: 'session/isP9User',
        value: false,
        url: routeDetail.upliftPath,
      }],
      store,
    });
  }
};
