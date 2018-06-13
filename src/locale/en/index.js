export default {
  language: 'en',
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
    403: {
      pageHeader: 'Service unavailable',
      header: 'Sorry, you don\'t currently have access to this service',
      subheader: '',
      message: 'Contact your GP surgery for more information.',
    },
  },
  noConnection: {
    header: 'Connection error.',
    subheader: 'Please check your internet connection and try again.',
    retryButtonText: 'Try again',
    message: 'Please try again later. If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For immediate medical advice, call 111.',
  },
  appointments: {
    index: {
      successText: 'Appointment Booked',
      bookButtonText: 'Book new appointment',
      empty: {
        header: 'You don\'t currently have any appointments booked',
        text1: 'Once you\'ve booked an appointment here, you\'ll be able to view details, cancel it and see your appointment history.',
        text2: 'If you have an upcoming appointment that isn\'t shown here, contact your GP surgery for more information.',
      },
    },
    noSlotErrorMessage: {
      summary: 'There are no appointments available at the moment',
      info: 'If you need an appointment, please contact your GP.',
    },
    noConnection: {
      message:
        'If the problem persists and you need to book an appointment immediately please contact your GP practice.',
    },
    booking: {
      bookButtonText: 'Book this appointment',
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
    confirmation: {
      headerLabel: 'What is this appointment for?',
      label: 'Describe your symptoms',
      maxReasonDesc: 'Description must be shorter than 150 characters (about 25 words)',
      confirmButtonText: 'Confirm and book appointment',
      changeButtonText: 'Change this appointment',
      noReasonDialogError: 'Please, enter a reason for this appointment',
      noReasonError: 'Please describe your symptoms',
    },
  },
  prescriptions: {
    myRepeatPrescriptions: {
      orderDate: 'Order date',
      orderRepeatPrescriptionButton: 'Order a repeat prescription',
      orderSuccessText: 'Your prescription has been ordered. The order status will be updated once it\'s been reviewed by your GP.',
    },
    repeatCourses: {
      noMedicinesSelected: 'Select at least one medicine',
      specialRequestsLabel: 'Special requests relating to this order (optional)',
      backToYourPrescriptionsButton: 'Back',
      continue: 'Continue',
    },
    noPrescriptionsAvailable: {
      title: 'Looks like you have no repeat prescriptions ordered here.',
      contactGp: 'If you think you have a pending repeat prescription, please contact your GP.',
      orderRepeatPrescription: 'Otherwise you can order a new repeat prescription now.',
    },
    noRepeatPrescriptionsYouCanOrder: {
      header: 'Medication currently available to order',
      title: 'You don\'t have any medications available to order right now.',
      contactGp: 'If you have medications available on repeat prescription that aren’t shown here, contact your GP surgery for more information.',
    },
    confirmPrescriptionOrder: {
      header: 'Check your prescription details before ordering',
      changeButton: 'Change this prescription',
      confirmButtonText: 'Confirm and order repeat prescription',
    },
  },
  myRecord: {
    genericErrorMessage: 'An error has occurred trying to retrieve this data.',
    genericNoDataMessage: 'No information recorded for this section.',
    genericNoAccessMessage: 'You do not have access to this section.',
    name: 'Name',
    dateOfBirthday: 'Date of Birthday',
    sex: 'Sex',
    address: 'Address',
    nhsNumber: 'NHS Number',
    gpPractice: 'GP Practice',
    patient: 'Patient',
    myRecordWarning: {
      warningText: 'Your record may contain sensitive information.',
      title: 'Sensitive information may include:',
      bulletPoints: {
        bp1: 'Personal data, such as your details, allergies and care preferences',
        bp2: 'Medical history, such as your conditions and medications',
        bp3: 'Test results that you may not have discussed with your doctor',
        bp4: 'Clinical terms that you may not be familiar with',
      },
      agreementText: 'By clicking continue, you agree to viewing sensitive information within your medical record.',
      agreeButtonText: 'Agree and continue',
      backButtonText: 'Back to home',
    },
    noRecordAccess: {
      warningHeader: 'Sorry, you don\'t currently have access to this service',
      warningBody: 'Please contact your GP surgery for more information.',
    },
    patientInfo: {
      fieldLabelName: 'Name',
      fieldLabelDOB: 'Date of birth',
      fieldLabelSex: 'Sex',
      fieldLabelAddress: 'Address',
      fieldLabelNHS: 'NHS number',
      sectionHeader: 'My details',
    },
    viewRestOfHealthRecordWarning: 'You cannot view the rest of your health record online. To see more, ask your GP practice for access to your ‘Detailed Coded Record’.',
    allergiesAndAdverseReactions: {
      sectionHeader: 'Allergies and adverse reactions',
    },
    acuteMedications: {
      sectionHeader: 'Acute medications',
    },
    currentRepeatMedications: {
      sectionHeader: 'Current repeat medications',
    },
    discontinuedRepeatMedications: {
      sectionHeader: 'Discontinued repeat medications',
    },
    immunisations: {
      sectionHeader: 'Immunisations',
    },
    testResults: {
      sectionHeader: 'Test results',
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
    repeatPrescriptionCourses: 'Select medication',
    confirmPrescription: 'Confirm prescription',
    account: 'My account',
    appointments: 'My Appointments',
    appointmentBooking: 'Appointments',
    appointmentConfirmation: 'Check appointment details',
    myRecord: 'My medical record',
    more: 'More',
    login: 'Login',
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
      title: 'My record',
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
      title: 'Session Expired. Please sign in again.',
    },
    symptomsIcon: {
      title: 'Symptoms Checker',
    },
  },
};
