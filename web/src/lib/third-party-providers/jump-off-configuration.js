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
      messages: {
        type: 'messages',
        redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages',
      },
    },
  },
};
