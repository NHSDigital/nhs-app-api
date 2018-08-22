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
  auth_return: {
    errors: {
      pageHeader: 'Session error - NHS App',
      header: 'Session Error',
      subheader: 'Sorry, there\'s been a problem loading this page',
      message: 'Please go back to the homescreen and sign in again.\nIf the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For immediate medical advice, call 111.',
      retryButtonText: 'Back to home',
    },
  },
  rp01: {
    pageTitle: 'TBC',
    empty: {
      subHeader: 'You don\'t currently have any repeat prescriptions ordered',
      line1: 'Once you’ve placed an order here, you\'ll be able to view your repeat prescription status and history.',
      line2: 'If you have an existing order that isn’t shown here, contact your GP surgery or pharmacy for more information.',
    },
    orderPrescriptionButton: 'Order new repeat prescription',
  },
  rp02: {
    pageTitle: 'TBC',
    orderDate: 'Order date',
    status: 'Status',
    statusRejected: {
      subHeader: 'Rejected',
      description: 'Contact your GP',
    },
    statusRequested: {
      subHeader: 'Requested',
      description: 'Waiting for GP approval',
    },
    statusApproved: {
      subHeader: 'Approved',
      description: 'Order approved by GP',
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
    disclaimer: 'This text may not be seen by your GP. For important requests, contact your GP surgery.',
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
      bookButtonText: 'Book new appointment',
      cancelButtonText: 'Cancel appointment',
      empty: {
        header: 'You don\'t currently have any appointments booked',
        text1: 'Once you\'ve booked an appointment here, you\'ll be able to view details, cancel it and see your appointment history.',
        text2: 'If you have an upcoming appointment that isn\'t shown here, contact your GP surgery for more information.',
      },
      upcoming: {
        header: 'Upcoming appointments',
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
      bookButtonText: 'Book new appointment',
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
        403: {
          pageHeader: 'Cancel appointment',
          header: 'Contact your GP surgery to cancel',
          subheader: '',
          message: 'You can\'t cancel appointments online right now. Call your GP surgery as soon as possible to let them know you need to cancel.',
          retryButtonText: 'Back to my appointments',
        },
        461: {
          pageHeader: 'Cancel appointment',
          header: 'Contact your GP surgery to cancel',
          subheader: '',
          message: 'It\'s too late to cancel this appointment online. Call your GP surgery as soon as possible to let them know you need to cancel.',
          retryButtonText: 'Back to my appointments',
        },
      },
    },
    booking: {
      bookButtonText: 'Continue',
      backButtonText: 'Back to my appointments',
      noSlots: 'No appointments available',
      noAppointmentsAvailable: {
        title: 'No appointments available',
        line1: 'There are currently no appointments available to book online right now. If you need to book one now, call your GP surgery.',
        line2: 'If it\'s urgent and you don\'t know what to do, call 111 to get help near you.',
      },
      adjustSearch: {
        title: 'No appointments available',
        line1: 'Try selecting a different date and time, or without a preferred practice member selected. If you can\'t find the appointment you need, call your GP surgery.',
        line2: 'If it\'s urgent and you don\'t know what to do, call 111 to get help near you.',
      },
      validationErrors: {
        problemFound: 'There\'s a problem',
        type: 'Choose a type of appointment',
        location: 'Choose a location',
        slot: 'Select an appointment slot',
      },
      filters: {
        type: {
          label: 'Type of appointment',
          default_option: 'Select type',
        },
        location: {
          label: 'Location',
          default_option: 'Select location',
        },
        clinician: {
          label: 'Practice member',
          default_option: 'No preference',
        },
        date: {
          header: 'Select an appointment',
          label: 'Filter by date',
          options: {
            today: 'Today',
            tomorrow: 'Tomorrow',
            this_week: 'This week',
            next_week: 'Next week',
            all: 'All available',
          },
        },
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
        403: {
          pageHeader: 'Service unavailable',
          header: 'Sorry, you don\'t currently have access to this service',
          subheader: '',
          message: 'Contact your GP surgery for more information.',
        },
      },
    },
    confirmation: {
      headerLabel: 'Reason for this appointment',
      reasonDesc: {
        line1: 'Text must be shorter than 150 characters (about 25 words)',
        line2: 'This text may not be read by your GP or practice member until the day of your appointment.',
        line3: 'If it\'s urgent, contact your GP surgery before booking.',
      },
      confirmButtonText: 'Confirm and book appointment',
      changeButtonText: 'Change this appointment',
      noReasonDialogError: 'There\'s a problem',
      noReasonError: 'Enter a reason for this appointment',
      conflictErrorMessage: 'This slot is no longer available. Please select a different time.',
      info: 'Check your appointment details before booking',
      errors: {
        pageHeader: 'Error sending request',
        header: 'Sorry, there\'s been a problem sending your request',
        subheader: 'Please go back and try again.',
        message: 'If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.',
        retryButtonText: 'Back to my appointments',
        460: {
          pageHeader: 'Appointment limit reached',
          header: 'You can\'t book any more appointments right now.',
          subheader: 'Contact your GP surgery if you still need to book one.',
          message: 'You can go back to see what you\'ve already booked and cancel any appointments that you may no longer need.',
          additionalInfo: 'If it\'s urgent and you don\'t know what to do, call 111 to get help near you.',
          retryButtonText: 'Back to my appointments',
        },
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
  },
  my_record: {
    genericErrorMessage: 'An error has occurred trying to retrieve this data.',
    genericNoDataMessage: 'No information recorded',
    genericNoAccessMessage: 'You don\'t currently have access to this section',
    name: 'Name',
    dateOfBirthday: 'Date of Birthday',
    sex: 'Sex',
    address: 'Address',
    nhsNumber: 'NHS Number',
    gpPractice: 'GP Practice',
    patient: 'Patient',
    myRecordWarning: {
      warningText: 'Your record may contain sensitive information',
      title: 'Your record will display:',
      bulletPoints: {
        bp1: 'personal data, such as your details, allergies and medications',
        bp2: 'clinical terms that you may not be familiar with',
      },
      extraTitle: 'Depending on what your GP surgery has shared, you may also see:',
      extraBulletPoints: {
        bp1: 'your medical history, including problems and consultation notes',
        bp2: 'test results that you may not have discussed with your doctor',
      },
      agreementText: 'By continuing, you agree to viewing sensitive information within your medical record.',
      agreeButtonText: 'Agree and continue',
      backButtonText: 'Back to home',
    },
    noRecordAccess: {
      warningHeader: 'You don’t currently have online access to your medical record',
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
    viewRestOfHealthRecordWarning: 'This is a summary of your medical record. To view more detailed information here, such as test results and immunisations, contact your GP surgery to request access.',
    allergiesAndAdverseReactions: {
      sectionHeader: 'Allergies and adverse reactions',
    },
    acuteMedications: {
      sectionHeader: 'Acute (short-term) medications',
    },
    currentRepeatMedications: {
      sectionHeader: 'Repeat medications: current',
    },
    discontinuedRepeatMedications: {
      sectionHeader: 'Repeat medications: discontinued',
    },
    immunisations: {
      sectionHeader: 'Immunisations',
    },
    testResults: {
      sectionHeader: {
        tpp: 'Test results (past 6 months)',
        default: 'Test results',
      },
    },
    problems: {
      sectionHeader: 'Problems',
    },
    consultations: {
      sectionHeader: 'Consultations',
    },
    events: {
      sectionHeader: 'Consultations',
    },
    testresultdetail: {
      backButton: 'Back',
      testResultTitle: 'Test result',
      noTestResultData: 'There is no detail to display for this test result.',
      errors: {
        502: {
          pageTitle: 'Test result details data error - NHS App',
          pageHeader: 'Error retrieving data',
          header: 'Sorry, there\'s been a problem getting details of your test results',
          subheader: 'If the problem persists and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          message: '',
          retryButtonText: '',
        },
      },
    },
    clinicalTerms: {
      text: 'You may see medical abbreviations that you aren’t familiar with.',
      link: 'Help with abbreviations',
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
    login: 'Log in or create account',
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
    appointmentBooking: 'Book new appointment',
    appointmentCancelling: 'Cancel appointment',
    appointmentConfirmation: 'Confirm appointment',
    myRecord: 'My medical record',
    symptoms: 'Check my symptoms',
    more: 'More',
    login: 'Login',
  },
  myAccount: {
    termsAndConditions: 'Terms and conditions',
    privacyPolicy: 'Privacy policy',
    cookiesPolicy: 'Cookies policy',
    openSourceLicenses: 'Open source licenses',
    helpAndSupport: 'Help and support',
  },
  sc04: {
    organDonation: {
      subheader: 'Set organ donation preferences',
      body: 'Help save thousands of lives in the UK every year by signing up to become a donor on the NHS Organ Donor Register.',
    },
    dataSharing: {
      subheader: 'Manage your choice for sharing data',
      body: 'Find out why your data matters and choose whether or not it can be used for research and planning.',
    },
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
    checker: 'Check my symptoms',
  },
  login: {
    desc: 'Want to book an appointment or order a repeat prescription?',
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
      title: 'For your security, you need to sign in again',
    },
    symptomsIcon: {
      title: 'Symptoms Checker',
    },
  },
  sy01: {
    pageHeader: 'Check my symptoms',
    a_z: {
      subheader: 'A-Z of conditions and treatments',
      body: 'Search trusted information and advice on hundreds of conditions',
    },
    111: {
      subheader: 'Check if I need urgent help',
      body: 'Answer online questions to get instant advice or medical help near you',
    },
  },
};

