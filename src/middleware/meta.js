/* eslint-disable no-param-reassign */
const PRESCRIPTIONS = 'prescriptions';
const APPOINTMENTS = 'appointments';
const INDEX = 'index';
const MORE = 'more';
const REPEAT_PRESCRIPTION_COURSES = 'repeat-prescription-courses';
const APPOINTMENT_CONFIRMATIONS = 'appointment-confirmation';
const ACCOUNT = 'account';

export default function ({ route, store, app }) {

  switch (route.name) {
    case INDEX:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaderTitles.home';
      break;
    case APPOINTMENTS:
      route.meta.headerKey = 'pageHeaderTitles.appointments';
      break;
    case MORE:
      route.meta.headerKey = 'pageHeaderTitles.more';
      break;
    case REPEAT_PRESCRIPTION_COURSES:
      route.meta.headerKey = 'pageHeaderTitles.repeatPrescriptionCourses';
      break;
    case PRESCRIPTIONS:
      route.meta.headerKey = 'pageHeaderTitles.prescriptions';
      break;
    case APPOINTMENT_CONFIRMATIONS:
      route.meta.headerKey = 'pageHeaderTitles.appointmentConfirmation';
      break;
    case ACCOUNT:
      route.meta.headerKey = 'pageHeaderTitles.account';
      break;
    case 'login':
      route.meta.headerKey = '';
      break;
    default:
      route.meta.headerKey = '';
      break;
  }

  store.dispatch('http/clearApiErrorResponse');

  if (process.client) {
    if (route.meta.headerKey === '') {
      store.dispatch('header/updateHeaderText', '');
    } else {
      const headerText = app.i18n.tc(route.meta.headerKey);
      store.dispatch('header/updateHeaderText', headerText);
    }
  }
}
