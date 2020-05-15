import components from './components';
import thirdPartyProviders from './third-party-providers';

export default {
  language: 'en-GB',
  appTitle: 'NHS App',
  errors: {
    pageHeader: 'Server error',
    header: 'We\'re experiencing technical difficulties',
    subheader: '',
    message: {
      text: 'Try again later. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
      label: 'Try again later. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
    },
    404: {
      pageTitle: 'Page not found',
      header: 'Page not found',
      subheader: 'If you entered a web address, check it was correct.',
      message: {
        text: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need. For urgent medical advice, call 111.',
        label: 'You can go directly to book an appointment or order a repeat prescription, or use the menu buttons to find the service you need. For urgent medical advice, call one one one.',
      },
    },
    502: {
      pageTitle: 'Service currently unavailable',
      pageHeader: 'Service currently unavailable',
      header: 'This service is not available right now',
      retryButtonText: 'Try again',
      message: 'Try again in a few moments.',
    },
  },
  messageIconText: {
    success: 'Success',
    warning: 'Warning',
    error: 'Error',
    message: 'Message',
    important: 'Important',
  },
  generic: {
    questions: {
      attachment: {
        label: 'File',
      },
      boolean: {
        labels: {
          true: 'Yes',
          false: 'No',
        },
      },
      quantity: {
        initialUnitDropdownValue: 'Select unit',
        labels: {
          unit: 'Unit',
          quantity: 'Quantity',
        },
      },
      date: {
        labels: {
          day: 'Day',
          month: 'Month',
          year: 'Year',
        },
      },
      time: {
        labels: {
          hour: 'Hour',
          minute: 'Minute',
        },
      },
    },
    backButton: {
      text: 'Back',
    },
    contactUsButton: {
      text: 'Contact us',
    },
    tryAgainButton: {
      text: 'Try again',
    },
    input: {
      errors: {
        messagePrefix: 'Error: ',
      },
    },
    table: {
      errors: {
        noData: 'Error loading table data',
      },
    },
  },
  auth_return: {
    error: {
      title: {
        loginFailed: 'Login failed',
      },
      backButtonText: 'Back to home',
      default: {
        line1: 'We cannot log you in to the NHS App.',
        line2: 'Go back to the home screen and try logging in again.',
        line3: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        line4: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
      400: {
        line1: 'Go back to the home screen and try logging in again.',
        line2: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        contactUs: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
      403: {
        line1: 'We cannot get your details from your GP surgery.',
        line2: 'Go back to the home screen and try logging in again.',
        line3: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        line4: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
      464: {
        line1: 'This can be one of two problems:',
        uList: {
          item1: {
            id: '1',
            text: 'we cannot find your GP surgery',
            label: 'we cannot find your GP surgery',
          },
          item2: {
            id: '2',
            text: 'we cannot find your NHS number',
            label: 'we cannot find your NHS number',
          },
        },
        contactUs: 'Contact us and quote the error code {errorCode} to help us resolve the problem more quickly.',
        message: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one',
        },
      },
      465: {
        title: 'Login failed',
        message: {
          text: 'Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. You can still contact your GP surgery to access your NHS services. For urgent medical advice, go to 111.nhs.uk or call 111.',
          label: 'Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. You can still contact your GP surgery to access your NHS services. For urgent medical advice, go to 111.nhs.uk or call one one one.',
        },
      },
      500: {
        line1: 'We cannot log you in to the NHS App.',
        line3: 'Go back to the home screen and try logging in again.',
        line4: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        line5: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
      502: {
        listTitle: 'This can be one of two problems:',
        uList: {
          item1: {
            id: '1',
            text: 'we cannot get your NHS login details',
            label: 'we cannot get your NHS login details',
          },
          item2: {
            id: '2',
            text: 'we cannot connect to your GP surgery',
            label: 'we cannot connect to your GP surgery',
          },
        },
        line3: 'Go back to the home screen and try logging in again.',
        line4: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        message: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
      504: {
        listTitle: 'This can be one of two problems:',
        uList: {
          item1: {
            id: '1',
            text: 'we cannot get your NHS login details',
            label: 'we cannot get your NHS login details',
          },
          item2: {
            text: 'we cannot connect to your GP surgery',
            id: '2',
            label: 'we cannot connect to your GP surgery',
          },
        },
        line3: 'Go back to the home screen and try logging in again.',
        line4: 'If you keep seeing this message, contact us and quote the error code {errorCode}.',
        message: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
    },
  },
  rp01: {
    pageTitle: 'TBC',
    empty: {
      subHeader: 'You don\'t currently have any repeat prescriptions ordered',
      line1: 'Once you\'ve placed an order here, you\'ll be able to view your repeat prescription status and history.',
      line2: 'If you have an existing order that is not shown here, contact your GP surgery or pharmacy for more information.',
    },
    hiddenText: {
      nominatedPharmacy: 'Your nominated pharmacy',
      noNominatedPharmacy: 'How you get your prescriptions',
    },
    changePharmacyLink: 'Change',
    orderPrescriptionButton: 'Order a repeat prescription',
    nominatedPharmacy: 'Your nominated pharmacy is:',
    noNominatedPharmacy: 'You do not have a nominated pharmacy.',
    dispensingPractice: 'My dispensing practice',
    glossary: {
      headerText: 'You may see medical abbreviations that you are not familiar with.',
      linkText: 'Help with abbreviations',
    },
  },
  rp02: {
    pageTitle: 'TBC',
    orderDate: 'Ordered',
    proxyOrderedByDisplayPrefix: 'by ',
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
    medicationCourse: {
      line1: 'Medications currently available for repeat prescription',
      paragraph1: '',
    },
    specialRequestRequired: 'Enter any special requests relating to this order',
    specialRequestsLabelOptional: 'Special requests relating to this order (optional)',
    specialRequestsLabelMandatory: 'Special requests relating to this order',
    maxSpecialRequest: 'Limit is 1000 characters (about 150 words)',
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
    nominatedPharmacyHeader: 'Your nominated pharmacy',
    dispensingPracticeHeader: 'Your dispensing practice',
    confirmButton: 'Confirm and order prescription',
    backButton: 'Change this prescription',
  },
  rp05: {
    confirmationMessage: 'The order status will be updated once it’s been reviewed by your GP.​',
  },
  rp12: {
    reasonMissing: {
      summarySubHeader: 'There\'s a problem',
      summaryBody: 'Select a medication',
      inline: 'Select a medication',
    },
  },
  nominatedPharmacySearchResults: {
    highStreet: {
      header: 'High street pharmacies near "{searchQuery}"',
      title: 'High street pharmacies near "{searchQuery}"',
    },
    online: {
      random: {
        header: 'Choose an online-only pharmacy',
        title: 'Choose an online-only pharmacy',
        information: 'This is a selection of online-only pharmacies.',
      },
      search: {
        header: 'You searched "{searchQuery}"',
        title: 'You searched "{searchQuery}"',
      },
    },
    resultSummary: {
      distanceInformation: 'Distances given are in a straight line but travel routes may be longer.',
      beMoreSpecific: 'We can only show {max} results for your search. To improve these results, be as specific as possible with the name of the online-only pharmacy.',
    },
    backButton: 'Search again',
    distanceAway: '{distance} miles away',
    telephoneLabel: 'Telephone: ',
    searchAgain: 'Search again',
    errors: {
      noResultsFound: {
        header: 'No results found for "{searchQuery}"',
        title: 'No results found for "{searchQuery}"',
        foundNoResults: 'We could not find any results for "{searchQuery}". Make sure you enter a valid English postcode.',
      },
    },
  },
  nominatedPharmacyNotFound: {
    warningText: 'This means you\'ll collect a paper prescription from your GP.',
    line: 'If you nominate a pharmacy, your prescription will be sent there.',
    nominatedPharmacyLink: 'Nominate a pharmacy',
    continueButton: 'Continue without nominating',
    backButton: 'Back',
    noPharmacyButton: 'You\'ve not nominated a pharmacy',
  },
  nominatedPharmacyOnlineOnlyChoices: {
    continueButton: 'Continue',
    yesLabel: 'Yes, I know the name of the online-only pharmacy I want',
    noLabel: 'No',
    errorMessageHeader: 'There\'s a problem',
    errorMessageText: 'Select yes if you know the name of your online-only pharmacy',
  },
  nominatedPharmacyOnlineOnlySearch: {
    inputLabelText: 'What is the name of the online-only pharmacy?',
    searchButton: 'Search',
    errorMessageHeader: 'There\'s a problem',
    errorMessageText: 'Enter the name of the online-only pharmacy',
    noResultsHelpText: 'We could not find any results for "{searchQuery}". Make sure you enter the pharmacy name correctly.',
    searchAgainMessage: 'Search again',
  },
  nominatedPharmacyCannotChange: {
    line1: 'This is because your nominated pharmacy forms part of your GP surgery.',
    howToHeader: 'How to change your nominated pharmacy',
    line2: 'To nominate a new pharmacy, you need to contact your GP surgery directly.',
    backButton: 'Back',
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
    analyticsCookieCheckBoxText: 'I accept the use of optional analytic cookies used to improve the performance of the NHS App.',
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
  onlineConsultations: {
    unavailable: {
      paragraph: 'This service is normally available during your GP surgery\'s opening hours.',
      coronavirusHeading: 'If you think you might have coronavirus',
      coronavirusParagraph: 'Stay at home and avoid close contact with other people.',
      coronavirusLink: 'Use the 111 coronavirus service to see if you need medical help',
    },
    demographics: {
      checkboxLabel: 'Use my name, date of birth, NHS number and postal address with the online consultation service as described in the',
      checkboxLink: 'NHS App privacy policy',
    },
    warning: {
      warningText: 'This service is provided by an online consultation service provider, {providerName}, on behalf of your GP surgery.',
      warningLink: 'Find out more about online consultation services.',
    },
    validationErrors: {
      header: 'There\'s a problem',
      message: {
        attachment: 'Select a file',
        boolean: 'Select an option',
        choice: 'Select an option',
        date: 'Enter a real date and include a day, month and year',
        dateEmpty: 'Enter a date',
        dateTime: 'Enter the full date and time',
        dateTimeEmpty: 'Enter a date and time',
        decimal: 'Enter a number',
        image: 'Select the area on the image',
        integer: 'Enter a number',
        multiple_choiceOnlyOneOption: 'Select the option',
        multiple_choiceAtLeastOneRequired: 'Select one or more options',
        multiple_choiceAllRequired: 'All options are required',
        quantity: 'Enter a number',
        quantityUnit: 'Select an option',
        quantityTooHigh: 'Enter {value} or less',
        quantityTooLow: 'Enter {value} or more',
        string: 'Answer the question',
        stringTooLong: 'Answer must be {value} characters or less',
        text: 'Answer the question',
        textTooLong: 'Answer must be {value} characters or less',
        time: 'Enter a valid time',
        // eslint-disable-next-line no-template-curly-in-string
        overMaxValueNumber: 'Enter {value} or less',
        // eslint-disable-next-line no-template-curly-in-string
        underMinValueNumber: 'Enter {value} or more',
        default: 'Answer all required questions',
      },
    },
    question: {
      optionalLabel: 'optional',
    },
    orchestrator: {
      continueButton: 'Continue',
      endMyConsultationButton: 'End my consultation',
      backToHomeButton: 'Back to home',
      backButton: 'Back',
    },
  },
  appointments: {
    index: {
      successText: 'Your appointment has been booked. You can view details or cancel it here.',
      successAndCancellationDisabledText: 'Your appointment has been booked. You can view details here.',
      bookButtonText: 'Book an appointment',
      cancelButtonText: 'Cancel this appointment',
      cancellationDisabledText: 'To cancel this appointment, contact your GP surgery.',
      locationLabel: 'Location',
      appointmentTypeLabel: 'Appointment type',
      empty: {
        header: 'Upcoming appointments',
        text1: 'If you have an upcoming appointment that is not shown here, contact your GP surgery for more information.',
      },
      emptyPast: {
        header: 'Past appointments',
        text1: 'You have no recent past appointments. To find out about older appointments, contact your GP surgery.',
      },
      upcoming: {
        header: 'Upcoming appointments',
        info: 'Click in the appointment if you need to cancel it',
        telephoneMessage: 'We will call you on ',
      },
      past: {
        header: 'Past appointments',
        telephoneMessage: 'The phone number you gave us was ',
      },
    },
    guidance: {
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
      menuItem1: {
        header: 'Get help with symptoms',
        text: 'Find information about specific conditions',
      },
      menuItem2: {
        header: 'Additional GP services',
        text: 'Get sick notes and GP letters or ask about recent tests',
      },
      menuItem3: {
        header: 'Ask your GP for advice',
        text: 'Consult your GP through an online form. Your GP surgery will reply by phone or email.',
      },
      symptomButtonText: 'Check symptoms',
      bookButtonText: 'Book an appointment',
      backDesktopLinkText: 'Back',
      backButtonText: 'Back',
    },
    noSlotErrorMessage: {
      summary: 'There are no appointments available at the moment',
      info: 'If you need an appointment, please contact your GP.',
    },
    error: {
      title: {
        problem: 'Sorry, there is a problem - Appointments',
        problemLoading: 'Sorry, there is a problem with loading GP appointments',
        unavailable: 'GP appointment booking unavailable',
      },
      header: {
        problem: 'Sorry, there is a problem',
      },
      message: {
        goBackAndTryContact: 'Go back and try again. If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        ifItContinues: {
          text: 'If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.',
          label: 'If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call one one one.',
        },
        tryAgainNow: 'Try again now. If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        ifItContinuesBookOrCancel: {
          text: 'If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.',
          label: 'If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call one one one.',
        },
      },
      400: {
        message: {
          text: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
      403: {
        line1: 'You are not currently able to book or view GP appointments with the NHS App.',
        line2: {
          text: 'Contact your GP surgery for more information. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'Contact your GP surgery for more information. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
        coronaHeader: 'If you think you might have coronavirus',
        coronaText: 'Stay at home and avoid close contact with other people.',
        coronaServiceLink: {
          text: 'Use the 111 coronavirus service to see if you need medical help',
          label: 'Use the one one one coronavirus service to see if you need medical help',
        },
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
      error: {
        403: {
          title: 'Sorry, there is a problem with the service - Appointments',
          header: 'Contact your GP surgery to cancel',
          message: 'You cannot cancel appointments online right now. Call your GP surgery as soon as possible to let them know you need to cancel.',
        },
        409: {
          title: 'Sorry, you cannot cancel this appointment',
          message: 'This may be because the appointment is already cancelled or it occurs in the past.',
        },
        461: {
          title: 'Contact your GP surgery to cancel',
          message: 'It\'s too late to cancel this appointment online. Call your GP surgery as soon as possible to let them know you need to cancel.',
        },
      },
    },
    booking: {
      desktopBackButtonText: 'Back',
      backButtonText: 'Back to your appointments',
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
        line2: 'If it\'s urgent and you do not know what to do, call 111 to get help near you.',
      },
      adjustSearch: {
        title: 'No appointments available',
        line1: 'Try to filter appointments by a different period or select "No preference" for the practice member. If you cannot find the appointment you need, call your GP surgery.',
        line2: 'If it\'s urgent and you do not know what to do, call 111 to get help near you.',
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
          label: 'Practice member (optional)',
          default_option: 'No preference',
        },
        date: {
          header: 'Available appointments',
          label: 'Filter available appointments',
          options: {
            today: 'Today',
            tomorrow: 'Tomorrow',
            this_week: 'This week',
            next_week: 'Next week',
            all: 'Next eight weeks',
          },
        },
      },
    },
    confirmation: {
      telephoneNumberLabel: 'Choose a phone number for this appointment',
      telephoneNumberDescription: 'This number will only be used for this appointment. It will not be saved.',
      headerLabel: 'Give a reason for this appointment',
      headerLabelSuffix: ' (optional)',
      useOtherPhoneNumberLabel: 'Use other phone number',
      reasonDesc: {
        line1: 'Text must be shorter than 150 characters (about 25 words).',
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
      info: 'Check your appointment details before booking.',
      error: {
        409: {
          title: 'Sorry, the GP appointment is no longer available',
          message: 'Please choose a different appointment.',
        },
        460: {
          title: 'Sorry, you have reached your GP appointment limit',
          line1: 'You cannot book any more appointments right now.',
          line2: 'Contact your GP surgery if you still need to book an appointment.',
          line3: 'You can go back to see what you have already booked and cancel any appointments that you may no longer need.',
          line4: {
            text: 'For urgent medical advice, go to 111.nhs.uk or call 111.',
            label: 'For urgent medical advice, go to 111.nhs.uk or call one one one.',
          },
        },
      },
    },
    bookingSuccess: {
      proxyMessage: 'You have booked an appointment for {name}.',
      message: 'Your GP appointment has been booked',
      back: 'Go to your appointments',
    },
    cancellingSuccess: {
      proxyMessage: 'You have cancelled {name}\'s appointment.',
      message: 'Your GP appointment has been cancelled',
    },
    informatica: {
      header: 'Appointment booking is not currently available directly through the NHS App',
      description: 'Your GP surgery uses Appointments Online to book appointments, and you’ll need a username and password from your GP surgery.',
      link: {
        prefix: 'If you have a username and password, ',
        text: 'log in to Appointments Online',
      },
    },
    gp_advice: {
      unavailable: {
        headerCaption: 'Ask your GP for advice',
        header: 'Online consultations are currently unavailable',
      },
      demographicsQuestion: {
        p1: 'It takes around 5 minutes to answer a few questions about your condition.',
        p2: 'To save you typing in personal information the online consultation service needs, you can use the personal information we already hold.',
      },
      errors: {
        pageTitle: 'Server error',
        pageHeader: 'Server error',
        header: 'Sorry, we\'re experiencing technical difficulties',
        subheader: 'Please try again later.',
        message: {
          text: 'If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
      },
      conditions: {
        paragraph: 'To ensure we ask you relevant questions, choose your condition.',
        link: 'I cannot find my condition',
      },
    },
    admin_help: {
      unavailable: {
        headerCaption: 'Additional GP services',
        header: 'Online consultations are currently unavailable',
      },
      demographicsQuestion: {
        p1: 'Use this service to contact your GP surgery for things like test results, sick notes, GP letters and medical reports.',
        p2: 'It takes around 5 minutes to answer a few questions.',
        p3: 'To save you typing in personal information the online consultation service needs, you can use the personal information we already hold.',
      },
      errors: {
        pageTitle: 'Server error',
        pageHeader: 'Server error',
        header: 'Sorry, we\'re experiencing technical difficulties',
        subheader: 'Please try again later.',
        message: {
          text: 'If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'If the problem persists and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
      },
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
  prescriptions: {
    errors: {
      pageTitle: 'Prescription data error',
      pageHeader: 'Prescription data error',
      header: 'There\'s been a problem getting your prescription information',
      subheader: '',
      message: {
        text: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        label: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      },
      retryButtonText: '',
    },
    view_orders: {
      errors: {
        pageTitle: 'Prescription data error',
        pageHeader: 'Prescription data error',
        header: 'There\'s been a problem getting your prescription information',
        subheader: '',
        message: {
          text: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: '',
        504: {
          pageTitle: 'Prescription data error',
          pageHeader: 'Prescription data error',
          header: 'There\'s been a problem getting your prescription information',
          subheader: '',
          message: {
            text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: 'Try again',
        },
        403: {
          pageTitle: 'Repeat prescriptions unavailable',
          pageHeader: 'Repeat prescriptions unavailable',
          header: 'You are not currently able to order repeat prescriptions online',
          subheader: '',
          message: {
            text: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
            label: 'Contact your GP surgery for more information. For urgent medical help, call one one one.',
          },
        },
      },
    },
    repeat_courses: {
      errors: {
        pageTitle: 'Prescription data error',
        pageHeader: 'Prescription data error',
        header: 'There\'s been a problem getting your prescription information',
        subheader: '',
        message: {
          text: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: '',
        504: {
          pageTitle: 'Prescription data error',
          pageHeader: 'Prescription data error',
          header: 'There\'s been a problem getting your prescription information',
          subheader: '',
          message: {
            text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: 'Try again',
        },
        403: {
          pageTitle: 'Repeat prescriptions unavailable',
          pageHeader: 'Repeat prescriptions unavailable',
          header: 'You are not currently able to order repeat prescriptions online',
          subheader: '',
          message: {
            text: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
            label: 'Contact your GP surgery for more information. For urgent medical help, call one one one.',
          },
        },
      },
    },
    confirm_prescription_details: {
      errors: {
        pageTitle: 'Prescription order error',
        pageHeader: 'Error sending order',
        header: 'There\'s been a problem sending your order',
        subheader: '',
        message: {
          text: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Go back and try again. If the problem continues and you need to order a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: 'Go to your prescriptions',
        466: {
          pageTitle: 'Error submitting request',
          pageHeader: 'Error submitting request',
          header: 'We cannot complete this order',
          subheader: 'You previously ordered at least one of these medications in the last 30 days.',
          message: 'If you need more medication sooner, contact your GP.',
        },
      },
    },
    partialSuccess: {
      medicationNotOrdered: 'Medication not ordered',
      needMedicationNow: 'If you need to order this medication now, contact your GP surgery directly.  For urgent medical advice, visit 111.nhs.uk or call 111.',
      medicationOrdered: 'Medication ordered successfully',
      orderStatusUpdate: 'The order status will be updated once it\'s been reviewed by your GP.',
      backButton: 'Go to your prescription orders',
    },
    orderSuccess: {
      proxyMessage: 'You have ordered a prescription on behalf of {name}. The order status will be updated once it has been reviewed by {givenName}\'s GP.',
      message: 'Your prescription has been ordered. The order status will be updated once it\'s been reviewed by your GP.',
      back: 'Go to your prescription orders',
    },
  },
  nominated_pharmacy: {
    telephoneLabel: 'Telephone: ',
    changePharmacyLink: 'Change your nominated pharmacy',
    openingTimes: 'Opening times',
    continueButton: 'Continue',
    closed: 'Closed',
    nominatedPharmacyInstruction: 'If you order repeat medication using the NHS App, this pharmacy is where they will be sent.',
    dispensingPracticeInstruction: 'If you order repeat medication using the NHS App, this is where they will be sent.',
    confirm: {
      confirmButton: 'Confirm',
      confirmHelpText: 'This is the pharmacy where your prescriptions will be sent in future.',
      pharmacyChanged: 'You changed your nominated pharmacy.',
      pharmacyChosen: 'You\'ve chosen your nominated pharmacy.',
      line1: 'If you order prescriptions using the NHS App, this pharmacy is where they will be sent.',
      telephoneLabel: 'Telephone: ',
      errors: {
        pageHeader: 'Error updating nomination',
        pageTitle: 'Error updating nomination',
        header: 'There\'s been a problem updating your pharmacy nomination',
        message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
        retryButtonText: 'Back to prescriptions',
      },
    },
    changeSuccess: {
      header: 'Your nominated pharmacy is: ',
      highStreetPrescription: 'Your prescriptions will be sent to {pharmacyName} from now on.',
      highStreetPrescriptionCollectionMessage: 'Collect your prescriptions from {pharmacyName} when they are ready to collect.',
      whatHappensNext: 'What happens next',
      registrationWarning: 'You may need to register with {pharmacyName} separately at ',
      registrationWarningWithNoUrl: 'You may need to register with {pharmacyName} separately.',
      fullStop: '.',
      postalWarning: 'Your prescriptions will be sent to {pharmacyName}. Once they’ve checked and prepared your prescriptions, {pharmacyNameRepeated} will send them to you in the post.',
      telephoneLabel: 'Telephone: ',
      linkLabel: 'Go to your prescriptions',
    },
    chooseType: {
      highStreet: 'High street pharmacies',
      highStreetHint: 'You can collect your prescriptions in person at a high street pharmacy local to you. They may also deliver. You\'ll be able to nominate a high street pharmacy in the NHS App.',
      online: 'Online-only pharmacies',
      onlineHint: 'These send your prescriptions to you in the post. It\'s not possible to nominate an online-only pharmacy in the NHS App. You\'ll need to register with the pharmacy directly instead.',
      buttonText: 'Continue',
      errorMessage: 'Choose a type of pharmacy',
      errorHeading: 'There is a problem',
      backLinkText: 'Back',
    },
    internetPharmacy: 'This is an internet pharmacy.',
    warning: {
      changeDispensingPractice: {
        line1: 'You cannot change your dispensing practice with the NHS App.',
        line2: 'To nominate a new dispensing practice or pharmacy, contact your GP surgery directly.',
      },
    },
    search: {
      pageTitle: 'Change your nominated pharmacy',
      searchInputLabel: 'Enter a postcode',
      searchInputHint: 'For example, LS1',
      searchButton: 'Search',
      link1: 'See all internet pharmacies',
      link2: 'See all dispensing appliance contractors',
      errorMessageHeader: 'There\'s a problem',
      emptySearchError: 'Enter a valid English postcode',
      warning: {
        paragraph1: 'The new pharmacy you choose is where your repeat prescription will be sent in future. Any outstanding repeat prescriptions may still arrive at your current nominated pharmacy.',
        paragraph2: 'Your new nominated pharmacy can help to check this for you.',
      },
      errors: {
        pageHeader: 'Service currently unavailable',
        pageTitle: 'Service currently unavailable',
        header: 'This service is not available right now',
        message: 'Try again in a few moments',
        retryButtonText: 'Back to prescriptions',
      },
    },
    interrupt: {
      nominatedPharmacyFoundLine1: 'The new pharmacy you choose is where your prescriptions will be sent in future.',
      nominatedPharmacyFoundLine2: 'Your new pharmacy can help to check on any outstanding prescriptions for you.',
      continueButton: 'Continue',
    },
    dspInterrupt: {
      paragraph: 'To nominate an online-only pharmacy, you must register with the pharmacy through their website or contact them.',
      visitOnlineListText: 'View a list of online-only pharmacies',
      returnToPrescriptionsText: 'Go back to your prescriptions',
    },
  },
  gp_medical_record: {
    testResults: {
      errors: {
        502: {
          pageTitle: 'Test result details data error',
          pageHeader: 'Test result details data error',
          header: 'There\'s been a problem getting details of your test results',
          subheader: '',
          message: {
            text: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: '',
        },
        504: {
          pageHeader: 'Medical record error',
          header: 'There\'s been a problem getting your medical record information',
          subheader: '',
          message: {
            text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: 'Try again',
        },
      },
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
          message: {
            text: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: '',
        },
      },
    },
    errors: {
      pageHeader: 'Medical record error',
      header: 'There\'s been a problem getting your medical record information',
      subheader: '',
      message: {
        text: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        label: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      },
      retryButtonText: '',
      502: {
        pageHeader: 'Medical record error',
        header: 'There\'s been a problem getting your medical record information',
        subheader: '',
        message: {
          text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: '',
      },
      504: {
        pageHeader: 'Medical record error',
        header: 'There\'s been a problem getting your medical record information',
        subheader: '',
        message: {
          text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: 'Try again',
      },
    },
  },
  my_record: {
    errors: {
      pageHeader: 'Medical record error',
      header: 'There\'s been a problem getting your medical record information',
      subheader: '',
      message: {
        text: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        label: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      },
      retryButtonText: '',
      502: {
        pageHeader: 'Medical record error',
        header: 'There\'s been a problem getting your medical record information',
        subheader: '',
        message: {
          text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: '',
      },
      504: {
        pageHeader: 'Medical record error',
        header: 'There\'s been a problem getting your medical record information',
        subheader: '',
        message: {
          text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
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
    personalRecordText: {
      warningText: {
        wt1: 'Your record may contain sensitive information. If someone is pressuring you for this information, contact your GP surgery immediately.',
        wt2: 'You have a legal right to access the information in your record.',
      },
      body: 'Your record shows personal data, such as your details, allergies and medications.',
      bulletPointHeader: 'Depending on what your GP surgery shares, you may also see:',
      bulletPoints: {
        bp1: 'your medical history, including problems and consultation notes',
        bp2: 'test results that you may not have discussed with your doctor',
      },
      agreeButtonText: 'Continue',
      backButtonText: 'Back to home',
    },
    noRecordAccess: {
      warningHeader: 'You do not currently have online access to your medical record',
      warningBody: 'Contact your GP surgery for more information.',
    },
    noRecordsOrNoAccess: {
      warningHeader: 'Sorry, this information isn\'t available through the NHS App. To access it, contact your GP surgery.',
    },
    records: 'records',
    patientInfo: {
      fieldLabelName: 'Name',
      fieldLabelDOB: 'Date of birth',
      fieldLabelAge: 'Age',
      fieldLabelSex: 'Sex',
      fieldLabelAddress: 'Address',
      fieldLabelNHS: 'NHS number',
      sectionHeader: 'Your details',
    },
    viewRestOfHealthRecordWarning: 'This is a summary of your medical record. To view more detailed information here, such as test results and immunisations, contact your GP surgery to request access.',
    allergiesAndAdverseReactions: {
      sectionHeader: 'Allergies and adverse reactions',
    },
    acuteMedications: {
      sectionHeader: 'Acute (short-term) medications',
    },
    consultations: {
      sectionHeader: 'Consultations and events',
    },
    events: {
      sectionHeader: 'Consultations and events',
    },
    currentRepeatMedications: {
      sectionHeader: 'Repeat medications: current',
    },
    discontinuedRepeatMedications: {
      sectionHeader: 'Repeat medications: discontinued',
    },
    documents: {
      detail: {
        errors: {
          502: {
            pageTitle: 'Server error',
            pageHeader: 'Server error',
            header: 'We\'re experiencing technical difficulties',
            subheader: '',
            message: {
              text: 'Try again later. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
              label: 'Try again later. If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
            },
          },
        },
      },
      sectionHeader: 'Documents',
      documentsLink: 'View your documents',
      documentMenuItemTitle: 'added on {date}',
      documentPageSubtext: 'Document added on',
      docTypePageSubtext: 'added on',
      documentUnavailableSubtext: 'To access it, contact your GP surgery directly.',
      documentUnavailableHeader: 'The document added on {date} is not available through the NHS App',
      documentTypeUnavailableHeader: 'The {type} added on {date} is not available through the NHS App',
      documentUnavailablePageTitle: 'The document added on {date} is not available',
      documentTypeUnavailablePageTitle: 'The {type} added on {date} is not available',
      actions: {
        view: 'View',
        download: 'Download',
      },
      downloadWarning: 'When you download this document, you become responsible for keeping it confidential.',
      commentsHeader: 'Comments',
    },
    immunisations: {
      sectionHeader: 'Immunisations',
      nextDate: 'Next Date: ',
      status: 'Status: ',
    },
    medicalHistory: {
      sectionHeader: 'Medical history',
    },
    medicines: {
      sectionHeader: 'Medicines',
      acuteMedicines: {
        sectionHeader: 'Acute (short-term) medicines',
      },
      currentMedicines: {
        sectionHeader: 'Repeat medicines: current',
      },
      discontinuedMedicines: {
        sectionHeader: 'Repeat medicines: discontinued',
      },
    },
    recalls: {
      sectionHeader: 'Recalls',
      result: 'Result: ',
      nextDate: 'Next Date: ',
      status: 'Status: ',
    },
    encounters: {
      sectionHeader: 'Encounters',
      value: 'Value: ',
      unit: 'Units: ',
    },
    referrals: {
      sectionHeader: 'Referrals',
      description: 'Reason: ',
      speciality: 'Speciality: ',
      ubrn: 'UBRN: ',
    },
    testResults: {
      sectionHeader: {
        tpp: 'Test results (past 6 months)',
        default: 'Test results',
      },
      errors: {
        502: {
          pageTitle: 'Test result details data error',
          pageHeader: 'Test result details data error',
          header: 'There\'s been a problem getting details of your test results',
          subheader: '',
          message: {
            text: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: '',
        },
      },
      visionDetailsLink: 'View your Test Results',
    },
    healthConditions: {
      sectionHeader: 'Health conditions',
    },
    consultationsAndEvents: {
      sectionHeader: 'Consultations and events',
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
          message: {
            text: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: '',
        },
      },
    },
    testresultdetails: {
      backButton: 'Back',
      testResultTitle: 'Test results',
      noTestResultData: 'There is no detail to display here.',
      errors: {
        502: {
          pageTitle: 'Test result details data error',
          pageHeader: 'Test result details data error',
          header: 'There\'s been a problem getting details of your test results',
          subheader: '',
          message: {
            text: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          retryButtonText: '',
        },
      },
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
      text: 'You may see medical abbreviations that you are not familiar with.',
      link: 'Help with abbreviations',
    },
    noStartDate: 'Unknown Date',
  },
  im01: {
    viewFullMessage: 'View full message',
    sendMessageButtonText: 'Send a message',
    subheader: 'Your messages',
    noMessages: 'You have no messages.',
    lastMessageRecieved: 'Last message received at {date}',
    summary: {
      hiddenWithSubject: 'Conversation with {recipient}. Subject: {subject}. The last message in this conversation was sent on {date}.',
      hiddenWithoutSubject: 'Conversation with {recipient}. The last message in this conversation was sent on {date}. View full message.',
    },
  },
  im02: {
    isUrgentChoiceLabel: 'Yes, I need advice now',
    isNotUrgentChoiceLabel: 'No, my message is not urgent',
    noOptionSelectedErrorHeader: 'There\'s a problem',
    noOptionSelectedErrorText: 'You need to select yes or no',
    continueButtonText: 'Continue',
    noRecipients: 'You cannot currently send messages',
    noRecipientsMessage: 'Contact your GP surgery for more information. For urgent medical advice, go to ',
    or: 'or',
    nhs111Link: '111.nhs.uk',
    call111Link: 'call 111',
    ariaLabel: 'Contact your GP surgery for more information. For urgent medical advice, go to 111.nhs.uk or call one one one.',
  },
  im03: {
    info: {
      paragraph1: 'Messaging is for non-urgent advice.',
      paragraph2: {
        part1: 'For advice now, contact your GP surgery directly, go to ',
        part2: '111.nhs.uk',
        part3: ' or ',
        part4: 'call 111',
        ariaLabel: 'For advice now, contact your GP surgery directly, go to 111.nhs.uk or call one one one.',
      },
    },
    careCard: {
      heading: 'Call 999 now if you have:',
      symptoms: [{
        title: 'signs of a heart attack',
        description: 'pain like a very tight band, heavy weight or squeezing in the centre of your chest',
      }, {
        title: 'signs of a stroke',
        description: 'face drooping on one side, cannot hold both arms up, difficulty speaking',
      }, {
        title: 'severe difficulty breathing',
        description: 'gasping, not being able to get words out, choking or lips turning blue',
      }, {
        title: 'heavy bleeding',
        description: 'that will not stop',
      }, {
        title: 'severe injuries',
        description: 'or deep cuts after a serious accident',
      }, {
        title: 'seizure (fit)',
        description: 'someone is shaking or jerking because of a fit, or is unconscious (cannot be woken up)',
      }],
    },
  },
  im04: {
    info: 'This is who your GP surgery lets you message. Your message may be read by any member of staff.',
  },
  gp_messages: {
    view_details: {
      you: 'You',
      unreadMessages: 'Unread messages',
      unreadMessage: 'Unread message',
      deleteMenuItemText: 'Delete conversation',
      replyInformation: {
        header: 'You cannot reply to this message',
        subText: 'To discuss this message further, send a new message or phone your GP surgery.',
      },
      sendMessageMenuItemText: 'Send a new message',
      view: 'View',
      download: 'Download',
      attachment: 'Attachment',
      backButtonText: {
        text: 'Back',
      },
    },
    createMessage: {
      messageLabelText: 'Message',
      messageTextError: 'Enter a message',
      messageHintText: 'Text must be shorter than 450 characters (about 75 words).',
      subjectLabelText: 'Subject',
      subjectTextError: 'Enter a subject',
      subjectHintText: 'Text must be shorter than 64 characters (about 10 words).',
      subHeader: 'For advice now, contact your GP surgery directly, go to ',
      nhs111Link: '111.nhs.uk',
      or: 'or',
      call111Link: 'call 111',
      sendButtonText: 'Send message',
    },
    downloadAttachment: {
      buttonText: 'Download file',
      downloadWarning: 'When you download this file, you become responsible for keeping it confidential.',
    },
    delete: {
      firstParagraph: 'Deleting your conversation will remove it from your list of messages.',
      secondParagraph: 'Your conversation will still be saved in your GP health record.',
      deleteButtonText: 'Delete conversation',
      backButtonText: {
        text: 'Cancel',
      },
    },
    deleteSuccess: {
      back: 'Go to your messages',
    },
  },
  messages: {
    gp_messages: {
      errors: {
        400: {
          pageTitle: 'Messages error',
          pageHeader: 'Messages error',
          header: 'There is a problem getting your messages',
          message: 'Try again now.',
          retryButtonText: 'Try again',
        },
        403: {
          pageTitle: 'Messaging unavailable',
          pageHeader: 'Messaging unavailable',
          header: 'You are not currently able to use messaging.',
          subheader: '',
          message: {
            text: 'Contact your GP surgery for more information. For urgent medical advice, go to 111.nhs.uk or call 111.',
            label: 'Contact your GP surgery for more information. For urgent medical advice, go to 111.nhs.uk or call one one one.',
          },
        },
      },
      view_details: {
        errors: {
          400: {
            pageTitle: 'Message error',
            pageHeader: 'Message error',
            header: 'There is a problem getting your message',
            message: 'Try again now. If the problem continues and you need this information now, contact the person directly.',
            retryButtonText: 'Try again',
          },
        },
      },
      delete: {
        errors: {
          400: {
            pageTitle: 'Error deleting conversation',
            pageHeader: 'Error deleting conversation',
            header: 'Sorry, we could not delete your conversation',
            message: 'Try again now.',
            retryButtonText: 'Try again',
          },
        },
      },
    },
    app_messaging: {
      errors: {
        500: {
          pageTitle: 'Messages error',
          pageHeader: 'Messages error',
          header: 'There is a problem getting your messages',
          message: 'Try again now.',
          retryButtonText: 'Try again',
        },
        502: {
          pageTitle: 'Messages error',
          pageHeader: 'Messages error',
          header: 'There is a problem getting your messages',
          message: 'Try again now.',
          retryButtonText: 'Try again',
        },
      },
      app_message: {
        errorText: 'If the problem continues and you need this information now, contact {senderName} directly.',
        errors: {
          500: {
            pageTitle: 'Messages error',
            pageHeader: 'Messages error',
            header: 'There is a problem getting your messages',
            message: 'Try again now.',
            retryButtonText: 'Try again',
          },
          502: {
            pageTitle: 'Messages error',
            pageHeader: 'Messages error',
            header: 'There is a problem getting your messages',
            message: 'Try again now.',
            retryButtonText: 'Try again',
          },
        },
      },
    },
  },
  app_messaging: {
    index: {
      subHeader: 'Your messages',
      hidden: {
        intro: 'Messages from: {sender}. The last message was sent on {date}. ',
        unread: 'You have {count} unread message{plural}. ',
      },
      noMessages: 'You have no messages',
    },
    messages: {
      titlePrefix: 'Messages from:',
      unreadMessages: 'Unread messages',
    },
  },
  user_research: {
    contactYou: 'We would like to contact you about taking part in user research to improve the NHS App and connected services.',
    whatIsInvolved: {
      header: 'What\'s involved?',
      addYou: 'We\'ll add you to our user research panel and email you a short survey to fill in about you and your health. Your answers will help make sure you get invited to user research that\'s relevant to you.',
      signUp: {
        label: 'Once you\'re signed up, you might be asked to:',
        benefits: [
          'try out new features',
          'answer more questions by email',
          'talk to our researchers about your experience of using the app',
        ],
        isOptional: 'You can always say no to an invite and you can leave the user research panel at any time.',
      },
      restriction: {
        prefix: 'Your information will only be used to contact you about the NHS App user research panel. It will not be shared with anyone else and you can unsubscribe at any time. ',
        linkText: 'Read our privacy policy',
        suffix: ' to find out how we use and protect your data.',
      },
    },
    question: {
      label: 'Can we contact you to take part in NHS App user research?',
      yes: 'Yes, you can contact me about taking part in user research',
      no: 'No, do not contact me',
    },
    continue: 'Continue',
  },
  common: {
    or: 'OR',
  },
  homeHeader: {
    welcome: 'Welcome!',
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
  loginButton: {
    login: 'Continue with NHS login',
  },
  loginLink: {
    login: 'Login',
  },
  preRegistrationInformation: {
    buttonText: 'Continue',
  },
  signOutButton: {
    signOut: 'Log out',
  },
  externalServiceWarning: {
    warningText: 'Your GP surgery provides this service',
  },
  pageHeaders: {
    home: 'Home',
    acuteMedicines: 'Acute (short-term) medicines',
    currentMedicines: 'Repeat medicines: current',
    diagnosisV2: 'Diagnosis',
    discontinuedMedicines: 'Repeat medicines: discontinued',
    examinationsV2: 'Examinations',
    medicines: 'Medicines',
    prescriptions: 'Prescriptions',
    viewPrescriptionsOrder: 'Your orders',
    proceduresV2: 'Procedures',
    repeatPrescriptionCourses: 'Select medication',
    confirmPrescription: 'Confirm prescription',
    prescriptionProxyOrderSuccess: '{name}\'s prescription has been ordered',
    prescriptionOrderSuccess: 'Your prescription has been ordered',
    consultationsAndEvents: 'Consultations and events',
    account: 'My account',
    encounters: 'Encounters',
    settings: 'Settings',
    cookies: 'Manage cookies',
    appointments: 'Appointments',
    gpAppointments: 'Your GP appointments',
    hospitalAppointments: 'Hospital and other appointments',
    allergiesAndReactions: 'Allergies and adverse reactions',
    testResults: 'Test results',
    testResult: 'Test result',
    healthConditions: 'Health conditions',
    immunisations: 'Immunisations',
    appointmentAdminHelp: 'Additional GP services',
    appointmentGpAdvice: 'Ask your GP for advice',
    appointmentGuidance: 'Things to try before you book a GP appointment',
    appointmentBooking: 'Book a GP appointment',
    appointmentCancelling: 'Cancel GP appointment',
    appointmentProxyCancellingSuccess: '{name}\'s GP appointment has been cancelled',
    appointmentCancellingSuccess: 'Your GP appointment has been cancelled',
    appointmentConfirmation: 'Confirm your GP appointment',
    appointmentProxyBookingSuccess: '{name}\'s GP appointment has been booked',
    appointmentBookingSuccess: 'Your GP appointment has been booked',
    medicalHistory: 'Medical history',
    myRecord: 'Your GP medical record',
    healthRecords: 'Health records',
    myRecordDocuments: 'Documents',
    notifications: 'Manage notifications',
    organDonation: 'Your organ donation decision',
    symptoms: 'Symptoms',
    /* Data sharing header should be updated in Android, iOS, and Web if changed */
    dataSharingOverview: 'Overview',
    dataSharingWhereUsed: 'How confidential patient information is used',
    dataSharingDoesNotApply: 'When your choice does not apply',
    dataSharingMakeYourChoice: 'Make your choice',
    more: 'More',
    linkedProfiles: 'Linked profiles',
    switchProfile: 'You are acting on behalf of {fullName}',
    linkedProfilesSummary: 'Switch to {fullName}\'s profile to act on their behalf',
    login: 'Login',
    termsAndConditions: 'Accept conditions of use',
    nominatedPharmacy: 'Your nominated pharmacy',
    dispensingPractice: 'Your dispensing practice',
    recalls: 'Recalls',
    referrals: 'Referrals',
    confirmNominatedPharmacy: 'Check your nominated pharmacy details',
    searchNominatedPharmacy: 'Find a high street pharmacy',
    nominatedPharmacyChangeSuccess: 'You have nominated a pharmacy',
    nominatedPharmacyNotFound: 'You have not nominated a pharmacy',
    nominatedPharmacyOnlineOnlyChoices: 'Is there a specific online-only pharmacy that you want to use?',
    nominatedPharmacyOnlineOnlySearch: 'What is the name of the online-only pharmacy?',
    nominatedPharmacyNotFoundInterrupt: 'The pharmacy you choose is where your prescriptions will be sent',
    nominatedPharmacyFoundInterrupt: 'Any outstanding prescriptions may still arrive at your current nominated pharmacy',
    nominatedPharmacyDspInterrupt: 'Register with the online-only pharmacy directly',
    nominatedPharmacyOnlineOnlySearchNoResults: 'No results found for "{searchQuery}"',
    nominatedPharmacyFound: 'Check the pharmacy this will be sent to',
    dispensingPracticeFound: 'Check the dispensing practice this will be sent to',
    cannotChangePharmacy: 'You cannot change your nominated pharmacy with the NHS App',
    serviceUnavailable: 'Service unavailable',
    repeatPrescriptionsPartialSuccess: 'Part of your prescription has not been ordered',
    messages: 'Messages',
    healthAndInformationUpdates: 'Health information and updates',
    messageDetails: 'Messages',
    gpMessages: 'GP surgery messages',
    gpMessagesUrgency: 'Do you need urgent advice?',
    gpMessagesUrgencyContactYourGp: 'Call your GP or use NHS 111',
    gpMessagesRecipients: 'Select who to message',
    gpMessagesDownloadAttachment: 'Download file',
    gpMessagesAttachmentUnavailable: 'This file is not available in the NHS App',
    nominatedPharmacyChooseType: 'Choose a type of pharmacy',
    preRegistrationInformation: 'Before you start',
    userResearch: 'Help improve the NHS App',
  },
  pageTitles: {
    home: 'Home',
    appointments: 'Appointments',
    gpAppointments: 'Your GP appointments',
    hospitalAppointments: 'Hospital and other appointments',
    appointmentAdminHelp: 'Additional GP services',
    appointmentGpAdvice: 'Ask your GP for advice',
    appointmentGuidance: 'Things to try before you book a GP appointment',
    appointmentBooking: 'Book a GP appointment',
    appointmentCancelling: 'Cancel GP appointment',
    appointmentCancellingSuccess: 'Your GP appointment has been cancelled',
    appointmentProxyCancellingSuccess: '{name}\'s GP appointment has been cancelled',
    appointmentConfirmation: 'Confirm your GP appointment',
    appointmentProxyBookingSuccess: '{name}\'s GP appointment has been booked',
    appointmentBookingSuccess: 'Your GP appointment has been booked',
    gpMessages: 'GP surgery messages',
    gpMessagesViewMessage: 'Conversation with {name}',
    gpMessagesViewAttachment: 'View file - Messages',
    gpMessagesDownloadAttachment: 'Download file - Messages',
    gpMessagesAttachmentUnavailable: 'This file is not available',
    gpMessagesCreateMessage: 'Send your message to {name}',
    gpMessagesDeleteMessage: 'Delete your conversation with {name}',
    gpMessagesDeleteMessageSuccess: 'Your conversation with {name} is deleted',
    prescriptions: 'Prescriptions',
    viewPrescriptionsOrder: 'Your orders',
    repeatPrescriptionCourses: 'Select medication - Repeat prescriptions',
    confirmPrescription: 'Confirm prescription - Repeat prescriptions',
    prescriptionProxyOrderSuccess: '{fullName}\'s prescription has been ordered',
    prescriptionOrderSuccess: 'Your prescription has been ordered',
    account: 'My account',
    healthRecords: 'Health records',
    myRecord: 'Sensitive information - Your GP medical record',
    allergiesAndReactions: 'Allergies and adverse reactions - Your GP medical record',
    medicines: 'Medicines - Your GP medical record',
    acuteMedicines: 'Acute (short-term) medicines - Your GP medical record',
    currentMedicines: 'Repeat medicines: current - Your GP medical record',
    discontinuedMedicines: 'Repeat medicines: discontinued - Your GP medical record',
    immunisations: 'Immunisations - Your GP medical record',
    healthConditions: 'Heath conditions - Your GP medical record',
    testResults: 'Test results - Your GP medical record',
    testResult: 'Test result - Your GP medical record',
    consultationsAndEvents: 'Consultations and events - Your GP medical record',
    settings: 'Settings',
    cookies: 'Manage cookies',
    myRecordDocuments: 'Documents - Your GP medical record',
    recalls: 'Recalls - Your GP medical record',
    encounters: 'Encounters - Your GP medical record',
    referrals: 'Referrals - Your GP medical record',
    diagnosisV2: 'Diagnosis - Your GP medical record',
    examinationsV2: 'Examinations - Your GP medical record',
    proceduresV2: 'Procedures - Your GP medical record',
    medicalHistory: 'Medical history - Your GP medical record',
    notifications: 'Manage notifications',
    organDonation: 'Your organ donation decision',
    organDonationWithdraw: 'Withdraw your organ donation decision',
    symptoms: 'Symptoms',
    dataSharingOverview: 'Overview',
    dataSharingWhereUsed: 'How confidential patient information is used',
    dataSharingDoesNotApply: 'When your choice does not apply',
    dataSharingMakeYourChoice: 'Make your choice',
    more: 'More',
    login: 'Login',
    linkedProfiles: 'Linked profiles',
    switchProfile: 'You are acting on behalf of {fullName}',
    linkedProfilesSummary: 'Switch to {fullName}\'s profile to act on their behalf',
    termsAndConditions: 'Accept conditions of use',
    nominatedPharmacy: 'Your nominated pharmacy',
    dispensingPractice: 'Your dispensing practice',
    searchNominatedPharmacy: 'Find a high street pharmacy',
    nominatedPharmacyNotFound: 'You have not nominated a pharmacy',
    nominatedPharmacyNotFoundInterrupt: 'The pharmacy you choose is where your prescriptions will be sent',
    nominatedPharmacyFoundInterrupt: 'Any outstanding prescriptions may still arrive at your current nominated pharmacy',
    nominatedPharmacyDspInterrupt: 'Register with the online-only pharmacy directly',
    nominatedPharmacyOnlineOnlyChoices: 'Is there a specific online-only pharmacy that you want to use?',
    nominatedPharmacyOnlineOnlySearch: 'What is the name of the online-only pharmacy?',
    nominatedPharmacyOnlineOnlySearchNoResults: 'No results found for "{searchQuery}"',
    confirmNominatedPharmacy: 'Check your nominated pharmacy details',
    nominatedPharmacyChangeSuccess: 'You have nominated a pharmacy',
    nominatedPharmacyFound: 'Check the pharmacy this will be sent to',
    dispensingPracticeFound: 'Check the dispensing practice this will be sent to',
    cannotChangePharmacy: 'You cannot change your nominated pharmacy with the NHS App',
    serviceUnavailable: 'Service unavailable',
    repeatPrescriptionsPartialSuccess: 'Part of your prescription has not been ordered',
    messages: 'Messages',
    healthAndInformationUpdates: 'Health information and updates',
    gpMessagesUrgency: 'Do you need urgent advice?',
    gpMessagesUrgencyContactYourGp: 'Call your GP or use NHS 111',
    gpMessagesRecipients: 'Select who to message',
    nominatedPharmacyChooseType: 'Choose a type of pharmacy',
    preRegistrationInformation: 'Before you start',
    userResearch: 'Help improve the NHS App',
  },
  crumbName: {
    backTo: 'Back to {crumbName}',
    account: 'Settings',
    accountSignOut: 'Sign Out',
    cookies: 'Manage cookies',
    home: 'Home',
    allergiesAndReactions: 'Allergies and adverse reactions',
    encounters: 'Encounters',
    consultations: 'Consultations',
    events: 'Events',
    testResults: 'Test results',
    healthConditions: 'Health conditions',
    immunisations: 'Immunisations',
    appointments: 'Appointments',
    gpAppointments: 'Your GP appointments',
    appointmentsGuidanceBooking: 'Booking',
    appointmentsCancelling: 'Cancel',
    appointmentsBooking: 'Book an appointment',
    appointmentsConfirmation: 'Confirm',
    checkYourSymptoms: 'Symptoms',
    medicines: 'Medicines',
    medicalHistory: 'Medical history',
    myRecord: 'Your GP medical record',
    myRecordNoAccess: 'Your GP medical record',
    more: 'More',
    myRecordTestResult: 'your medical results',
    myRecordDiagnosisDetail: 'your medical results',
    myRecordExaminationsDetail: 'your medical results',
    myRecordProceduresDetail: 'your medical results',
    myRecordTestResultsDetail: 'your medical results',
    myRecordDocuments: 'Documents',
    switchProfile: 'Home',
    organ_donation: 'Organ Donation',
    gpMessages: 'GP surgery messages',
    gpMessagesViewDetails: 'Your conversation',
    prescriptions: 'Prescriptions',
    recalls: 'Recalls',
    referrals: 'Referrals',
    symptoms: 'Symptoms',
    termsAndConditions: 'Terms',
    legacyMyRecordWarning: 'My Record',
    nominatedPharmacy: 'Your nominated pharmacy',
    linkedProfiles: 'Linked profiles',
    healthAndInformationUpdates: 'Health information and updates',
    messages: 'Messages',
  },
  myAccount: {
    detailsHeading: 'Details',
    accountSettings: {
      header: 'Account settings',
      linkedProfilesOptions: 'Linked profiles',
      passwordOptions: 'Login and password options',
      notificationOptions: 'Notifications',
    },
    aboutUsHeading: 'About the NHS App',
    termsAndConditions: 'Terms of use',
    privacyPolicy: 'Privacy policy',
    cookiesLink: 'Cookies',
    cookiesPolicy: 'Cookies policy',
    linkedProfilesLink: 'Linked profiles',
    openSourceLicences: 'Open source licences',
    helpAndSupport: 'Help and support',
    accessibilityStatement: 'Accessibility statement',
    cookies: {
      p1: 'We\'ve put some small files called cookies on your device to make the NHS App work.',
      p2: 'We will not use any other cookies unless you choose to turn them on.',
      toggleLabel: 'Allow optional analytic cookies',
      toggleHint: 'I accept the use of optional analytic cookies used to improve the performance of the NHS App',
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
      viewMedicalRecord: 'View their GP medical record',
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
        subHeader: 'You do not have access to {name}\'s medical record',
        summary: 'Contact {name}\'s GP surgery to request access.',
        switch: 'Switch to your profile to view your GP medical record.',
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
  account: {
    notifications: {
      paragraphs: [
        'You can choose whether to allow notifications on your device.',
        'If you share this device with other people, they may be able to see your notifications.',
      ],
      toggleLabel: 'Allow notifications',
      settingsLinkText: 'Manage how notifications appear on your device',
      errors: {
        pageTitle: 'Notifications error',
        pageHeader: 'Notifications error',
        header: 'Sorry, there is a problem with the service',
        subheader: '',
        message: 'Go back to settings and try again.',
        retryButtonText: 'Back to settings',
        500: {
          10001: {
            header: 'Notifications are turned off on your device',
            message: 'To turn on notifications, go to your device settings and allow notifications. Then return to the app and try again.',
            retryButtonText: 'Try again',
          },
          10002: {
            header: 'Sorry, we could not change your notifications choice',
            message: 'This might be because notifications are turned off in your device settings.',
            additionalInfo: 'Go to your device settings and check notifications are turned on, then try again.',
            retryButtonText: 'Try again',
          },
        },
      },
    },
  },
  sc04: {
    organDonation: {
      subheader: 'Manage your organ donation decision',
      body: 'Help save thousands of lives in the UK every year by signing up to become a donor on the NHS Organ Donor Register.',
    },
    dataSharing: {
      subheader: 'Find out why your data matters',
      body: 'Find out how the NHS uses your confidential patient information and choose whether or not it can be used for research and planning.',
    },
    requestGpHelp: {
      subheader: 'Additional GP services',
      body: 'Get sick notes and GP letters or ask about recent tests.',
    },
    messages: {
      subheader: 'Messages',
      body: 'Send or view messages from your GP surgery and other health services',
    },
  },
  webHeader: {
    title: '{pageTitle} - NHS App',
    nhsLogoAriaLabel: 'Access your NHS services homepage',
    links: {
      account: 'Settings',
      logout: 'Log out',
    },
    toggleMenu: {
      ariaLabel: 'Open menu',
      buttonText: 'Menu',
    },
  },
  skipLink: {
    linkText: 'Skip to main content',
  },
  webFooter: {
    hiddenHeaderText: 'Support links',
    copyrightText: 'Crown copyright',
  },
  navigationMenu: {
    menuLabel: 'Menu',
    appointmentsLabel: 'Appointments',
    moreLabel: 'More',
    myRecordLabel: 'Your record',
    prescriptionsLabel: 'Prescriptions',
    symptomsLabel: 'Symptoms',
    accountLabel: 'Settings',
    logoutLabel: 'Log out',
    close: 'Close the menu',
  },
  navigationMenuList: {
    symptoms: 'Check your symptoms',
    appointments: 'Book and manage appointments',
    prescriptions: 'Order a repeat prescription',
    myRecord: 'View your GP medical record',
    organDonation: 'Manage your organ donation decision',
    linkedProfiles: 'Linked profiles',
    messages: 'View your messages',
  },
  gpPrescriptionsHub: {
    menuOptions: {
      orderRepeat: 'Order a repeat prescription',
      viewOrders: 'View your orders',
      nominatePharmacy: 'Nominate a pharmacy',
      yourNominatedPharmacy: 'Your nominated pharmacy',
      viewOrdersHelpText: 'See repeat prescriptions you have ordered',
      nominatePharmacyHelpText: 'Choose a pharmacy for your prescriptions to be sent to',
    },
  },
  organ_donation: {
    errors: {
      contact: {
        email: 'Email',
      },
      pageTitle: 'Something went wrong',
      pageHeader: 'Something went wrong',
      header: 'Something went wrong',
      500: {
        message: 'You can contact NHS Blood and Transplant to get help with this.',
      },
      502: {
        message: 'You can contact NHS Blood and Transplant to get help with this.',
        1: {
          retryButtonText: 'Try again',
          message: 'If the problem persists you can contact NHS Blood and Transplant to get help with this.',
        },
      },
    },
    review_your_decision: {
      errors: {
        pageTitle: 'Something went wrong',
        pageHeader: 'Something went wrong',
        header: 'Something went wrong',
        500: {
          message: 'You can contact NHS Blood and Transplant to get help with this.',
        },
        502: {
          message: 'You can contact NHS Blood and Transplant to get help with this.',
          1: {
            retryButtonText: 'Try again',
            message: 'If the problem persists you can contact NHS Blood and Transplant to get help with this.',
          },
        },
        504: {
          pageTitle: 'Decision being processed',
          pageHeader: 'Decision being processed',
          header: 'Decision being processed',
          message: 'Check back later to confirm that your decision has been registered.',
        },
      },
    },
  },
  organDonation: {
    additionalDetails: {
      continueButton: 'Continue',
      description: 'This optional information is only used by the NHS to understand the make up of the NHS Organ Donor Register and is not stored against your registration.',
      ethnicity: {
        placeholder: 'Please select',
        label: 'Ethnicity (optional)',
      },
      religion: {
        placeholder: 'Please select',
        label: 'Religion (optional)',
      },
      subheader: 'Additional details',
    },
    faith: {
      subheader: 'Faith / beliefs',
      continueButtonText: 'Continue',
      errorMsgHeader: 'There\'s a problem',
      errorMsgText: 'Respond to the faith/belief declaration. Choose yes, no or prefer not to say.',
      inlineErrorMessage: 'Choose yes, no or prefer not to say.',
      body: {
        paragraph1: 'When you die, NHS staff can ask your family (and anyone else appropriate) about your faith and beliefs. This is how NHS staff will find out about any end of life wishes you might have.',
        paragraph2: 'Record here whether you want our specialist nurses to discuss your faith or beliefs with your family when you die, at the same time they approach them about organ donation.',
      },
      endOfLifeWishes: {
        header: 'Examples of end of life wishes ',
        listItems: [
          'Requesting a faith representative for your family',
          'When to say prayers',
          'Rituals or traditions regards washing and dressing',
          'Being buried within a certain time period'],
      },
      choices: {
        header: 'I would like NHS staff to speak to my family and anyone else appropriate about how organ donation can go ahead in line with my faith or beliefs.',
        yes: {
          title: 'Yes - this is applicable to me',
        },
        no: {
          title: 'No - this is not applicable to me',
        },
        preferNotToSay: {
          title: 'Prefer not to say',
        },
      },
    },
    links: {
      amendDecisionText: 'I want to change my decision',
      alreadyRegisteredText: 'Think you have registered already?',
      findOutMoreText: 'Find out more about organ donation',
      reaffirmDecisionText: 'This is still my decision',
    },
    otherThings: {
      subheader: 'Other things you can do',
      bloodDonation: {
        subheader: 'Register to be a blood donor',
        body: 'You could save lives by giving blood. It’s simple. You can find your local centre and book an appointment via the app.',
      },
      withdraw: {
        subheader: 'Withdraw your previous decision',
        body: 'You can withdraw your previous decision from the Organ Donor Register at any time. If you do this, changes to the law around organ donation may affect you.',
      },
    },
    register: {
      inset: {
        text: 'If you have not registered your decision, ',
        linkText: 'changes to the law around organ donation may affect you',
      },
      subheaderRegister: 'Register your decision',
      subheaderAmend: 'Change your decision',
      noButton: {
        header: 'NO',
        subheader: 'I do not want to donate my organs',
      },
      yesButton: {
        header: 'YES',
        subheader: 'I want to donate all or some of my organs',
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
        phoneLabel: 'To check, or change, your appointed representative call the organ donation line:',
      },
      faithAndBeliefs: {
        Yes: 'When I die, I would like NHS staff to speak with my family (and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs.',
        No: 'When I die, I do not want NHS staff to speak with my family (and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs.',
        NotStated: 'I prefer not to say whether I want NHS staff to speak with my family (and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs.',
      },
    },
    yourChoice: {
      subheader: 'Your choice',
      description: 'You can choose to donate some, or all, of your organs and tissue.',
      continueButtonText: 'Continue',
      errorMessageHeader: 'There\'s a problem',
      errorMessageText: 'Choose to donate all or some of your organs.',
      choices: {
        all: {
          title: 'All my organs and tissue',
          description: 'Help up to nine people through organ donation and even more through tissue donation.',
        },
        some: {
          title: 'Some organs and tissue',
          description: 'Choose which of your organs and tissue to donate.',
        },
      },
    },
    reviewYourDecision: {
      header: 'About you',
      errorMsgHeader: 'There\'s a problem',
      submitButton: 'Submit my decision',
      personalDetails: {
        subheader: 'Personal details',
        nameheader: 'Name',
        dateofbirthheader: 'Date of birth',
        genderheader: 'Gender',
        nhsnumberheader: 'NHS number',
        description: 'Contact your GP surgery to amend your personal details.',
      },
      contactDetails: {
        subheader: 'Contact details',
        text: 'We will only contact you about your organ donation registration.',
        addressheader: 'Postal address',
        changeDetailsText: 'Contact your GP surgery to amend your postal address.',
      },
      additionalInformation: {
        subheader: 'Additional information',
        ethnicityheader: 'Ethnicity',
        religionheader: 'Religion',
        noDecision: 'You did not answer',
        text: 'This optional information is only used by the NHS to understand the make up of the NHS Organ Donor Register and is not stored against your registration.',
      },
      faith: {
        subheader: 'Faith / beliefs details',
        description: 'I would like NHS staff to speak to my family and anyone else appropriate about how organ donation can go ahead in line with my faith / beliefs',
        declaration: {
          Yes: 'Yes - this is applicable to me',
          No: 'No - this is not applicable to me',
          NotStated: 'Prefer not to say',
        },
      },
      decisionDetails: {
        subheader: 'Decision details',
        allOrgansText: 'I want to donate all my organs and tissue.',
        someOrgansText: 'I want to donate some organs and tissue.',
        chosenHeader: 'You have chosen to donate:',
        notChosenHeader: 'You have chosen not to donate:',
        notStatedHeader: 'We do not have a decision for:',
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
        optoutDecisionText: 'No I do not want to donate my organs',
        optinDecisionText: 'Yes I want to donate my organs',
        optinSomeDecisionText: 'Yes I want to donate my organs',
        withdrawDecisionText: 'Withdraw my decision from the register',
      },
      confirmation: {
        subheader: 'Confirmation',
        accuracyText: 'I confirm that the information given in this form is true, complete and accurate',
        privacyText1: 'I have read the ',
        privacyText2: ' and give consent for the use of my information in accordance with the terms',
        privacyLinkText: 'privacy statement',
        errors: {
          accuracy: 'Check your information. Confirm if it is accurate.',
          privacy: 'Read the privacy statement. Confirm if you give your consent.',
        },
      },
      withdraw: {
        subheader: 'What this means',
        body: 'We will no longer know your decision about organ donation. Therefore, it will be considered that you have agreed to be an organ donor unless you are in an excluded group.',
      },
    },
    stillYourDecision: {
      subheader: 'Is this still your decision?',
      text: 'Keeping your registration up to date will help your family, should organ donation be possible.',
    },
    viewDecision: {
      conflictedState: {
        messageText: 'Your registration is currently being processed.',
        dialogText: 'Decision found',
        registrationHeader: 'We are still processing your registration',
        registrationText: 'Please check back in 2 working days. You’ll then be able to view and amend your ' +
          'decision via the NHS App.',
      },
      decisionSubmitted: {
        messageText: 'We have successfully received your organ donation decision.',
        dialogText: 'Decision submitted',
        registrationHeader: 'What happens next',
        registrationText: 'We will process your registration and you will then be able to view and ' +
          'amend this via the NHS App. This may take up to 2 working days.',
      },
      successMessageText: 'Your decision has been recorded',
      successMessageDialogText: 'Success',
      nextSteps: {
        subheader: 'Next steps',
        shareDecision: {
          subheader: 'Share that you are a donor',
          body: 'Help promote organ donation on social media by telling people you are a donor.',
        },
        tellFamily: {
          subheader: 'Tell your family and friends',
          body: 'Do your family and friends know what you want? Help them to support your decision by talking about it.',
        },
        optOutText: 'Please inform your family about your decision.',
      },
    },
    someOrgans: {
      subheader: 'Your choice',
      continueButtonText: 'Continue',
      errorMsgHeader: 'There\'s a problem',
      allSelectedValidationText: 'Choose either ‘yes’ or ‘no’ for each organ.',
      yesRequiredValidationText: 'To continue, choose ‘yes’ for at least one organ.',
      inlineErrorMessage: 'Choose either ‘yes’ or ‘no’ for ',
      moreAboutOrgansLinkText: 'Find out more about organs and tissue',
      choices: {
        subheader: 'Please select which organs and tissue you would like to donate:',
        yes: 'Yes',
        no: 'No',
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
    moreAboutOrgans: {
      header: 'About organs and tissue',
      contentItems: [
        {
          subheader: 'Heart',
          body: 'Blood being pumped around your body by your heart carries oxygen and nutrients. Without the heart, your body wouldn’t get oxygen. Your heart can be transplanted whole or in some cases the valves (pulmonary and aortic) can be transplanted.',
        },
        {
          subheader: 'Lungs',
          body: 'Your lungs supply oxygen to your blood and clear carbon dioxide from your body. Without healthy lungs you couldn’t breathe properly.',
        },
        {
          subheader: 'Kidneys',
          body: 'Your kidneys filter wastes from your blood and convert them to urine. When your kidneys stop working you can develop kidney failure. Harmful wastes and fluids build up in your body and your blood pressure may rise. You can live healthily with one kidney.',
        },
        {
          subheader: 'Liver',
          body: 'Your liver produces bile to clean out your body. If your liver isn’t working right, you will begin to feel tired, experience nausea, vomiting, decreased appetite, brown urine, or even jaundice - yellowing in the whites of your eyes. Your liver can be transplanted whole or in some cases the cells (hepatocytes) can be transplanted.',
        },
        {
          subheader: 'Corneas',
          body: 'The cornea lets light into your eyes, without them you wouldn’t be able to see. The gift of sight is precious. Every day 100 people in the UK start to lose their sight. Almost 2 million people in the UK are living with significant sight loss. Your donation can help someone regain their sight.',
        },
        {
          subheader: 'Pancreas',
          body: 'Your pancreas is in your abdomen. It produces insulin to control your blood sugar levels. If your pancreas is not working correctly your blood sugar level rises, which can lead to diabetes. Your pancreas can be transplanted whole or in some cases the cells (islet cells) can be transplanted.',
        },
        {
          subheader: 'Tissue',
          body: 'Tissue is a group of cells that carry out a particular job in your body. Tissue donations such as skin, bone and tendons save hundreds of lives every year. One tissue donor can enhance the lives of more than 50 people.',
        },
        {
          subheader: 'Small bowel',
          body: 'The small bowel (also small intestine) absorbs nutrients and minerals from food we eat. If your small intestine fails, you wouldn’t be able to digest food. You would need to get nutrition from an alternative method, such as through a drip into your vein.',
        },
      ],
    },
    withdrawn: {
      dialogText: 'Decision withdrawn',
      messageTextItems: [
        'You no longer have a decision recorded on the NHS Organ Donor Register.',
        'If you die in circumstances where donation is possible, it will be considered that you have agreed to be an organ donor unless you are in an excluded group.',
      ],
      messageLink: 'More information about these changes to the law around organ donation',
      recordNewDecisionReminder: 'You can record a new decision at any time.',
      whatNext: {
        header: 'What to do next',
        bodyItems: [
          'Let your family know that you have withdrawn your decision from the register. They will not know what you want unless you tell them.',
        ],
      },
    },
    withdrawReason: {
      continueButton: 'Continue',
      errorMessageHeader: 'There\'s a problem',
      errorMessageText: 'Give a reason for withdrawing your decision',
      subheader: 'Withdraw your previous organ donation decision',
      explanations: [
        'Withdrawing from the NHS Organ Donor Register is different from recording a decision not to donate (opting out). If you withdraw, we will not know your decision.',
        'In line with changes to the law around organ donation, you are considered to have agreed to be an organ donor, unless:',
      ],
      exclusions: [
        'you have recorded a decision not to donate',
        'you are in an excluded group',
      ],
      moreAboutLawText: 'Find out more about the ',
      moreAboutLawLinkText: 'law and excluded groups',
      amendBeforeLink: 'If you do not want to be an organ donor, the best way to tell us is to ',
      amendLink: 'update your decision',
      amendAfterLink: ' You can change your decision at any time.',
      familyText: 'Whatever you decide, please make sure your family know your decision.',
      reason: {
        label: 'Reason for withdrawing',
        placeholder: 'Select reason',
      },
    },
  },
  symptomBanner: {
    howAreYouFeeling: 'How are you feeling today?',
    checker: 'Check symptoms',
  },
  login: {
    desc: 'To access your NHS services',
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
    helpIcon: {
      title: 'Help and support',
      desc: 'Access help and support',
    },
    homeIcon: {
      title: 'NHS Online',
      desc: 'Go back to the home screen.',
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
  },
  appointmentHubPage: {
    pageHeader: 'Appointments',
    gpSurgeryAppointments: {
      subheader: 'GP surgery appointments',
      body: 'View and manage appointments at your surgery',
    },
    hospitalAppointments: {
      subheader: 'Hospital and other appointments',
      body: 'View and manage appointments, like your referral appointments',
    },
  },
  healthRecordHubPage: {
    gpMedicalRecord: {
      subheader: 'GP medical record',
      body: 'View your GP medical record',
    },
  },
  messagesHub: {
    noMessages: 'You have no messages.',
    im1Messaging: {
      subheader: 'GP surgery messages',
      body: 'Send or view messages from your GP surgery',
    },
    appMessaging: {
      subHeader: 'Health information and updates',
      body: 'View messages from health services and the NHS App',
    },
  },
  ds01: {
    header: 'Find out why your data matters',
    mainHeader: 'Choose if data from your health records is shared for research and planning',
    titles: {
      p1: 'Overview',
      p2: 'How confidential patient information is used',
      p3: 'When your choice does not apply',
      p4: 'Make your choice',
    },
    subtitle: 'Manage your data choice',
    startNowButton: 'Start now',
    nextButton: 'Next',
    previousButton: 'Previous',
    pages: {
      p1: {
        intro: {
          paragraph1: 'Your health records contain a type of data called confidential patient information. This data can be used to help with research and planning. You can choose to stop your confidential patient information being used for research and planning.',
          paragraph2: 'Your choice will only apply to the health and care system in England. This does not apply to health or care services accessed in Scotland, Wales or Northern Ireland.',
        },
        confidential: {
          title: 'What is confidential patient information',
          paragraph1: 'Confidential patient information is when 2 types of information from your health records are joined together.',
          paragraph2: 'The 2 types of information are:',
          listItems: [
            'something that can identify you',
            'something about your health care or treatment',
          ],
          paragraph3: 'For example, your name joined with what medicine you take.',
          paragraph4: 'Identifiable information on its own is used by health and care services to contact patients and this is not confidential patient information.',
        },
        patientInformation: {
          title: 'How we use your confidential patient information',
          yourIndividualCareSubtitle: 'Your individual care',
          yourIndividualCareParagraph: 'Health and care staff may use your confidential patient information to help with your treatment and care. For example, when you visit your GP, they may look at your records for important information about your health.',
          researchAndPlanningSubtitle: 'Research and planning',
          researchAndPlanningParagraph: 'Confidential patient information might also be used to:',
          researchAndPlanningListItems: [
            'plan and improve health and care services',
            'research and develop cures for serious illnesses',
          ],
        },
        yourChoice: {
          title: 'Your choice',
          paragraph1: 'You can stop your confidential patient information being used for research and planning.',
          paragraph2: 'If you’re happy with your confidential patient information being used for research and planning you do not need to do anything.',
          paragraph3: 'Any choice you make will not impact your individual care.',
        },
        moreOptions: {
          title: 'More options',
          paragraph: {
            nhsWebsiteLink: 'Visit the NHS website',
            part2: ' for more information or to read our privacy notice. You can also find out how to make a choice for someone else. For example, if you’re a parent or guardian of a child under the age of 13.',
          },
        },
      },
      p2: {
        intro: {
          paragraph1: 'The NHS collects confidential patient information from:',
          listItems1: [
            'all NHS organisations, trusts and local authorities',
            'private organisations, such as private hospitals providing NHS funded care',
          ],
          paragraph2: 'Research bodies and organisations can request access to this information. This includes:',
          listItems2: [
            'university researchers',
            'hospital researchers',
            'medical royal colleges',
            'pharmaceutical companies researching new treatments',
          ],
        },
        thoseWhoCant: {
          title: 'Who cannot use confidential patient information',
          paragraph1: 'Access to confidential patient information will not be given for:',
          listItems: [
            'marketing purposes',
            'insurance purposes',
          ],
          paragraph2: '(unless you request this)',
        },
        dataProtection: {
          title: 'How confidential patient information is protected',
          paragraph1: 'Your confidential patient information is looked after in accordance with good practice and the law.',
          paragraph2: 'Every organisation that provides health and care services will take every step to:',
          listItems: [
            'keep data secure',
            'use data that cannot identify you whenever possible',
            'use data to benefit health and care',
            'not use data for marketing or insurance purposes (unless you request this)',
            'make it clear why and how data is being used',
          ],
          paragraph3: 'All NHS organisations must provide information on the type of data they collect and how it\'s used. Data release registers are published by NHS Digital and Public Health England, showing records of the data they have shared with other organisations.',
        },
      },
      p3: {
        intro: {
          paragraph: 'If you choose to stop your confidential patient information being used for research and planning, your data might still be used in some situations.',
        },
        requiredByLaw: {
          title: 'When required by law',
          paragraph: 'If there’s a legal requirement to provide it, such as a court order.',
        },
        givenConsent: {
          title: 'When you have given consent',
          paragraph: 'If you have given your consent, such as for a medical research study.',
        },
        publicInterest: {
          title: 'When there is an overriding public interest',
          paragraph: 'In an emergency or in a situation when the safety of others is most important. For example, to help manage contagious diseases like meningitis and stop them spreading.',
        },
        informationRemoved: {
          title: 'When information that can identify you is removed',
          paragraph: 'Information about your health care or treatment might still be used in research and planning if the information that can identify you is removed first.',
        },
        specificExclusion: {
          title: 'When there is a specific exclusion',
          paragraph: 'Your confidential patient information can still be used in a small number of situations. For example, for official national statistics like a population census.',
        },
      },
      p4: {
        paragraph1: 'Use this service to:',
        listItems1: [
          'choose if your confidential patient information is used for research and planning',
          'change or check your current choice',
        ],
        paragraph2: {
          text: 'If you want to make a choice for someone else, find out how to on the ',
          nhsWebsiteLink: 'NHS website',
        },
        paragraph3: 'Your choice will be applied by:',
        listItems2: [
          'NHS Digital and Public Health England',
          'all other health and care organisations by March 2020',
        ],
        paragraph4: 'Any choice you make will not impact your individual care.',
      },
      ndop: {
        paragraphs: [
          'You\'re choosing if data from your health records is used across the health and care system in England.',
          'You\'re not choosing if the NHS App uses your data.',
        ],
      },
    },
  },
  web: {
    home: {
      title: 'Access your NHS services',
      bulletListDescription: 'Use this service to:',
      bullets: {
        one: 'book and manage appointments at your GP surgery',
        two: 'order repeat prescriptions',
        three: 'check symptoms and get instant advice',
        four: 'view your medical record securely',
      },
      checkSymptoms: {
        title: 'How are you feeling right now?',
      },
      beforeYouStartTitle: 'Before you start',
      beforeYouStartBulletListDescription: 'To use this service you must be:',
      beforeYouStartBullets: {
        one: 'registered with a GP surgery in England',
        two: 'aged 13 and over',
      },
      aged13To15InformationTitle: 'What to do if you\'re aged 13 to 15',
      aged13To15Description: 'You\'ll need to contact your GP surgery first and request access to GP online services.',
      otherServicesTitle: 'Other services you can use without logging in:',
      otherServicesBullets: {
        one: 'Check if you have coronavirus symptoms',
        two: 'Search conditions and treatments',
        three: 'Use NHS 111 online to check if you need urgent help',
      },
      appStorePanelTitle: 'Get the NHS App on your smartphone or tablet',
    },
    sessionExpiry: {
      warningDurationInformation:
        'For security reasons, you\'ll be logged out in 1 minute. ' +
        '| For security reasons, you\'ll be logged out in {time} minutes.',
      warningGetMoreTime: 'Stay logged in',
      warningLogOut: 'Log out',
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
  messageDateTimeFormats: {
    midday: 'midday',
    midnight: 'midnight',
    yesterday: 'Yesterday',
    sentDateAndTimeFormat: '[Sent ]{dateFormat}[ at ]{timeFormat}',
    sentAtTimeTodayFormat: '[Sent today at ]{timeFormat}',
    sentAtTimeYesterdayFormat: '[Sent yesterday at ]{timeFormat}',
  },
  components,
  thirdPartyProviders,
};
