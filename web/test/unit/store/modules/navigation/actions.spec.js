import '@/static/js/v1/nhsapp';
import consola from 'consola';
import actions from '@/store/modules/navigation/actions';
import { APPOINTMENTS, HEALTH_RECORDS, INDEX, MESSAGES, PRESCRIPTIONS, SYMPTOMS } from '@/lib/routes';

describe('navigation actions', () => {
  let dispatch;

  beforeEach(() => {
    dispatch = jest.fn();
    consola.error = jest.fn();
  });

  describe('goToPage', () => {
    describe('with unknown page', () => {
      beforeEach(() => {
        actions.goToPage({ dispatch }, 'foo');
      });

      it('will not dispatch', () => {
        expect(dispatch).not.toBeCalled();
      });

      it('will log an error', () => {
        expect(consola.error).toBeCalled();
      });
    });

    describe.each([
      [window.nhsapp.navigation.AppPage.HOME_PAGE, INDEX.path],
      [window.nhsapp.navigation.AppPage.APPOINTMENTS, APPOINTMENTS.path],
      [window.nhsapp.navigation.AppPage.PRESCRIPTIONS, PRESCRIPTIONS.path],
      [window.nhsapp.navigation.AppPage.HEALTH_RECORDS, HEALTH_RECORDS.path],
      [window.nhsapp.navigation.AppPage.SYMPTOMS, SYMPTOMS.path],
      [window.nhsapp.navigation.AppPage.MESSAGES, MESSAGES.path],
    ])('with `%s` page', (page, path) => {
      beforeEach(() => {
        actions.goToPage({ dispatch }, page);
      });

      it(`will dispatch "goTo" with ${path}`, () => {
        expect(dispatch).toBeCalledWith('goTo', path);
      });

      it('will not log an error', () => {
        expect(consola.error).not.toBeCalled();
      });
    });
  });
});
