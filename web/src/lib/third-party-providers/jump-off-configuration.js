export default {
  thirdPartyProvider: {
    ers: {
      manageYourReferral: {
        jumpOffId: 'manageYourReferral',
        redirectPath: '/nhslogin',
        provider: 'ers',
        serviceType: 'secondaryAppointments',
      },
    },
    pkb: {
      appointments: {
        jumpOffId: 'appointments',
        redirectPath: '/nhs-login/login?phrPath=/diary/listAppointments.action',
        provider: 'pkb',
        serviceType: 'secondaryAppointments',
      },
      carePlans: {
        jumpOffId: 'carePlans',
        redirectPath: '/nhs-login/login?phrPath=/auth/listPlans.action',
        provider: 'pkb',
        serviceType: 'carePlans',
      },
      healthTrackers: {
        jumpOffId: 'healthTrackers',
        redirectPath: '/nhs-login/login?phrPath=/pkbNhsMenu.action',
        provider: 'pkb',
        serviceType: 'healthTrackers',
      },
      messages: {
        jumpOffId: 'messages',
        redirectPath: '/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
        provider: 'pkb',
        serviceType: 'messages',
      },
      sharedLinks: {
        jumpOffId: 'sharedLinks',
        redirectPath: '/nhs-login/login?phrPath=/library/manageLibrary.action',
        provider: 'pkb',
        serviceType: 'libraries',
      },
      medicines: {
        jumpOffId: 'medicines',
        redirectPath: '/nhs-login/login?phrPath=/auth/manageMedications.action?tab=treatments',
        provider: 'pkb',
        serviceType: 'medicines',
      },
      testResults: {
        jumpOffId: 'testResults',
        redirectPath: '/nhs-login/login?phrPath=/test/myTests.action',
        provider: 'pkb',
        serviceType: 'testResults',
      },
      appointmentsCie: {
        jumpOffId: 'appointmentsCie',
        redirectPath: '/nhs-login/login?phrPath=/diary/listAppointments.action&brand=cie',
        provider: 'pkbCie',
        serviceType: 'secondaryAppointments',
      },
      carePlansCie: {
        jumpOffId: 'carePlansCie',
        redirectPath: '/nhs-login/login?phrPath=/auth/listPlans.action&brand=cie',
        provider: 'pkbCie',
        serviceType: 'carePlans',
      },
      healthTrackersCie: {
        jumpOffId: 'healthTrackersCie',
        redirectPath: '/nhs-login/login?phrPath=/pkbNhsMenu.action&brand=cie',
        provider: 'pkbCie',
        serviceType: 'healthTrackers',
      },
      messagesCie: {
        jumpOffId: 'messagesCie',
        redirectPath: '/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages&brand=cie',
        provider: 'pkbCie',
        serviceType: 'messages',
      },
      sharedLinksCie: {
        jumpOffId: 'sharedLinksCie',
        redirectPath: '/nhs-login/login?phrPath=/library/manageLibrary.action&brand=cie',
        provider: 'pkbCie',
        serviceType: 'libraries',
      },
      medicinesCie: {
        jumpOffId: 'medicinesCie',
        redirectPath: '/nhs-login/login?phrPath=/auth/manageMedications.action?tab=treatments&brand=cie',
        provider: 'pkbCie',
        serviceType: 'medicines',
      },
      testResultsCie: {
        jumpOffId: 'testResultsCie',
        redirectPath: '/nhs-login/login?phrPath=/test/myTests.action&brand=cie',
        provider: 'pkbCie',
        serviceType: 'testResults',
      },
    },
    substraktPatientPack: {
      lifestyleGuides: {
        jumpOffId: 'lifestyleGuides',
        redirectPath: '/lifestyle_guides',
        provider: 'substraktPatientPack',
        serviceType: 'carePlans',
      },
    },
    gncr: {
      appointments: {
        jumpOffId: 'appointments',
        redirectPath: '/appointment',
        provider: 'gncr',
        serviceType: 'secondaryAppointments',
      },
      correspondence: {
        jumpOffId: 'correspondence',
        redirectPath: '/correspondence',
        provider: 'gncr',
        serviceType: 'messages',
      },
      admin: {
        jumpOffId: 'admin',
        redirectPath: '/patient/preferences',
        provider: 'gncr',
        serviceType: 'accountAdmin',
      },
    },
    engage: {
      admin: {
        jumpOffId: 'admin',
        redirectPath: '/?sso_route=admin',
        provider: 'engage',
        serviceType: 'consultationsAdmin',
      },
      medical: {
        jumpOffId: 'medical',
        redirectPath: '/?sso_route=medical',
        provider: 'engage',
        serviceType: 'consultations',
      },
      messages: {
        jumpOffId: 'messages',
        redirectPath: '/?sso_route=messages',
        provider: 'engage',
        serviceType: 'messages',
      },
    },
    'silver-third-party-api-test': {
      messages: {
        jumpOffId: 'messages',
        redirectPath: '/index.html',
      },
    },
  },
};
