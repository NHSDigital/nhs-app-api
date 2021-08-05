import HealthRecordErrors from '@/components/errors/pages/health-record/HealthRecordErrors';
import i18n from '@/plugins/i18n';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { createStore, shallowMount } from '../../../helpers';

describe('HealthRecordErrors', () => {
  let $store;
  const mountPage = ({
    status = 500,
    hasRetried = false,
    serviceDeskReference,
    sjrEnabled = false,
  } = {}) => {
    Storage.prototype.getItem = jest.fn('hasRetried').mockImplementation(() => hasRetried);

    $store = createStore({
      getters: {
        'session/isLoggedIn': () => true,
        'serviceJourneyRules/silverIntegrationEnabled': () => sjrEnabled,
      },
    });

    return shallowMount(HealthRecordErrors, {
      $store,
      propsData: {
        error: {
          status,
        },
        tryAgainRoute: GP_MEDICAL_RECORD_PATH,
        referenceCode: serviceDeskReference,
      },
      mountOpts: { i18n, reload: jest.fn() },
    });
  };

  describe('errors', () => {
    let wrapper;

    it('will set the flag in the sessionStorage when tryAgain is called', async () => {
      Storage.prototype.setItem = jest.fn();

      wrapper = await mountPage();
      wrapper.vm.tryAgain();

      expect(sessionStorage.setItem).toBeCalledWith('hasRetried', true);
    });

    it('will show the try again component if hasRetried is false', async () => {
      wrapper = await mountPage();

      expect(wrapper.find('#error-dialog-599').exists()).toBe(true);
    });

    it('will return the correct status code on the 599 error', async () => {
      const error = { status: 599, serviceDeskReference: 'xxxxxx' };

      wrapper = await mountPage({ repeatError: error, serviceDeskReference: 'xxxxxx' });
      expect(wrapper.vm.referenceCode).toBe('xxxxxx');
    });

    it('will show other options', async () => {
      wrapper = await mountPage({ hasRetried: true, sjrEnabled: true });
      expect(wrapper.find('#otherThingsToDo').exists()).toBe(true);
    });

    it('will not show other options', async () => {
      wrapper = await mountPage({ hasRetried: true });
      expect(wrapper.find('#otherThingsToDo').exists()).toBe(false);
    });
  });
});
