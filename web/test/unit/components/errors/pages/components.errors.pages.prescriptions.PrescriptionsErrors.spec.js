/* eslint-disable import/no-extraneous-dependencies */
import PrescriptionsErrors from '@/components/errors/pages/prescriptions/PrescriptionsErrors';
import i18n from '@/plugins/i18n';
import { PRESCRIPTION_REPEAT_COURSES_PATH } from '@/router/paths';
import { createStore, shallowMount } from '../../../helpers';

describe('Prescriptions errors', () => {
  let $store;

  const mountWrapper = ({
    repeatError,
    prescriptionsError,
    hasRetried = false,
    timestamp = 123,
    specialRequestIds = [],
    isValid = false,
    specialRequestValid = false,
    errorStatus,
    serviceDeskReference,
  } = {}) => {
    Storage.prototype.getItem = jest.fn('hasRetried').mockImplementation(() => hasRetried);
    Storage.prototype.setItem = jest.fn();

    $store = createStore({
      dispatch: jest.fn(() => Promise.resolve()),
      app: {
        router: {
          push: jest.fn(),
        },
      },
      state: {
        repeatPrescriptionCourses: {
          error: repeatError,
        },
        prescriptions: {
          error: prescriptionsError,
        },
        session: {},
      },
      getters: {
        'repeatPrescriptionCourses/selectedIds': specialRequestIds,
        'repeatPrescriptionCourses/isValid': isValid,
        'repeatPrescriptionCourses/specialRequestValid': specialRequestValid,
        'session/isLoggedIn': () => true,
      },
    });

    return shallowMount(PrescriptionsErrors, {
      $store,
      propsData: {
        error: {
          status: errorStatus,
        },
        referenceCode: serviceDeskReference,
        tryAgainRoute: PRESCRIPTION_REPEAT_COURSES_PATH,
      },
      mountOpts: { i18n, reload: jest.fn() },
      $route: {
        query: {
          ts: timestamp,
        },
      },
    });
  };

  describe('error', () => {
    let page;

    it('will show the try again error if the error is a GP session error and has not retried', async () => {
      const error = { status: 599 };

      page = mountWrapper({ repeatError: error });

      expect(page.find('#shutter-dialog-599').exists()).toBe(true);
      expect(page.vm.hasRetried).toBe(false);
    });

    it('will return the correct status code on the 599 error', async () => {
      const error = { status: 599, serviceDeskReference: 'xxxxxx' };

      page = await mountWrapper({ repeatError: error, serviceDeskReference: 'xxxxxx' });
      expect(page.vm.referenceCode).toBe('xxxxxx');
    });

    it('will set hasRetried to true on try again', async () => {
      page = await mountWrapper();
      page.vm.tryAgain();
      expect(sessionStorage.setItem).toBeCalledWith('hasRetried', true);
    });

    it('will show the permanent error when hasRetried is true and the status is 599', async () => {
      const error = { status: 599, serviceDeskReference: 'xxxxxx' };
      page = await mountWrapper({ repeatError: error, hasRetried: true, serviceDeskReference: 'xxxxxx' });
      page.setData({ $route: { query: { ts: '12345678' } } });

      expect(page.find('#presciptionsGpSessionError').exists()).toBe(true);
    });

    it('will dispatch to set the breadcrumb to the onDemandPrescriptionCrumb', async () => {
      expect($store.dispatch).toBeCalledWith('navigation/setRouteCrumb', 'onDemandPrescriptionCrumb');
    });
  });
});
