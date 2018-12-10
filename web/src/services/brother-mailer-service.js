/* eslint-disable prefer-destructuring */
/* eslint-disable dot-notation */
import axios from 'axios';

export default {
  postEmailToBrotherMailer: (store, query) => {
    const appUrl = store.state.device.isNativeApp ?
      store.app.$env.NATIVE_CID_REDIRECT_URI : store.app.$env.CID_REDIRECT_URI;

    const urlParts = appUrl.split('/');
    const returnUrl = `${urlParts[0]}//${urlParts[2]}/gp-finder/sending-email-result`;

    const formContent =
      `userid=${process.env['GP_LOOKUP_BROTHER_MAILER_USER_ID']}&` +
      `${process.env['GP_LOOKUP_BROTHER_MAILER_SIG']}=&` +
      `addressbookid=${process.env['GP_LOOKUP_BROTHER_MAILER_ADDRESSBOOK_ID']}&` +
      `ReturnURL=${returnUrl}&` +
      'ci_consenturl=&' +
      `email=${encodeURI(`${query.email}`)}&` +
      `cd_ODSCODE=${query.odscode}`;

    const method = 'POST';
    const url = process.env['GP_LOOKUP_BROTHER_MAILER_URL'];
    const headers = {
      'Content-Type': 'application/x-www-form-urlencoded',
    };
    return axios({ url,
      crossDomain: true,
      withCredentials: true,
      method,
      headers,
      data: formContent });
  },
};
