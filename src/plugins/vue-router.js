import Vue from 'vue';
import VueRouter from 'vue-router';
import routes from '@/routes';
import store from '@/store';

Vue.use(VueRouter);

export const router = new VueRouter({
  mode: 'history',
  routes,
});

router.beforeEach((to, from, next) => {
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
  }
});

Vue.router = router;

export default {
  router,
};
