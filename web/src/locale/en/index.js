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
  generic: {
    backButton: {
      text: 'Back',
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
        subheader: 'You are too young to use the NHS App',
        message: 'Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, call 111.',
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
  betaBanner: {
    label: 'BETA',
    caption: 'This is a new service.',
  },
  cookieBanner: {
    caption: {
      line1: 'The NHS website uses cookies to improve your on-site experience.',
      linkText: 'Find out more about cookies',
    },
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
  updatedTermsAndConditions: {
    title: 'Updated conditions of use',
    errorMsgHeader: 'There\'s a problem',
    errorMsgText: 'You cannot continue without agreeing',
    body1: 'We\'ve made some important changes to our conditions of use. To continue using the NHS App, you need to agree to our updated',
    body2: 'If you don\'t agree, you won\'t be able to continue accessing or using the NHS App.',
    link1: 'terms of use',
    link2: 'privacy policy',
    link3: 'cookies policy',
    checkBoxError: 'You cannot use the NHS App without agreeing',
    checkBoxText1: 'I understand and agree to the updated',
    checkBoxText2: 'I agree to the use of \'strictly necessary\' cookies as described in the updated',
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
        header: 'Upcoming appointments',
        text1: 'You don\'t currently have any appointments booked.',
        text2: 'Once you\'ve booked an appointment here, you\'ll be able to view details and cancel it.',
        text3: 'If you have an upcoming appointment that isn\'t shown here, contact your GP surgery for more information.',
      },
      emptyPast: {
        header: 'Past appointments',
        text1: 'You have no recent past appointments. To find out about older appointments, contact your GP surgery.',
      },
      upcoming: {
        header: 'Upcoming appointments',
        info: 'Click in the appointment if you need to cancel it',
      },
      past: {
        header: 'Past appointments',
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
    cancelling: {
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
      desktopBackButtonText: 'Back',
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
      telephoneNumberLabel: 'Phone number for appointment',
      telephoneNumberDescription: 'This number will only be used for this appointment. It will not be saved.',
      headerLabel: 'Reason for this appointment',
      headerLabelSuffix: ' (Optional)',
      reasonDesc: {
        line1: 'Text must be shorter than 150 characters (about 25 words)',
        line2: 'This text may not be read by your GP or practice member until the day of your appointment.',
        line3: 'If it\'s urgent, contact your GP surgery before booking.',
      },
      confirmButtonText: 'Confirm and book appointment',
      changeButtonText: 'Change this appointment',
      backButtonText: 'Back',
      errorDialog: 'There\'s a problem',
      noReasonError: 'Enter a reason for this appointment',
      noPhoneNumberError: 'Enter a telephone number',
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
        466: {
          pageTitle: 'Error submitting request',
          pageHeader: 'Error submitting request',
          header: 'We cannot complete this order',
          subheader: 'You previously ordered at least one of these medications in the last 30 days.',
          message: 'If you need more medication sooner, contact your GP.',
        },
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
    warning: {
      warningText: 'Your record may contain sensitive information. If someone is pressuring you for this information, contact your GP surgery immediately.',
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
      visionDetailsLink: 'View your Test Results',
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
    testresultdetails: {
      backButton: 'Back',
      testResultTitle: 'Test results',
      noTestResultData: 'There is no detail to display here.',
    },
    diagnosis: {
      sectionHeader: {
        default: 'Diagnosis',
      },
      visionDetailsLink: 'View your Diagnosis records',
    },
    diagnosisDetails: {
      backButton: 'Back',
      diagnosisTitle: 'Diagnosis',
      noDiagnosisData: 'No information recorded for this section',
    },
    examinations: {
      visionDetailsLink: 'View your Examination records',
      sectionHeader: {
        default: 'Examinations',
      },
    },
    examinationDetails: {
      backButton: 'Back',
      examinationTitle: 'Examinations',
      noExaminationData: 'No information recorded for this section',
    },
    procedures: {
      visionDetailsLink: 'View your Procedures records',
      sectionHeader: {
        default: 'Procedures',
      },
    },
    procedureDetails: {
      backButton: 'Back',
      procedureTitle: 'Procedures',
      noProcedureData: 'No information recorded for this section',
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
  loginLink: {
    login: 'Login',
  },
  signOutButton: {
    signOut: 'Log out',
  },
  pageHeaders: {
    home: 'Home',
    gpFinder: 'Check what features you can use',
    gpFinderParticipation: 'GP surgery features',
    gpFinderWaitingListSignup: 'Waiting list',
    gpFinderWaitingListJoined: 'Next steps',
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
    organDonation: 'My organ donation decision',
    symptoms: 'Check my symptoms',
    dataSharing: 'Find out why your data matters',
    more: 'More',
    login: 'Login',
    termsAndConditions: 'Accept conditions of use',
  },
  pageTitles: {
    home: 'Home',
    gpFinder: 'GP Finder',
    gpFinderParticipation: 'Features used by your GP surgery',
    gpFinderWaitingListSignup: 'Want us to email you when all features are available at your GP surgery?',
    gpFinderWaitingListJoined: 'What happens next',
    prescriptions: 'Repeat prescriptions',
    repeatPrescriptionCourses: 'Select medication - Repeat prescriptions',
    confirmPrescription: 'Confirm prescription - Repeat prescriptions',
    account: 'My account',
    appointments: 'My appointments',
    appointmentGuidance: 'Check before you book - My appointments',
    appointmentBooking: 'Book new appointment',
    appointmentCancelling: 'Cancel appointment',
    appointmentConfirmation: 'Confirm appointment',
    myRecord: 'Sensitive information - My record',
    organDonation: 'My organ donation decision',
    symptoms: 'Check my symptoms',
    dataSharing: 'Find out why your data matters',
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
    passwordOptions: 'Login & password options',
    accessibilityStatement: 'Accessibility statement',
  },
  sc04: {
    organDonation: {
      subheader: 'Set organ donation preferences',
      body: 'Help save thousands of lives in the UK every year by signing up to become a donor on the NHS Organ Donor Register.',
    },
    dataSharing: {
      subheader: 'Find out why your data matters',
      body: 'Find out how the NHS uses your confidential patient information and choose whether or not it can be used for research and planning.',
    },
  },
  webHeader: {
    title: '{pageTitle} - NHS App',
  },
  navigationMenu: {
    appointmentsLabel: 'Appointments',
    moreLabel: 'More',
    myRecordLabel: 'My medical record',
    prescriptionsLabel: 'Repeat prescriptions',
    symptomsLabel: 'Symptoms',
    accountLabel: 'Account',
    logoutLabel: 'Log out',
  },
  navigationMenuList: {
    symptoms: 'Check my symptoms',
    appointments: 'Book and manage appointments',
    prescriptions: 'Order a repeat prescription',
    myRecord: 'View my medical record',
    organDonation: 'Set organ donation preferences',
  },
  organDonation: {
    additionalDetails: {
      continueButton: 'Continue',
      description: 'This optional information is only used by NHSBT for analysis of the NHS Organ Donor Register and is not stored against your registration.',
      ethnicity: {
        placeholder: 'Please select',
        label: 'Ethnicity (Optional)',
      },
      religion: {
        placeholder: 'Please select',
        label: 'Religion (Optional)',
      },
      subheader: 'Additional details',
    },
    faith: {
      subheader: 'Faith / beliefs',
      continueButtonText: 'Continue',
      errorMsgHeader: 'There\'s a problem',
      errorMsgText: 'You cannot continue without making a selection',
      why: {
        header: 'Why they matter for organ donation',
        paragraphs: [
          'When organ donation is a possibility, NHS staff will always speak to a donor\'s family about the donor\'s decision, medical history, and anything else that would be relevant to organ donation.',
          'We recognise that for some people, this will include their faith or beliefs and they would want organ donation to go ahead in a way that is in line with their beliefs or customs.',
        ],
      },
      help: {
        header: 'What we can do to help',
        description: 'Let us know if you want your faith and beliefs to be a part of discussions between NHS staff, your family, and anyone suggested by your family, when organ donation is a possibility.',
      },
      choices: {
        header: 'Would you like NHS staff to speak to your family (and anyone else appropriate) about how organ donation can go ahead in line with your faith or beliefs?',
        yes: {
          title: 'Yes',
          description: 'I want NHS staff to talk to my family (and other relevant people) about how organ donation works with my faith/beliefs.',
        },
        no: {
          title: 'No',
          description: 'I do not want NHS staff to talk to anyone about organ donation and my faith/beliefs.',
        },
        preferNotToSay: {
          title: 'Prefer not to say',
        },
      },
    },
    links: {
      alreadyRegisteredText: 'Think you have registered already?',
      amendText: 'Amend your decision',
      findOutMoreText: 'Find out more about organ donation',
    },
    register: {
      subheader: 'Register your organ donation decision',
      noButton: {
        header: 'NO',
        subheader: 'I do not want to donate my organs',
      },
      yesButton: {
        header: 'YES',
        subheader: 'I want to donate my organs',
      },
    },
    registered: {
      yourDecision: {
        subheader: 'Your decision',
      },
      decisionDetails: {
        all: 'I want to donate all my organs and tissue.',
      },
      appointedRep: {
        phoneLabel: 'To view, or change, your appointed representative call the organ donation line:',
      },
    },
    yourChoice: {
      subheader: 'Your choice',
      description: 'You can select to donate some, or all of your organs and tissue.',
      continueButtonText: 'Continue',
      errorMessageHeader: 'There\'s a problem',
      errorMessageText: 'Make a decision to continue',
      choices: {
        all: {
          title: 'All my organs and tissue',
          description: 'Help up to nine people through organ donation and even more through tissue',
        },
        some: {
          title: 'Specific organs and tissue',
          description: 'Choose which of your organs and tissue to donate',
        },
      },
    },
    reviewYourDecision: {
      header: 'Check your details before submitting',
      errorMsgHeader: 'There\'s a problem',
      submitButton: 'Yes I want to be a donor',
      submitNoButton: 'No I do not want to be a donor',
      aboutYou: {
        subheader: 'About you',
        nameheader: 'Name',
        dateofbirthheader: 'Date of birth',
        genderheader: 'Gender',
        nhsnumberheader: 'NHS number',
        addressheader: 'Address',
        description: 'The details above are retrieved from your GP services record, please contact your GP to amend them.',
      },
      additionalInformation: {
        subheader: 'Additional information',
        ethnicityheader: 'Ethnicity',
        religionheader: 'Religion',
      },
      faith: {
        subheader: 'Faith / beliefs details',
        description: 'I would like NHS staff to speak to my family and anyone else appropriate about how organ donation can go ahead in line with my faith or beliefs',
        declaration: {
          Yes: 'Yes',
          No: 'No',
          NotStated: 'Prefer not to say',
        },
      },
      decisionDetails: {
        chosenHeader: 'You have chosen to donate:',
        notChosenHeader: 'You have chosen not to donate:',
        choices: {
          heart: 'Heart',
          lungs: 'Lungs',
          kidney: 'Kidney',
          liver: 'Liver',
          corneas: 'Corneas',
          pancreas: 'Pancreas',
          tissue: 'Tissue',
          smallBowel: 'Small bowel',
        },
      },
      yourDecision: {
        subheader: 'Your decision',
        appointedrepDecisionText: 'I have appointed a representative',
        optoutDecisionText: 'No, I do not want to donate my organs',
        optinDecisionText: 'Yes, I do want to donate my organs',
        optinSomeDecisionText: 'Specific organs and tissue',
      },
      confirmation: {
        subheader: 'Confirmation',
        accuracyText: 'I confirm that the information given in this form is true, complete and accurate',
        privacyText1: 'I have read the ',
        privacyText2: ' and give consent for the use of my information in accordance with the terms',
        privacyLinkText: 'privacy statement',
        errors: {
          accuracy: 'You must confirm that the information provided is true, complete and accurate.',
          privacy: 'You must confirm that you have read the privacy statement and consent to your information being used accordingly.',
        },
      },
    },
    viewDecision: {
      conflictedState: {
        messageText: 'Your registration is currently being processed.',
        dialogText: 'Decision found',
        registrationHeader: 'We are still processing your registration',
        registrationText: 'Please check back in 2 days. You’ll then be able to view and amend your ' +
          'decision via the NHS App. Remember to let your family know your decision about organ donation.',
      },
      decisionSubmitted: {
        messageText: 'We have successfully received your organ donation decision.',
        dialogText: 'Decision submitted',
        registrationHeader: 'What happens next',
        registrationText: 'We will process your decision and you will then be able to view and ' +
          'amend this via the NHS App. This may take up to 4 days. Remember to let ' +
          'your family know your decision about organ donation.',
      },
      successMessageText: 'We have updated your decision',
      successMessageDialogText: 'Success',
      otherThings: {
        subheader: 'Other things you can do',
        bloodDonation: {
          subheader: 'Register to be a blood donor',
          body: 'If you want to give more, why not sign up to give blood? You can easily book an appointment and find your local centre via the app.',
        },
      },
      nextSteps: {
        subheader: 'Next steps',
        shareDecision: {
          subheader: 'Share that you are a donor',
          body: 'Help promote organ donation on social media by telling people you are a donor.',
        },
        tellFamily: {
          subheader: 'Tell your family',
          body: 'Use our message templates and conversation guides to tell your family and friends you are a donor.',
        },
        optOutText: 'Please inform your family about your decision.',
      },
    },
    someOrgans: {
      subheader: 'Your choice',
      description: 'Please select which organs and tissue you wish to donate:',
      continueButtonText: 'Continue',
      errorMsgHeader: 'There\'s a problem',
      allSelectedValidationText: 'Make a decision for all categories to continue',
      yesRequiredValidationText: 'At least one category must be set to yes to continue',
      choiceYes: 'Yes',
      choiceNo: 'No',
      heartTitle: 'Heart',
      lungsTitle: 'Lungs',
      kidneyTitle: 'Kidney',
      liverTitle: 'Liver',
      corneasTitle: 'Corneas',
      pancreasTitle: 'Pancreas',
      tissueTitle: 'Tissue',
      smallBowelTitle: 'Small bowel',
    },
  },
  symptomBanner: {
    howAreYouFeeling: 'How are you feeling today?',
    checker: 'Check symptoms',
  },
  login: {
    desc: 'To access your NHS services',
    moreFeaturesComingSoon: 'More features will be coming soon to this GP surgery',
    notMyGpSurgery: 'This isn\'t my GP surgery',
    checkWhatFeaturesYouCanUse: 'Check what features you can use',
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
    organDonation: {
      appointedRepIcon: {
        title: 'Appointed Representative',
        description: 'Indicates that you have appointed a representative',
      },
      noIcon: {
        title: 'No',
        description: 'Indicates not to donate organs',
      },
      yesIcon: {
        title: 'Yes',
        description: 'Indicates a wish to donate organs',
      },
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
    emptySearchError: 'Enter postcode, town or GP surgery name',
  },
  th03: {
    header: 'Select your GP surgery',
    title: 'Select your GP surgery',
    errors: {
      backButton: 'Back',
      serviceUnavailable: {
        header: 'Technical problems',
        title: 'Technical problems',
        errorDialogHeader: 'We are experiencing technical problems',
        errorDialogText: 'Something has gone wrong with this service. It wasn\'t your fault.',
        mainContent: 'Come back later. If it still isn\'t working then, contact us about the problem.',
      },
      tooManyResults: {
        header: 'Too many results found',
        title: 'Too many results found',
        tooManyResultsHeader: 'Can\'t find your GP surgery?',
        foundTooManyResults: 'We can only show 20 results for what you search. The more specific your search, the better the results.',
        suggestionHeader: 'Please try searching:',
        suggestions: [
          'Your GP surgery postcode',
          'The name of your GP surgery',
          'A combination of these',
        ],
      },
      noResultsFound: {
        header: 'No results found',
        title: 'No results found',
        noResultsFoundHeader: 'No results found',
        foundNoResults: 'We found no GP surgeries near "{searchQuery}".',
        suggestionHeader: 'Please try:',
        suggestions: [
          'Checking you spelt the GP surgery name or town correctly',
          'Searching with a postcode',
        ],
      },
    },
  },
  th04: {
    featuresHeader: 'NHS App features used by',
    currentlyAvailableHeader: 'Currently available',
    comingSoonHeader: 'Coming soon',
    defaultFeatures: [
      'Check symptoms',
    ],
    conditionalFeatures: [
      'Book and manage appointments',
      'Order repeat prescriptions',
      'View your medical records',
    ],
    createAccountMessage: 'To use the NHS App fully, you\'ll need to create an NHS account.',
    ctaContinue: 'Continue',
    ctaNotMySurgery: 'This is not my GP surgery',
    limitingFeaturesWarning: 'Each GP surgery manages its own online services. They may choose to limit some of these features. Contact your surgery for more information.',
  },
  th05: {
    header: 'Waiting list',
    notEnteredEmailError: 'Enter your email address',
    invalidEmailError: 'Enter a valid email address',
    submissionError: 'There was a problem adding you. Please try again',
    connectionError: 'There was a problem adding you. Please try again',
    choiceError: 'Choose if you\'d like to be updated by email',
    emailFeatureText: 'Get an email when all features are available at your GP surgery',
    yesRadioButtonText: 'Yes, email me when my GP surgery has these features',
    noRadioButtonText: 'No, I will check with my GP surgery myself to see when they have those features',
    privacyStatement: 'We will only use this email address to contact you about the NHS App. We will store your email address for a maximum of 12 months, as outlined in ',
    privacyPolicyLinkText: 'our privacy policy.',
    emailText: 'Email address',
    callToAction: 'Continue',
  },
  th06: {
    header: 'Waiting list',
    whatHappensNextHeading: 'What happens next',
    whatHappensNextJoinedParagraph1: 'We\'ve just sent you an email. You need to confirm you want to be updated by us about your GP surgery. You do this by following a link in that email. It may be in your junk folder.',
    whatHappensNextJoinedParagraph2: 'When your GP surgery can use all the features of the app, we\'ll email you. You\'ll then be able to create an NHS login.',
    whatHappensNextNotJoinedParagraph: 'Check in with your GP surgery to find out when they\'ll be using all the features of the app. When they are, they will help you set up an NHS login.',
    untilThenHeading: 'What you can do until then',
    untilThenParagraph: 'You can check your symptoms in the app. \'Check if you need urgent help\' will direct you to medical help, if you need it.',
    gpSurgeryFeatureText: 'When your GP surgery can use all features of the app, we’ll send you an email.',
    homeButton: 'Go to home screen',
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
    header: 'Find out why your data matters',
    titles: {
      p1: 'Overview',
      p2: 'Where confidential patient information is used',
      p3: 'Where your choice does not apply',
    },
    subtitle: 'Manage your data choice',
    startNowButton: 'Start now',
    nextButton: 'Next',
    previousButton: 'Previous',
    pages: {
      p1: {
        intro: {
          paragraph: 'Your health records contain confidential patient information, which can be used to help with research and planning. If you would like this to stop, you can opt out of this. Your choice will only apply to the health and care system in England. This does not apply to health or care services accessed in Scotland, Wales or Northern Ireland.',
        },
        confidential: {
          title: 'What confidential patient information is',
          paragraph1: 'Two types of information join together to become confidential patient information. This is information that:',
          listItems: [
            'can identify you',
            'says something about your health care or treatment',
          ],
          paragraph2: 'One example can include your name and address (identifies you) along with what medicine you take (health care or treatment). Identifiable information on its own is used by health and care services to contact patients and this is not confidential patient information.',
        },
        patientInformation: {
          title: 'How we use your confidential patient information',
          yourIndividualCareSubtitle: 'Your individual care',
          yourIndividualCareParagraph: 'Health and care staff may use your confidential patient information to help with your treatment and care. For example, when you visit your GP, they may look at your records for important information about your health.',
          researchAndPlanningSubtitle: 'Research and planning',
          researchAndPlanningParagraph: 'Confidential patient information is also used to:',
          researchAndPlanningListItems: [
            'plan and improve health and care services',
            'research and develop cures for serious illnesses',
          ],
        },
        yourChoice: {
          title: 'Where you have a choice',
          paragraph: 'You can stop your confidential patient information being used for research and planning. Your confidential patient information will still be used for your individual care. Any choice you make will not change this.',
        },
        moreOptions: {
          title: 'More options',
          paragraph: {
            nhsWebsiteLink: 'Visit the NHS.UK website',
            part2: ' for more information or to read our privacy notice. You can also find out how to manage a choice on behalf of another person. For example, if you are a parent or guardian of a child under the age of 13.',
          },
        },
      },
      p2: {
        intro: {
          paragraphs: [
            'The NHS collects confidential patient information from all NHS organisations, trusts and local authorities. Confidential patient information is also collected from private organisations, such as private hospitals providing NHS funded care. Research bodies and organisations can request access to this information.',
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
          title: 'Who cannot use confidential patient information',
          paragraphs: [
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
          paragraph: '(unless you specifically request this)',
        },
        dataProtection: {
          title: 'How confidential patient information is protected',
          paragraphs: [
            'Protection of your confidential patient information is taken very seriously and it is looked after in accordance with good practice and the law. There are very strict rules on how any of your data can and cannot be used, and you have clear legal rights.',
            'Every organisation that provides health and care services must take every step to:',
          ],
          listItems: [
            'ensure all data remains secure',
            'use data that does not identify you whenever possible',
            'use confidential patient information to benefit health and care only',
            'not use confidential patient information for marketing or insurance purposes (unless you specifically request this)',
            'make it clear why and how data is being used',
            'respect your decision if you decide to opt out',
            'only use information about you where allowed by the law',
          ],
          paragraph: 'All NHS organisations must provide information on the type of data they collect and how it is used. Data release registers are published by NHS Digital and Public Health England, showing records of the data they have shared with other organisations.',
        },
      },
      p3: {
        paragraphs: [
          'If you choose not to allow your confidential patient information to be used for research and planning, your data may still be used in some situations.',
        ],
        listItems: [
          {
            title: 'When required by law',
            text: 'Your confidential patient information may still be used when there is a legal requirement to provide it, such as a court order.',
          },
          {
            title: 'When you have given consent',
            text: 'Your confidential patient information may still be used when you have given your consent. Such as, for a medical research study.',
          },
          {
            title: 'Where there is overriding public interest',
            text: 'Your confidential patient information may still be used in an emergency or in situations where there is an overriding benefit to others. For example, to help manage contagious diseases and stop them spreading, like meningitis. In these situations, the safety of others is most important.',
          },
          {
            title: 'When information that can identify you is removed',
            text: 'Information about your health care or treatment may still be used in research and planning if the information that can identify you is removed first.',
          },
          {
            title: 'Where there is a specific exclusion',
            text: 'Your choice does not apply to a small number of specific exclusions. In these cases, your confidential patient information may still be used at any time. For example, when information is used to collect official national statistics, like the Population Census.',
          },
        ],
      },
    },
  },
  web: {
    home: {
      title: 'NHS App',
      bullets: {
        one: 'Book and manage your appointments at any time',
        two: 'Order repeat prescriptions from wherever you are',
        three: 'Check your symptoms and get instant advice on what to do',
        four: 'Track your medication history, with secure access to your medical record',
      },
      checkSymptoms: {
        title: 'How are you feeling right now?',
      },
    },
  },
};
