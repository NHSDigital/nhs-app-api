import get from 'lodash/fp/get';
import proofLevel from '@/lib/proofLevel';

const thirdPartyProvider = {
  ers: {
    manageYourReferral: {
      acceptablePathsRegex: '^\\/nhslogin(\\/?\\?.*)?$',
      jumpOffId: 'manageYourReferral',
      redirectPath: '/nhslogin',
      provider: 'ers',
      serviceType: 'secondaryAppointments',
    },
    manageYourReferralWayfinder: {
      acceptablePathsRegex: '^\\/nhslogin(\\/?\\?.*)?$',
      jumpOffId: 'manageYourReferralWayfinder',
      redirectPath: '/nhslogin',
      provider: 'ers',
      serviceType: 'secondaryAppointments',
    },
  },
  pkb: {
    appointments: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/diary\\/listAppointments\\.action.*$',
      jumpOffId: 'appointments',
      redirectPath: '/nhs-login/login?phrPath=%2Fdiary%2FlistAppointments.action',
      provider: 'pkb',
      serviceType: 'secondaryAppointments',
    },
    carePlans: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/auth\\/listPlans\\.action.*$',
      jumpOffId: 'carePlans',
      redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FlistPlans.action',
      provider: 'pkb',
      serviceType: 'carePlans',
    },
    healthTrackers: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/pkbNhsMenu\\.action.*$',
      jumpOffId: 'healthTrackers',
      redirectPath: '/nhs-login/login?phrPath=%2FpkbNhsMenu.action',
      provider: 'pkb',
      serviceType: 'healthTrackers',
    },
    messages: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/auth\\/getInbox\\.action.*$',
      jumpOffId: 'messages',
      redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FgetInbox.action%3Ftab%3Dmessages',
      provider: 'pkb',
      serviceType: 'messages',
    },
    sharedLinks: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/library\\/manageLibrary\\.action.*$',
      jumpOffId: 'sharedLinks',
      redirectPath: '/nhs-login/login?phrPath=%2Flibrary%2FmanageLibrary.action',
      provider: 'pkb',
      serviceType: 'libraries',
    },
    medicines: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/auth\\/manageMedications\\.action.*$',
      jumpOffId: 'medicines',
      redirectPath: '/nhs-login/login?phrPath=%2Fauth%2FmanageMedications.action%3Ftab%3Dtreatments',
      provider: 'pkb',
      serviceType: 'medicines',
    },
    recordSharing: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/patient\\/myConsentTeam\\.action.*$',
      jumpOffId: 'recordSharing',
      redirectPath: '/nhs-login/login?phrPath=%2Fpatient%2FmyConsentTeam.action%3Ftab%3Dinvitations%26subTab%3DmyClinicians',
      provider: 'pkb',
      serviceType: 'recordSharing',
    },
    testResults: {
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/pkbNhsResultsMenu\\.action.*$',
      jumpOffId: 'testResults',
      redirectPath: '/nhs-login/login?phrPath=%2FpkbNhsResultsMenu.action',
      provider: 'pkb',
      serviceType: 'testResults',
    },
  },
  substraktPatientPack: {
    messages: {
      acceptablePathsRegex: '^\\/jump\\/forms(\\/?\\?.*)?$',
      jumpOffId: 'messages',
      redirectPath: '/jump/forms',
      provider: 'substraktPatientPack',
      serviceType: 'messages',
    },
    accountAdmin: {
      acceptablePathsRegex: '^\\/jump\\/update-details(\\/?\\?.*)?$',
      jumpOffId: 'accountAdmin',
      redirectPath: '/jump/update-details',
      provider: 'substraktPatientPack',
      serviceType: 'accountAdmin',
    },
    patientParticipationGroups: {
      acceptablePathsRegex: '^\\/jump\\/join-ppg(\\/?\\?.*)?$',
      jumpOffId: 'patientParticipationGroups',
      redirectPath: '/jump/join-ppg',
      provider: 'substraktPatientPack',
      serviceType: 'participation',
    },
  },
  gncr: {
    appointments: {
      acceptablePathsRegex: '^\\/appointment(\\/?\\?.*)?$',
      jumpOffId: 'appointments',
      redirectPath: '/appointment',
      provider: 'gncr',
      serviceType: 'secondaryAppointments',
    },
    correspondence: {
      acceptablePathsRegex: '^\\/correspondence(\\/?\\?.*)?$',
      jumpOffId: 'correspondence',
      redirectPath: '/correspondence',
      provider: 'gncr',
      serviceType: 'messages',
    },
    admin: {
      acceptablePathsRegex: '^\\/patient\\/preferences(\\/?\\?.*)?$',
      jumpOffId: 'admin',
      redirectPath: '/patient/preferences',
      provider: 'gncr',
      serviceType: 'accountAdmin',
    },
  },
  engage: {
    admin: {
      acceptablePathsRegex: '^\\/\\?sso_route=admin$',
      jumpOffId: 'admin',
      redirectPath: '/?sso_route=admin',
      provider: 'engage',
      serviceType: 'consultationsAdmin',
    },
    medical: {
      acceptablePathsRegex: '^\\/\\?sso_route=medical$',
      jumpOffId: 'medical',
      redirectPath: '/?sso_route=medical',
      provider: 'engage',
      serviceType: 'consultations',
    },
    messages: {
      acceptablePathsRegex: '^\\/\\?sso_route=messages$',
      jumpOffId: 'messages',
      redirectPath: '/?sso_route=messages',
      provider: 'engage',
      serviceType: 'messages',
    },
  },
  'silver-third-party-api-test': {
    messages: {
      acceptablePathsRegex: '^\\/index.html(\\/?\\?.*)?$',
      jumpOffId: 'messages',
      redirectPath: '/index.html',
      provider: 'testSilverThirdPartyProvider',
      serviceType: 'messages',
    },
  },
  nhsd: {
    vaccineRecord: {
      acceptablePathsRegex: '^\\/sso(\\/?\\?.*)?$',
      jumpOffId: 'vaccineRecord',
      redirectPath: '/sso',
      provider: 'nhsd',
      serviceType: 'vaccineRecord',
    },
  },
  netCompany: {
    vaccineRecord: {
      acceptablePathsRegex: '^\\/covid-status-sso$',
      jumpOffId: 'vaccineRecord',
      redirectPath: '/covid-status-sso',
      provider: 'netCompany',
      serviceType: 'vaccineRecord',
    },
    vaccineRecordP5: {
      acceptablePathsRegex: '^\\/covid-status-sso\\?proofLevel=p5$',
      jumpOffId: 'vaccineRecordP5',
      redirectPath: '/covid-status-sso?proofLevel=p5',
      provider: 'netCompanyP5',
      serviceType: 'vaccineRecord',
      proofLevel: proofLevel.P5,
    },
  },
  wellnessAndPrevention: {
    healthTrackers: {
      jumpOffId: 'wellnessAndPrevention',
      redirectPath: '/sso',
      provider: 'wellnessAndPrevention',
      serviceType: 'healthTrackers',
    },
  },
  accurx: {
    medical: {
      acceptablePathsRegex: '^\\/api\\/OpenIdConnect\\/AuthenticatePatientTriage\\?requestType=medical$',
      jumpOffId: 'medical',
      redirectPath: '/api/OpenIdConnect/AuthenticatePatientTriage?requestType=medical',
      provider: 'accurx',
      serviceType: 'consultations',
    },
    messages: {
      acceptablePathsRegex: '^\\/api\\/OpenIdConnect\\/AuthenticatePatientTriage\\?requestType=admin$',
      jumpOffId: 'messages',
      redirectPath: '/api/OpenIdConnect/AuthenticatePatientTriage?requestType=admin',
      provider: 'accurx',
      serviceType: 'messages',
    },
  },
  wayfinder: {
    ers: {
      jumpOffId: 'ers',
      acceptablePathsRegex: '^\\/nhslogin(\\/?\\?.*)?$',
      redirectPath: '/nhslogin?',
    },
    pkb: {
      jumpOffId: 'pkb',
      acceptablePathsRegex: '^\\/nhs-login\\/login\\?phrPath=\\/diary\\/viewAppointment\\.action.*$',
      redirectPath: '/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action?',
    },
  },
};

export const getJumpOffConfiguration = id => get(id)(thirdPartyProvider);

export default { thirdPartyProvider };
