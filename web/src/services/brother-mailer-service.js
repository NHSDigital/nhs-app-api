/* eslint-disable prefer-destructuring */

import axios from 'axios';

export default {
  postEmailToBrotherMailer: (store, query) => {
    const appUrl = store.state.device.isNativeApp ?
      store.app.$env.NATIVE_CID_REDIRECT_URI : store.app.$env.CID_REDIRECT_URI;

    const urlParts = appUrl.split('/');
    const returnUrl = `${urlParts[0]}//${urlParts[2]}/gp-finder/sending-email-result`;

    const formContent =
      'userid=233847&' +
      'SIG340c7799670a244e283c2d568d8b9031f7cdee374315bb20c1e8c109c535c4c7=&' +
      'addressbookid=6113&' +
      `ReturnURL=${returnUrl}&` +
      'ci_consenturl=&' +
      `email=${encodeURI(`${query.email}`)}&` +
      `cd_ODSCODE=${query.odscode}`;

    const method = 'POST';
    const url = store.app.$env.GP_LOOKUP_BROTHER_MAILER_URL;
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
