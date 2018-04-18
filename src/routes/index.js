import More from '@/components/More';
import Appointments from '@/components/Appointments';
import AuthReturn from '@/components/AuthReturn';
import HomeLoggedOut from '@/components/HomeLoggedOut';
import HomeLoggedIn from '@/components/HomeLoggedIn';

export default [
  {
    path: '/',
    name: 'home.index',
    meta: {
      auth: true,
      headerKey: 'pageHeaderTitles.home',
    },
    component: HomeLoggedIn,
  },
  {
    path: '/login',
    name: 'login.index',
    component: HomeLoggedOut,
    meta: {
      guest: true,
    },
  }, {
    path: '/appointments',
    name: 'appointments',
    component: Appointments,
    meta: {
      auth: true,
      headerKey: 'pageHeaderTitles.appointments',
    },
  }, {
    path: '/more',
    name: 'more',
    component: More,
    meta: {
      auth: true,
      headerKey: 'pageHeaderTitles.more',
    },
  }, {
    path: '/auth-return',
    name: 'authReturn',
    component: AuthReturn,
    meta: {
      guest: true,
    },
  },
];
