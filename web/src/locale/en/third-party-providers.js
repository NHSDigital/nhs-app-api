export default {
  warningConjunctions: {
    h2: 'This is a connected service',
    p: '{{ servicePurchaser }} has chosen this provided {{ serviceType }} by',
    button: 'Continue',
    a: 'Find out more about {{ serviceType }}.',
  },
  pkb: {
    serviceId: 'pkb',
    providerName: 'Patient knows best',
    jumpOffs: [
      {
        id: 'messages',
        path: 'nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftag%3D',
        jumpOffContent: {
          headerText: 'Messages and consultations',
          descriptionText: 'Message your doctor or healthcare professional through ' +
            'Patients Know Best',
        },
        thirdPartyWarning: {
          featureName: 'Third party warning',
          servicePurchaser: 'Your GP surgery or hospital',
          serviceType: 'personal health record service',
        },
      },
    ],
  },
};
