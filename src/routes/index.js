import More from '@/components/More';
import Prescriptions from '@/components/Prescriptions';
import Appointments from '@/components/appointments/Appointments';
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
    path: '/prescriptions',
    name: 'prescriptions',
    component: Prescriptions,
    meta: {
      auth: true,
      headerKey: 'pageHeaderTitles.prescriptions',
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
