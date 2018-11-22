export default {
  language: 'en-GB',
  appTitle: 'NHS App',
  errors: {
    pageHeader: 'Server error',
    header: 'We\'re experiencing technical difficulties',
    subheader: '',
    message: 'Try again later. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
    502: {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      retryButtonText: 'Try again',
      message: 'Try again in a few moments.',
    },
  },
  auth_return: {
    errors: {
      pageTitle: 'Session error',
      pageHeader: 'Session error',
      header: 'Session error',
      subheader: 'There\'s been a problem loading this page.',
      message: 'Go back to the home screen and log in again.',
      additionalInfo: 'If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      additionalInfoLabel: 'If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      retryButtonText: 'Back to home',
      464: {
        pageHeader: 'Service unavailable',
        header: 'Service unavailable',
        subheader: 'You cannot currently use this service',
        message: 'You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, call 111.',
      },
      465: {
        pageHeader: 'Service unavailable',
        header: 'Service unavailable',
        subheader: 'You cannot currently use this service',
        message: 'As you’re under 16, you cannot currently access the NHS App. You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, call 111.',
      },
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
    glossary: {
      headerText: 'You may see medical abbreviations that you aren\'t familiar with.',
      linkText: 'Help with abbreviations',
    },
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
    specialRequestRequired: 'Enter any special requests relating to this order',
    specialRequestsLabelOptional: 'Special requests relating to this order (optional)',
    specialRequestsLabelMandatory: 'Special requests relating to this order',
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
    header: 'Connection error',
    subheader: 'There\'s an issue with your internet connection',
    retryButtonText: 'Try again',
    message: 'Check your connection and try again. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
  },
  termsAndConditions: {
    title: 'Accept conditions of use',
    errorMsgHeader: 'There\'s a problem',
    errorMsgText: 'You cannot continue without agreeing',
    body1: 'To use the NHS App you must agree to our',
    body2: 'You should read these carefully before using the app.',
    link1: 'terms of use',
    link2: 'privacy policy',
    link3: 'cookies policy',
    link4: 'manage your cookies',
    listTitle: 'Key points: ',
    listItems: [
      'The NHS App is intended to provide you with information and services to help you manage certain medical conditions or treatments - it is not a substitute for seeking medical advice from a GP or other healthcare professional. Always follow any medical advice given by your GP or other healthcare professional',
      'Information available through the NHS App comes from third parties, so we cannot be responsible for its content or relevance to you. In particular, the settings used by your GP surgery may affect what medical record information you can access',
      'The NHS App gives you access to a range of NHS services that may have their own terms and privacy policies. You should read these so that you understand your rights and how your data is used',
    ],
    body3: 'If you do not agree, you won\'t be able to access or use the NHS App.',
    cookiesTitle: 'Cookies',
    cookiesText1: 'The NHS App puts small files (known as cookies) on your device. These are used to make the app work and improve your experience. You can ',
    cookiesText2: ' to opt out of using some of them.',
    checkBoxError: 'You cannot use the NHS App without agreeing',
    checkBoxText1: 'I understand and accept the',
    checkBoxText2: 'I accept the use of \'strictly necessary\' cookies as detailed in the',
    analyticsCookieCheckBoxText: 'I accept the use of optional cookies used to improve the performance of the NHS App.',
    btnAccept: 'Continue',
  },
  appointments: {
    index: {
      successText: 'Your appointment has been booked. You can view details or cancel it here.',
      succcessAndCancellationDisabledText: 'Your appointment has been booked. You can view details here.',
      bookButtonText: 'Book new appointment',
      cancelButtonText: 'Cancel appointment',
      cancellationDisabledText: 'To cancel this appointment, contact your GP surgery.',
      empty: {
        header: 'You don\'t currently have any appointments booked',
        text1: 'Once you\'ve booked an appointment here, you\'ll be able to view details and cancel it.',
        text2: 'If you have an upcoming appointment that isn\'t shown here, contact your GP surgery for more information.',
      },
      upcoming: {
        header: 'Upcoming appointments',
        info: 'Click in the appointment if you need to cancel it',
      },
      errors: {
        pageTitle: 'Appointment data error',
        pageHeader: 'Appointment data error',
        header: 'There\'s been a problem getting your appointment history',
        subheader: '',
        message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        504: {
          subheader: '',
          retryButtonText: 'Try again',
          message: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        },
        403: {
          pageTitle: 'Appointment booking unavailable',
          pageHeader: 'Appointment booking unavailable',
          header: 'You are not currently able to book appointments online.',
          subheader: '',
          message: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
        },
      },
    },
    guidance: {
      header: 'Want to avoid waiting?',
      text: 'Three things to try before you book an appointment:',
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
      symptomButttonText: 'Check symptoms',
      bookButtonText: 'Book new appointment',
    },
    noSlotErrorMessage: {
      summary: 'There are no appointments available at the moment',
      info: 'If you need an appointment, please contact your GP.',
    },
    errors: {
      pageHeader: 'Appointment data error',
      header: 'There\'s been a problem getting your appointment history',
      subheader: '',
      message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      504: {
        subheader: '',
        message: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        retryButtonText: 'Try again',
      },
      403: {
        pageHeader: 'Appointment booking unavailable',
        header: 'You are not currently able to book appointments online',
        subheader: '',
        message: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
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
        pageTitle: 'Appointment request error',
        pageHeader: 'Error sending request',
        header: 'There\'s been a problem sending your request',
        subheader: '',
        message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
        retryButtonText: 'Back to my appointments',
        403: {
          pageHeader: 'Cancel appointment',
          header: 'Contact your GP surgery to cancel',
          message: 'You can\'t cancel appointments online right now. Call your GP surgery as soon as possible to let them know you need to cancel.',
          retryButtonText: 'Back to my appointments',
        },
        461: {
          pageHeader: 'Cancel appointment',
          header: 'Contact your GP surgery to cancel',
          message: 'It\'s too late to cancel this appointment online. Call your GP surgery as soon as possible to let them know you need to cancel.',
          retryButtonText: 'Back to my appointments',
        },
      },
    },
    booking: {
      backButtonText: 'Back to my appointments',
      noSlots: 'No appointments available',
      nojs: {
        findButton: 'Find available appointments',
      },
      availableAppointmentsScreenReaderMessage: '0 appointments available | 1 appointment available | {appointmentsCount} appointments available',
      gpMessage: {
        header: 'Which type of appointment do I need?',
      },
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
          header: 'Available appointments',
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
        pageTitle: 'Appointment data error',
        pageHeader: 'Appointment data error',
        header: 'There\'s been a problem loading this page',
        subheader: '',
        message: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
        504: {
          message: 'Try again now. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
          subheader: '',
          retryButtonText: 'Try again',
        },
        403: {
          pageTitle: 'Appointment booking unavailable',
          pageHeader: 'Appointment booking unavailable',
          header: 'You are not currently able to book appointments online',
          subheader: '',
          message: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
        },
      },
    },
    confirmation: {
      headerLabel: 'Reason for this appointment',
      headerLabelSuffix: ' (Optional)',
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
        pageTitle: 'Appointment request error',
        pageHeader: 'Error sending request',
        header: 'There\'s been a problem sending your request',
        subheader: '',
        message: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
        retryButtonText: 'Back to my appointments',
        460: {
          pageHeader: 'Appointment limit reached',
          header: 'You can\'t book any more appointments right now',
          subheader: 'Contact your GP surgery if you still need to book one.',
          message: 'You can go back to see what you\'ve already booked and cancel any appointments that you may no longer need.',
          additionalInfo: 'If it\'s urgent and you don\'t know what to do, call 111 to get help near you.',
          retryButtonText: 'Back to my appointments',
        },
        409: {
          pageHeader: 'Confirm appointment',
          header: 'This slot is no longer available',
          subheader: '',
          message: 'Please select a different time.',
          retryButtonText: 'Back',
        },
      },
    },
  },
  prescriptions: {
    errors: {
      pageTitle: 'Prescription data error',
      pageHeader: 'Prescription data error',
      header: 'There\'s been a problem getting your prescription information',
      subheader: '',
      message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      retryButtonText: '',
      403: {
        pageTitle: 'Repeat prescriptions unavailable',
        pageHeader: 'Repeat prescriptions unavailable',
        header: 'You are not currently able to order repeat prescriptions online',
        subheader: '',
        message: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
      },
      504: {
        pageTitle: 'Prescription data error',
        pageHeader: 'Prescription data error',
        header: 'There\'s been a problem getting your prescription information',
        subheader: '',
        message: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        retryButtonText: 'Try again',
      },
    },
    repeat_courses: {
      errors: {
        pageTitle: 'Prescription data error',
        pageHeader: 'Prescription data error',
        header: 'There\'s been a problem getting your prescription information',
        subheader: '',
        message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        retryButtonText: '',
        504: {
          pageTitle: 'Prescription data error',
          pageHeader: 'Prescription data error',
          header: 'There\'s been a problem getting your prescription information',
          subheader: '',
          message: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          retryButtonText: 'Try again',
        },
        403: {
          pageTitle: 'Repeat prescriptions unavailable',
          pageHeader: 'Repeat prescriptions unavailable',
          header: 'You are not currently able to order repeat prescriptions online',
          subheader: '',
          message: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
        },
      },
    },
    confirm_prescription_details: {
      errors: {
        pageTitle: 'Prescription order error',
        pageHeader: 'Error sending order',
        header: 'There\'s been a problem sending your order',
        subheader: '',
        message: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
        retryButtonText: 'Back to my repeat prescriptions',
      },
    },
  },
  my_record: {
    errors: {
      pageHeader: 'Medical record error',
      header: 'There\'s been a problem getting your medical record information',
      subheader: '',
      message: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
      retryButtonText: '',
      502: {
        pageHeader: 'Medical record error',
        header: 'There\'s been a problem getting your medical record information',
        subheader: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        message: '',
        retryButtonText: '',
      },
      504: {
        pageHeader: 'Medical record error',
        header: 'There\'s been a problem getting your medical record information',
        subheader: '',
        message: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        retryButtonText: 'Try again',
      },
    },
    genericErrorMessage: 'An error has occurred trying to retrieve this data.',
    genericNoDataMessage: 'No information recorded',
    genericNoAccessMessage: 'You do not currently have access to this section',
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
      warningHeader: 'You do not currently have online access to your medical record',
      warningBody: 'Contact your GP surgery for more information.',
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
          pageTitle: 'Test result details data error',
          pageHeader: 'Test result details data error',
          header: 'There\'s been a problem getting details of your test results',
          subheader: '',
          message: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
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
    signOut: 'Log out',
  },
  pageHeaders: {
    home: 'Home',
    gpFinder: 'GP Finder',
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
    myRecordWarning: 'My medical record',
    symptoms: 'Check my symptoms',
    dataSharing: 'Sharing health data preferences',
    more: 'More',
    login: 'Login',
    termsAndConditions: 'Accept conditions of use',
  },
  pageTitles: {
    home: 'Home',
    gpFinder: 'GP Finder',
    prescriptions: 'Repeat prescriptions',
    repeatPrescriptionCourses: 'Select medication - Repeat prescriptions',
    confirmPrescription: 'Confirm prescription - Repeat prescriptions',
    account: 'My account',
    appointments: 'My appointments',
    appointmentGuidance: 'Check before you book - My appointments',
    appointmentBooking: 'Book new appointment',
    appointmentCancelling: 'Cancel appointment',
    appointmentConfirmation: 'Confirm appointment',
    myRecord: 'My record',
    myRecordWarning: 'Sensitive information - My record',
    symptoms: 'Check my symptoms',
    dataSharing: 'Sharing health data preferences',
    more: 'More',
    login: 'Login',
    termsAndConditions: 'Accept conditions of use',

  },
  myAccount: {
    detailsHeading: 'Details',
    accountSettingsHeading: 'Account settings',
    aboutUsHeading: 'About us',
    termsAndConditions: 'Terms of use',
    privacyPolicy: 'Privacy policy',
    cookiesPolicy: 'Cookies policy',
    openSourceLicences: 'Open source licences',
    helpAndSupport: 'Help and support',
    fingerprintID: 'Fingerprint ID',
    accessibilityStatement: 'Accessibility statement',
  },
  sc04: {
    organDonation: {
      subheader: 'Set organ donation preferences',
      body: 'Help save thousands of lives in the UK every year by signing up to become a donor on the NHS Organ Donor Register.',
    },
    dataSharing: {
      subheader: 'Choose how the NHS uses your data',
      body: 'Find out how the NHS uses your confidential patient information and choose whether or not it can be used for research and planning.',
    },
  },
  navigationMenu: {
    appointmentsLabel: 'Appointments',
    moreLabel: 'More',
    myRecordLabel: 'My record',
    prescriptionsLabel: 'Prescriptions',
    symptomsLabel: 'Symptoms',
  },
  navigationMenuList: {
    symptoms: 'Check my symptoms',
    appointments: 'Book and manage appointments',
    prescriptions: 'Order a repeat prescription',
    myRecord: 'View my medical record',
    organDonation: 'Set organ donation preferences',
  },
  symptomBanner: {
    howAreYouFeeling: 'How are you feeling today?',
    checker: 'Check symptoms',
  },
  login: {
    desc: 'To access your GP services',
  },
  surveyBar: {
    barText: 'Help us make this service better.',
    linkText: ' Complete our quick survey.',
  },
  icons: {
    accountIcon: {
      title: 'My Account',
      desc: 'Access my account settings',
    },
    appointmentsIcon: {
      title: 'Appointments',
    },
    clinicianIcon: {
      title: 'Clinician',
      description: 'Clinician Graphic',
    },
    helpIcon: {
      title: 'Help and support',
      desc: 'Access help and support',
    },
    homeIcon: {
      title: 'NHS Online',
      desc: 'Go back to the home screen.',
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
      title: 'For your security, you need to log in again',
    },
    symptomsIcon: {
      title: 'Symptoms Checker',
    },
  },
  th02: {
    banner: {
      labelText: 'NEW',
      message: 'We\'re still testing this app',
    },
    heading1: 'Check what features you can use',
    heading2: 'Find your GP surgery',
    hintText: 'Enter postcode, town or GP surgery name',
    callToAction: 'Continue',
    hasAnAccountLink: 'I\'m already using the NHS App',
  },
  th03: {
    header: 'Select your GP surgery',
    errors: {
      unableToRetrieveResults: 'Unable to retrieve search results.',
      noGpPracticesFound: 'No GP Practices found',
    },
  },
  sy01: {
    pageHeader: 'Check my symptoms',
    a_z: {
      subheader: 'A-Z of conditions and treatments',
      subheaderAriaLabel: 'A-to-Z of conditions and treatments',
      body: 'Search trusted information and advice on hundreds of conditions',
    },
    111: {
      subheader: 'Check if I need urgent help',
      body: 'Answer online questions to get instant advice or medical help near you',
    },
  },
  ds01: {
    header: 'Sharing health data preferences',
    titles: {
      p1: 'Overview',
      p2: 'Benefits of data sharing',
      p3: 'How your data is used',
      p4: 'Where an opt-out doesn\'t apply',
      p5: 'Manage your choice',
    },
    subtitle: 'Manage your data choice',
    startNowButton: 'Start now',
    nextButton: 'Next',
    previousButton: 'Previous',
    pages: {
      p1: {
        intro: {
          paragraph: 'This service allows you to choose whether or not your confidential patient information is used for research and planning. You can make or change your decision at any time. Unless you have chosen to opt out, your confidential patient information can and may be used for research and planning.',
        },
        confidential: {
          title: 'Confidential patient information',
          paragraphs: [
            'Confidential patient information identifies you and says something about your health care or treatment. Information that only identifies you like your name and address is not confidential patient information.',
            'Health and care professionals may use your confidential patient information to help with your treatment and care. This information is also used to research cures for serious illnesses and to improve health and care services.',
          ],
        },
        yourChoice: {
          title: 'Your choice',
          paragraph: 'If you don\'t want your confidential patient information to be used for research and planning, you can opt out of this. Your confidential patient information will still be used to support your individual care as any choice you set will not change this. If you do not wish to opt out, you don\'t have to do anything at all.',
          manageChoiceLink: 'Manage your choice',
        },
        moreOptions: {
          title: 'More options',
          paragraph: {
            part1: 'Visit the ',
            nhsWebsiteLink: 'NHS website',
            part3: ' for more information or to read our privacy notice. You can also find out how to manage a choice on behalf of another person.',
          },
        },
      },
      p2: {
        research: {
          title: 'Research',
          listItems: [
            'Prevent serious illness',
            'Develop new treatments',
            'Learn more about diseases',
          ],
          paragraphs: [
            'Your confidential patient information provides numerous benefits. It is used in research to find cures and better treatments for diseases like diabetes and cancer.',
            'With your data, we are better able to develop and improve health and care services for the future.',
          ],
        },
        planning: {
          title: 'Planning',
          listItems: [
            'Plan NHS health services',
            'Make services safer',
            'Improving quality of care',
          ],
          paragraphs: [
            'Confidential patient information can also be used to plan health and care services more effectively. The NHS and local authorities can plan where they need to provide further care services more efficiently.',
            'This helps to improve health and social care for you and your family.',
          ],
        },
      },
      p3: {
        intro: {
          paragraphs: [
            'The NHS collects health and care data from all NHS organisations, trusts and local authorities. Data is also collected from private organisations, such as private hospitals providing NHS funded care. Research bodies and organisations can request access to this data.',
            'Research bodies and organisations include:',
          ],
          listItems: [
            'university researchers',
            'hospital researchers',
            'medical royal colleges',
            'pharmaceutical companies researching new treatments',
          ],
        },
        thoseWhoCant: {
          title: 'Who can\'t use your data',
          paragraphs: [
            'There are very strict rules on how your data can and cannot be used, and you have clear data rights.',
            {
              part1: 'Access to confidential patient information will ',
              emphasised: 'not',
              part3: ' be given for:',
            },
          ],
          listItems: [
            'marketing purposes',
            'insurance purposes',
          ],
          paragraph: '(unless you specifically request this).',
        },
        dataProtection: {
          title: 'How your data is protected',
          paragraphs: [
            'Protection of your confidential patient information is taken very seriously and is looked after in accordance with good practice and the law.',
            'Every organisation that provides health and care services will take every step to:',
          ],
          listItems: [
            'ensure data remains secure',
            'use anonymised data whenever possible',
            'use confidential patient information to benefit health and care',
            'not use confidential patient information for marketing or insurance purposes (unless you specifically request this)',
            'make it clear why and how data is being used',
            'respect your decision if you decide to opt out',
            'only use information about you where allowed by the law',
          ],
          paragraph: 'All NHS organisations must provide information on the type of data they collect and how it is used. Data release registers are published by NHS Digital and Public Health England, showing records of the data they have shared with other organisations.',
        },
      },
      p4: {
        paragraphs: [
          'There are some circumstances where opt-outs do not apply. In these circumstances, your confidential patient information may still be used.',
          'Opting out will not apply:',
        ],
        listItems: [
          'where the information is used for purposes relating to your individual care',
          'where the confidential patient information does not contain your NHS number if obtaining the number would involve disproportionate effort',
          'if you have given consent for your data to be used for a specific reason, like a medical research study',
          'where data is anonymised which means you cannot be identified from the information',
          'where there is a legal requirement to provide information, such as an order of court',
          'where there is an overriding public interest in the disclosure of your data',
          'where information is used to support the management of communicable diseases and other risks to public health, like meningitis',
          'to information given to the Office for National Statistics for official statistics, like the Population Census',
          'to national patient experience surveys sent out before April 2019',
          'to data shared with Public Health England for the National Cancer Registration Service, the National Congenital Anomalies and Rare Diseases Registration Service and the oversight of population screening programmes',
          'where data is used to make sure people with learning disabilities and/or autism receive the best care possible when in hospital for mental health or challenging behaviour issues (also known as assuring transformation)',
          'where data is used to make sure correct payment is made when there is no contract. For example, if a patient lives in Bromley but is treated in hospital in Devon, an invoice will be sent from Devon to the Clinical Commissioning Group (CCG) in Bromley that holds the budget for the patient',
        ],
      },
      p5: {
        paragraphs: [
          'If you decide to opt out, this will be respected and applied by NHS Digital and Public Health England. These organisations collect, process and release health and adult social care data on a national basis. Your decision will also be respected and applied by all other organisations that are responsible for health and care information by March 2020.',
          'An opt-out will only apply to the health and care system in England. This does not apply to your health data where you have accessed health or care services outside of England, such as in Scotland and Wales.',
        ],
      },
    },
  },
};
