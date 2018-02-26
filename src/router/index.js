import Vue from 'vue';
import Router from 'vue-router';
import Home from '@/components/Home';
import More from '@/components/More';

Vue.use(Router);

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    }, {
      path: '/more',
      name: 'more',
      component: More,
    },
  ],
});
