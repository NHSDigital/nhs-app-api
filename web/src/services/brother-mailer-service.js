/* eslint-disable prefer-destructuring */
/* eslint-disable dot-notation */
import axios from 'axios';

export default {
  postEmailToBrotherMailer: (email, odscode) => {
    const formContent =
      `userid=${process.env['GP_LOOKUP_BROTHER_MAILER_USER_ID']}&` +
      `${process.env['GP_LOOKUP_BROTHER_MAILER_SIG']}=&` +
      `addressbookid=${process.env['GP_LOOKUP_BROTHER_MAILER_ADDRESSBOOK_ID']}&` +
      'ci_consenturl=&' +
      `email=${encodeURI(`${email}`)}&` +
      `cd_ODSCODE=${odscode}`;

    const method = 'POST';
    const url = process.env['GP_LOOKUP_BROTHER_MAILER_URL'];
    const headers = {
      'Content-Type': 'application/x-www-form-urlencoded',
    };
    return axios({ url,
      crossDomain: true,
      withCredentials: true,
      validateStatus: status => status === 302,
      method,
      headers,
      maxRedirects: 0,
      data: formContent });
  },
};
