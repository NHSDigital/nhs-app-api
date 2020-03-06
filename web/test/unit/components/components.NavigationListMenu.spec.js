import NavigationListMenu from '@/components/NavigationListMenu';
import { mount, createStore, createRouter, createEvent } from '../helpers';
import {
  PATIENT_PRACTICE_MESSAGING,
  MESSAGING,
} from '@/lib/routes';

let wrapper;
let $store;
let $router;

const mountAs = ({
  appMessagingEnabled = false,
  im1MessagingEnabled = false,
  patientPracticeMessagingToggleEnabled = true,
  isNativeApp = false,
  isLinkedEnabledEnabled = false,
} = {}) => {
  $router = createRouter();
  $store = createStore({
    state: {
      device: {
        isNativeApp,
      },
      practiceSettings: {
        im1MessagingEnabled,
      },
    },
    getters: {
      'linkedAccounts/hasLinkedAccounts': isLinkedEnabledEnabled,
      'serviceJourneyRules/messagingEnabled': appMessagingEnabled,
      'serviceJourneyRules/im1MessagingEnabled': patientPracticeMessagingToggleEnabled,
    },
    $env: {
      YOUR_NHS_DATA_MATTERS_URL: 'testYourDataMattersUrl.com',
    },
  });
  return mount(NavigationListMenu, { $store, $router });
};

beforeEach(() => {
  wrapper = mountAs();
  window.open = jest.fn();
});

describe('Navigation Links ', () => {
  describe('linked profiles visiblitiy', () => {
    it('shows Linked profiles link', () => {
      wrapper = mountAs({ isLinkedEnabledEnabled: true });
      expect(wrapper.find('#menu-item-linkedProfiles').exists()).toBe(true);
    });

    it('does not show Linked profiles link', () => {
      wrapper = mountAs({ isLinkedEnabledEnabled: false });
      expect(wrapper.find('#menu-item-linkedProfiles').exists()).toBe(false);
    });
  });
  describe('messaging link', () => {
    const getMessagingLink = wrapperObj => wrapperObj.find('#btn_messaging');

    describe('sjr messaging disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ appMessagingEnabled: false });
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
        wrapper = mountAs({ appMessagingEnabled: true, isNativeApp: true });
        messagingLink = getMessagingLink(wrapper);
        expect(messagingLink.exists()).toBe(true);
      });

      it('will not show the link on desktop if enabled', () => {
        wrapper = mountAs({ appMessagingEnabled: true, isNativeApp: false });
        messagingLink = getMessagingLink(wrapper);
        expect(messagingLink.exists()).toBe(false);
      });

      it('will redirect to MESSAGING when clicked', () => {
        wrapper = mountAs({ appMessagingEnabled: true, isNativeApp: true });
        messagingLink = getMessagingLink(wrapper);
        messagingLink.trigger('click');
        expect($router.push).toBeCalledWith(MESSAGING.path);
      });
    });

    describe('im1 messaging disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ im1MessagingEnabled: false });
      });

      it('will not show link', () => {
        expect(getMessagingLink(wrapper).exists()).toBe(false);
      });
    });

    describe('im1 messaging enabled', () => {
      let messagingLink;

      beforeEach(() => {
        global.digitalData = {};
        wrapper = mountAs({ im1MessagingEnabled: true });
        messagingLink = getMessagingLink(wrapper);
      });

      it('will show link', () => {
        expect(messagingLink.exists()).toBe(true);
      });

      it('will redirect to PATIENT_PRACTICE_MESSAGING when clicked', () => {
        messagingLink.trigger('click');
        expect($router.push).toBeCalledWith(PATIENT_PRACTICE_MESSAGING.path);
      });
    });
  });
  describe('methods', () => {
    it('will navigate to /messaging when appMessagingEnabled' +
      ' and messaging menu item clicked', () => {
      wrapper = mountAs({ appMessagingEnabled: true, isNativeApp: true });
      wrapper.find('#btn_messaging').trigger('click');
      const event = createEvent({ currentTarget: { pathname: MESSAGING.path } });
      wrapper.vm.navigate(event);

      expect($router.push).toHaveBeenCalledWith(MESSAGING.path);
    });

    it('will navigate to /patient-practice-messaging when im1MessagingEnabled ' +
      'and messaging menu item clicked', () => {
      wrapper = mountAs({ im1MessagingEnabled: true });
      wrapper.find('#btn_messaging').trigger('click');
      const event = createEvent({ currentTarget: { pathname: PATIENT_PRACTICE_MESSAGING.path } });
      wrapper.vm.navigate(event);

      expect($router.push).toHaveBeenCalledWith(PATIENT_PRACTICE_MESSAGING.path);
    });

    it('will not display messaging menu item when patient practice toggle is off even if ' +
      'the practice has it enabled', () => {
      wrapper = mountAs({
        im1MessagingEnabled: true,
        patientPracticeMessagingToggleEnabled: false,
      });

      const messagingMenuItem = wrapper.find('#btn_messaging');

      expect(messagingMenuItem.exists()).toBe(false);
    });
  });
});
