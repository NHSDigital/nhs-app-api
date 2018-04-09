import Vue from 'vue';
import Router from 'vue-router';
import More from '@/components/More';
import AuthReturn from '@/components/AuthReturn';
import { requireAuth, isLoggedIn } from '@/services/authorization-service';
import HomeLoggedOut from '@/components/HomeLoggedOut';
import HomeLoggedIn from '@/components/HomeLoggedIn';

Vue.use(Router);

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      name: 'home',
      component: {
        functional: true,
        render(h) {
          return h('div', [isLoggedIn() ? h(HomeLoggedIn) : h(HomeLoggedOut)]);
        },
        components: {
          HomeLoggedOut,
          HomeLoggedIn,
        },
      },
    }, {
      path: '/more',
      name: 'more',
      component: More,
      beforeEnter: requireAuth,
    }, {
      path: '/auth-return',
      name: 'authReturn',
      component: AuthReturn,
    },
  ],
});
