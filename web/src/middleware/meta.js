/* eslint-disable no-param-reassign */
import {
  ACCOUNT,
  ACCOUNT_COOKIES,
  ACCOUNT_NOTIFICATIONS,
  ACUTE_MEDICINES,
  ALLERGIESANDREACTIONS,
  APPOINTMENT_ADMIN_HELP,
  APPOINTMENT_BOOKING,
  APPOINTMENT_BOOKING_GUIDANCE,
  APPOINTMENT_BOOKING_SUCCESS,
  APPOINTMENT_CANCELLING,
  APPOINTMENT_CANCELLING_SUCCESS,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENT_GP_ADVICE,
  APPOINTMENT_GP_AT_HAND,
  APPOINTMENT_INFORMATICA,
  APPOINTMENTS,
  BEGINLOGIN,
  CHECKYOURSYMPTOMS,
  CONSULTATIONS,
  CURRENT_MEDICINES,
  DATA_SHARING_DOES_NOT_APPLY,
  DATA_SHARING_MAKE_YOUR_CHOICE,
  DATA_SHARING_OVERVIEW,
  DATA_SHARING_WHERE_USED,
  DIAGNOSIS_V2,
  DISCONTINUED_MEDICINES,
  DOCUMENT,
  DOCUMENT_DETAIL,
  DOCUMENTS,
  ENCOUNTERS,
  EVENTS,
  EXAMINATIONS_V2,
  GP_APPOINTMENTS,
  GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
  HEALTH_CONDITIONS,
  HOSPITAL_APPOINTMENTS,
  IMMUNISATIONS,
  INDEX,
  INTERSTITIAL_REDIRECTOR,
  LINKED_PROFILES,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS,
  LINKED_PROFILES_SHUTTER_MORE,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS,
  LINKED_PROFILES_SHUTTER_SETTINGS,
  LINKED_PROFILES_SHUTTER_SYMPTOMS,
  LINKED_PROFILES_SUMMARY,
  LOGIN,
  LOGOUT,
  MEDICAL_HISTORY,
  MEDICINES,
  MESSAGING,
  MESSAGING_MESSAGES,
  MORE,
  MY_RECORD_VISION_DIAGNOSIS_DETAIL,
  MY_RECORD_VISION_EXAMINATIONS_DETAIL,
  MY_RECORD_VISION_PROCEDURES_DETAIL,
  MY_RECORD_VISION_TEST_RESULTS_DETAIL,
  MYRECORD,
  MYRECORD_GP_AT_HAND,
  MYRECORDNOACCESS,
  MYRECORDTESTRESULT,
  NOMINATED_PHARMACY,
  NOMINATED_PHARMACY_CHANGE_SUCCESS,
  NOMINATED_PHARMACY_CHECK,
  NOMINATED_PHARMACY_CHOOSE_TYPE,
  NOMINATED_PHARMACY_CONFIRM,
  NOMINATED_PHARMACY_DSP_INTERRUPT,
  NOMINATED_PHARMACY_INTERRUPT,
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH,
  NOMINATED_PHARMACY_SEARCH,
  NOMINATED_PHARMACY_SEARCH_RESULTS,
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
  PATIENT_PRACTICE_MESSAGING,
  PATIENT_PRACTICE_MESSAGING_CREATE,
  PATIENT_PRACTICE_MESSAGING_DELETE,
  PATIENT_PRACTICE_MESSAGING_DELETE_SUCCESS,
  PATIENT_PRACTICE_MESSAGING_RECIPIENTS,
  PATIENT_PRACTICE_MESSAGING_URGENCY,
  PATIENT_PRACTICE_MESSAGING_URGENCY_CONTACT_GP,
  PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTION_REPEAT_COURSES,
  PRESCRIPTIONS,
  PRESCRIPTIONS_GP_AT_HAND,
  PRESCRIPTIONS_ORDER_SUCCESS,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS,
  PROCEDURES_V2,
  RECALLS,
  REFERRALS,
  SWITCH_PROFILE,
  SYMPTOMS,
  TERMSANDCONDITIONS,
  TESTRESULTID,
  TESTRESULTS,
  TESTRESULTSDETAIL,
} from '@/lib/routes';

