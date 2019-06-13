import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createEvent, createStore, mount, createRouter } from '../../helpers';
import { DATA_SHARING_PREFERENCES, APPOINTMENT_ADMIN_HELP } from '@/lib/routes';

describe('more', () => {
  let wrapper;
  let $store;
  let $env;
  let $router;

  const mountAs = (enabled) => {
    $router = createRouter();
    $env = { ...$env, ONLINE_CONSULTATIONS_ENABLED: enabled };
    $store = createStore({
      $env,
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });

    return mount(More, { $store, $env, $router });
  };

  it('will include the organ donation link', () => {
    wrapper = mountAs(false);
    const link = wrapper.find(OrganDonationLink);
    expect(link.exists()).toBe(true);
  });

  it('will not include the request Gp help link if online consultations disabled', () => {
    wrapper = mountAs(false);
    expect(wrapper.find('#btn_gp_help').exists()).toBe(false);

    wrapper = mountAs('false');
    expect(wrapper.find('#btn_gp_help').exists()).toBe(false);
  });

  it('will include the request Gp help link if online consultations enabled', () => {
    wrapper = mountAs(true);
    expect(wrapper.find('#btn_gp_help').exists()).toBe(true);

    wrapper = mountAs('true');
    expect(wrapper.find('#btn_gp_help').exists()).toBe(true);
  });

  describe('Methods', () => {
    it('will return false when Online consultations env variable is falsy', () => {
      wrapper = mountAs(false);
      expect(wrapper.vm.isOnlineConsultationsEnabled).toBe(false);
    });

    it('will return true when Online consultations env variable is truthy', () => {
      wrapper = mountAs(true);
      expect(wrapper.vm.isOnlineConsultationsEnabled).toBe(true);
    });

    it('will navigate to data preferences when data preferences menu item clicked', () => {
      wrapper = mountAs(false);
      wrapper.find('#btn_data_sharing').trigger('click');
      const event = createEvent({ currentTarget: { pathname: DATA_SHARING_PREFERENCES.path } });
      wrapper.vm.navigate(event);

      expect($router.push).toHaveBeenCalledWith(DATA_SHARING_PREFERENCES.path);
      expect(event.preventDefault).toHaveBeenCalled();
    });

    it('will navigate to admin help when request gp admin help menu item clicked', () => {
      wrapper = mountAs(true);
      wrapper.find('#btn_gp_help').trigger('click');
      const event = createEvent({ currentTarget: { pathname: APPOINTMENT_ADMIN_HELP.path } });
      wrapper.vm.navigate(event);

      expect($router.push).toHaveBeenCalledWith(APPOINTMENT_ADMIN_HELP.path);
    });
  });
});
