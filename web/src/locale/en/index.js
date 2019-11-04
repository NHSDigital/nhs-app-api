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
    insetContent: {
      heading: 'Information: ',
    },
  },
  auth_return: {
    errors: {
      pageTitle: 'Login failed',
      pageHeader: 'Login failed',
      header: 'Login failed',
      subheader: 'There\'s been a problem loading this page.',
      message: 'Go back to the home screen and log in again.',
      additionalInfo: {
        text: 'If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call 111.',
        label: 'If the problem continues and you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      },
      retryButtonText: 'Back to home',
      default: {
        header: 'Login failed',
        title: 'Login failed - NHS App',
        line1: {
          text: 'We cannot log you in to the NHS App.',
          label: 'We cannot log you in to the NHS App.',
        },
        line3: {
          text: 'Go back to the home screen and try logging in again.',
          label: 'Go back to the home screen and try logging in again.',
        },
        line4: {
          text: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
          label: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        },
        line5: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
        contactUsButtonText: {
          text: 'Contact us',
          label: 'Contact us',
        },
        backButtonText: {
          text: 'Back to home',
          label: 'back to home',
        },
      },
      400: {
        header: 'Login failed',
        title: 'Login failed - NHS App',
        line1: {
          text: 'Go back to the home screen and try logging in again.',
          label: 'Go back to the home screen and try logging in again.',
        },
        line2: {
          text: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
          label: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        },
        contactUs: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
        backButtonText: {
          text: 'Back to home',
          label: 'back to home',
        },
        contactUsButtonText: {
          text: 'Contact us',
          label: 'Contact us',
        },
      },
      403: {
        header: 'Login failed',
        title: 'Login failed - NHS App',
        line1: {
          text: 'We cannot get your details from your GP surgery.',
          label: 'We cannot get your details from your GP surgery.',
        },
        line2: {
          text: 'Go back to the home screen and try logging in again.',
          label: 'Go back to the home screen and try logging in again.',
        },
        line3: {
          text: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
          label: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        },
        line4: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
        contactUsButtonText: {
          text: 'Contact us',
          label: 'Contact us',
        },
        backButtonText: {
          text: 'Back to home',
          label: 'back to home',
        },
      },
      464: {
        header: 'Login failed',
        title: 'Login failed - NHS App',
        line2: {
          text: 'This can be one of two problems:',
          label: 'This can be one of two problems:',
        },
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
        contactUs: {
          text: 'Contact us and quote the error code {errorCode} to help us resolve the problem more quickly.',
          label: 'Contact us and quote the error code {errorCode} to help us resolve the problem more quickly.',
        },
        message: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one',
        },
        contactUsButtonText: {
          text: 'contact us',
          label: 'contact us',
        },
      },
      465: {
        pageHeader: 'You are too young to use the NHS App',
        header: 'You are too young to use the NHS App',
        message: {
          text: 'Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
      },
      500: {
        header: 'Login failed',
        title: 'Login failed - NHS App',
        line1: {
          text: 'We cannot log you in to the NHS App.',
          label: 'We cannot log you in to the NHS App.',
        },
        line3: {
          text: 'Go back to the home screen and try logging in again.',
          label: 'Go back to the home screen and try logging in again.',
        },
        line4: {
          text: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
          label: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        },
        line5: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call one one one.',
        },
        contactUsButtonText: {
          text: 'Contact us',
          label: 'Contact us',
        },
        backButtonText: {
          text: 'Back to home',
          label: 'back to home',
        },
      },
      502: {
        title: 'Login failed - NHS App',
        header: 'Login failed',
        listTitle: {
          text: 'This can be one of two problems:',
          label: 'This can be one of two problems:',
        },
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
        line3: {
          text: 'Go back to the home screen and try logging in again.',
          label: 'Go back to the home screen and try logging in again.',
        },
        line4: {
          text: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
          label: 'If you keep seeing this message, contact us. Quote the error code {errorCode} to help us resolve the problem more quickly.',
        },
        message: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
        },
        backButtonText: {
          text: 'Back to home',
          label: 'back to home',
        },
        contactUsButtonText: {
          text: 'Contact us',
          label: 'Contact us',
        },
      },
      504: {
        title: 'Login failed - NHS App',
        header: 'Login failed',
        listTitle: {
          text: 'This can be one of two problems:',
          label: 'This can be one of two problems:',
        },
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

        line3: {
          text: 'Go back to the home screen and try logging in again.',
          label: 'Go back to the home screen and try logging in again.',
        },
        line4: {
          text: 'If you keep seeing this message, contact us and quote the error code {errorCode}.',
          label: 'If you keep seeing this message, contact us and quote the error code {errorCode}.',
        },
        message: {
          text: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
          label: 'If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.',
        },
        backButtonText: {
          text: 'Back to home',
          label: 'back to home',
        },
        contactUsButtonText: {
          text: 'Contact us',
          label: 'Contact us',
        },
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
    nominatedPharmacy: 'My nominated pharmacy',
    dispensingPractice: 'My dispensing practice',
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
    nominatedPharmacyHeader: 'My nominated pharmacy',
    dispensingPracticeHeader: 'My dispensing practice',
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
  nominatedPharmacySearchResults: {
    header: 'Choose my nominated pharmacy',
    title: 'Choose my nominated pharmacy',
    resultSummary: {
      showingPharmaciesNear: 'Pharmacies near "{searchQuery}"',
      distanceInformation: 'Distances given are in a straight line but travel routes may be longer.',
      showingAll: 'Showing all pharmacies.',
    },
    backButton: 'Back',
    distanceAway: '{distance} miles away',
    errors: {
      noResultsFound: {
        header: 'No results found',
        title: 'No results found',
        foundNoResults: 'We couldn\'t find any matches for "{searchQuery}".',
        message: 'Make sure you enter your postcode correctly.',
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
      line1: 'The NHS website uses cookies to improve your on-site experience.',
      linkText: 'Find out more about cookies',
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
  onlineConsultations: {
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
        date: 'Date must include a day, month and year',
        dateEmpty: 'Enter a date',
        dateTime: 'Enter the full date and time',
        dateTimeEmpty: 'Enter a date and time',
        decimal: 'Enter a number',
        image: 'Select the area on the image',
        integer: 'Enter a number',
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
      errors: {
        pageTitle: 'Appointment data error',
        pageHeader: 'Appointment data error',
        header: 'There\'s been a problem getting your appointment history',
        subheader: '',
        message: {
          text: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        504: {
          subheader: '',
          retryButtonText: 'Try again',
          message: {
            text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
        },
        403: {
          pageTitle: 'Appointment booking unavailable',
          pageHeader: 'Appointment booking unavailable',
          header: 'You are not currently able to book appointments online.',
          subheader: '',
          message: {
            text: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
            label: 'Contact your GP surgery for more information. For urgent medical help, call one one one.',
          },
        },
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
    errors: {
      pageHeader: 'Appointment data error',
      header: 'There\'s been a problem getting your appointment history',
      subheader: '',
      message: {
        text: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
        label: 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
      },
      504: {
        subheader: '',
        message: {
          text: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again now. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: 'Try again',
      },
      403: {
        pageHeader: 'Appointment booking unavailable',
        header: 'You are not currently able to book appointments online',
        subheader: '',
        message: {
          text: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
          label: 'Contact your GP surgery for more information. For urgent medical help, call one one one.',
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
      errors: {
        pageTitle: 'Appointment request error',
        pageHeader: 'Error sending request',
        header: 'There\'s been a problem sending your request',
        subheader: '',
        message: {
          text: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
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
          label: 'Practice member',
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
            all: 'Next four weeks',
          },
        },
      },
      errors: {
        pageTitle: 'Appointment data error',
        pageHeader: 'Appointment data error',
        header: 'There\'s been a problem loading this page',
        subheader: '',
        message: {
          text: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        504: {
          message: {
            text: 'Try again now. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
            label: 'Try again now. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
          },
          subheader: '',
          retryButtonText: 'Try again',
        },
        403: {
          pageTitle: 'Appointment booking unavailable',
          pageHeader: 'Appointment booking unavailable',
          header: 'You are not currently able to book appointments online',
          subheader: '',
          message: {
            text: 'Contact your GP surgery for more information. For urgent medical help, call 111.',
            label: 'Contact your GP surgery for more information. For urgent medical help, call one one one.',
          },
        },
      },
    },
    confirmation: {
      telephoneNumberLabel: 'Choose a phone number for this appointment',
      telephoneNumberDescription: 'This number will only be used for this appointment. It will not be saved.',
      headerLabel: 'Give a reason for this appointment',
      headerLabelSuffix: ' (Optional)',
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
      info: 'Check your appointment details before booking',
      errors: {
        pageTitle: 'Appointment request error',
        pageHeader: 'Error sending request',
        header: 'There\'s been a problem sending your request',
        subheader: '',
        message: {
          text: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111.',
          label: 'Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call one one one.',
        },
        retryButtonText: 'Back to my appointments',
        460: {
          pageHeader: 'Appointment limit reached',
          header: 'You can\'t book any more appointments right now',
          subheader: 'Contact your GP surgery if you still need to book one.',
          message: 'You can go back to see what you\'ve already booked and cancel any appointments that you may no longer need.',
          additionalInfo: 'If it\'s urgent and you do not know what to do, call 111 to get help near you.',
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
    informatica: {
      header: 'Appointment booking is not currently available directly through the NHS App',
      description: 'Your GP surgery uses Appointments Online to book appointments, and you’ll need a username and password from your GP surgery.',
      link: {
        prefix: 'If you have a username and password, ',
        text: 'log in to Appointments Online',
      },
    },
    gp_advice: {
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
    partialSuccess: {
      medicationNotOrdered: 'Medication not ordered',
      needMedicationNow: 'If you need to order this medication now, contact your GP surgery directly.  For urgent medical advice, call 111.',
      medicationOrdered: 'Medication ordered successfully',
      orderStatusUpdate: 'The order status will be updated once it\'s been reviewed by your GP.',
      backButton: 'Back to my prescriptions',
    },
  },
  nominated_pharmacy: {
    changePharmacyLink: 'Change my nominated pharmacy',
    openingTimes: 'Opening times',
    continueButton: 'Continue',
    closed: 'Closed',
    nominatedPharmacyInstruction: 'If you order repeat medication using the NHS App, this pharmacy is where they will be sent.',
    dispensingPracticeInstruction: 'If you order repeat medication using the NHS App, this is where they will be sent.',
    confirm: {
      confirmButton: 'Confirm',
      pharmacyChanged: 'You changed your nominated pharmacy.',
      pharmacyChosen: 'You\'ve chosen your nominated pharmacy.',
      line1: 'This is the pharmacy where your repeat prescriptions will be sent in future.',
      errors: {
        pageHeader: 'Error updating nomination',
        pageTitle: 'Error updating nomination',
        header: 'There\'s been a problem updating your pharmacy nomination',
        message: 'Go back and try again. If the problem continues and you need to nominate a pharmacy now, contact your GP surgery directly.',
        retryButtonText: 'Back to prescriptions',
      },
    },
    internetPharmacy: 'This is an internet pharmacy.',
    warning: {
      changeInternetPharmacy: 'If you change your pharmacy from {pharmacyName}, you cannot change it back to {pharmacyName} using the NHS App.',
      changeDispensingPractice: {
        line1: 'You cannot change your dispensing practice with the NHS App.',
        line2: 'To nominate a new dispensing practice or pharmacy, contact your GP surgery directly.',
      },
    },
    search: {
      pageTitle: 'Change my nominated pharmacy',
      line1: 'The pharmacy you choose is where your repeat prescriptions will be sent.',
      subHeader: 'Find a pharmacy',
      line2: 'Enter a postcode',
      searchButton: 'Search',
      link1: 'See all internet pharmacies',
      link2: 'See all dispensing appliance contractors',
      emptySearchError: 'Enter a postcode',
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
      documentPageSubtext: 'Document added on',
      actions: {
        view: 'View',
      },
    },
    immunisations: {
      sectionHeader: 'Immunisations',
      nextDate: 'Next Date: ',
      status: 'Status: ',
    },
    medicalHistory: {
      sectionHeader: 'Medical History',
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
      text: 'You may see medical abbreviations that you aren’t familiar with.',
      link: 'Help with abbreviations',
    },
    noStartDate: 'Unknown Date',
  },
  messaging: {
    index: {
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
  loginButton: {
    login: 'Continue with NHS login',
  },
  loginLink: {
    login: 'Login',
  },
  signOutButton: {
    signOut: 'Log out',
  },
  externalServiceWarning: {
    warningText: 'This service is provided by your GP surgery',
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
    consultationsAndEvents: 'Consultations and events',
    account: 'My account',
    settings: 'Settings',
    appointments: 'My appointments',
    allergiesAndReactions: 'Allergies and adverse reactions',
    testResults: 'Test results',
    testResult: 'Test result',
    immunisations: 'Immunisations',
    appointmentAdminHelp: 'Additional GP services',
    appointmentGpAdvice: 'Ask your GP for advice',
    appointmentGuidance: 'Things to try before you book an appointment',
    appointmentBooking: 'Book an appointment',
    appointmentCancelling: 'Cancel appointment',
    appointmentConfirmation: 'Confirm appointment',
    myRecord: 'My GP medical record',
    myRecordDocuments: 'Documents',
    notifications: 'Notifications',
    organDonation: 'My organ donation decision',
    symptoms: 'Check my symptoms',
    /* Data sharing header should be updated in Android, iOS, and Web if changed */
    dataSharing: 'Find out why your data matters',
    more: 'More',
    linkedProfiles: 'Linked profiles',
    login: 'Login',
    termsAndConditions: 'Accept conditions of use',
    nominatedPharmacy: 'My nominated pharmacy',
    dispensingPractice: 'My dispensing practice',
    confirmNominatedPharmacy: 'Confirm my nominated pharmacy',
    searchNominatedPharmacy: 'Change my nominated pharmacy',
    nominateMyPharmacy: 'Nominate my pharmacy',
    changeNominatedPharmacy: 'Choose my nominated pharmacy',
    nominatedPharmacyNotFound: 'You have not nominated a pharmacy',
    nominatedPharmacyFound: 'Check the pharmacy this will be sent to',
    dispensingPracticeFound: 'Check the dispensing practice this will be sent to',
    cannotChangePharmacy: 'You cannot change your nominated pharmacy with the NHS App',
    serviceUnavailable: 'Service unavailable',
    repeatPrescriptionsPartialSuccess: 'Part of your prescription has not been ordered',
    messaging: 'Messages',
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
    consultationsAndEvents: 'Consultations and events',
    account: 'My account',
    allergiesAndReactions: 'Allergies and adverse reactions',
    testResults: 'Test results',
    testResult: 'Test result',
    immunisations: 'Immunisations',
    appointments: 'My appointments',
    appointmentAdminHelp: 'Additional GP services',
    appointmentGpAdvice: 'Ask your GP for advice',
    appointmentGuidance: 'Things to try before you book an appointment',
    appointmentBooking: 'Book an appointment',
    appointmentCancelling: 'Cancel appointment',
    appointmentConfirmation: 'Confirm appointment',
    myRecord: 'Sensitive information - My record',
    myRecordDocuments: 'Documents - Your GP medical record',
    notifications: 'Notifications',
    organDonation: 'My organ donation decision',
    symptoms: 'Check my symptoms',
    dataSharing: 'Find out why your data matters',
    more: 'More',
    login: 'Login',
    linkedProfiles: 'Linked profiles',
    termsAndConditions: 'Accept conditions of use',
    nominatedPharmacy: 'My nominated pharmacy',
    dispensingPractice: 'My dispensing practice',
    nominateMyPharmacy: 'Nominate my pharmacy',
    searchNominatedPharmacy: 'Change my nominated pharmacy',
    changeNominatedPharmacy: 'Choose my nominated pharmacy',
    nominatedPharmacyNotFound: 'You have not nominated a pharmacy',
    confirmNominatedPharmacy: 'Confirm my nominated pharmacy',
    nominatedPharmacyFound: 'Check the pharmacy this will be sent to',
    dispensingPracticeFound: 'Check the dispensing practice this will be sent to',
    cannotChangePharmacy: 'You cannot change your nominated pharmacy with the NHS App',
    serviceUnavailable: 'Service unavailable',
    repeatPrescriptionsPartialSuccess: 'Part of your prescription has not been ordered',
    messaging: 'Messages',
  },
  crumbName: {
    backTo: 'Back to {crumbName}',
    account: 'My Account',
    accountSignOut: 'Sign Out',
    home: 'Home',
    allergiesAndReactions: 'Allergies and adverse reactions',
    consultations: 'Consultations',
    events: 'Events',
    testResults: 'Test results',
    immunisations: 'Immunisations',
    appointments: 'My appointments',
    appointmentsGuidanceBooking: 'Booking',
    appointmentsCancelling: 'Cancel',
    appointmentsBooking: 'Booking',
    appointmentsConfirmation: 'Confirm',
    checkYourSymptoms: 'Symptoms',
    myRecord: 'My GP medical record',
    myRecordNoAccess: 'My GP medical record',
    more: 'More',
    myRecordTestResult: 'My medical results',
    myRecordDiagnosisDetail: 'My medical results',
    myRecordExaminationsDetail: 'My medical results',
    myRecordProceduresDetail: 'My medical results',
    myRecordTestResultsDetail: 'My medical results',
    myRecordDocuments: 'Documents',
    organ_donation: 'Organ Donation',
    prescriptions: 'My repeat prescriptions',
    prescriptionRepeatCourses: 'My repeat prescriptions',
    prescriptionConfirmCourses: 'My repeat prescriptions',
    symptoms: 'Symptoms',
    termsAndConditions: 'Terms',
    legacyMyRecordWarning: 'My Record',
    nominatedPharmacy: 'My nominated pharmacy',
    linkedProfiles: 'Linked profiles',
    messaging: 'Messages',
  },
  myAccount: {
    detailsHeading: 'Details',
    accountSettings: {
      header: 'Account settings',
      passwordOptions: 'Login and password options',
      notificationOptions: 'Notifications',
    },
    aboutUsHeading: 'About us',
    termsAndConditions: 'Terms of use',
    privacyPolicy: 'Privacy policy',
    cookiesPolicy: 'Cookies policy',
    openSourceLicences: 'Open source licences',
    helpAndSupport: 'Help and support',
    accessibilityStatement: 'Accessibility statement',
  },
  linkedProfiles: {
    actingAsOtherUserBannerWarningText: 'Acting on behalf of',
    linkedInformation: 'You have proxy access for the following people.',
    informationHeaders: {
      dob: 'Date of birth',
      nhsNumber: 'NHS number',
      gpPractice: 'GP practice',
    },
    switchProfileButton: 'Switch to this profile',
    thingsYouCanDoOnBehalfOf: {
      text: 'Things you can do on behalf of',
      bookAnAppointment: 'Book an appointment',
      orderRepeatPrescription: 'Order repeat prescription',
      viewMedicalRecord: 'View medical record',
    },
  },
  notifications: {
    paragraphs: [
      'You can choose whether to allow notifications on your device.',
      'If you share this device with other people, they may be able to see your notifications.',
    ],
    toggleLabel: 'Allow notifications',
    settingsLinkText: 'Manage your device\'s notifications settings',
  },
  sc04: {
    organDonation: {
      subheader: 'Manage organ donation decision',
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
    messaging: {
      subheader: 'Your messages',
      body: 'Get messages from your GP Practice and other NHS services.',
    },
  },
  webHeader: {
    title: '{pageTitle} - NHS App',
    links: {
      account: 'Account',
      logout: 'Log out',
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
    myRecordLabel: 'My GP medical record',
    prescriptionsLabel: 'Repeat prescriptions',
    symptomsLabel: 'Symptoms',
    accountLabel: 'Account',
    logoutLabel: 'Log out',
    close: 'Close the menu',
  },
  navigationMenuList: {
    symptoms: 'Check my symptoms',
    appointments: 'Book and manage appointments',
    prescriptions: 'Order a repeat prescription',
    myRecord: 'View my GP medical record',
    organDonation: 'Manage organ donation decision',
    linkedProfiles: 'Linked profiles',
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
        subheader: 'Withdraw my decision',
        body: 'Remove an existing registration from the Organ Donor Register. There will be no recorded decision for you about organ donation.',
      },
    },
    register: {
      subheaderRegister: 'Register your organ donation decision',
      subheaderAmend: 'Change your organ donation decision',
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
        body: 'Your family will be asked to make a decision for you, when you die.',
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
          body: 'Use our message templates and conversation guides to tell your family and friends you are a donor.',
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
        'You can record a new decision at any time.',
      ],
      whatNext: {
        header: 'What to do next',
        bodyItems: [
          'Let your family know that you have withdrawn your details from the register. If you die in circumstances where donation is possible, we will ask your family if you expressed a verbal decision. If you did not express a verbal decision, we will ask your family to make a decision on your behalf.',
          'Your family won\'t know what you want unless you tell them, so help them now to support your decision at a difficult time.',
        ],
      },
    },
    withdrawReason: {
      continueButton: 'Continue',
      errorMessageHeader: 'There\'s a problem',
      errorMessageText: 'Give a reason for withdrawing your decision',
      subheader: 'Withdraw my decision',
      bodyItems: [
        'Withdrawing your decision means there will be no recorded decision for you, and without this your family will be asked to decide for you, when you die.',
        'If you are certain you do not want to donate your organs or tissue, you need to register a \'no\' decision.',
        'Whatever you decide, please make sure your family know your decision.',
      ],
      reason: {
        label: 'Reason for withdrawing',
        placeholder: 'Select reason',
      },
    },
  },
  shared: {
    organDonation: {
      recordDecision: 'Record my organ donation decision',
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
  loginBanner: {
    alreadyHaveNHSLogin: 'Already have an NHS Login?',
    loginLink: 'Login to NHS App',
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
  th02: {
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
    resultsFound: ' result(s) for : ',
    searchAgain: 'Search again',
    errors: {
      backButton: 'Back',
      serviceUnavailable: {
        header: 'Technical problems',
        title: 'Technical problems',
        errorDialogHeader: 'We are experiencing technical problems',
        errorDialogText1: 'Something has gone wrong with this service. It wasn\'t your fault.',
        errorDialogText2: 'Come back later. If it still isn\'t working then, contact us about the problem.',
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
    organDonationNotParticipating: [
      'Record organ donation decision',
    ],
    createAccountMessage: 'To use the NHS App fully, you\'ll need to create an NHS login.',
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
    untilThenSymptomsParagraph: 'You can check your symptoms in the app. \'Check if you need urgent help\' will direct you to medical help, if you need it.',
    untilThenOrganDonationParagraph: 'You can also record your organ donation decision.',
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
    mainHeader: 'Choose if data from your health records is shared for research and planning',
    titles: {
      p1: 'Overview',
      p2: 'Where confidential patient information is used',
      p3: 'Where your choice does not apply',
      p4: 'Make your choice',
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
      p4: {
        paragraphs: [
          'If you decide to opt out, this will be respected and applied by NHS Digital and Public Health England. These organisations collect, process and release health and adult social care data on a national basis. Your decision will also be respected and applied by all other organisations that are responsible for health and care information by March 2020.',
          'An opt-out will only apply to the health and care system in England. This does not apply to your health data where you have accessed health or care services outside of England, such as in Scotland and Wales.',
          'If you choose to opt out, your data may still be used during some specific situations. For example, during an epidemic where there might be a risk to other people\'s health.',
        ],
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
    sessionExpiry: {
      warningDurationInformation:
                        'For security reasons, you\'ll be logged out in 1 minute. ' +
                        '| For security reasons, you\'ll be logged out in {time} minutes.',
      warningGetMoreTime: 'Stay logged in',
      warningLogOut: 'Log out',
    },
  },
};