import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import get from 'lodash/fp/get';

function setPageTitle(route, store, app) {
  let header = '';
  let title = '';

  if (route.meta.headerKey !== '') {
    header = app.i18n.tc(route.meta.headerKey, null, route.meta.formatArguments);
  }
  if (route.meta.title !== '') {
    title = app.i18n.tc(route.meta.pageTitleKey, null, route.meta.formatArguments);
  }

  store.dispatch('header/updateHeaderCaption', '');
  store.dispatch('header/updateHeaderText', header);
  store.dispatch('pageTitle/updatePageTitle', title);
}

export default function ({ route, store, app }) {
  switch (route.name) {
    case ACCOUNT.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.settings';
      route.meta.pageTitleKey = 'pageTitles.settings';
      break;
    case ACCOUNT_COOKIES.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.cookies';
      route.meta.pageTitleKey = 'pageTitles.cookies';
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
    case ALLERGIESANDREACTIONS.name:
      route.meta.headerKey = 'pageHeaders.allergiesAndReactions';
      route.meta.pageTitleKey = 'pageTitles.allergiesAndReactions';
      break;
    case APPOINTMENT_ADMIN_HELP.name:
      route.meta.headerKey = 'pageHeaders.appointmentAdminHelp';
      route.meta.pageTitleKey = 'pageTitles.appointmentAdminHelp';
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
    case APPOINTMENT_BOOKING_SUCCESS.name: {
      store.dispatch('navigation/setNewMenuItem', 1);
      if (store.getters['session/isProxying']) {
        const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
        route.meta.headerKey = 'pageHeaders.appointmentProxyBookingSuccess';
        route.meta.pageTitleKey = 'pageTitles.appointmentProxyBookingSuccess';
        route.meta.formatArguments = { name: givenName };
      } else {
        route.meta.headerKey = 'pageHeaders.appointmentBookingSuccess';
        route.meta.pageTitleKey = 'pageHeaders.appointmentBookingSuccess';
      }
      break;
    }
    case APPOINTMENT_CANCELLING.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentCancelling';
      route.meta.pageTitleKey = 'pageTitles.appointmentCancelling';
      break;
    case APPOINTMENT_CANCELLING_SUCCESS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      if (store.getters['session/isProxying']) {
        const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
        route.meta.headerKey = 'pageHeaders.appointmentProxyCancellingSuccess';
        route.meta.pageTitleKey = 'pageTitles.appointmentProxyCancellingSuccess';
        route.meta.formatArguments = { name: givenName };
      } else {
        route.meta.headerKey = 'pageHeaders.appointmentCancellingSuccess';
        route.meta.pageTitleKey = 'pageTitles.appointmentCancellingSuccess';
      }
      break;
    case APPOINTMENT_CONFIRMATIONS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointmentConfirmation';
      route.meta.pageTitleKey = 'pageTitles.appointmentConfirmation';
      break;
    case APPOINTMENT_GP_ADVICE.name:
      route.meta.headerKey = 'pageHeaders.appointmentGpAdvice';
      route.meta.pageTitleKey = 'pageTitles.appointmentGpAdvice';
      break;
    case APPOINTMENT_GP_AT_HAND.name:
    case APPOINTMENT_INFORMATICA.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.serviceUnavailable';
      route.meta.pageTitleKey = 'pageTitles.serviceUnavailable';
      break;
    case APPOINTMENTS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.appointments';
      route.meta.pageTitleKey = 'pageTitles.appointments';
      break;
    case BEGINLOGIN.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.login';
      route.meta.pageTitleKey = 'pageTitles.login';
      break;
    case CHECKYOURSYMPTOMS.name:
    case SYMPTOMS.name:
      store.dispatch('navigation/setNewMenuItem', 0);
      route.meta.headerKey = 'pageHeaders.symptoms';
      route.meta.pageTitleKey = 'pageTitles.symptoms';
      break;
    case CONSULTATIONS.name:
    case EVENTS.name:
      route.meta.headerKey = 'pageHeaders.consultationsAndEvents';
      route.meta.pageTitleKey = 'pageTitles.consultationsAndEvents';
      break;
    case CURRENT_MEDICINES.name:
      route.meta.headerKey = 'pageHeaders.currentMedicines';
      route.meta.pageTitleKey = 'pageTitles.currentMedicines';
      break;
    case DATA_SHARING_DOES_NOT_APPLY.name:
      route.meta.headerKey = 'pageHeaders.dataSharingDoesNotApply';
      route.meta.pageTitleKey = 'pageTitles.dataSharingDoesNotApply';
      break;
    case DATA_SHARING_MAKE_YOUR_CHOICE.name:
      route.meta.headerKey = 'pageHeaders.dataSharingMakeYourChoice';
      route.meta.pageTitleKey = 'pageTitles.dataSharingMakeYourChoice';
      break;
    case DATA_SHARING_OVERVIEW.name:
      route.meta.headerKey = 'pageHeaders.dataSharingOverview';
      route.meta.pageTitleKey = 'pageTitles.dataSharingOverview';
      break;
    case DATA_SHARING_WHERE_USED.name:
      route.meta.headerKey = 'pageHeaders.dataSharingWhereUsed';
      route.meta.pageTitleKey = 'pageTitles.dataSharingWhereUsed';
      break;
    case DIAGNOSIS_V2.name:
      route.meta.headerKey = 'pageHeaders.diagnosisV2';
      route.meta.pageTitleKey = 'pageTitles.diagnosisV2';
      break;
    case DISCONTINUED_MEDICINES.name:
      route.meta.headerKey = 'pageHeaders.discontinuedMedicines';
      route.meta.pageTitleKey = 'pageTitles.discontinuedMedicines';
      break;
    case DOCUMENT.name:
    case DOCUMENTS.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.myRecordDocuments';
      route.meta.pageTitleKey = 'pageTitles.myRecordDocuments';
      break;
    case DOCUMENT_DETAIL.name:
      route.meta.pageTitleKey = 'pageTitles.myRecordDocuments';
      store.dispatch('navigation/setNewMenuItem', 3);
      break;
    case ENCOUNTERS.name:
      route.meta.headerKey = 'pageHeaders.encounters';
      route.meta.pageTitleKey = 'pageTitles.encounters';
      break;
    case EXAMINATIONS_V2.name:
      route.meta.headerKey = 'pageHeaders.examinationsV2';
      route.meta.pageTitleKey = 'pageTitles.examinationsV2';
      break;
    case GP_APPOINTMENTS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.gpAppointments';
      route.meta.pageTitleKey = 'pageTitles.gpAppointments';
      break;
    case GP_MEDICAL_RECORD.name:
    case MY_RECORD_VISION_DIAGNOSIS_DETAIL.name:
    case MY_RECORD_VISION_EXAMINATIONS_DETAIL.name:
    case MY_RECORD_VISION_PROCEDURES_DETAIL.name:
    case MY_RECORD_VISION_TEST_RESULTS_DETAIL.name:
    case MYRECORD.name:
    case MYRECORDNOACCESS.name:
    case MYRECORDTESTRESULT.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.myRecord';
      route.meta.pageTitleKey = 'pageTitles.myRecord';
      break;
    case GP_MEDICAL_RECORD_GP_AT_HAND.name:
    case MYRECORD_GP_AT_HAND.name:
      store.dispatch('navigation/setNewMenuItem', 3);
      route.meta.headerKey = 'pageHeaders.serviceUnavailable';
      route.meta.pageTitleKey = 'pageTitles.serviceUnavailable';
      break;
    case HEALTH_CONDITIONS.name:
      route.meta.headerKey = 'pageHeaders.healthConditions';
      route.meta.pageTitleKey = 'pageTitles.healthConditions';
      break;
    case HOSPITAL_APPOINTMENTS.name:
      store.dispatch('navigation/setNewMenuItem', 1);
      route.meta.headerKey = 'pageHeaders.hospitalAppointments';
      route.meta.pageTitleKey = 'pageTitles.hospitalAppointments';
      break;
    case IMMUNISATIONS.name:
      route.meta.headerKey = 'pageHeaders.immunisations';
      route.meta.pageTitleKey = 'pageTitles.immunisations';
      break;
    case INDEX.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'pageHeaders.home';
      route.meta.pageTitleKey = 'pageTitles.home';
      break;
    case INTERSTITIAL_REDIRECTOR.name:
      route.meta.headerKey = '';
      route.meta.pageTitleKey = '';
      break;
    case LINKED_PROFILES.name:
      route.meta.headerKey = 'pageHeaders.linkedProfiles';
      route.meta.pageTitleKey = 'pageTitles.linkedProfiles';
      break;
    case LINKED_PROFILES_SHUTTER_APPOINTMENTS.name: {
      store.dispatch('navigation/setNewMenuItem', 1);
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      route.meta.headerKey = 'linkedProfiles.shutter.appointments.header';
      route.meta.pageTitleKey = 'linkedProfiles.shutter.appointments.header';
      route.meta.formatArguments = { name: givenName };
      break;
    }
    case LINKED_PROFILES_SHUTTER_MORE.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'linkedProfiles.shutter.more.header';
      route.meta.pageTitleKey = 'linkedProfiles.shutter.more.header';
      break;
    case LINKED_PROFILES_SHUTTER_PRESCRIPTIONS.name: {
      store.dispatch('navigation/setNewMenuItem', 2);
      const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
      route.meta.headerKey = 'linkedProfiles.shutter.prescriptions.header';
      route.meta.pageTitleKey = 'linkedProfiles.shutter.prescriptions.header';
      route.meta.formatArguments = { name: givenName };
      break;
    }
    case LINKED_PROFILES_SHUTTER_SETTINGS.name:
      store.dispatch('navigation/clearPreviousSelectedMenuItem');
      route.meta.headerKey = 'linkedProfiles.shutter.settings.header';
      route.meta.pageTitleKey = 'linkedProfiles.shutter.settings.header';
      break;
    case LINKED_PROFILES_SHUTTER_SYMPTOMS.name:
      store.dispatch('navigation/setNewMenuItem', 0);
      route.meta.headerKey = 'linkedProfiles.shutter.symptoms.header';
      route.meta.pageTitleKey = 'linkedProfiles.shutter.symptoms.header';
      break;
    case LINKED_PROFILES_SUMMARY.name:
      route.meta.headerKey = '';
      route.meta.pageTitleKey = '';
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
    case MEDICAL_HISTORY.name:
      route.meta.headerKey = 'pageHeaders.medicalHistory';
      route.meta.pageTitleKey = 'pageTitles.medicalHistory';
      break;
    case MEDICINES.name:
      route.meta.headerKey = 'pageHeaders.medicines';
      route.meta.pageTitleKey = 'pageTitles.medicines';
      break;
    case MESSAGING.name:
    case MESSAGING_MESSAGES.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.messaging';
      route.meta.pageTitleKey = 'pageTitles.messaging';
      break;
    case MORE.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.more';
      route.meta.pageTitleKey = 'pageTitles.more';
      break;
    case NOMINATED_PHARMACY.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.nominatedPharmacy';
      route.meta.pageTitleKey = 'pageTitles.nominatedPharmacy';
      break;
    case NOMINATED_PHARMACY_CHANGE_SUCCESS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.nominatedPharmacyChangeSuccess';
      route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyChangeSuccess';
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
    case NOMINATED_PHARMACY_CHOOSE_TYPE.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.nominatedPharmacyChooseType';
      route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyChooseType';
      break;
    case NOMINATED_PHARMACY_CONFIRM.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.confirmNominatedPharmacy';
      route.meta.pageTitleKey = 'pageTitles.confirmNominatedPharmacy';
      break;
    case NOMINATED_PHARMACY_DSP_INTERRUPT.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.nominatedPharmacyDspInterrupt';
      route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyDspInterrupt';
      break;
    case NOMINATED_PHARMACY_INTERRUPT.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      if (store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined) {
        route.meta.headerKey = 'pageHeaders.nominatedPharmacyNotFoundInterrupt';
        route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyNotFoundInterrupt';
      } else {
        route.meta.headerKey = 'pageHeaders.nominatedPharmacyFoundInterrupt';
        route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyFoundInterrupt';
      }
      break;
    case NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.nominatedPharmacyOnlineOnlyChoices';
      route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyOnlineOnlyChoices';
      break;
    case NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.nominatedPharmacyOnlineOnlySearch';
      route.meta.pageTitleKey = 'pageTitles.nominatedPharmacyOnlineOnlySearch';
      break;
    case NOMINATED_PHARMACY_SEARCH.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.searchNominatedPharmacy';
      route.meta.pageTitleKey = 'pageTitles.searchNominatedPharmacy';
      break;
    case NOMINATED_PHARMACY_SEARCH_RESULTS.name: {
      store.dispatch('navigation/setNewMenuItem', 2);
      const pharmacyTypeChoice = store.state.nominatedPharmacy.chosenType;
      if (pharmacyTypeChoice === PharmacyTypeChoice.ONLINE_PHARMACY) {
        if (store.state.nominatedPharmacy.onlineOnlyKnownOption === true) {
          route.meta.headerKey = 'nominatedPharmacySearchResults.online.search.header';
          route.meta.pageTitleKey = 'nominatedPharmacySearchResults.online.search.title';
          route.meta.formatArguments = { searchQuery: store.state.nominatedPharmacy.searchQuery };
        } else {
          route.meta.headerKey = 'nominatedPharmacySearchResults.online.random.header';
          route.meta.pageTitleKey = 'nominatedPharmacySearchResults.online.random.title';
        }
      } else {
        route.meta.headerKey = 'nominatedPharmacySearchResults.highStreet.header';
        route.meta.pageTitleKey = 'nominatedPharmacySearchResults.highStreet.title';
        route.meta.formatArguments = { searchQuery: store.state.nominatedPharmacy.searchQuery };
      }
      break;
    }
    case ORGAN_DONATION.name:
    case ORGAN_DONATION_ADDITIONAL_DETAILS.name:
    case ORGAN_DONATION_AMEND.name:
    case ORGAN_DONATION_FAITH.name:
    case ORGAN_DONATION_MORE_ABOUT_ORGANS.name:
    case ORGAN_DONATION_REVIEW_YOUR_DECISION.name:
    case ORGAN_DONATION_SOME_ORGANS.name:
    case ORGAN_DONATION_VIEW_DECISION.name:
    case ORGAN_DONATION_YOUR_CHOICE.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.organDonation';
      route.meta.pageTitleKey = 'pageTitles.organDonation';
      break;
    case ORGAN_DONATION_WITHDRAW_REASON.name:
    case ORGAN_DONATION_WITHDRAWN.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.organDonation';
      route.meta.pageTitleKey = 'pageTitles.organDonationWithdraw';
      break;
    case PATIENT_PRACTICE_MESSAGING.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.patientPracticeMessaging';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessaging';
      break;
    case PATIENT_PRACTICE_MESSAGING_CREATE.name: {
      const name = get('state.patientPracticeMessaging.selectedMessageRecipient.name')(store);
      route.meta.headerKey = 'pageTitles.patientPracticeMessagingCreateMessage';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessagingCreateMessage';
      route.meta.formatArguments = { name };
      break;
    }
    case PATIENT_PRACTICE_MESSAGING_DELETE.name: {
      const name = get('state.patientPracticeMessaging.selectedMessageRecipient.name')(store);
      route.meta.headerKey = 'pageTitles.patientPracticeMessagingDeleteMessage';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessagingDeleteMessage';
      route.meta.formatArguments = { name };
      break;
    }
    case PATIENT_PRACTICE_MESSAGING_DELETE_SUCCESS.name: {
      const name = get('state.patientPracticeMessaging.selectedMessageRecipient.name')(store);
      route.meta.headerKey = 'pageTitles.patientPracticeMessagingDeleteMessageSuccess';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessagingDeleteMessageSuccess';
      route.meta.formatArguments = { name };
      break;
    }
    case PATIENT_PRACTICE_MESSAGING_RECIPIENTS.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.patientPracticeMessagingRecipients';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessagingRecipients';
      break;
    case PATIENT_PRACTICE_MESSAGING_URGENCY.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.patientPracticeMessagingUrgency';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessagingUrgency';
      break;
    case PATIENT_PRACTICE_MESSAGING_URGENCY_CONTACT_GP.name:
      store.dispatch('navigation/setNewMenuItem', 4);
      route.meta.headerKey = 'pageHeaders.patientPracticeMessagingUrgencyContactYourGp';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessagingUrgencyContactYourGp';
      break;
    case PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE.name: {
      const name = get('state.patientPracticeMessaging.selectedMessageRecipient.name')(store);
      route.meta.headerKey = 'pageTitles.patientPracticeMessagingViewMessage';
      route.meta.pageTitleKey = 'pageTitles.patientPracticeMessagingViewMessage';
      route.meta.formatArguments = { name };
      break;
    }
    case PRESCRIPTION_CONFIRM_COURSES.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.confirmPrescription';
      route.meta.pageTitleKey = 'pageTitles.confirmPrescription';
      break;
    case PRESCRIPTION_REPEAT_COURSES.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.repeatPrescriptionCourses';
      route.meta.pageTitleKey = 'pageTitles.repeatPrescriptionCourses';
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
    case PRESCRIPTIONS_ORDER_SUCCESS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      if (store.getters['session/isProxying']) {
        const givenName = get('state.linkedAccounts.actingAsUser.givenName')(store);
        route.meta.headerKey = 'pageHeaders.prescriptionProxyOrderSuccess';
        route.meta.pageTitleKey = 'pageTitles.prescriptionProxyOrderSuccess';
        route.meta.formatArguments = { name: givenName };
      } else {
        route.meta.headerKey = 'pageHeaders.prescriptionOrderSuccess';
        route.meta.pageTitleKey = 'pageTitles.prescriptionOrderSuccess';
      }
      break;
    case PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS.name:
      store.dispatch('navigation/setNewMenuItem', 2);
      route.meta.headerKey = 'pageHeaders.repeatPrescriptionsPartialSuccess';
      route.meta.pageTitleKey = 'pageTitles.repeatPrescriptionsPartialSuccess';
      break;
    case PROCEDURES_V2.name:
      route.meta.headerKey = 'pageHeaders.proceduresV2';
      route.meta.pageTitleKey = 'pageTitles.proceduresV2';
      break;
    case RECALLS.name:
      route.meta.headerKey = 'pageHeaders.recalls';
      route.meta.pageTitleKey = 'pageTitles.recalls';
      break;
    case REFERRALS.name:
      route.meta.headerKey = 'pageHeaders.referrals';
      route.meta.pageTitleKey = 'pageTitles.referrals';
      break;
    case SWITCH_PROFILE.name:
      route.meta.headerKey = 'pageHeaders.switchProfile';
      route.meta.pageTitleKey = 'pageTitles.switchProfile';
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
    default:
      route.meta.headerKey = 'errors.404.header';
      route.meta.pageTitleKey = 'errors.404.pageTitle';
      break;
  }
  store.dispatch('http/cancelRequests');
  store.dispatch('flashMessage/clear');
  store.dispatch('errors/setRoutePath', route);

  // clear errors
  store.dispatch('myAppointments/clearError');
  store.dispatch('availableAppointments/clearError');

  setPageTitle(route, store, app);
}
