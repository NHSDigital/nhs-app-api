import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createEvent, createStore, mount, createRouter } from '../../helpers';
import { DATA_SHARING_PREFERENCES, APPOINTMENT_ADMIN_HELP } from '@/lib/routes';

describe('more', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({ cdssAdminEnabled = false } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
    $store.getters['serviceJourneyRules/cdssAdminEnabled'] = cdssAdminEnabled;
    return mount(More, { $store, $router });
  };

  it('will dispatch device/unlockNavBar when page mounted', () => {
    wrapper = mountAs();
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  it('will include the organ donation link', () => {
    wrapper = mountAs();
    const link = wrapper.find(OrganDonationLink);
    expect(link.exists()).toBe(true);
  });

  it('will not include the request Gp help link if cdssAdmin disabled', () => {
    wrapper = mountAs();
    expect(wrapper.find('#btn_gp_help').exists()).toBe(false);
  });

  it('will include the request Gp help link if cdssAdmin enabled', () => {
    wrapper = mountAs({ cdssAdminEnabled: true });
    expect(wrapper.find('#btn_gp_help').exists()).toBe(true);
  });

  describe('Methods', () => {
    it('will navigate to data preferences when data preferences menu item clicked', () => {
      wrapper = mountAs();
      wrapper.find('#btn_data_sharing').trigger('click');
      const event = createEvent({ currentTarget: { pathname: DATA_SHARING_PREFERENCES.path } });
      wrapper.vm.navigate(event);

      expect($router.push).toHaveBeenCalledWith(DATA_SHARING_PREFERENCES.path);
      expect(event.preventDefault).toHaveBeenCalled();
    });

    it('will navigate to admin help when request gp admin help menu item clicked', () => {
      wrapper = mountAs({ cdssAdminEnabled: true });
      wrapper.find('#btn_gp_help').trigger('click');
      const event = createEvent({ currentTarget: { pathname: APPOINTMENT_ADMIN_HELP.path } });
      wrapper.vm.navigate(event);

      expect($router.push).toHaveBeenCalledWith(APPOINTMENT_ADMIN_HELP.path);
    });
  });
});
