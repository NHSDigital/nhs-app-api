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
          linkHref: 'ONLINE_CONSULTATIONS_PRIVACY_URL',
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
          linkHref: 'ONLINE_CONSULTATIONS_PRIVACY_URL',
        },
      },
      {
        id: 'messages',
        jumpOffContent: {
          headerText: 'Online consultations',
          descriptionText: 'View your online consultations and any responses from your GP surgery',
        },
        thirdPartyWarning: {
          featureName: 'Online consultations',
          servicePurchaser: 'Your GP surgery',
          serviceType: 'online consultation service',
          serviceTypePlural: 'online consultation services',
          linkHref: 'ONLINE_CONSULTATIONS_PRIVACY_URL',
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
      {
        id: 'manageYourReferralWayfinder',
        jumpOffContent: {
          headerText: 'View and manage other referrals and appointments',
          descriptionText: 'If you have a referral, you can manage your ' +
            'first appointment in the NHS e-Referral Service',
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
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'appointmentsWayfinder',
        jumpOffContent: {
          headerText: 'View and manage other appointments',
          descriptionText: 'This includes other hospital, mental health and social care appointments currently not shown',
        },
      },
      {
        id: 'correspondence',
        jumpOffContent: {
          headerText: 'Hospital and other healthcare documents',
          descriptionText: 'View letters and documents from your hospital, mental health or social care teams',
        },
        thirdPartyWarning: {
          featureName: 'Hospital and other healthcare documents',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'admin',
        jumpOffContent: {
          headerText: 'Great North Care Record preferences',
          descriptionText: '',
        },
        thirdPartyWarning: {
          featureName: 'Great North Care Record preferences',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service ',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
    ],
  },
  netCompany: {
    serviceId: 'netCompany',
    providerName: 'the Department of Health and Social Care',
    jumpOffs: [
      {
        id: 'vaccineRecord',
        jumpOffContent: {
          headerText: 'NHS COVID Pass',
          descriptionText: 'View and download your COVID Pass for travel abroad',
        },
        thirdPartyWarning: {
          featureName: 'Get your NHS COVID Pass',
          serviceTypePlural: 'this service',
          linkHref: 'https://www.nhs.uk/nhs-app/nhs-app-help-and-support/health-records-in-the-nhs-app/about-the-nhs-covid-pass-service/',
        },
      },
      {
        id: 'vaccineRecordP5',
        jumpOffContent: {
          headerText: 'NHS COVID Pass',
          descriptionText: 'View and download your COVID Pass for travel abroad',
        },
      },
    ],
  },
  nhsd: {
    serviceId: 'nhsd',
    providerName: 'NHS Digital',
    jumpOffs: [
      {
        id: 'vaccineRecord',
        jumpOffContent: {
          headerText: 'Check your COVID-19 vaccine record',
          descriptionText: 'View your vaccination details, like the name and batch number, and report any side effects you have experienced',
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
          descriptionText: 'See your upcoming and past hospital and other appointments',
        },
        thirdPartyWarning: {
          featureName: 'View appointments',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'carePlans',
        jumpOffContent: {
          headerText: 'Care plans',
          descriptionText: 'View your care plans from your hospital or other care provider, or add your own',
        },
        thirdPartyWarning: {
          featureName: 'Care plans',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
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
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'messages',
        jumpOffContent: {
          headerText: 'Consultations, events and messages',
          descriptionText: 'See details of your visits and treatments, view clinical documents, message your health team, or fill in a consultation form',
        },
        thirdPartyWarning: {
          featureName: 'Messages and online consultations',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'sharedLinks',
        jumpOffContent: {
          headerText: 'Shared health links',
          descriptionText: 'View links your doctor or health professional has shared with you',
        },
        thirdPartyWarning: {
          featureName: 'Shared health links',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'medicines',
        jumpOffContent: {
          headerText: 'Hospital and other medicines',
          descriptionText: 'View your current and past medicines or add a record of your own',
        },
        thirdPartyWarning: {
          featureName: 'Hospital and other medicines',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'recordSharing',
        jumpOffContent: {
          headerText: 'Record sharing',
          descriptionText: 'Choose and manage information you share with your health teams',
        },
        thirdPartyWarning: {
          featureName: 'Record Sharing',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'testResults',
        jumpOffContent: {
          headerText: 'Test results and imaging',
          descriptionText: 'View test results and imaging from your hospital and other healthcare providers, or add your own',
        },
        thirdPartyWarning: {
          featureName: 'Test results and imaging',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
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
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
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
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
        },
      },
      {
        id: 'patientParticipationGroups',
        jumpOffContent: {
          headerText: 'Join a patient participation group',
          descriptionText: '',
        },
        thirdPartyWarning: {
          featureName: 'Join a patient participation group',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
          serviceTypePlural: 'personal health record services',
          linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
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
  wellnessAndPrevention: {
    serviceId: 'wellnessAndPrevention',
    providerName: 'Wellness and Prevention',
    jumpOffs: [
      {
        id: 'wellnessAndPrevention',
        jumpOffContent: {
          headerText: 'Wellness and Prevention',
          descriptionText: 'Sample jump off for Wellness and Prevention',
        },
        thirdPartyWarning: {
          featureName: 'Third Party Feature Name',
          servicePurchaser: 'Service Purchaser',
          serviceType: 'Service Type',
          serviceTypePlural: 'Service Type Plural',
          linkHref: 'WELLNESS_AND_PREVENTION_PRIVACY_URL',
        },
      },
    ],
  },
  accurx: {
    serviceId: 'accurx',
    providerName: 'accuRx Limited',
    jumpOffs: [
      {
        id: 'medical',
        jumpOffContent: {
          headerText: 'Ask your GP for medical advice',
          descriptionText: 'Answer questions online and get a response from your GP surgery',
        },
        thirdPartyWarning: {
          featureName: 'Ask your GP for medical advice',
          servicePurchaser: 'Your GP surgery',
          serviceType: 'online consultation service',
          serviceTypePlural: 'online consultation services',
          linkHref: 'ONLINE_CONSULTATIONS_PRIVACY_URL',
        },
      },
      {
        id: 'messages',
        jumpOffContent: {
          headerText: 'Ask your GP surgery a question',
          descriptionText: 'Fill out a form to send a request, get advice or ask a question',
        },
        thirdPartyWarning: {
          featureName: 'Ask your GP surgery a question',
          servicePurchaser: 'Your GP surgery',
          serviceType: 'online consultation service',
          serviceTypePlural: 'online consultation services',
          linkHref: 'ONLINE_CONSULTATIONS_PRIVACY_URL',
        },
      },
    ],
  },
  wayfinder: {
    id: 'wayfinder',
    jumpOffContent: {
      headerText: 'View appointments',
      descriptionText: '',
    },
    wayfinderJumpOffs: {
      ers: {
        serviceId: 'ers',
        providerName: 'Electronic Referral Service',
      },
      drDoctor: {
        serviceId: 'drDoctor',
        providerName: 'Dr Doctor',
      },
      pkb: {
        serviceId: 'pkb',
        providerName: 'Patients Know Best',
      },
    },
    thirdPartyWarning: {
      featureName: 'View appointments',
      servicePurchaser: 'Your GP surgery or hospital',
      serviceType: 'personal health record service',
      serviceTypePlural: 'personal health record services',
      linkHref: 'PERSONAL_HEALTH_RECORDS_PRIVACY_URL',
    },
  },
};
