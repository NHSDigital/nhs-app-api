/* eslint-disable no-param-reassign */
import Routes from '../Routes';

function setPageTitle(route, store, app) {
  let header = '';

  if (route.meta.headerKey !== '') {
    header = app.i18n.tc(route.meta.headerKey);
  }

  store.dispatch('header/updateHeaderText', header);
}

export default function ({ route, store, app }) {
  switch (route.name) {
    case Routes.INDEX.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaderTitles.home';
      break;
    case Routes.LOGIN.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = '';
      break;
    case Routes.ACCOUNT.name:
      route.meta.headerKey = 'pageHeaderTitles.account';
      break;
    case Routes.APPOINTMENTS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointments';
      break;
    case Routes.APPOINTMENT_BOOKING_GUIDANCE.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointmentGuidance';
      break;
    case Routes.APPOINTMENT_CANCELLING.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointmentCancelling';
      break;
    case Routes.APPOINTMENT_BOOKING.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointmentBooking';
      break;
    case Routes.APPOINTMENT_CONFIRMATIONS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointmentConfirmation';
      break;
    case Routes.PRESCRIPTIONS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaderTitles.prescriptions';
      break;
    case Routes.PRESCRIPTION_REPEAT_COURSES.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaderTitles.repeatPrescriptionCourses';
      break;
    case Routes.PRESCRIPTION_CONFIRM_COURSES.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaderTitles.confirmPrescription';
      break;
    case Routes.MYRECORD.name:
    case Routes.MYRECORDNOACCESS.name:
    case Routes.MYRECORDWARNING.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaderTitles.myRecord';
      break;
    case Routes.MORE.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaderTitles.more';
      break;
    default:
      route.meta.headerKey = '';
      break;
  }

  store.dispatch('http/cancelRequests');
  store.dispatch('errors/setRoutePath', route.path);

  setPageTitle(route, store, app);
}
