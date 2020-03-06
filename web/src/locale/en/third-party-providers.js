export default {
  warningConjunctions: {
    h2: 'This is a connected service',
    p: '{{ servicePurchaser }} has chosen this {{ serviceType }} provided by',
    button: 'Continue',
    a: 'Find out more about {{ serviceType }}.',
  },
  pkb: {
    serviceId: 'pkb',
    providerName: 'Patients know Best',
    jumpOffs: [
      {
        id: 'messages',
        path: '/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages',
        jumpOffContent: {
          headerText: 'Messages and online consultations',
          descriptionText: 'Message your healthcare team, or answer questions online ' +
            'and get a response from a health professional',
        },
        thirdPartyWarning: {
          featureName: 'Third party warning',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
        },
      },
    ],
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
};
