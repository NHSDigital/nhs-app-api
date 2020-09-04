import account from './account';
import apiErrors from './apiErrors';
import appointments from './appointments';
import components from './components';
import dataSharing from './dataSharing';
import generic from './generic';
import gpSessionErrors from './gpSessionErrors';
import login from './login';
import loginSettings from './loginSettings';
import messages from './messages';
import myRecord from './myRecord';
import navigation from './navigation';
import nominatedPharmacy from './nominatedPharmacy';
import onlineConsultations from './onlineConsultations';
import organDonation from './organDonation';
import prescriptions from './prescriptions';
import termsAndConditions from './termsAndConditions';
import thirdPartyProviders from './third-party-providers';
import userResearch from './userResearch';

export default {
  language: 'en-GB',
  appTitle: 'NHS App',
  errors: {
    referenceCode: 'Reference: {reference}',
    reportAProblemLink: 'Report a problem',
    tryAgainNow: 'Try again now.',
    404: {
      pageTitle: 'Page not found',
      header: 'Page not found',
      subheader: 'If you entered a web address, check it was correct.',
      message: {
        text: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need. For urgent medical advice, call 111.',
        label: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need. For urgent medical advice, call one one one.',
      },
    },
  },
  noConnection: {
    header: 'Internet connection error',
    subheader: 'There is a problem with your internet connection',
    retryButtonText: 'Try again',
    message: {
      text: 'Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      label: 'Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
    },
  },
  cookieBanner: {
    caption: {
      line1: 'We\'ve put some small files called cookies on your device. These are the strictly necessary cookies needed to make the NHS App work.',
      line2: 'We will not use any other cookies unless you choose to turn them on, as described in our ',
      linkText: 'cookies policy',
    },
  },
  biometricBanner: {
    header: 'Login options',
    message: {
      text: 'If your mobile device supports fingerprint or face recognition, you can use it to log in to the NHS App instead of a password and security code.',
      settingsButton: 'Open settings',
      dismissLink: 'Dismiss',
    },
  },
  gp_at_hand: {
    appointments: {
      headerTag: 'book GP appointments',
      contentTag: 'book appointments',
    },
    myRecord: {
      headerTag: 'access your GP medical record',
      contentTag: 'view your GP medical record',
    },
    prescriptions: {
      headerTag: 'order prescriptions',
      contentTag: 'order prescriptions',
    },
    content: {
      header: 'Sorry, you cannot {headerTag} through the NHS App',
      paragraphs: [
        {
          prefix: 'To {contentTag} with Babylon GP at Hand, please ',
          linkText: 'use the Babylon app',
          linkUrl: 'https://www.gpathand.nhs.uk/download-app',
          suffix: '.',
        },
        {
          prefix: 'Please contact Babylon GP at Hand on 0330 808 2217 or ',
          linkText: 'gpathand@nhs.net',
          linkUrl: 'mailto:gpathand@nhs.net',
          suffix: ' if you have any problems.',
        },
      ],
    },
  },
  homeLoggedIn: {
    welcome: 'Welcome',
    description: 'Get medical advice, book GP appointments and order repeat prescriptions any time.',
  },
  homeProxyMode: {
    informationHeaders: {
      age: 'Age',
      gpSurgery: 'GP surgery',
    },
  },
  linkedProfiles: {
    lossProxyError: 'Sorry, there is a problem with this service. It may not be possible to access services in the app on behalf of other people right now.',
    actingAsOtherUserBannerWarningText: 'Acting on behalf of',
    linkedInformation: 'You can access services in the app for the following people.',
    informationHeaders: {
      age: 'Age',
      gpPractice: 'GP practice',
    },
    ageLabels: {
      lessThanOneMonth: 'Less than 1 month old',
      oneMonth: ' month old',
      greaterThanOneMonthLessThan1Year: ' months old',
      oneYear: ' year old',
      greaterThanOneYearOld: ' years old',
    },
    switchProfileButton: 'Switch to {givenName}\'s profile',
    switchProfileButtonWithoutName: 'Switch to this profile',
    switchToMyProfileButton: 'Switch to my profile',
    featuresOnBehalfOf: {
      text: 'Services you can access for {fullName}',
      bookAnAppointment: 'Book a GP appointment',
      orderRepeatPrescription: 'Order a repeat prescription',
      viewMedicalRecord: 'View their GP health record',
    },
    featuresNoSummary: 'To access services in the app for {fullName}, you need to use their profile.',
    shutter: {
      prescriptions: {
        header: 'You do not have access to {name}\'s repeat prescriptions',
        summary: 'Contact {name}\'s GP surgery to request access.',
        switch: 'Switch to your profile to order repeat prescriptions for yourself.',
      },
      appointments: {
        header: 'You do not have access to {name}\'s GP appointments',
        summary: 'Contact {name}\'s GP surgery for more information. For urgent medical advice, go to 111.nhs.uk or call 111.',
        summaryLabel: 'Contact {name}\'s GP surgery for more information. For urgent medical advice, go to one one one.nhs.uk or call one one one.',
        coronaVirus: {
          header: 'If you think {name} might have coronavirus',
          body: 'They must stay at home and avoid close contact with other people.',
          link: 'Use the 111 coronavirus service to find out what to do',
          linkLabel: 'Use the one one one coronavirus service to find out what to do',
        },
        switch: 'Switch to your profile to book appointments for yourself.',
      },
      medicalRecord: {
        subHeader: 'You do not have access to {name}\'s health record',
        summary: 'Contact {name}\'s GP surgery to request access.',
        switch: 'Switch to your profile to view your GP health record.',
      },
      more: {
        header: 'More',
        summary: 'It\'s not possible to access this section while acting on {name}\'s behalf.',
        switch: 'Switch to your profile to access this section.',
      },
      settings: {
        header: 'Settings',
        switch: 'Switch to your profile to access your settings.',
      },
      symptoms: {
        header: 'Symptoms',
        summary: 'It\'s not possible to check your symptoms while acting on {name}\'s behalf.',
        switch: 'Switch to your profile to check your symptoms.',
      },
    },
  },
  switchProfile: {
    informationHeaders: {
      age: 'Age',
      gpPractice: 'GP surgery',
    },
    switchToMyProfileButton: 'Switch to my profile',
  },
  surveyBar: {
    barText: 'Help us make this service better.',
    linkText: ' Complete our quick survey.',
  },
  sy01: {
    pageHeader: 'Symptoms',
    corona: {
      subheader: 'Get advice about coronavirus',
      body: 'Find out what to do if you think you have coronavirus',
    },
    conditionsTreatments: {
      subheader: 'Search conditions and treatments',
      body: 'Find trusted NHS information on hundreds of conditions',
    },
    111: {
      subheader: 'Use NHS 111 online',
      subheaderAriaLabel: 'Use NHS one one one online',
      body: 'Check if you need urgent help and find out what to do next',
    },
    askGp: {
      forAdvice: 'Ask your GP for advice',
      consultThroughOnlineForm: 'Consult your GP through an online form. Your GP surgery will reply by phone or email',
    },
  },
  web: {
    sessionExpiry: {
      warningDurationInformation:
        'For security reasons, you\'ll be logged out in 1 minute. ' +
        '| For security reasons, you\'ll be logged out in {time} minutes.',
      warningGetMoreTime: 'Stay logged in',
      warningLogOut: 'Log out',
    },
    pageLeavingWarning: {
      header: 'Leave this page?',
      warning: 'If you have entered any information, it will not be saved.',
      stayButtonText: 'Stay on this page',
      leaveButtonText: 'Leave this page',
    },
  },
  careCard: {
    headingPrefix: {
      nonUrgent: 'Non-urgent advice:',
      urgent: 'Urgent advice:',
      immediate: 'Immediate advice:',
    },
  },
  coronaVirus: {
    title: 'Coronavirus',
    paragraphText1: 'Do not book a GP appointment if you think you might have coronavirus.',
    paragraphText2: 'Stay at home and avoid close contact with other people.',
    linkText1: 'Use the 111 coronavirus service to see if you need medical help',
  },
  coronaVirusBanner: {
    header: 'Coronavirus (COVID-19)',
    text: 'Get information about coronavirus on NHS.UK',
  },
  glossary: {
    headerText: 'You may see medical abbreviations that you are not familiar with.',
    linkText: 'Help with abbreviations',
  },
  account,
  apiErrors,
  appointments,
  components,
  dataSharing,
  generic,
  gpSessionErrors,
  login,
  loginSettings,
  messages,
  myRecord,
  navigation,
  nominatedPharmacy,
  onlineConsultations,
  organDonation,
  prescriptions,
  termsAndConditions,
  thirdPartyProviders,
  userResearch,
};
