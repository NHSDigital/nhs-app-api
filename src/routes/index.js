import More from '@/components/More';
import Prescriptions from '@/components/Prescriptions';
import RepeatPrescriptionCourses from '@/components/RepeatPrescriptionCourses';
import Appointments from '@/components/appointments/Appointments';
import AppointmentConfirmation from '@/components/appointments/AppointmentConfirmation';
import AuthReturn from '@/components/AuthReturn';
import HomeLoggedOut from '@/components/HomeLoggedOut';
import HomeLoggedIn from '@/components/HomeLoggedIn';
import store from '@/store';

export default [
  {
    path: '/',
    name: 'home.index',
    meta: {
      auth: true,
      headerKey: 'pageHeaderTitles.home',
    },
    component: HomeLoggedIn,
    beforeEnter: (to, from, next) => {
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      next();
    },
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
    path: '/appointment-confirmation',
    name: 'appointmentConfirmation',
    component: AppointmentConfirmation,
    meta: {
      auth: true,
      headerKey: 'pageHeaderTitles.appointmentConfirmation',
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
    path: '/repeat-prescription-courses',
    name: 'repeatPrescriptionCourses',
    component: RepeatPrescriptionCourses,
    meta: {
      auth: true,
      headerKey: 'pageHeaderTitles.repeatPrescriptionCourses',
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
