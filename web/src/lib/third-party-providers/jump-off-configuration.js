export default {
  thirdPartyProvider: {
    ers: {
      manageYourReferral: {
        type: 'manageYourReferral',
        redirectPath: '/nhslogin',
      },
    },
    pkb: {
      appointments: {
        type: 'appointments',
        redirectPath: '/nhs-login/login?phrPath=%2Fdiary%2FlistAppointments.action',
      },
      carePlans: {
        type: 'carePlans',
        redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FlistPlans.action',
      },
      healthTrackers: {
        type: 'healthTrackers',
        redirectPath: '/nhs-login/login?phrPath=%2FpkbNhsMenu.action',
      },
      messages: {
        type: 'messages',
        redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages',
      },
      sharedLinks: {
        type: 'sharedLinks',
        redirectPath: '/nhs-login/login?phrPath=%2Flibrary%2FmanageLibrary.action',
      },
      medicines: {
        type: 'medicines',
        redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FmanageMedications.action%3Ftab%3Dtreatments',
      },
      testResults: {
        type: 'testResults',
        redirectPath: '/nhs-login/login?phrPath=%2Ftest%2FmyTests.action',
      },
      appointmentsCie: {
        type: 'appointmentsCie',
        redirectPath: '/nhs-login/login?phrPath=%2Fdiary%2FlistAppointments.action&brand=cie',
      },
      carePlansCie: {
        type: 'carePlansCie',
        redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FlistPlans.action&brand=cie',
      },
      healthTrackersCie: {
        type: 'healthTrackersCie',
        redirectPath: '/nhs-login/login?phrPath=%2FpkbNhsMenu.action&brand=cie',
      },
      messagesCie: {
        type: 'messagesCie',
        redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages&brand=cie',
      },
      sharedLinksCie: {
        type: 'sharedLinksCie',
        redirectPath: '/nhs-login/login?phrPath=%2Flibrary%2FmanageLibrary.action&brand=cie',
      },
      medicinesCie: {
        type: 'medicinesCie',
        redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FmanageMedications.action%3Ftab%3Dtreatments&brand=cie',
      },
      testResultsCie: {
        type: 'testResultsCie',
        redirectPath: '/nhs-login/login?phrPath=%2Ftest%2FmyTests.action&brand=cie',
      },
    },
    substraktPatientPack: {
      lifestyleGuides: {
        type: 'lifestyleGuides',
        redirectPath: '/lifestyle_guides',
      },
    },
    gncr: {
      appointments: {
        type: 'appointments',
        redirectPath: '/appointment',
      },
    },
    engage: {
      admin: {
        type: 'admin',
        redirectPath: '/?sso_route=admin',
      },
      medical: {
        type: 'medical',
        redirectPath: '/?sso_route=medical',
      },
      messages: {
        type: 'messages',
        redirectPath: '/?sso_route=messages',
      },
    },
    testProvider: {
      messages: {
        type: 'messages',
        redirectPath: '/index.html',
      },
    },
  },
};
