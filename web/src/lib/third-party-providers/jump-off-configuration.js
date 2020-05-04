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
    },
    testProvider: {
      messages: {
        type: 'messages',
        redirectPath: '/index.html',
      },
    },
  },
};
