import Vue from 'vue';
import VueRouter from 'vue-router';

import generalRoutes from '@/router/routes/general';
import loginRoutes from '@/router/routes/login';
import getHealthAdviceRoutes from '@/router/routes/get-health-advice';
import appointmentsRoutes from '@/router/routes/appointments';
import prescriptionRoutes from '@/router/routes/prescriptions';
import adviceRoutes from '@/router/routes/advice';
import accountRoutes from '@/router/routes/account';
import moreRoutes from '@/router/routes/more';
import nominatedPharmacyRoutes from '@/router/routes/nominated-pharmacy';
import messagesRoutes from '@/router/routes/messages';
import medicalRecordRoutes from '@/router/routes/medical-record';
import linkedProfilesRoutes from '@/router/routes/linked-profiles';
import logoutRoute from '@/router/routes/logout';
import organDonationRoutes from '@/router/routes/organ-donation';
import silverIntegrationRoutes from '@/router/routes/silver-integration';
import store from '@/store';

import middlewarePipeline from '@/router/middlewarePipeline';
import authCookieConfig from '@/middleware/authCookieConfig';
import appConfig from '@/middleware/appConfig';
import termsAndConditions from '@/middleware/termsAndConditions';
import auth from '@/middleware/auth';
import knownServices from '@/middleware/knownServices';
import nativeNavigation from '@/middleware/nativeNavigation';
import globalResets from '@/middleware/globalResets';
import onlineConsultations from '@/middleware/onlineConsultations';
import session from '@/middleware/session';
import conditionalRedirect from '@/middleware/conditionalRedirect';
import setSource from '@/middleware/setSource';
import sjrRedirect from '@/middleware/sjrRedirect';
import upliftRedirect from '@/middleware/upliftRedirect';

import configureAnalytics from '@/services/analytics-service';
import { resetPageFocus } from '@/lib/utils';

import NhsukLayout from '@/layouts/nhsuk-layout';

import { INDEX_PATH } from '@/router/paths';

const routerPush = VueRouter.prototype.push;
VueRouter.prototype.push = function push(location) {
  return routerPush.call(this, location).catch((error) => {
    if (error || !error) {
      // router push cancelled
    }
  });
};

const globalMiddleware = [
  setSource,
  authCookieConfig,
  appConfig,
  auth,
  termsAndConditions,
  session,
  nativeNavigation,
  globalResets,
  knownServices,
  onlineConsultations,
  conditionalRedirect,
  sjrRedirect,
  upliftRedirect,
];

Vue.use(VueRouter);

export const allRoutes = [
  ...loginRoutes,
  ...getHealthAdviceRoutes,
  {
    name: '',
    path: INDEX_PATH,
    component: NhsukLayout,
    children: [
      ...logoutRoute,
      ...adviceRoutes,
      ...moreRoutes,
      ...appointmentsRoutes,
      ...messagesRoutes,
      ...nominatedPharmacyRoutes,
      ...prescriptionRoutes,
      ...medicalRecordRoutes,
      ...accountRoutes,
      ...linkedProfilesRoutes,
      ...organDonationRoutes,
      ...silverIntegrationRoutes,
      // Route matching happens in order, generalRoutes should
      // be last so to not match before all other routes have been checked,
      // as it contains a catch-all NOT_FOUND route
      ...generalRoutes,
    ],
  },
];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: allRoutes,
});

router.beforeEach((to, from, next) => {
  const context = {
    to,
    from,
    next,
    store,
  };

  let specificRouteMiddleware = [].concat(globalMiddleware);

  if (to.meta.middleware && to.meta.middleware.length > 0) {
    const { middleware } = to.meta;
    specificRouteMiddleware = specificRouteMiddleware.concat(middleware);
  }

  return middlewarePipeline(context, specificRouteMiddleware, 0, next);
});

router.afterEach((to) => {
  resetPageFocus(store);

  Vue.nextTick(() => {
    configureAnalytics(store.app, store, to, router);
  });
});

export const isAnonymous = route => (route.meta && route.meta.isAnonymous);

export default router;
