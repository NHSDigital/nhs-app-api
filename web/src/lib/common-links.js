import {
  ACCESSIBILITY_STATEMENT_URL,
  PRIVACY_POLICY_URL,
  TERMS_AND_CONDITIONS_URL,
} from '@/router/externalLinks';

export const footerLinks = env =>
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
      url: env.BASE_NHS_APP_HELP_URL,
      localeLabel: 'more.helpAndSupport',
    },
    {
      url: ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'more.accessibilityStatement',
    },
  ];

export default {
  footerLinks,
};
