export default {
  appointments: {
    headerTag: 'book GP appointments',
    contentTag: 'book appointments',
  },
  myRecord: {
    headerTag: 'access your medical record',
    contentTag: 'view your medical record',
  },
  prescriptions: {
    headerTag: 'order prescriptions',
    contentTag: 'order prescriptions',
  },
  content: {
    header: 'You cannot {headerTag} through the NHS App',
    paragraphs: [
      {
        prefix: 'To {contentTag} with Babylon GP at Hand, ',
        linkText: 'use the Babylon app',
        linkUrl: 'GPATHAND_APP_DOWNLOAD_URL',
        suffix: '.',
      },
      {
        prefix: 'Call Babylon GP at Hand on 0330 303 8000 if you have any problems.',
      },
    ],
  },
};
