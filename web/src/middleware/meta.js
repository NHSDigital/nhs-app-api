/* eslint-disable no-param-reassign */
const INDEX = 'index';
const LOGIN = 'login';
const ACCOUNT = 'account';

const APPOINTMENTS = 'appointments';
const APPOINTMENT_BOOKING_GUIDANCE = 'appointments-booking-guidance';
const APPOINTMENT_BOOKING = 'appointments-booking';
const APPOINTMENT_CONFIRMATIONS = 'appointments-confirmation';

const PRESCRIPTIONS = 'prescriptions';
const REPEAT_PRESCRIPTION_COURSES = 'prescriptions-repeat-courses';
const CONFIRM_COURSES = 'prescriptions-confirm-prescription-details';

const MYRECORD = 'my-record';
const MYRECORDWARNING = 'my-record-myrecordwarning';
const MYRECORDNOACCESS = 'my-record-noaccess';
const MORE = 'more';

function setPageTitle(route, store, app) {
  let header = '';

  if (route.meta.headerKey !== '') {
    header = app.i18n.tc(route.meta.headerKey);
  }

  store.dispatch('header/updateHeaderText', header);
}

export default function ({ route, store, app }) {
  switch (route.name) {
    case INDEX:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaderTitles.home';
      break;
    case LOGIN:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = '';
      break;
    case ACCOUNT:
      route.meta.headerKey = 'pageHeaderTitles.account';
      break;
    case APPOINTMENTS:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointments';
      break;
    case APPOINTMENT_BOOKING_GUIDANCE:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointmentGuidance';
      break;
    case APPOINTMENT_BOOKING:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointmentBooking';
      break;
    case APPOINTMENT_CONFIRMATIONS:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointmentConfirmation';
      break;
    case PRESCRIPTIONS:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaderTitles.prescriptions';
      break;
    case REPEAT_PRESCRIPTION_COURSES:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaderTitles.repeatPrescriptionCourses';
      break;
    case CONFIRM_COURSES:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaderTitles.confirmPrescription';
      break;
    case MYRECORD:
    case MYRECORDNOACCESS:
    case MYRECORDWARNING:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaderTitles.myRecord';
      break;
    case MORE:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaderTitles.more';
      break;
    default:
      route.meta.headerKey = '';
      break;
  }

  store.dispatch('http/cancelRequests');
  store.dispatch('errors/clearAllApiErrors');

  setPageTitle(route, store, app);
}
