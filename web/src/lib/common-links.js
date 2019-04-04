
export const accountLinks = env =>
  [
    {
      url: env.TERMS_AND_CONDITIONS_URL,
      localeLabel: 'myAccount.termsAndConditions',
    },
    {
      url: env.PRIVACY_POLICY_URL,
      localeLabel: 'myAccount.privacyPolicy',
    },
    {
      url: env.COOKIES_POLICY_URL,
      localeLabel: 'myAccount.cookiesPolicy',
    },
    {
      url: env.OPEN_SOURCE_LICENCES_URL,
      localeLabel: 'myAccount.openSourceLicences',
    },
    {
      url: env.HELP_AND_SUPPORT_URL,
      localeLabel: 'myAccount.helpAndSupport',
    },
    {
      url: env.ACCESSIBILITY_STATEMENT,
      localeLabel: 'myAccount.accessibilityStatement',
    },
  ];

export const footerLinks = env =>
  [
    {
      url: env.TERMS_AND_CONDITIONS_URL,
      localeLabel: 'myAccount.termsAndConditions',
    },
    {
      url: env.PRIVACY_POLICY_URL,
      localeLabel: 'myAccount.privacyPolicy',
    },
    {
      url: env.HELP_AND_SUPPORT_URL,
      localeLabel: 'myAccount.helpAndSupport',
    },
    {
      url: env.ACCESSIBILITY_STATEMENT,
      localeLabel: 'myAccount.accessibilityStatement',
    },
  ];

export default {
  accountLinks,
  footerLinks,
};
