
export const accountLinks = env =>
  [
    {
      url: env.HELP_AND_SUPPORT_URL,
      localeLabel: 'myAccount.helpAndSupport',
    },
    {
      url: env.ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'myAccount.accessibilityStatement',
    },
    {
      url: env.OPEN_SOURCE_LICENCES_URL,
      localeLabel: 'myAccount.openSourceLicences',
    },
    {
      url: env.PRIVACY_POLICY_URL,
      localeLabel: 'myAccount.privacyPolicy',
    },
    {
      url: env.TERMS_AND_CONDITIONS_URL,
      localeLabel: 'myAccount.termsAndConditions',
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
      url: env.ACCESSIBILITY_STATEMENT_URL,
      localeLabel: 'myAccount.accessibilityStatement',
    },
  ];

export const cookieLink = env => [{
  url: env.COOKIES_POLICY_URL,
  localeLabel: 'myAccount.cookiesPolicy',
}];

export default {
  accountLinks,
  footerLinks,
  cookieLink,
};
