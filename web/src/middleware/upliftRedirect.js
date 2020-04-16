import { conditionalRedirector } from '@/middleware/conditionalRedirect';
import { findByName } from '@/lib/routes';

export default ({ redirect, route, store }) => {
  const routeDetail = findByName(route.name);

  if (routeDetail && routeDetail.proofLevel && routeDetail.upliftPath) {
    conditionalRedirector({
      redirect,
      path: routeDetail.path,
      redirectRules: [{
        condition: 'session/shouldUplift',
        url: routeDetail.upliftPath,
        context: routeDetail.proofLevel,
      }],
      store,
    });
  }
};
