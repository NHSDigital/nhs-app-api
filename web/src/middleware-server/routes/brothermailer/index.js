/* eslint-disable prefer-destructuring */
import get from 'lodash/fp/get';
import { Router } from 'express';
import { GP_FINDER_SENDING_EMAIL, GP_FINDER_SENDING_EMAIL_RESULT, BROTHERMAILER_SIGNUP_NOJS } from '@/lib/routes';
import BrotherMailerService from '@/services/brother-mailer-service';
import { URL } from 'url';

export default () => {
  const router = Router();

  const submissionError = `${GP_FINDER_SENDING_EMAIL.path}?error=submissionError`;
  const invalidEmailError = `${GP_FINDER_SENDING_EMAIL.path}?error=invalidEmailError`;
  const connectionError = `${GP_FINDER_SENDING_EMAIL.path}?error=connectionError`;

  router.post(BROTHERMAILER_SIGNUP_NOJS.noJsApiPath, async (req, res) => {
    const { odsCode, email, appUrl } = get('body')(req) || {};
    let parsedAppUrl;

    if (!email || email.indexOf('@') === -1) {
      return res.redirect(invalidEmailError);
    }

    try {
      parsedAppUrl = new URL(appUrl);
    } catch (error) {
      return res.redirect(submissionError);
    }
    if (!odsCode || !parsedAppUrl) {
      return res.redirect(submissionError);
    }

    return BrotherMailerService.postEmailToBrotherMailer(parsedAppUrl, email, odsCode)
      .then((resp) => {
        const queryString = resp.request.path.split('?')[1];
        const query = { reason: undefined, result: undefined };

        if (queryString) {
          const queryParams = queryString.split('&');
          queryParams.forEach((item) => {
            const param = item.split('=');
            query[param[0]] = param[1];
          });
        }

        if (query.result === 'success') {
          return res.redirect(GP_FINDER_SENDING_EMAIL_RESULT.path);
        } else if (query.reason === 'invalidemail') {
          return res.redirect(invalidEmailError);
        }
        return res.redirect(submissionError);
      })
      .catch(() => res.redirect(connectionError));
  });

  return router;
};
