/* eslint-disable prefer-destructuring */
/* eslint-disable dot-notation */
import axios from 'axios';
import { GP_FINDER_SENDING_EMAIL_RESULT } from '@/lib/routes';

export default {
  postEmailToBrotherMailer: (appUrl, email, odscode) => {
    const returnUrl = appUrl;
    returnUrl.pathname = GP_FINDER_SENDING_EMAIL_RESULT.path;

    const formContent =
      `userid=${process.env['GP_LOOKUP_BROTHER_MAILER_USER_ID']}&` +
      `${process.env['GP_LOOKUP_BROTHER_MAILER_SIG']}=&` +
      `addressbookid=${process.env['GP_LOOKUP_BROTHER_MAILER_ADDRESSBOOK_ID']}&` +
      `ReturnURL=${returnUrl}&` +
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
      method,
      headers,
      data: formContent });
  },
};
