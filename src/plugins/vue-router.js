import Vue from 'vue';
import VueRouter from 'vue-router';
import routes from '@/routes';
import store from '@/store';
import { i18n } from './vue-i18n';

Vue.use(VueRouter);

export const router = new VueRouter({
  mode: 'history',
  routes,
});

router.beforeEach((to, from, next) => {
  let nextPageHeaderKey = '';

  if (to.matched.some(m => m.meta.auth) && !store.state.auth.loggedIn) {
    next({
      name: 'login.index',
    });
  } else if (to.matched.some(m => m.meta.guest) && store.state.auth.loggedIn) {
    next({
      name: 'home.index',
    });
  } else {
    next();
    if (to.matched.some(m => m.meta.headerKey)) {
      nextPageHeaderKey = to.meta.headerKey;
    }
  }

  const headerText = i18n.tc(nextPageHeaderKey);
  store.dispatch('header/updateHeaderText', headerText);
});

Vue.router = router;

export default {
  router,
};
