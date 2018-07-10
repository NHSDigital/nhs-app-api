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
  },
  rp01: {
    pageTitle: 'TBC',
    empty: {
      subHeader: 'You don\'t currently have any repeat prescriptions ordered',
      line1: 'Once you’ve placed an order here, you\'ll be able to view your repeat prescription status and history.',
      line2: 'If you have an existing order that isn’t shown here, contact your GP surgery or pharmacy for more information.',
    },
    orderPrescriptionButton: 'Order a repeat prescription',
  },
  rp02: {
    pageTitle: 'TBC',
    orderDate: 'Order date',
    status: 'Status',
    statusRejected: {
      subHeader: 'Rejected',
      description: 'Rejected: contact your GP',
    },
    statusRequested: {
      subHeader: 'Requested',
      description: 'Requested: waiting for GP approval',
    },
    statusApproved: {
      subHeader: 'Approved',
      description: 'Approved: contact your nominated pharmacy for collection information',
    },
  },
  rp06: {
    pageTitle: 'TBC',
    empty: {
      subHeader: 'You don\'t have any medication available to order right now',
      body: 'If you have medications available on repeat prescription that aren\'t shown here, contact your GP surgery for more information.',
    },
    backButton: 'Back',
  },
  rp03: {
    pageTitle: 'TBC',
    subHeader: 'Medication currently available to order',
    noMedicinesSelected: 'Select at least one medicine',
    specialRequestsLabel: 'Special requests relating to this order (optional)',
    maxSpecialRequest: 'Special requests must be shorter than 1000 characters (about 150 words)',
    changePharmacyText: 'To discuss your medication or change your chosen pharmacy, contact your GP surgery before ordering.​',
    noSpecialRequestDefaultText: 'None',
    continueButton: 'Continue',
    backButton: 'Back',
  },
  rp04: {
    pageTitle: 'TBC',
    subHeader: 'Check your prescription details before ordering',
    specialRequestsLabel: 'Special requests relating to this order',
    confirmButton: 'Confirm and order prescription',
    backButton: 'Change this prescription',
  },
  rp05: {
    confirmationMessage: 'Your prescription has been ordered. The order status will be updated once it’s been reviewed by your GP.​',
  },
  rp12: {
    reasonMissing: {
      summarySubHeader: 'There\'s a problem',
      summaryBody: 'Select a medication',
      inline: 'Select a medication',
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
      successText: 'Your appointment has been booked. You can view details or cancel it here.',
      bookButtonText: 'Book an appointment',
      cancelButtonText: 'Cancel appointment',
      empty: {
        header: 'You don\'t currently have any appointments booked',
        text1: 'Once you\'ve booked an appointment here, you\'ll be able to view details, cancel it and see your appointment history.',
        text2: 'If you have an upcoming appointment that isn\'t shown here, contact your GP surgery for more information.',
      },
      upcoming: {
        header: 'Next appointments',
        info: 'Click in the appointment if you need to cancel it',
      },
    },
    guidance: {
      header: 'Want to avoid waiting?',
      text: 'If it isn\'t urgent, you can try three things before booking an appointment:',
      li1: {
        header: 'Self care​',
        text: 'Many minor problems can be treated at home, for example through rest or appropriate over-the-counter medicines​',
      },
      li2: {
        header: 'Check your symptoms​',
        text: 'Using trusted NHS online information​',
      },
      li3: {
        header: 'Get advice from a pharmacist​',
        text: 'They\'re highly skilled healthcare professionals who can offer valuable advice​',
      },
      symptomButttonText: 'Check your symptoms',
      bookButtonText: 'Book an appointment',
    },
    noSlotErrorMessage: {
      summary: 'There are no appointments available at the moment',
      info: 'If you need an appointment, please contact your GP.',
    },
    noConnection: {
      message:
        'If the problem persists and you need to book an appointment immediately please contact your GP practice.',
    },
    errors: {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      504: {
        subheader: 'Please try again',
        message: 'If the problem persists and you need this information now, please contact your GP surgery directly.',
        retryButtonText: 'Try again',
      },
      403: {
        pageHeader: 'Service unavailable',
        header: 'Sorry, you don\'t currently have access to this service',
        subheader: '',
        message: 'Contact your GP surgery for more information.',
      },
    },
    cancel: {
      info: 'Check your appointment details before cancelling',
      form_label: 'Reason for cancelling',
      backButtonText: 'Back',
      cancelButtonText: 'Cancel appointment',
      noReasonDialogError: 'There\'s a problem',
      noReasonError: 'Select a reason for cancelling',
      dropdownDefaultOption: 'Select reason',
      successText: 'Your appointment has been cancelled.',
      errors: {
        pageHeader: 'Error sending request',
        header: 'Sorry, there\'s been a problem sending your request',
        subheader: 'Please go back and try again.',
        message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
        retryButtonText: 'Back to my appointments',
      },
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
        403: {
          pageHeader: 'Service unavailable',
          header: 'Sorry, you don\'t currently have access to this service',
          subheader: '',
          message: 'Contact your GP surgery for more information.',
        },
      },
    },
    confirmation: {
      headerLabel: 'What is this appointment for?',
      label: 'Describe your symptoms',
      maxReasonDesc: 'Description must be shorter than 150 characters (about 25 words)',
      confirmButtonText: 'Confirm and book appointment',
      changeButtonText: 'Change this appointment',
      noReasonDialogError: 'There\'s a problem',
      noReasonError: 'Enter a reason for this appointment',
      errors: {
        pageHeader: 'Error sending request',
        header: 'Sorry, there\'s been a problem sending your request',
        subheader: 'Please go back and try again.',
        message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
        retryButtonText: 'Back to my appointments',
      },
    },
  },
  prescriptions: {
    errors: {
      pageHeader: 'Error retrieving data',
      header: 'Sorry, there\'s been a problem getting your prescription information',
      subheader: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
      message: '',
      retryButtonText: '',
      403: {
        pageHeader: 'Service unavailable',
        header: 'Sorry, you don\'t currently have access to this service',
        subheader: '',
        message: 'Contact your GP surgery for more information.',
      },
      504: {
        pageHeader: 'Error retrieving data',
        header: 'Sorry, there\'s been a problem getting your prescription information',
        subheader: 'Please try again',
        message: 'If the problem persists and you need this information now, please contact your GP surgery directly.',
        retryButtonText: 'Try again',
      },
    },
    repeat_courses: {
      errors: {
        pageHeader: 'Error retrieving data',
        header: 'Sorry, there\'s been a problem getting your prescription information',
        subheader: 'Please try again later. If the problem persists and you need this information now, please contact your GP surgery directly.',
        message: '',
        retryButtonText: '',
        504: {
          pageHeader: 'Error retrieving data',
          header: 'Sorry, there\'s been a problem getting your prescription information',
          subheader: 'Please try again',
          message: 'If the problem persists and you need this information now, please contact your GP surgery directly.',
          retryButtonText: 'Try again',
        },
        403: {
          pageHeader: 'Service unavailable',
          header: 'Sorry, you don\'t currently have access to this service',
          subheader: '',
          message: 'Contact your GP surgery for more information.',
        },
      },
    },
    confirm_prescription_details: {
      errors: {
        pageHeader: 'Error sending request',
        header: 'Sorry, there\'s been a problem sending your request',
        subheader: 'Please go back and try again.',
        message: 'If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly.',
        retryButtonText: 'Back to my repeat prescriptions',
      },
    },
    prescriptionStatus: {
      rejected: {
        text: 'Rejected',
        description: 'Contact your GP',
      },
      requested: {
        text: 'Requested',
        description: 'Waiting for GP approval',
      },
      approved: {
        text: 'Approved',
        description: 'Contact your chosen pharmacy for collection information',
      },
    },
  },
  myRecord: {
    genericErrorMessage: 'An error has occurred trying to retrieve this data.',
    genericNoDataMessage: 'No information recorded for this section',
    genericNoAccessMessage: 'You do not have access to this section',
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
    problems: {
      sectionHeader: 'Problems',
    },
    events: {
      sectionHeader: 'Events',
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
    appointments: 'My appointments',
    appointmentGuidance: 'Check before you book',
    appointmentBooking: 'Book an appointment',
    appointmentCancelling: 'Cancel appointment',
    appointmentConfirmation: 'Confirm appointment',
    myRecord: 'My medical record',
    more: 'More',
    login: 'Login',
  },
  more: {
    organDonationLabel: 'Organ donation preferences',
    organDonationDesc: 'Help us save thousands of lives in the UK every year by signing up to become an organ donor. Register your decision and choose what you want to donate on the NHS Organ Donor Register.',
    organDonationButtonText: 'Set organ donation preferences',
    dataSharingHeaderText: 'Sharing health data',
    dataSharingInfoText: 'The NHS uses identifiable health and care information (like your name or NHS number) for your individual care. This data can also be used in research to find cures for life-threatening diseases, and to plan health services and make them safer.',
    dataSharingButtonText: 'Set health data sharing preferences',
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
