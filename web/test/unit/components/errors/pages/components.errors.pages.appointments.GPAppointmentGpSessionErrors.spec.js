import GpAppointmentGpSessionErrors from '@/components/errors/pages/appointments/GpAppointmentGpSessionErrors';
import i18n from '@/plugins/i18n';
import { createStore, shallowMount } from '../../../helpers';

describe('GpAppointmentGpSessionErrors', () => {
  let $store;
  const mountPage = ({
    status = 500,
    hasRetried = false,
  } = {}) => {
    Storage.prototype.getItem = jest.fn('hasRetried').mockImplementation(() => hasRetried);

    $store = createStore({
      getters: {
        'session/isLoggedIn': () => true,
      },
    });

    return shallowMount(GpAppointmentGpSessionErrors, {
      $store,
      propsData: {
        error: {
          status,
        },
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

      expect(wrapper.find('#error-dialog-500').exists()).toBe(true);
      expect(wrapper.find('#alternative_actions').exists()).toBe(false);
    });

    it('will show the alternative actions component if hasRetried is true', async () => {
      wrapper = await mountPage({ hasRetried: true });

      expect(wrapper.find('#alternative_actions').exists()).toBe(true);
      expect(wrapper.find('#error-dialog-500').exists()).toBe(false);
    });

    it('will dispatch to set the breadcrumb to the onDemandAppointmentCrumb', async () => {
      expect($store.dispatch).toBeCalledWith('navigation/setRouteCrumb', 'onDemandAppointmentCrumb');
    });
  });
});
