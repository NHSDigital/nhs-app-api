/* eslint-disable no-param-reassign */
/* eslint-disable import/extensions */
import {
  ACCOUNT,
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CONFIRMATIONS,
  CHECKYOURSYMPTOMS,
  GP_FINDER,
  GP_FINDER_RESULTS,
  DATA_SHARING_PREFERENCES,
  INDEX,
  LOGIN,
  BEGINLOGIN,
  MORE,
  MYRECORD,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  MYRECORDWARNING,
  PRESCRIPTIONS,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTION_REPEAT_COURSES,
  SYMPTOMS,
  TERMSANDCONDITIONS,
} from '@/lib/routes';

function setPageTitle(route, store, app) {
  let header = '';
  let title = '';

  if (route.meta.headerKey !== '') {
    header = app.i18n.tc(route.meta.headerKey);
  }
  if (route.meta.title !== '') {
    title = app.i18n.tc(route.meta.pageTitleKey);
  }

  store.dispatch('header/updateHeaderText', header);
  store.dispatch('pageTitle/updatePageTitle', title);
}

export default function ({ route, store, app }) {
  switch (route.name) {
    case INDEX.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.home';
      route.meta.pageTitleKey = 'pageTitles.home';
      break;
    case LOGIN.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = '';
      route.meta.pageTitleKey = '';
      break;
    case BEGINLOGIN.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = '';
      route.meta.pageTitleKey = '';
      break;
    case ACCOUNT.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.account';
      route.meta.pageTitleKey = 'pageTitles.account';
      break;
    case GP_FINDER.name:
    case GP_FINDER_RESULTS.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.gpFinder';
      route.meta.pageTitleKey = 'pageTitles.gpFinder';
      break;
    case SYMPTOMS.name:
    case CHECKYOURSYMPTOMS.name:
      store.dispatch('navigation/setNewMenuItem', 0);
      route.meta.headerKey = 'pageHeaders.symptoms';
      route.meta.pageTitleKey = 'pageTitles.symptoms';
      break;
    case APPOINTMENTS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointments';
      route.meta.pageTitleKey = 'pageTitles.appointments';
      break;
    case APPOINTMENT_BOOKING_GUIDANCE.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentGuidance';
      route.meta.pageTitleKey = 'pageTitles.appointmentGuidance';
      break;
    case APPOINTMENT_CANCELLING.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentCancelling';
      route.meta.pageTitleKey = 'pageTitles.appointmentCancelling';
      break;
    case APPOINTMENT_BOOKING.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentBooking';
      route.meta.pageTitleKey = 'pageTitles.appointmentBooking';
      break;
    case APPOINTMENT_CONFIRMATIONS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentConfirmation';
      route.meta.pageTitleKey = 'pageTitles.appointmentConfirmation';
      break;
    case PRESCRIPTIONS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.prescriptions';
      route.meta.pageTitleKey = 'pageTitles.prescriptions';
      break;
    case PRESCRIPTION_REPEAT_COURSES.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.repeatPrescriptionCourses';
      route.meta.pageTitleKey = 'pageTitles.repeatPrescriptionCourses';
      break;
    case PRESCRIPTION_CONFIRM_COURSES.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.confirmPrescription';
      route.meta.pageTitleKey = 'pageTitles.confirmPrescription';
      break;
    case MYRECORDWARNING.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.myRecordWarning';
      route.meta.pageTitleKey = 'pageTitles.myRecordWarning';
      break;
    case MYRECORD.name:
    case MYRECORDNOACCESS.name:
    case MYRECORDTESTRESULT.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.myRecord';
      route.meta.pageTitleKey = 'pageTitles.myRecord';
      break;
    case MORE.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.more';
      route.meta.pageTitleKey = 'pageTitles.more';
      break;
    case DATA_SHARING_PREFERENCES.name:
      route.meta.headerKey = 'pageHeaders.dataSharing';
      route.meta.pageTitleKey = 'pageTitles.dataSharing';
      break;
    case TERMSANDCONDITIONS.name:
      route.meta.headerKey = 'pageHeaders.termsAndConditions';
      route.meta.pageTitleKey = 'pageTitles.termsAndConditions';
      break;
    default:
      route.meta.headerKey = '';
      route.meta.pageTitleKey = '';
      break;
  }

  store.dispatch('http/cancelRequests');
  store.dispatch('flashMessage/validate');
  store.dispatch('errors/setRoutePath', route.path);

  setPageTitle(route, store, app);
}
