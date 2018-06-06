/* eslint-disable no-param-reassign */
const PRESCRIPTIONS = 'prescriptions';
const APPOINTMENTS = 'appointments';
const INDEX = 'index';
const MORE = 'more';
const REPEAT_PRESCRIPTION_COURSES = 'prescriptions-repeat-courses';
const CONFIRM_COURSES = 'prescriptions-confirm-prescription-details';
const APPOINTMENT_CONFIRMATIONS = 'appointments-confirmation';
const APPOINTMENT_BOOKED = 'appointments-booked';
const ACCOUNT = 'account';
const MYRECORDWARNING = 'my-record-myrecordwarning';
const MYRECORD = 'my-record';
const MYRECORDNOACCESS = 'my-record-noaccess';
const LOGIN = 'login';

export default function ({ route, store, app }) {
  switch (route.name) {
    case INDEX:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaderTitles.home';
      break;
    case APPOINTMENTS:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaderTitles.appointments';
      break;
    case MORE:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaderTitles.more';
      break;
    case REPEAT_PRESCRIPTION_COURSES:
      route.meta.headerKey = 'pageHeaderTitles.repeatPrescriptionCourses';
      break;
    case PRESCRIPTIONS:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaderTitles.prescriptions';
      break;
    case CONFIRM_COURSES:
      route.meta.headerKey = 'pageHeaderTitles.confirmPrescription';
      break;
    case APPOINTMENT_CONFIRMATIONS:
      route.meta.headerKey = 'pageHeaderTitles.appointmentConfirmation';
      break;
    case APPOINTMENT_BOOKED:
      route.meta.headerKey = 'pageHeaderTitles.appointmentBooked';
      break;
    case ACCOUNT:
      route.meta.headerKey = 'pageHeaderTitles.account';
      break;
    case MYRECORD:
    case MYRECORDNOACCESS:
    case MYRECORDWARNING:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaderTitles.myRecord';
      break;
    case LOGIN:
      route.meta.headerKey = '';
      break;
    default:
      route.meta.headerKey = '';
      break;
  }

  store.dispatch('http/clearApiErrorResponse');

  if (process.client) {
    let headerText = '';

    if (route.meta.headerKey !== '') {
      headerText = app.i18n.tc(route.meta.headerKey);
    }

    store.dispatch('header/updateHeaderText', headerText);
  }
}
