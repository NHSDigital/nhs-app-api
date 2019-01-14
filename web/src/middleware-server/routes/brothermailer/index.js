/* eslint-disable prefer-destructuring */
import get from 'lodash/fp/get';
import {
  Router,
} from 'express';
import {
  GP_FINDER_SENDING_EMAIL,
  GP_FINDER_WAITING_LIST_JOINED,
  BROTHERMAILER_SIGNUP_NOJS,
} from '@/lib/routes';
import BrotherMailerService from '@/services/brother-mailer-service';

export default () => {
  const router = Router();

  const errorPath = `${GP_FINDER_SENDING_EMAIL.path}?error=`;
  const successPath = `${GP_FINDER_WAITING_LIST_JOINED.path}?choice=`;

  const choiceError = `${errorPath}choiceError`;
  const submissionError = `${errorPath}submissionError`;
  const invalidEmailError = `${errorPath}invalidEmailError`;
  const notEnteredEmailError = `${errorPath}notEnteredEmailError`;
  const connectionError = `${errorPath}connectionError`;
  const waitingListJoined = `${successPath}yes`;
  const waitingListNotJoined = `${successPath}no`;

  router.post(BROTHERMAILER_SIGNUP_NOJS.noJsApiPath, async (req, res) => {
    const {
      odsCode,
      email,
      choice,
    } = get('body')(req) || {};

    if (choice === undefined) {
      return res.redirect(choiceError);
    }

    if (choice !== 'yes') {
      return res.redirect(waitingListNotJoined);
    }

    if (!email) {
      return res.redirect(notEnteredEmailError);
    }

    if (email.indexOf('@') === -1) {
      return res.redirect(invalidEmailError);
    }

    if (!odsCode) {
      return res.redirect(submissionError);
    }

    return BrotherMailerService.postEmailToBrotherMailer(email, odsCode)
      .then((response) => {
        if (response.data.includes('?result=success')) {
          return res.redirect(waitingListJoined);
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
