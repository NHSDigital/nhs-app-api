import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createEvent, createStore, mount, createRouter } from '../../helpers';
import { DATA_SHARING_PREFERENCES, APPOINTMENT_ADMIN_HELP, MESSAGING } from '@/lib/routes';

describe('more', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({ cdssAdminEnabled = false, messagingEnabled = false } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
    $store.getters['serviceJourneyRules/cdssAdminEnabled'] = cdssAdminEnabled;
    $store.getters['serviceJourneyRules/messagingEnabled'] = messagingEnabled;
    return mount(More, { $store, $router });
  };

  beforeEach(() => {
    wrapper = mountAs();
  });

  it('will dispatch device/unlockNavBar when page mounted', () => {
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  it('will include the organ donation link', () => {
    expect(wrapper.find(OrganDonationLink).exists()).toBe(true);
  });

  describe('gp help link', () => {
    const gpHelpId = 'btn_gp_help';

    describe('sjr cdss admin disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ cdssAdminEnabled: false });
      });

      it('will not show link', () => {
        expect(wrapper.find(`#${gpHelpId}`).exists()).toBe(false);
      });
    });

    describe('sjr cdss admin enabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ cdssAdminEnabled: true });
      });

      it('will show link', () => {
        expect(wrapper.find(`#${gpHelpId}`).exists()).toBe(true);
      });
    });
  });

  describe('messaging link', () => {
    const getMessagingLink = wrapperObj => wrapperObj.find('#btn_messaging');

    describe('sjr messaging disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ messagingEnabled: false });
      });

      it('will not show link', () => {
        expect(getMessagingLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr messaging enabled', () => {
      let messagingLink;

      beforeEach(() => {
        global.digitalData = {};
        wrapper = mountAs({ messagingEnabled: true });
        messagingLink = getMessagingLink(wrapper);
      });

      it('will show link', () => {
        expect(messagingLink.exists()).toBe(true);
      });

      it('will redirect to MESSAGING when clicked', () => {
        messagingLink.trigger('click');
        expect($router.push).toBeCalledWith(MESSAGING.path);
      });
    });
  });

  describe('methods', () => {
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

    it('will navigate to messaging when messaging menu item clicked', () => {
      wrapper = mountAs({ messagingEnabled: true });
      wrapper.find('#btn_messaging').trigger('click');
      const event = createEvent({ currentTarget: { pathname: MESSAGING.path } });
      wrapper.vm.navigate(event);

      expect($router.push).toHaveBeenCalledWith(MESSAGING.path);
    });
  });
});
