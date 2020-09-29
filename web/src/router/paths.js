import { TERMSANDCONDITIONS_NAME } from '@/router/names';

export const EMPTY_PATH = '/';
export const INDEX_PATH_PARAM = ':patientId?';
export const INDEX_PATH = `/patient/${INDEX_PATH_PARAM}/`;
export const NOT_FOUND_PATH = '*';
export const LOGIN_PATH = '/login';
export const LOGIN_BIOMETRIC_ERROR_PATH = '/biometric-login-error';
export const PRE_REGISTRATION_INFORMATION_PATH = '/pre-registration-information';
export const BEGINLOGIN_PATH = '/begin-login';
export const IOS_COMPATIBILITY_PATH = '/ios-compatibility';
export const AUTH_RETURN_PATH = '/auth-return';
export const TERMSANDCONDITIONS_PATH = '/terms-and-conditions';
export const LOGOUT_PATH = 'logout';
export const MORE_PATH = 'more';
export const ADVICE_PATH = 'advice';
export const SYMPTOMS_PATH = 'symptoms';
export const GET_HEALTH_ADVICE_PATH = '/get-health-advice';
export const CHECKYOURSYMPTOMS_PATH = '/check-your-symptoms';
export const ACCOUNT_PATH = 'account';
export const ACCOUNT_COOKIES_PATH = 'account/cookies';
export const ACCOUNT_NOTIFICATIONS_PATH = 'account/notifications';
export const LOGIN_SETTINGS_PATH = 'account/login-settings';
export const LOGIN_SETTINGS_ERROR_PATH = 'account/login-settings/error';
export const INTERSTITIAL_REDIRECTOR_PATH = 'redirector';
export const APPOINTMENTS_PATH = 'appointments';
export const GP_APPOINTMENTS_PATH = 'appointments/gp-appointments';
export const HOSPITAL_APPOINTMENTS_PATH = 'appointments/hospital-appointments';
export const APPOINTMENT_ADMIN_HELP_PATH = 'appointments/gp-appointments/admin-help';
export const APPOINTMENT_GP_ADVICE_PATH = 'appointments/gp-appointments/gp-advice';
export const APPOINTMENT_BOOKING_PATH = 'appointments/gp-appointments/booking';
export const APPOINTMENT_BOOKING_GUIDANCE_PATH = 'appointments/gp-appointments/booking-guidance';
export const APPOINTMENT_CANCELLING_PATH = 'appointments/gp-appointments/cancelling';
export const APPOINTMENT_CANCELLING_SUCCESS_PATH = 'appointments/gp-appointments/cancelling-success';
export const APPOINTMENT_CONFIRMATIONS_PATH = 'appointments/gp-appointments/confirmation';
export const APPOINTMENT_BOOKING_SUCCESS_PATH = 'appointments/gp-appointments/booking-success';
export const APPOINTMENT_ADD_TO_CALENDAR_PATH = 'appointments/gp-appointments/add-to-calendar-interrupt';
export const APPOINTMENT_GP_AT_HAND_PATH = 'appointments/gp-at-hand';
export const APPOINTMENT_INFORMATICA_PATH = 'appointments/informatica';
export const DATA_SHARING_OVERVIEW_PATH = 'data-sharing';
export const DATA_SHARING_WHERE_USED_PATH = 'data-sharing/where-used';
export const DATA_SHARING_DOES_NOT_APPLY_PATH = 'data-sharing/does-not-apply';
export const DATA_SHARING_MAKE_YOUR_CHOICE_PATH = 'data-sharing/make-your-choice';
export const MY_RECORD_PATH = 'my-record';
export const HEALTH_RECORDS_PATH = 'health-records';
export const GP_MEDICAL_RECORD_PATH = 'health-records/gp-medical-record';
export const GP_MEDICAL_RECORD_GP_AT_HAND_PATH = 'health-records/gp-at-hand';
export const DISCONTINUED_MEDICINES_PATH = 'health-records/gp-medical-record/medicines/discontinued-medicines';
export const ALLERGIESANDREACTIONS_PATH = 'health-records/gp-medical-record/allergies-and-reactions';
export const ACUTE_MEDICINES_PATH = 'health-records/gp-medical-record/medicines/acute-medicines';
export const CURRENT_MEDICINES_PATH = 'health-records/gp-medical-record/medicines/current-medicines';
export const CONSULTATIONS_PATH = 'health-records/gp-medical-record/consultations';
export const CONSULTATIONS_AND_EVENTS_PATH = 'health-records/gp-medical-record/consultations';
export const EVENTS_PATH = 'health-records/gp-medical-record/events';
export const ENCOUNTERS_PATH = 'health-records/gp-medical-record/encounters';
export const TESTRESULTS_PATH = 'health-records/gp-medical-record/test-results';
export const TESTRESULTSDETAIL_PATH = 'health-records/gp-medical-record/test-results-detail';
export const TESTRESULTID_PATH = 'health-records/gp-medical-record/testresultdetail/:testResultId';
export const IMMUNISATIONS_PATH = 'health-records/gp-medical-record/immunisations';
export const DIAGNOSIS_V2_PATH = 'health-records/gp-medical-record/diagnosis';
export const EXAMINATIONS_V2_PATH = 'health-records/gp-medical-record/examinations';
export const PROCEDURES_V2_PATH = 'health-records/gp-medical-record/procedures';
export const MEDICINES_PATH = 'health-records/gp-medical-record/medicines';
export const MEDICAL_HISTORY_PATH = 'health-records/gp-medical-record/medical-history';
export const DOCUMENTS_PATH = 'health-records/gp-medical-record/documents';
export const DOCUMENT_PATH = 'health-records/gp-medical-record/documents/:id';
export const DOCUMENT_DETAIL_PATH = 'health-records/gp-medical-record/documents/detail/:id';
export const RECALLS_PATH = 'health-records/gp-medical-record/recalls';
export const REFERRALS_PATH = 'health-records/gp-medical-record/referrals';
export const HEALTH_CONDITIONS_PATH = 'health-records/gp-medical-record/health-conditions';
export const NOMINATED_PHARMACY_PATH = 'nominated-pharmacy';
export const NOMINATED_PHARMACY_SEARCH_PATH = 'nominated-pharmacy/search';
export const NOMINATED_PHARMACY_CONFIRM_PATH = 'nominated-pharmacy/confirm';
export const NOMINATED_PHARMACY_CHANGE_SUCCESS_PATH = 'nominated-pharmacy/change-success';
export const NOMINATED_PHARMACY_INTERRUPT_PATH = 'nominated-pharmacy/interrupt';
export const NOMINATED_PHARMACY_DSP_INTERRUPT_PATH = 'nominated-pharmacy/dsp-interrupt';
export const NOMINATED_PHARMACY_SEARCH_RESULTS_PATH = 'nominated-pharmacy/results';
export const NOMINATED_PHARMACY_CHECK_PATH = 'nominated-pharmacy/check';
export const NOMINATED_PHARMACY_CHOOSE_TYPE_PATH = 'nominated-pharmacy/choose-type';
export const NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES_PATH = 'nominated-pharmacy/online-only-choices';
export const NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_PATH = 'nominated-pharmacy/online-only-search';
export const ORGAN_DONATION_PATH = 'organ-donation';
export const ORGAN_DONATION_ADDITIONAL_DETAILS_PATH = 'organ-donation/additional-details';
export const ORGAN_DONATION_AMEND_PATH = 'organ-donation/amend';
export const ORGAN_DONATION_FAITH_PATH = 'organ-donation/faith';
export const ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH = 'organ-donation/more-about-organs';
export const ORGAN_DONATION_SOME_ORGANS_PATH = 'organ-donation/some-organs';
export const ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH = 'organ-donation/review-your-decision';
export const ORGAN_DONATION_VIEW_DECISION_PATH = 'organ-donation/view-decision';
export const ORGAN_DONATION_WITHDRAW_REASON_PATH = 'organ-donation/withdraw-reason';
export const ORGAN_DONATION_WITHDRAWN_PATH = 'organ-donation/withdrawn';
export const ORGAN_DONATION_YOUR_CHOICE_PATH = 'organ-donation/your-choice';
export const MESSAGES_PATH = 'messages';
export const HEALTH_INFORMATION_UPDATES_PATH = 'messages/app-messaging';
export const HEALTH_INFORMATION_UPDATES_MESSAGES_PATH = 'messages/app-messaging/app-message';
export const GP_MESSAGES_PATH = 'messages/gp-messages';
export const GP_MESSAGES_VIEW_ATTACHMENT_PATH = 'messages/gp-messages/view-attachment';
export const GP_MESSAGES_DOWNLOAD_ATTACHMENT_PATH = 'messages/gp-messages/download-attachment';
export const GP_MESSAGES_DELETE_PATH = 'messages/gp-messages/delete';
export const GP_MESSAGES_URGENCY_PATH = 'messages/gp-messages/urgency';
export const GP_MESSAGES_URGENCY_CONTACT_GP_PATH = 'messages/gp-messages/urgency/contact-your-gp';
export const GP_MESSAGES_RECIPIENTS_PATH = 'messages/gp-messages/recipients';
export const GP_MESSAGES_VIEW_MESSAGE_PATH = 'messages/gp-messages/view-details';
export const GP_MESSAGES_CREATE_PATH = 'messages/gp-messages/send-message';
export const GP_MESSAGES_DELETE_SUCCESS_PATH = 'messages/gp-messages/delete-success';
export const PRESCRIPTIONS_PATH = 'prescriptions';
export const PRESCRIPTIONS_VIEW_ORDERS_PATH = 'prescriptions/view-orders';
export const PRESCRIPTION_CONFIRM_COURSES_PATH = 'prescriptions/confirm-prescription-details';
export const PRESCRIPTIONS_GP_AT_HAND_PATH = 'prescriptions/gp-at-hand';
export const PRESCRIPTION_REPEAT_COURSES_PATH = 'prescriptions/repeat-courses';
export const PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_PATH = 'prescriptions/repeat-partial-success';
export const PRESCRIPTIONS_ORDER_SUCCESS_PATH = 'prescriptions/order-success';
export const LINKED_PROFILES_PATH = 'linked-profiles';
export const LINKED_PROFILES_SUMMARY_PATH = 'linked-profiles/summary';
export const LINKED_PROFILES_SHUTTER_MORE_PATH = 'linked-profiles/shutter/more';
export const LINKED_PROFILES_SHUTTER_ADVICE_PATH = 'linked-profiles/shutter/advice';
export const LINKED_PROFILES_SHUTTER_SETTINGS_PATH = 'linked-profiles/shutter/settings';
export const LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH = 'linked-profiles/shutter/appointments';
export const LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_PATH = 'linked-profiles/shutter/prescriptions';
export const SWITCH_PROFILE_PATH = 'switch-profile';
export const UPLIFT_APPOINTMENTS_PATH = 'uplift/appointments';
export const UPLIFT_GP_MEDICAL_RECORD_PATH = 'uplift/gp-medical-record';
export const UPLIFT_PRESCRIPTIONS_PATH = 'uplift/prescriptions';
export const UPLIFT_MORE_PATH = 'uplift/more';
export const USER_RESEARCH_PATH = '/user-research';

