import each from 'jest-each';
import i18n from '@/plugins/i18n';
import Index from '@/pages/index';
import { createStore, mount, createRouter } from '../helpers';

describe('index', () => {
  let wrapper;
  let $store;
  let $router;

  const createPublicHealthNotification = id => ({
    id,
    type: 'callout',
    urgency: 'warning',
    title: `Health notification ${id}`,
    body: `Health notification body ${id}`,
  });

  const mountAs = ({
    isProxying = false,
    homeScreen = {},
    isNativeApp = false,
    isProofLevel9 = true,
    hasUnreadAppMessages = false,
    hasUnreadGpMessages = false,
    messagesUnavailable = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        practiceSettings: {
          im1MessagingEnabled: true,
        },
        gpMessages: {
          hasUnread: hasUnreadGpMessages,
          gpMessagingSessionUnavailable: messagesUnavailable,
        },
        messaging: { hasUnread: hasUnreadAppMessages },
        knownServices: {
          knownServices: [{
            id: 'pkb',
            url: 'www.url.com',
          }],
        },
        linkedAccounts: {
          actingAsUser: {
            id: 'user-id-0',
            name: 'mr user 0',
            ageMonths: '10',
            ageYears: '25',
            gpPracticeName: 'practice x',
          },
          config: {
            patientId: '1234-abc-dddd',
          },
        },
        serviceJourneyRules: { rules: { homeScreen } },
        device: {
          isNativeApp,
        },
      },
      getters: {
        'session/isProofLevel9': isProofLevel9,
        'session/isProxying': isProxying,
        'session/currentProfile': { name: '' },
      },
    });
    return mount(Index, { $store, $router, mountOpts: { i18n }, stubs: ['BiometricBanner'] });
  };

  it('will display the navigation items when not proxying', () => {
    wrapper = mountAs({ isProxying: false });
    const navMenu = wrapper.find('[data-sid="navigation-list-menu"]');
    expect(navMenu.exists()).toBe(true);
  });

  it('will display the banner when user is p5', () => {
    wrapper = mountAs({ isProofLevel9: false });
    const blueBanner = wrapper.find('#upliftBlueBanner');
    expect(blueBanner.exists()).toBe(true);
  });

  it('will not display the banner when user is p9', () => {
    wrapper = mountAs({ isProofLevel9: true });
    const blueBanner = wrapper.find('#upliftBlueBanner');
    expect(blueBanner.exists()).toBe(false);
  });

  it('will not display the navigation items when proxying', () => {
    wrapper = mountAs({ isProxying: true });
    const navMenu = wrapper.find('[data-sid="navigation-list-menu"]');
    expect(navMenu.exists()).toBe(false);
  });

  it('will display the proxy users information when proxying', () => {
    wrapper = mountAs({ isProxying: true });
    const proxyWelcomeSection = wrapper.find('[data-sid="welcome-info-proxy"]');
    const proxyAge = wrapper.find('[data-sid="proxy-user-age"]');
    const proxyGpSurgery = wrapper.find('[data-sid="proxy-user-surgery"]');

    expect(proxyWelcomeSection.exists()).toBe(true);
    expect(proxyAge.exists()).toBe(true);
    expect(proxyGpSurgery.exists()).toBe(true);
  });

  it('will not show any public health notifications if none are configured', () => {
    wrapper = mountAs();
    expect(wrapper.findAll('.public-health-notification').length).toBe(0);
  });

  it('will display a public health notification for each in the home screen rules', () => {
    const notification1 = createPublicHealthNotification(1);
    const notification2 = createPublicHealthNotification(2);

    wrapper = mountAs({
      homeScreen: {
        publicHealthNotifications: [notification1, notification2],
      },
    });

    const publicHealthNotification1 = wrapper.find('#public-health-notification-1');
    const publicHealthNotification2 = wrapper.find('#public-health-notification-2');

    expect(publicHealthNotification1.find('[data-purpose="warning-callout-title"]').text()).toEqual(notification1.title);
    expect(publicHealthNotification2.find('[data-purpose="warning-callout-title"]').text()).toEqual(notification2.title);
    expect(publicHealthNotification1.find('[data-purpose="warning-callout-body"]').text()).toEqual(notification1.body);
    expect(publicHealthNotification2.find('[data-purpose="warning-callout-body"]').text()).toEqual(notification2.body);
  });

  each([
    ['will show the indicator when there is unread messages', true, true, true],
    ['will show the indicator when there is only unread app messages', false, true, true],
    ['will show the indicator when there is only unread GP messages', true, false, true],
    ['will not show the indicator when there is no unread messages', false, false, false],
  ]).it('%s',
    async (_, hasUnreadGpMessages, hasUnreadAppMessages, showIndicator) => {
      wrapper = mountAs({ hasUnreadAppMessages, hasUnreadGpMessages });
      await wrapper.vm.$nextTick();
      expect(wrapper.find('#btn_messages_discIndicator').exists()).toBe(showIndicator);
    });

  describe('messaging link', () => {
    const getMessagesLink = wrapperObj => wrapperObj.find('#btn_messages');

    let messagesLink;

    each([
      ['will have text \'View your unread messages\' when there are unread messages', true, true, 'View your unread messages'],
      ['will have text \'View your unread messages\' when there are only unread app messages', false, true, 'View your unread messages'],
      ['will have text \'View your unread messages\' when there are only unread GP messages', true, false, 'View your unread messages'],
      ['will have text \'View your messages\' when there are no unread messages', false, false, 'View your messages'],
    ]).it('%s',
      (_, hasUnreadGpMessages, hasUnreadAppMessages, expectedText) => {
        wrapper = mountAs({ hasUnreadAppMessages, hasUnreadGpMessages });
        messagesLink = getMessagesLink(wrapper);
        expect(messagesLink.exists()).toBe(true);
        expect(messagesLink.text()).toBe(expectedText);
      });

    it('will not show messages link when messages unavailable', () => {
      wrapper = mountAs({
        messagesUnavailable: true,
      });
      messagesLink = getMessagesLink(wrapper);
      expect(messagesLink.exists()).toBe(false);
    });
  });

  describe('gp health record link', () => {
    it('home page health record link will show text for gp health record hub page', () => {
      wrapper = mountAs({});
      const healthRecordsHubLink = wrapper.find('#menu-item-myRecord');
      expect(healthRecordsHubLink.text()).toBe('View your GP health record');
    });
  });
});
