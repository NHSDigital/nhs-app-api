/* eslint-disable no-param-reassign */
/* eslint-disable import/extensions */
import {
  ACCOUNT,
  ACCOUNT_NOTIFICATIONS,
  ALLERGIESANDREACTIONS,
  ACUTE_MEDICINES,
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_GP_ADVICE,
  APPOINTMENT_GP_AT_HAND,
  APPOINTMENT_INFORMATICA,
  CHECKYOURSYMPTOMS,
  CONSULTATIONS,
  EVENTS,
  CURRENT_MEDICINES,
  DISCONTINUED_MEDICINES,
  GP_FINDER,
  GP_FINDER_RESULTS,
  GP_FINDER_PARTICIPATION,
  GP_FINDER_SENDING_EMAIL,
  GP_FINDER_WAITING_LIST_JOINED,
  DATA_SHARING_PREFERENCES,
  INDEX,
  LINKED_PROFILES,
  LINKED_PROFILES_SUMMARY,
  LOGIN,
  LOGOUT,
  BEGINLOGIN,
  MEDICINES,
  MESSAGING,
  MESSAGING_MESSAGES,
  MORE,
  MYRECORD,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  MY_RECORD_VISION_EXAMINATIONS_DETAIL,
  MY_RECORD_VISION_PROCEDURES_DETAIL,
  MY_RECORD_VISION_TEST_RESULTS_DETAIL,
  MY_RECORD_VISION_DIAGNOSIS_DETAIL,
  MY_RECORD_DOCUMENTS,
  MY_RECORD_DOCUMENT,
  MY_RECORD_DOCUMENT_DETAIL,
  MYRECORD_GP_AT_HAND,
  GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
  ORGAN_DONATION,
  ORGAN_DONATION_ADDITIONAL_DETAILS,
  ORGAN_DONATION_AMEND,
  ORGAN_DONATION_FAITH,
  ORGAN_DONATION_MORE_ABOUT_ORGANS,
  ORGAN_DONATION_REVIEW_YOUR_DECISION,
  ORGAN_DONATION_SOME_ORGANS,
  ORGAN_DONATION_VIEW_DECISION,
  ORGAN_DONATION_WITHDRAW_REASON,
  ORGAN_DONATION_WITHDRAWN,
  ORGAN_DONATION_YOUR_CHOICE,
  PRESCRIPTIONS,
  PRESCRIPTIONS_GP_AT_HAND,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS,
  RECALLS,
  REFERRALS,
  SYMPTOMS,
  SWITCH_PROFILE,
  TERMSANDCONDITIONS,
  TESTRESULTS,
  TESTRESULTSDETAIL,
  IMMUNISATIONS,
  HEALTH_CONDITIONS,
  TESTRESULTID,
  NOMINATED_PHARMACY,
  NOMINATED_PHARMACY_SEARCH,
  NOMINATED_PHARMACY_SEARCH_RESULTS,
  NOMINATED_PHARMACY_CONFIRM,
  NOMINATED_PHARMACY_CHECK,
  NOMINATED_PHARMACY_CANNOT_CHANGE,
} from '@/lib/routes';

