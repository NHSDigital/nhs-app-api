export default {
  errors: {
    silverIntegrationNotAvailable: {
      heading: 'This service is not available',
      contactYourGpSurgery: 'If you need to access this service, contact your GP surgery for more information.',
      goToNHSAppHome: 'Go to NHS App homepage',
    },
  },
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
        jumpOffContent: {
          headerText: 'Hospital and other medicines',
          descriptionText: 'View your current and past medicines or add a record of your own',
        },
        thirdPartyWarning: {
          brandName: 'Care Information Exchange powered by Patients Know Best',
          featureName: 'Hospital and other medicines',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'testResultsCie',
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
      {
        id: 'medicinesPkbSecondaryCare',
        jumpOffContent: {
          headerText: 'Hospital and other medicines',
          descriptionText: 'View your current and past medicines or add a record of your own',
        },
        thirdPartyWarning: {
          featureName: 'Hospital and other medicines',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'medicinesPkbMyCareView',
        jumpOffContent: {
          headerText: 'Hospital and other medicines',
          descriptionText: 'View your current and past medicines or add a record of your own',
        },
        thirdPartyWarning: {
          brandName: 'MyCareView powered by Patients Know Best',
          featureName: 'Hospital and other medicines',
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
    providerName: 'Substrakt Health',
    jumpOffs: [
      {
        id: 'messages',
        jumpOffContent: {
          headerText: 'Ask your GP surgery a question',
          descriptionText: 'Fill out a form to send a request, get advice or ask a question',
        },
        thirdPartyWarning: {
          featureName: 'Ask your GP surgery a question',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'accountAdmin',
        jumpOffContent: {
          headerText: 'Update your personal details',
          descriptionText: 'Fill out a form to let your GP surgery know which details have changed',
        },
        thirdPartyWarning: {
          featureName: 'Update your personal details',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
      {
        id: 'patientParticipationGroups',
        jumpOffContent: {
          headerText: 'Patient participation groups',
          descriptionText: 'Take part in our patient participation groups to help shape and improve the patient experience',
        },
        thirdPartyWarning: {
          featureName: 'Patient participation groups',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/personal-health-records/',
        },
      },
    ],
  },
  silver_third_party_api_test: {
    serviceId: 'silver-third-party-api-test',
    providerName: 'Test Silver Third Party Provider',
    jumpOffs: [
      {
        id: 'messages',
        jumpOffContent: {
          headerText: 'Test Provider',
          descriptionText: 'For testing NHS App Javascript API',
        },
      },
    ],
  },
};
