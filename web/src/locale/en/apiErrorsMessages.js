export default {
  gp_messages: {
    400: {
      pageTitle: 'Messages error',
      pageHeader: 'Messages error',
      header: 'There is a problem getting your messages',
      message: 'Try again now.',
      retryButtonText: 'Try again',
    },
    403: {
      pageTitle: 'Cannot access GP messaging',
      pageHeader: 'Cannot access GP messaging',
      header: 'This feature has been turned off by your GP Surgery.',
      subheader: 'Contact your GP for more information or to access GP services.',
      message: {
        text: 'Contact your GP surgery for more information. For urgent medical advice, go to {111link} or call 111.',
        label: 'Contact your GP surgery for more information. For urgent medical advice, go to 111.nhs.uk or call one one one.',
      },
    },
    view_details: {
      400: {
        pageTitle: 'Message error',
        pageHeader: 'Message error',
        header: 'There is a problem getting your message',
        message: 'Try again now. If the problem continues and you need this information now, contact the person directly.',
        retryButtonText: 'Try again',
      },
      403: {
        pageTitle: 'Message error',
        pageHeader: 'Message error',
        header: 'There is a problem getting your message',
        message: 'Try again now. If the problem continues and you need this information now, contact the person directly.',
        retryButtonText: 'Try again',
      },
    },
  },
  delete: {
    400: {
      pageTitle: 'Error deleting conversation',
      pageHeader: 'Error deleting conversation',
      header: 'Sorry, we could not delete your conversation',
      message: 'Try again now.',
      retryButtonText: 'Try again',
    },
  },
};