import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';

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
  const isNative = $store =>
    ($store.state.device.source === 'android' || $store.state.device.source === 'ios');
  switch (route.name) {
    case INDEX.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.home';
      route.meta.pageTitleKey = 'pageTitles.home';
      break;
    case LOGIN.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.login';
      route.meta.pageTitleKey = 'pageTitles.login';
      break;
    case LOGOUT.name:
      route.meta.headerKey = '';
      route.meta.pageTitleKey = '';
      break;
    case BEGINLOGIN.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.login';
      route.meta.pageTitleKey = 'pageTitles.login';
      break;
    case ACCOUNT.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = isNative(store) ? 'pageHeaders.settings' : 'pageHeaders.account';
      route.meta.pageTitleKey = 'pageTitles.account';
      break;
    case ACCOUNT_NOTIFICATIONS.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.notifications';
      route.meta.pageTitleKey = 'pageTitles.notifications';
      break;
    case ACUTE_MEDICINES.name:
      route.meta.headerKey = 'pageHeaders.acuteMedicines';
      route.meta.pageTitleKey = 'pageTitles.acuteMedicines';
      break;
    case APPOINTMENT_ADMIN_HELP.name:
      route.meta.headerKey = 'pageHeaders.appointmentAdminHelp';
      route.meta.pageTitleKey = 'pageTitles.appointmentAdminHelp';
      break;
    case APPOINTMENT_GP_ADVICE.name:
      route.meta.headerKey = 'pageHeaders.appointmentGpAdvice';
      route.meta.pageTitleKey = 'pageTitles.appointmentGpAdvice';
      break;
    case CURRENT_MEDICINES.name:
      route.meta.headerKey = 'pageHeaders.currentMedicines';
      route.meta.pageTitleKey = 'pageTitles.currentMedicines';
      break;
    case DISCONTINUED_MEDICINES.name:
      route.meta.headerKey = 'pageHeaders.discontinuedMedicines';
      route.meta.pageTitleKey = 'pageTitles.discontinuedMedicines';
      break;
    case GP_FINDER.name:
    case GP_FINDER_RESULTS.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.gpFinder';
      route.meta.pageTitleKey = 'pageTitles.gpFinder';
      break;
    case GP_FINDER_PARTICIPATION.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.gpFinderParticipation';
      route.meta.pageTitleKey = 'pageTitles.gpFinderParticipation';
      break;
    case GP_FINDER_SENDING_EMAIL.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.gpFinderWaitingListSignup';
      route.meta.pageTitleKey = 'pageTitles.gpFinderWaitingListSignup';
      break;
    case GP_FINDER_WAITING_LIST_JOINED.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.gpFinderWaitingListJoined';
      route.meta.pageTitleKey = 'pageTitles.gpFinderWaitingListJoined';
      break;
    case SYMPTOMS.name:
    case CHECKYOURSYMPTOMS.name:
      store.dispatch('navigation/setNewMenuItem', 0);
      route.meta.headerKey = 'pageHeaders.symptoms';
      route.meta.pageTitleKey = 'pageTitles.symptoms';
      break;
    case CONSULTATIONS.name:
    case EVENTS.name:
      route.meta.headerKey = 'pageHeaders.consultationsAndEvents';
      route.meta.pageTitleKey = 'pageTitles.consultationsAndEvents';
      break;
    case APPOINTMENTS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointments';
      route.meta.pageTitleKey = 'pageTitles.appointments';
      break;
    case APPOINTMENT_BOOKING.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentBooking';
      route.meta.pageTitleKey = 'pageTitles.appointmentBooking';
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
    case APPOINTMENT_CONFIRMATIONS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentConfirmation';
      route.meta.pageTitleKey = 'pageTitles.appointmentConfirmation';
      break;
    case APPOINTMENT_GP_AT_HAND.name:
    case APPOINTMENT_INFORMATICA.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.serviceUnavailable';
      route.meta.pageTitleKey = 'pageTitles.serviceUnavailable';
      break;
    case LINKED_PROFILES.name:
      route.meta.headerKey = 'pageHeaders.linkedProfiles';
      route.meta.pageTitleKey = 'pageTitles.linkedProfiles';
      break;
    case SWITCH_PROFILE.name:
      route.meta.headerKey = 'pageHeaders.switchProfile';
      route.meta.pageTitleKey = 'pageTitles.switchProfile';
      break;
    case LINKED_PROFILES_SUMMARY.name:
      route.meta.headerKey = '';
      route.meta.pageTitleKey = '';
      break;
    case ORGAN_DONATION.name:
    case ORGAN_DONATION_ADDITIONAL_DETAILS.name:
    case ORGAN_DONATION_AMEND.name:
    case ORGAN_DONATION_FAITH.name:
    case ORGAN_DONATION_MORE_ABOUT_ORGANS.name:
    case ORGAN_DONATION_REVIEW_YOUR_DECISION.name:
    case ORGAN_DONATION_SOME_ORGANS.name:
    case ORGAN_DONATION_VIEW_DECISION.name:
    case ORGAN_DONATION_WITHDRAW_REASON.name:
    case ORGAN_DONATION_WITHDRAWN.name:
    case ORGAN_DONATION_YOUR_CHOICE.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.organDonation';
      route.meta.pageTitleKey = 'pageTitles.organDonation';
      break;
    case PRESCRIPTIONS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.prescriptions';
      route.meta.pageTitleKey = 'pageTitles.prescriptions';
      break;
    case PRESCRIPTIONS_GP_AT_HAND.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.serviceUnavailable';
      route.meta.pageTitleKey = 'pageTitles.serviceUnavailable';
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
    case PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.repeatPrescriptionsPartialSuccess';
      route.meta.pageTitleKey = 'pageTitles.repeatPrescriptionsPartialSuccess';
      break;
    case MESSAGING.name:
    case MESSAGING_MESSAGES.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.messaging';
      route.meta.pageTitleKey = 'pageTitles.messaging';
      break;
    case MYRECORD.name:
    case MYRECORDNOACCESS.name:
    case MY_RECORD_VISION_DIAGNOSIS_DETAIL.name:
    case MY_RECORD_VISION_EXAMINATIONS_DETAIL.name:
    case MY_RECORD_VISION_PROCEDURES_DETAIL.name:
    case MY_RECORD_VISION_TEST_RESULTS_DETAIL.name:
    case MYRECORDTESTRESULT.name:
    case GP_MEDICAL_RECORD.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.myRecord';
      route.meta.pageTitleKey = 'pageTitles.myRecord';
      break;
    case MY_RECORD_DOCUMENTS.name:
    case MY_RECORD_DOCUMENT.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.myRecordDocuments';
      route.meta.pageTitleKey = 'pageTitles.myRecordDocuments';
      break;
    case MY_RECORD_DOCUMENT_DETAIL.name:
      route.meta.pageTitleKey = 'pageTitles.myRecordDocuments';
      store.dispatch('navigation/setNewMenuItem', 3);
      break;
    case MYRECORD_GP_AT_HAND.name:
    case GP_MEDICAL_RECORD_GP_AT_HAND.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.serviceUnavailable';
      route.meta.pageTitleKey = 'pageTitles.serviceUnavailable';
      break;
    case ALLERGIESANDREACTIONS.name:
      route.meta.headerKey = 'pageHeaders.allergiesAndReactions';
      route.meta.pageTitleKey = 'pageTitles.allergiesAndReactions';
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
    case TESTRESULTS.name:
    case TESTRESULTSDETAIL.name:
      route.meta.headerKey = 'pageHeaders.testResults';
      route.meta.pageTitleKey = 'pageTitles.testResults';
      break;
    case TESTRESULTID.name:
      route.meta.headerKey = 'pageHeaders.testResult';
      route.meta.pageTitleKey = 'pageTitles.testResult';
      break;
    case IMMUNISATIONS.name:
      route.meta.headerKey = 'pageHeaders.immunisations';
      route.meta.pageTitleKey = 'pageTitles.immunisations';
      break;
    case HEALTH_CONDITIONS.name:
      route.meta.headerKey = 'pageHeaders.healthConditions';
      route.meta.pageTitleKey = 'pageTitles.healthConditions';
      break;
    case MEDICINES.name:
      route.meta.headerKey = 'pageHeaders.medicines';
      route.meta.pageTitleKey = 'pageTitles.medicines';
      break;
    case NOMINATED_PHARMACY_SEARCH.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      if (store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined) {
        route.meta.headerKey = 'pageHeaders.nominateMyPharmacy';
        route.meta.pageTitleKey = 'pageTitles.nominateMyPharmacy';
      } else {
        route.meta.headerKey = 'pageHeaders.searchNominatedPharmacy';
        route.meta.pageTitleKey = 'pageTitles.searchNominatedPharmacy';
      }
      break;
    case NOMINATED_PHARMACY_SEARCH_RESULTS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      if (store.state.nominatedPharmacy.searchResults.noResultsFound === true) {
        route.meta.headerKey = 'th03.errors.noResultsFound.header';
        route.meta.pageTitleKey = 'th03.errors.noResultsFound.title';
      } else {
        route.meta.headerKey = 'pageHeaders.changeNominatedPharmacy';
        route.meta.pageTitleKey = 'pageTitles.changeNominatedPharmacy';
      }
      break;
    case NOMINATED_PHARMACY.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      if (store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] &&
          !store.getters['nominatedPharmacy/justUpdated']) {
        route.meta.headerKey = 'pageHeaders.nominatedPharmacyNotFound';
        route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyNotFound';
      } else if (store.state.nominatedPharmacy.pharmacy.pharmacyType === PharmacyType.P3) {
        route.meta.headerKey = 'pageHeaders.dispensingPractice';
        route.meta.pageTitleKey = 'pageTitles.dispensingPractice';
      } else {
        route.meta.headerKey = 'pageHeaders.nominatedPharmacy';
        route.meta.pageTitleKey = 'pageTitles.nominatedPharmacy';
      }
      break;
    case NOMINATED_PHARMACY_CHECK.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      if (store.getters['nominatedPharmacy/hasNoNominatedPharmacy']) {
        route.meta.headerKey = 'pageHeaders.nominatedPharmacyNotFound';
        route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyNotFound';
      } else if (store.state.nominatedPharmacy.pharmacy.pharmacyType === PharmacyType.P3) {
        route.meta.headerKey = 'pageHeaders.dispensingPracticeFound';
        route.meta.pageTitleKey = 'pageTitles.dispensingPracticeFound';
      } else {
        route.meta.headerKey = 'pageHeaders.nominatedPharmacyFound';
        route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyFound';
      }
      break;
    case NOMINATED_PHARMACY_CONFIRM.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.confirmNominatedPharmacy';
      route.meta.pageTitleKey = 'pageTitles.confirmNominatedPharmacy';
      break;
    case NOMINATED_PHARMACY_CANNOT_CHANGE.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.cannotChangePharmacy';
      route.meta.pageTitleKey = 'pageTitles.cannotChangePharmacy';
      break;
    case RECALLS.name:
      route.meta.headerKey = 'pageHeaders.recalls';
      route.meta.pageTitleKey = 'pageTitles.recalls';
      break;
    case REFERRALS.name:
      route.meta.headerKey = 'pageHeaders.referrals';
      route.meta.pageTitleKey = 'pageTitles.referrals';
      break;
    default:
      route.meta.headerKey = 'errors.404.header';
      route.meta.pageTitleKey = 'errors.404.pageTitle';
      break;
  }
  store.dispatch('http/cancelRequests');
  store.dispatch('flashMessage/clear');
  store.dispatch('errors/setRoutePath', route);

  setPageTitle(route, store, app);
}
