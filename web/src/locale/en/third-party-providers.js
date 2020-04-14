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
        id: 'appointments',
        path: '/nhs-login/login?phrPath=/diary/listAppointments.action',
        jumpOffContent: {
          headerText: 'View appointments',
          descriptionText: 'See your upcoming and past hospital or other appointments',
        },
        thirdPartyWarning: {
          featureName: 'View appointments',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
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
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'sharedLinks',
        path: '/nhs-login/login?phrPath=/library/manageLibrary.action',
        jumpOffContent: {
          headerText: 'Shared links',
          descriptionText: 'View links your doctor or health professional has shared with you',
        },
        thirdPartyWarning: {
          featureName: 'Shared links',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
    ],
  },
};
