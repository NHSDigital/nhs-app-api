import {
  HELP_AND_SUPPORT_URL,
  ACCESSIBILITY_STATEMENT_URL,
  OPEN_SOURCE_LICENCES_URL,
  PRIVACY_POLICY_URL,
  TERMS_AND_CONDITIONS_URL,
} from '@/router/externalLinks';

export const accountLinks = () =>
  [
    {
      url: HELP_AND_SUPPORT_URL,
      localeLabel: 'myAccount.helpAndSupport',
    },
    {
      url: ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'myAccount.accessibilityStatement',
    },
    {
      url: OPEN_SOURCE_LICENCES_URL,
      localeLabel: 'myAccount.openSourceLicences',
    },
    {
      url: PRIVACY_POLICY_URL,
      localeLabel: 'myAccount.privacyPolicy',
    },
    {
      url: TERMS_AND_CONDITIONS_URL,
      localeLabel: 'myAccount.termsAndConditions',
    },
  ];

export const footerLinks = () =>
  [
    {
      url: TERMS_AND_CONDITIONS_URL,
      localeLabel: 'myAccount.termsAndConditions',
    },
    {
      url: PRIVACY_POLICY_URL,
      localeLabel: 'myAccount.privacyPolicy',
    },
    {
      url: HELP_AND_SUPPORT_URL,
      localeLabel: 'myAccount.helpAndSupport',
    },
    {
      url: ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'myAccount.accessibilityStatement',
    },
  ];

export default {
  accountLinks,
  footerLinks,
};
