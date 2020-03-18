export default {
  warningConjunctions: {
    heading2: 'This is a connected service',
    paragraph: '{{ servicePurchaser }} has chosen this {{ serviceType }} provided by',
    button: 'Continue',
    linkText: 'Find out more about {{ serviceTypePlural }}.',
  },
  ers: {
    serviceId: 'ers',
    providerName: 'Electronic Referral Service',
    jumpOffs: [
      {
        id: 'manageYourReferral',
        path: '/nhslogin',
        jumpOffContent: {
          headerText: 'Book or cancel your referral appointment',
          descriptionText: 'If you\'ve had a referral, '
            + 'you can book or cancel your first appointment here',
        },
      },
    ],
  },
  pkb: {
    serviceId: 'pkb',
    providerName: 'Patients Know Best',
    jumpOffs: [
      {
        id: 'messages',
        path: '/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
        jumpOffContent: {
          headerText: 'Messages and online consultations',
          descriptionText: 'Message your healthcare team, or answer questions online ' +
            'and get a response from a health professional',
        },
        thirdPartyWarning: {
          featureName: 'Messages and online consultations',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/privacy/personal-health-records',
        },
      },
    ],
  },
};
