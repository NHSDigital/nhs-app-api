export default {
  warningConjunctions: {
    heading: 'This service is provided by {providerName}',
    paragraph: '{{ servicePurchaser }} has chosen this {{ serviceType }} provider.',
    button: 'Continue',
    linkText: 'Find out more about {{ serviceTypePlural }}',
  },
  engage: {
    serviceId: 'engage',
    providerName: 'Engage Health Systems Limited',
    jumpOffs: [
      {
        id: 'admin',
        path: '/?sso_route=admin',
        jumpOffContent: {
          headerText: 'Additional GP services',
          descriptionText: 'Get sick notes and GP letters or ask your GP surgery about something else',
        },
        thirdPartyWarning: {
          featureName: 'Additional GP services',
          servicePurchaser: 'Your GP surgery',
          serviceType: 'online consultation service',
          serviceTypePlural: 'online consultation services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/online-consultations/',
        },
      },
      {
        id: 'medical',
        path: '/?sso_route=medical',
        jumpOffContent: {
          headerText: 'Ask your GP for advice',
          descriptionText: 'Answer questions online and get a response from your GP surgery',
        },
        thirdPartyWarning: {
          featureName: 'Ask your GP for advice',
          servicePurchaser: 'Your GP surgery',
          serviceType: 'online consultation service',
          serviceTypePlural: 'online consultation services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/online-consultations/',
        },
      },
      {
        id: 'messages',
        path: '/?sso_route=messages',
        jumpOffContent: {
          headerText: 'Online consultations',
          descriptionText: 'View your online consultations and any responses from your GP surgery',
        },
        thirdPartyWarning: {
          featureName: 'Messages',
          servicePurchaser: 'Your GP surgery',
          serviceType: 'online consultation service',
          serviceTypePlural: 'online consultation services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/online-consultations/',
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
  gncr: {
    serviceId: 'gncr',
    providerName: 'Great North Care Record',
    jumpOffs: [
      {
        id: 'appointments',
        path: '/appointment',
        jumpOffContent: {
          headerText: 'View and manage your hospital and other appointments',
          descriptionText: 'This includes your hospital, mental health and social care appointments',
        },
        thirdPartyWarning: {
          featureName: 'Hospital and other appointments',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'correspondence',
        path: '/correspondence',
        jumpOffContent: {
          headerText: 'Hospital and other healthcare letters',
          descriptionText: 'This includes your hospital, mental health and social care letters and documents',
        },
        thirdPartyWarning: {
          featureName: 'Hospital and other healthcare letters',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'admin',
        path: '/patient/preferences',
        jumpOffContent: {
          headerText: 'Great North Care Record service preferences',
          descriptionText: '',
        },
        thirdPartyWarning: {
          featureName: 'Service preferences [TBC - GNCR testing]',
          servicePurchaser: 'service purchaser',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
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
        id: 'testResults',
        path: '/nhs-login/login?phrPath=/test/myTests.action',
        jumpOffContent: {
          headerText: 'Test results',
          descriptionText: 'View test results from your hospital and other healthcare providers',
        },
        thirdPartyWarning: {
          featureName: 'Test results',
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
          descriptionText: 'View your care plans from your hospital or other care provider, or add your own',
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
          headerText: 'Consultations, events and messages',
          descriptionText: 'See details of your visits and treatments, view clinical documents, message your health team, or fill in a consultation form',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Consultations, events and messages',
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
          headerText: 'Shared health links',
          descriptionText: 'View links or documents your health team has shared with you, or add your own',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Shared health links',
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
          headerText: 'Hospital and other medicines',
          descriptionText: 'View your current and past medicines or add a record of your own',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Hospital and other medicines',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'testResultsCie',
        path: '/nhs-login/login?phrPath=/test/myTests.action&brand=cie',
        jumpOffContent: {
          headerText: 'Test results',
          descriptionText: 'View test results from your hospital and other healthcare providers',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange (Patients Know Best)',
          featureName: 'Test results',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
    ],
  },
  substraktPatientPack: {
    serviceId: 'substraktPatientPack',
    providerName: 'Patient Pack (Substrakt Health)',
    jumpOffs: [
      {
        id: 'lifestyleGuides',
        path: '/lifestyle_guides',
        jumpOffContent: {
          headerText: 'Lifestyle Guides',
          descriptionText: 'Patient Pack Lifestyle Guides',
        },
        thirdPartyWarning: {
          featureName: 'Feature name',
          servicePurchaser: 'service purchaser',
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
