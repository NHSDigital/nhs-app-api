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
        id: 'carePlans',
        path: '/nhs-login/login?phrPath=/auth/listPlans.action',
        jumpOffContent: {
          headerText: 'Care plans',
          descriptionText: 'View your personalised care plans',
        },
        thirdPartyWarning: {
          featureName: 'Care plans',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'healthTrackers',
        path: '/nhs-login/login?phrPath=/pkbNhsMenu.action',
        jumpOffContent: {
          headerText: 'Track your health',
          descriptionText: 'Record symptoms and add to your health journal',
        },
        thirdPartyWarning: {
          featureName: 'Track your health',
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
          headerText: 'Messages and consultations with a doctor or health professional',
          descriptionText: 'Send or view messages and online consultations with a doctor or health professional',
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
      {
        id: 'medicines',
        path: '/nhs-login/login?phrPath=/auth/manageMedications.action?tab=treatments',
        jumpOffContent: {
          headerText: 'Hospital and other prescriptions',
          descriptionText: 'See your current and past prescriptions',
        },
        thirdPartyWarning: {
          featureName: 'Hospital and other prescriptions',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'appointmentsCie',
        path: '/nhs-login/login?phrPath=/diary/listAppointments.action&brand=cie',
        jumpOffContent: {
          headerText: 'View appointments',
          descriptionText: 'See your upcoming and past hospital or other appointments',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'View appointments',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'carePlansCie',
        path: '/nhs-login/login?phrPath=/auth/listPlans.action&brand=cie',
        jumpOffContent: {
          headerText: 'Care plans',
          descriptionText: 'View your personalised care plans',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Care plans',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'healthTrackersCie',
        path: '/nhs-login/login?phrPath=/pkbNhsMenu.action&brand=cie',
        jumpOffContent: {
          headerText: 'Track your health',
          descriptionText: 'Record symptoms and add to your health journal',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Track your health',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'messagesCie',
        path: '/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages&brand=cie',
        jumpOffContent: {
          headerText: 'Messages and consultations with a doctor or health professional',
          descriptionText: 'Send or view messages and online consultations with a doctor or health professional',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Messages and online consultations',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'sharedLinksCie',
        path: '/nhs-login/login?phrPath=/library/manageLibrary.action&brand=cie',
        jumpOffContent: {
          headerText: 'Shared links',
          descriptionText: 'View links your doctor or health professional has shared with you',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Shared links',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'medicinesCie',
        path: '/nhs-login/login?phrPath=/auth/manageMedications.action?tab=treatments&brand=cie',
        jumpOffContent: {
          headerText: 'Hospital and other prescriptions',
          descriptionText: 'See your current and past prescriptions',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Hospital and other prescriptions',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
    ],
  },
  'silver-third-party-api-test': {
    serviceId: 'silver-third-party-api-test',
    providerName: 'Test Silver Third Party Provider',
    jumpOffs: [
      {
        id: 'messages',
        path: '/index.html',
        jumpOffContent: {
          headerText: 'Test Provider',
          descriptionText: 'For testing NHS App Javascript API',
        },
      },
    ],
  },
};
