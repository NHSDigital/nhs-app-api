export default {
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
};
