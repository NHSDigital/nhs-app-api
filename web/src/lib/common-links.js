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
      localeLabel: 'account.helpAndSupport',
    },
    {
      url: ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'account.accessibilityStatement',
    },
    {
      url: OPEN_SOURCE_LICENCES_URL,
      localeLabel: 'account.openSourceLicences',
    },
    {
      url: PRIVACY_POLICY_URL,
      localeLabel: 'account.privacyPolicy',
    },
    {
      url: TERMS_AND_CONDITIONS_URL,
      localeLabel: 'account.termsOfUse',
    },
  ];

export const footerLinks = () =>
  [
    {
      url: TERMS_AND_CONDITIONS_URL,
      localeLabel: 'account.termsOfUse',
    },
    {
      url: PRIVACY_POLICY_URL,
      localeLabel: 'account.privacyPolicy',
    },
    {
      url: HELP_AND_SUPPORT_URL,
      localeLabel: 'account.helpAndSupport',
    },
    {
      url: ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'account.accessibilityStatement',
    },
  ];

export default {
  accountLinks,
  footerLinks,
};
