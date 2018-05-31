export default {
  errors: {
    pageHeader: 'Server error',
    header: 'Sorry, we\'re experiencing technical difficulties',
    subheader: 'Please try again later.',
    message: 'If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For immediate medical advice, call 111.',
    502: {
      pageHeader: 'Service currently unavailable',
      header: 'Sorry, this service is unavailable right now',
      retryButtonText: 'Please try again later.',
      message: 'If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For immediate medical advice, call 111.',
    },
  },
  noConnection: {
    header: 'Connection error.',
    subheader: 'Please check your internet connection and try again.',
    retryButtonText: 'Try again',
    message: 'Please try again later. If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For immediate medical advice, call 111.',
  },
  appointments: {
    header: {
      title: 'Book an appointment',
    },
    noSlotErrorMessage: {
      summary: 'There are no appointments available at the moment',
      info: 'If you need an appointment, please contact your GP.',
    },
    noConnection: {
      message:
        'If the problem persists and you need to book an appointment immediately please contact your GP practice.',
    },
    bookAppointmentButtonText: 'Book this appointment',
    confirmation: {
      headerLabel: 'What is this appointment for?',
      label: 'Describe your symptoms',
      changeButtonText: 'Change this appointment',
    },
    errors: {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem loading this page',
      subheader: '',
      message: 'Please try again later. If the problem persists and you need to book an appointment now, contact your GP surgery directly.',
      504: {
        message: 'If the problem persists and you need to book an appointment now, contact your GP surgery directly.',
        subheader: 'Please try again',
        retryButtonText: 'Try again',
      },
    },
  },
  prescriptions: {
    orderRepeatPrescriptionButton: 'Order a repeat prescription',
    noPrescriptionsAvailable: {
      title: 'Looks like you have no repeat prescriptions ordered here.',
      contactGp:
        'If you think you have a pending repeat prescription, please contact your GP.',
      orderRepeatPrescription:
        'Otherwise you can order a new repeat prescription now.',
    },
    backToYourPrescriptionsButton: 'Back to my prescriptions',
    noRepeatPrescriptionsYouCanOrder: {
      title: "You don't have any medications available to order right now.",
      contactGp:
        "If you have medications available on repeat prescription that aren't showing here, contact your GP surgery for more information.",
    },
    myRepeatPrescriptionLabels: {
      orderDate: 'Order date',
    },
  },
  common: {
    or: 'OR',
  },
  homeHeader: {
    welcome: 'Welcome!',
  },
  homeLoggedIn: {
    welcome: 'Welcome',
    description:
      'Get medical advice, book GP appointments and order repeat prescriptions any time.',
  },
  loginButton: {
    login: 'Sign in with your NHS account',
  },
  loginOrRegister: {
    createAccount: 'Create an NHS account',
  },
  signOutButton: {
    signOut: 'Sign out',
  },
  pageHeaderTitles: {
    home: 'Home',
    prescriptions: 'My repeat prescriptions',
    repeatPrescriptionCourses: 'Order a repeat prescription',
    account: 'My account',
    appointments: 'Appointments',
    appointmentConfirmation: 'Check appointment details',
    myRecord: 'My record',
    more: 'More',
  },
  more: {
    organDonationButtonText: 'Organ donation register',
  },
  navigationMenu: {
    appointmentsLabel: 'Appointments',
    moreLabel: 'More',
    myRecordLabel: 'My record',
    prescriptionsLabel: 'Prescriptions',
    symptomsLabel: 'Symptoms',
  },
  symptomBanner: {
    howAreYouFeeling: 'How are you feeling right now?',
    checker: 'Check your symptoms',
  },
  icons: {
    accountIcon: {
      title: 'My Account',
    },
    appointmentsIcon: {
      title: 'Appointments',
    },
    clinicianIcon: {
      title: 'Clinician',
      description: 'Clinician Graphic',
    },
    homeIcon: {
      title: 'NHS Online',
      desc: 'Click here to go back to the home screen.',
    },
    locationIcon: {
      title: 'Location',
      description: 'Location Graphic',
    },
    moreIcon: {
      title: 'More',
    },
    myRecordIcon: {
      title: 'My Record',
    },
    nhsLogoIcon: {
      title: 'NHS app',
    },
    patientDetailsIcon: {
      title: 'Patient Details',
    },
    prescriptionsIcon: {
      title: 'Prescriptions',
    },
    sessionExpired: {
      title: 'Session Expired. Please Login again.',
    },
    symptomsIcon: {
      title: 'Symptoms Checker',
    },
  },
};
