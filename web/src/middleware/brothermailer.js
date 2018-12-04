/* eslint-disable no-param-reassign */
/* eslint-disable import/extensions */
/* eslint-disable prefer-destructuring */

import { BROTHERMAILER, GP_FINDER_SENDING_EMAIL_RESULT,
  GP_FINDER_SENDING_EMAIL } from '@/lib/routes';
import BrotherMailerService from '@/services/brother-mailer-service';

export default function ({ store, redirect, route }) {
  if (process.server) {
    if (route.name === BROTHERMAILER.name) {
      if (!route.query.email || route.query.email.indexOf('@') === -1) {
        return redirect(`${GP_FINDER_SENDING_EMAIL.path}?error=invalidEmailError`);
      }

      return BrotherMailerService.postEmailToBrotherMailer(store, route.query)
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
            return redirect(GP_FINDER_SENDING_EMAIL_RESULT);
          } else if (query.reason === 'invalidemail') {
            return redirect(`${GP_FINDER_SENDING_EMAIL.path}?error=invalidEmailError`);
          }
          return redirect(`${GP_FINDER_SENDING_EMAIL.path}?error=submissionError`);
        })
        .catch(() => {
          redirect(`${GP_FINDER_SENDING_EMAIL.path}?error=connectionError`);
        });
    }
  }
  return undefined;
}
