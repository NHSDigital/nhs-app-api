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
      localeLabel: 'more.helpAndSupport',
    },
    {
      url: ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'more.accessibilityStatement',
    },
    {
      url: OPEN_SOURCE_LICENCES_URL,
      localeLabel: 'more.openSourceLicences',
    },
    {
      url: PRIVACY_POLICY_URL,
      localeLabel: 'more.privacyPolicy',
    },
    {
      url: TERMS_AND_CONDITIONS_URL,
      localeLabel: 'more.termsOfUse',
    },
  ];

export const footerLinks = () =>
  [
    {
      url: TERMS_AND_CONDITIONS_URL,
      localeLabel: 'more.termsOfUse',
    },
    {
      url: PRIVACY_POLICY_URL,
      localeLabel: 'more.privacyPolicy',
    },
    {
      url: HELP_AND_SUPPORT_URL,
      localeLabel: 'more.helpAndSupport',
    },
    {
      url: ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'more.accessibilityStatement',
    },
  ];

export default {
  accountLinks,
  footerLinks,
};
