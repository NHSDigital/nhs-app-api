import each from 'jest-each';
import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createEvent, createStore, mount, createRouter } from '../../helpers';
import {
  PATIENT_PRACTICE_MESSAGING,
  DATA_SHARING_PREFERENCES,
  APPOINTMENT_ADMIN_HELP,
  MESSAGING,
} from '@/lib/routes';

describe('more', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    cdssAdminEnabled = false,
    sjrMessagingEnabled = false,
    practiceIm1MessagingEnabled = false,
    sjrIm1MessagingEnabled = false,
    isNativeApp = false,
    context = { serviceProvider: 'pkb',
      serviceType: 'messages' },
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp },
        practiceSettings: { im1MessagingEnabled: practiceIm1MessagingEnabled },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': () => (context),
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/messagingEnabled': sjrMessagingEnabled,
        'serviceJourneyRules/im1MessagingEnabled': sjrIm1MessagingEnabled,
      },
      $env: { YOUR_NHS_DATA_MATTERS_URL: 'testYourDataMattersUrl.com' },
    });
    return mount(More, { $store, $router });
  };

  beforeEach(() => {
    wrapper = mountAs();
    window.open = jest.fn();
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
        wrapper = mountAs({ sjrMessagingEnabled: false });
      });

      it('will not show link', () => {
        expect(getMessagingLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr messaging enabled', () => {
      let messagingLink;

      beforeEach(() => {
        global.digitalData = {};
      });

      it('will show link', () => {
        wrapper = mountAs({ sjrMessagingEnabled: true, isNativeApp: true });
        messagingLink = getMessagingLink(wrapper);
        expect(messagingLink.exists()).toBe(true);
      });

      it('will not show the link on desktop if enabled', () => {
        wrapper = mountAs({ sjrMessagingEnabled: true, isNativeApp: false });
        messagingLink = getMessagingLink(wrapper);
        expect(messagingLink.exists()).toBe(false);
      });

      it('will redirect to MESSAGING when clicked', () => {
        wrapper = mountAs({ sjrMessagingEnabled: true, isNativeApp: true });
        messagingLink = getMessagingLink(wrapper);
        messagingLink.trigger('click');
        expect($router.push).toBeCalledWith(MESSAGING.path);
      });
    });

    describe('im1 messaging', () => {
      describe('practice disabled or sjr disabled', () => {
        each([
          { practice: true, sjr: false },
          { practice: false, sjr: true },
          { practice: false, sjr: false },
        ]).it('will not show link', ({ practice, sjr }) => {
          wrapper = mountAs({
            practiceIm1MessagingEnabled: practice,
            sjrIm1MessagingEnabled: sjr,
          });
          expect(getMessagingLink(wrapper).exists()).toBe(false);
        });
      });

      describe('practice enabled and sjr enabled', () => {
        let messagingLink;

        beforeEach(() => {
          global.digitalData = {};
          wrapper = mountAs({ practiceIm1MessagingEnabled: true, sjrIm1MessagingEnabled: true });
          messagingLink = getMessagingLink(wrapper);
        });

        it('will show link', () => {
          expect(messagingLink.exists()).toBe(true);
        });

        it('will redirect to /patient-practice-messaging when clicked', () => {
          messagingLink.trigger('click');
          expect($router.push).toBeCalledWith(PATIENT_PRACTICE_MESSAGING.path);
        });
      });

      describe('app messaging enabled and im1 messaging enabled', () => {
        it('will redirect to /patient-practice-messaging when clicked', () => {
          wrapper = mountAs({
            sjrMessagingEnabled: true,
            practiceIm1MessagingEnabled: true,
            sjrIm1MessagingEnabled: true,
          });

          getMessagingLink(wrapper).trigger('click');
          expect($router.push).toBeCalledWith(PATIENT_PRACTICE_MESSAGING.path);
        });
      });
    });
  });

  describe('methods', () => {
    it('will navigate to data preferences when data preferences menu item clicked if native', () => {
      wrapper = mountAs({ isNativeApp: true });
      const event = createEvent({ currentTarget: { pathname: DATA_SHARING_PREFERENCES.path } });
      wrapper.vm.navigateToDataSharing(event);

      expect($router.push).toHaveBeenCalledWith(DATA_SHARING_PREFERENCES.path);
      expect(event.preventDefault).toHaveBeenCalled();
    });

    it('will navigate to ndop home page when data preferences menu item clicked if not native', () => {
      wrapper = mountAs();
      const event = createEvent({ currentTarget: { pathname: 'testYourDataMattersUrl.com' } });
      wrapper.vm.navigateToDataSharing(event);

      expect($router.push).not.toHaveBeenCalledWith('testYourDataMattersUrl.com');
      expect(event.preventDefault).not.toHaveBeenCalled();
      expect(window.open).toHaveBeenCalledWith('testYourDataMattersUrl.com', '_blank');
    });

    it('will navigate to admin help when request gp admin help menu item clicked', () => {
      wrapper = mountAs({ cdssAdminEnabled: true });
      wrapper.vm.navigate(
        createEvent({ currentTarget: { pathname: APPOINTMENT_ADMIN_HELP.path } }),
      );

      expect($router.push).toHaveBeenCalledWith(APPOINTMENT_ADMIN_HELP.path);
    });

    it('will navigate to /messaging when messaging enabled in SJR ' +
       'and messaging menu item clicked', () => {
      wrapper = mountAs({ sjrMessagingEnabled: true, isNativeApp: true });
      wrapper.vm.navigate(
        createEvent({ currentTarget: { pathname: MESSAGING.path } }),
      );

      expect($router.push).toHaveBeenCalledWith(MESSAGING.path);
    });

    it('will navigate to /patient-practice-messaging when im1Messaging is on in SJR ' +
       'and messaging menu item clicked', () => {
      wrapper = mountAs({
        practiceIm1MessagingEnabled: true,
        sjrIm1MessagingEnabled: true,
      });
      wrapper.vm.navigate(
        createEvent({ currentTarget: { pathname: PATIENT_PRACTICE_MESSAGING.path } }),
      );

      expect($router.push).toHaveBeenCalledWith(PATIENT_PRACTICE_MESSAGING.path);
    });
  });
});
