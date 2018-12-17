/* eslint-disable prefer-destructuring */
import get from 'lodash/fp/get';
import {
  Router,
} from 'express';
import {
  GP_FINDER_SENDING_EMAIL,
  GP_FINDER_SENDING_EMAIL_RESULT,
  BROTHERMAILER_SIGNUP_NOJS,
} from '@/lib/routes';
import BrotherMailerService from '@/services/brother-mailer-service';

export default () => {
  const router = Router();

  const submissionError = `${GP_FINDER_SENDING_EMAIL.path}?error=submissionError`;
  const invalidEmailError = `${GP_FINDER_SENDING_EMAIL.path}?error=invalidEmailError`;
  const connectionError = `${GP_FINDER_SENDING_EMAIL.path}?error=connectionError`;

  router.post(BROTHERMAILER_SIGNUP_NOJS.noJsApiPath, async (req, res) => {
    const {
      odsCode,
      email,
    } = get('body')(req) || {};
    if (!email || email.indexOf('@') === -1) {
      return res.redirect(invalidEmailError);
    }

    if (!odsCode) {
      return res.redirect(submissionError);
    }

    return BrotherMailerService.postEmailToBrotherMailer(email, odsCode)
      .then((response) => {
        if (response.data.includes('?result=success')) {
          return res.redirect(GP_FINDER_SENDING_EMAIL_RESULT.path);
        } else if (response.data.includes('?result=invalidemail')) {
          return res.redirect(invalidEmailError);
        }
        return res.redirect(submissionError);
      })
      .catch(() => {
        res.redirect(connectionError);
      });
  });

  return router;
};
