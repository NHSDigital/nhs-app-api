import OrganDonationErrors from '@/components/errors/pages/organ-donation/OrganDonationErrors';
import i18n from '@/plugins/i18n';
import { ORGAN_DONATION_PATH } from '@/router/paths';
import { createStore, shallowMount } from '../../../helpers';

describe('OrganDonationErrors', () => {
  let $store;
  const mountPage = ({
    status = 500,
    hasRetried = false,
    serviceDeskReference,
  } = {}) => {
    Storage.prototype.getItem = jest.fn('hasRetried').mockImplementation(() => hasRetried);

    $store = createStore({
      getters: {
        'session/isLoggedIn': () => true,
      },
    });

    return shallowMount(OrganDonationErrors, {
      $store,
      propsData: {
        error: {
          status,
        },
        tryAgainRoute: ORGAN_DONATION_PATH,
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

      expect(wrapper.find('#shutter-dialog-599').exists()).toBe(true);
    });
  });
});