export default {
  EMPTY_PATH,
  INDEX_PATH,
  NOT_FOUND_PATH,
  LOGIN_PATH,
  BEGINLOGIN_PATH,
  AUTH_RETURN_PATH,
  TERMSANDCONDITIONS_PATH,
  LOGOUT_PATH,
  MORE_PATH,
  ADVICE_PATH,
  GET_HEALTH_ADVICE_PATH,
  CHECKYOURSYMPTOMS_PATH,
  ACCOUNT_PATH,
  ACCOUNT_COOKIES_PATH,
  ACCOUNT_NOTIFICATIONS_PATH,
  LOGIN_SETTINGS_PATH,
  LOGIN_SETTINGS_ERROR_PATH,
  INTERSTITIAL_REDIRECTOR_PATH,
  LOGIN_BIOMETRIC_ERROR_PATH,
  APPOINTMENTS_PATH,
  GP_APPOINTMENTS_PATH,
  HOSPITAL_APPOINTMENTS_PATH,
  APPOINTMENT_ADMIN_HELP_PATH,
  APPOINTMENT_GP_ADVICE_PATH,
  APPOINTMENT_BOOKING_PATH,
  APPOINTMENT_BOOKING_GUIDANCE_PATH,
  APPOINTMENT_CANCELLING_PATH,
  APPOINTMENT_CANCELLING_SUCCESS_PATH,
  APPOINTMENT_CONFIRMATIONS_PATH,
  APPOINTMENT_BOOKING_SUCCESS_PATH,
  APPOINTMENT_ADD_TO_CALENDAR_PATH,
  APPOINTMENT_GP_AT_HAND_PATH,
  APPOINTMENT_INFORMATICA_PATH,
  DATA_SHARING_OVERVIEW_PATH,
  DATA_SHARING_WHERE_USED_PATH,
  DATA_SHARING_DOES_NOT_APPLY_PATH,
  DATA_SHARING_MAKE_YOUR_CHOICE_PATH,
  MY_RECORD_PATH,
  HEALTH_RECORDS_PATH,
  GP_MEDICAL_RECORD_PATH,
  GP_MEDICAL_RECORD_GP_AT_HAND_PATH,
  DISCONTINUED_MEDICINES_PATH,
  ALLERGIESANDREACTIONS_PATH,
  ACUTE_MEDICINES_PATH,
  CURRENT_MEDICINES_PATH,
  CONSULTATIONS_PATH,
  CONSULTATIONS_AND_EVENTS_PATH,
  EVENTS_PATH,
  ENCOUNTERS_PATH,
  TESTRESULTS_PATH,
  TESTRESULTSDETAIL_PATH,
  TESTRESULTID_PATH,
  IMMUNISATIONS_PATH,
  DIAGNOSIS_V2_PATH,
  EXAMINATIONS_V2_PATH,
  PROCEDURES_V2_PATH,
  MEDICINES_PATH,
  MEDICAL_HISTORY_PATH,
  DOCUMENTS_PATH,
  DOCUMENT_PATH,
  DOCUMENT_DETAIL_PATH,
  RECALLS_PATH,
  REFERRALS_PATH,
  HEALTH_CONDITIONS_PATH,
  IOS_COMPATIBILITY_PATH,
  NOMINATED_PHARMACY_PATH,
  NOMINATED_PHARMACY_SEARCH_PATH,
  NOMINATED_PHARMACY_CONFIRM_PATH,
  NOMINATED_PHARMACY_CHANGE_SUCCESS_PATH,
  NOMINATED_PHARMACY_INTERRUPT_PATH,
  NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
  NOMINATED_PHARMACY_SEARCH_RESULTS_PATH,
  NOMINATED_PHARMACY_CHECK_PATH,
  NOMINATED_PHARMACY_CHOOSE_TYPE_PATH,
  NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES_PATH,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH_PATH,
  ORGAN_DONATION_PATH,
  ORGAN_DONATION_ADDITIONAL_DETAILS_PATH,
  ORGAN_DONATION_AMEND_PATH,
  ORGAN_DONATION_FAITH_PATH,
  ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH,
  ORGAN_DONATION_SOME_ORGANS_PATH,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
  ORGAN_DONATION_VIEW_DECISION_PATH,
  ORGAN_DONATION_WITHDRAW_REASON_PATH,
  ORGAN_DONATION_WITHDRAWN_PATH,
  ORGAN_DONATION_YOUR_CHOICE_PATH,
  MESSAGES_PATH,
  HEALTH_INFORMATION_UPDATES_PATH,
  HEALTH_INFORMATION_UPDATES_MESSAGES_PATH,
  GP_MESSAGES_PATH,
  GP_MESSAGES_VIEW_ATTACHMENT_PATH,
  GP_MESSAGES_DOWNLOAD_ATTACHMENT_PATH,
  GP_MESSAGES_DELETE_PATH,
  GP_MESSAGES_URGENCY_PATH,
  GP_MESSAGES_URGENCY_CONTACT_GP_PATH,
  GP_MESSAGES_RECIPIENTS_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_CREATE_PATH,
  GP_MESSAGES_DELETE_SUCCESS_PATH,
  PRESCRIPTIONS_PATH,
  PRESCRIPTIONS_VIEW_ORDERS_PATH,
  PRESCRIPTION_CONFIRM_COURSES_PATH,
  PRESCRIPTIONS_GP_AT_HAND_PATH,
  PRE_REGISTRATION_INFORMATION_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_PATH,
  PRESCRIPTIONS_ORDER_SUCCESS_PATH,
  LINKED_PROFILES_PATH,
  LINKED_PROFILES_SUMMARY_PATH,
  LINKED_PROFILES_SHUTTER_MORE_PATH,
  LINKED_PROFILES_SHUTTER_ADVICE_PATH,
  LINKED_PROFILES_SHUTTER_SETTINGS_PATH,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS_PATH,
  LINKED_PROFILES_SHUTTER_PRESCRIPTIONS_PATH,
  SWITCH_PROFILE_PATH,
  UPLIFT_APPOINTMENTS_PATH,
  UPLIFT_GP_MEDICAL_RECORD_PATH,
  UPLIFT_PRESCRIPTIONS_PATH,
  UPLIFT_MORE_PATH,
  USER_RESEARCH_PATH,
};

/**
 * Used to define mappings for the home navigation
 * @param currentRouteName current route name
 * @returns {*|string} the resolved path for the home icon
 */
export const executeHomeNavigationRule = (currentRouteName) => {
  const mapping = {};
  mapping[TERMSANDCONDITIONS_NAME] = LOGOUT_PATH;
  return mapping[currentRouteName] || EMPTY_PATH;
};
