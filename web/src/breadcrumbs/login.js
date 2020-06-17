import { INDEX_CRUMB } from '@/breadcrumbs/general';

export const LOGIN_CRUMB = {
  nativeDisabled: true,
};

export const LOGIN_BIOMETRIC_ERROR_CRUMB = {
  nativeDisabled: true,
};

export const PRE_REGISTRATION_CRUMB = {
  i18nKey: 'preRegistrationInformation',
};

export const BEGINLOGIN_CRUMB = {
  i18nKey: 'beginLogin',
  defaultCrumb: [INDEX_CRUMB],
};

export const AUTH_RETURN_CRUMB = {
  i18nKey: 'authReturn',
  defaultCrumb: [INDEX_CRUMB],
};

export const TERMSANDCONDITIONS_CRUMB = {
  i18nKey: 'termsAndConditions',
};

export const USERRESEARCH_CRUMB = {
  nativeDisabled: true,
  i18nKey: 'userResearch',
  defaultCrumb: [],
};

export default {
  LOGIN_CRUMB,
  LOGIN_BIOMETRIC_ERROR_CRUMB,
  PRE_REGISTRATION_CRUMB,
  BEGINLOGIN_CRUMB,
  AUTH_RETURN_CRUMB,
  TERMSANDCONDITIONS_CRUMB,
  USERRESEARCH_CRUMB,
};
