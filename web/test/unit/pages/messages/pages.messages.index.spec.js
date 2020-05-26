import Messages from '@/pages/messages/';
import { mount, createRouter, createStore } from '../../helpers';
import each from 'jest-each';

let wrapper;
let $store;
let $router;

const mountPage = ({
  sjrMessagingEnabled = true,
  sjrIm1MessagingEnabled = true,
  practiceIm1MessagingEnabled = true,
  context = true,
  isProxying = false,
  isProofLevel9 = true,
} = {}) => {
  $router = createRouter();
  $store = createStore({
    state: {
      practiceSettings: { im1MessagingEnabled: practiceIm1MessagingEnabled },
      device: { isNativeApp: false },
      knownServices: {
        knownServices: [{
          id: 'pkb',
          url: 'www.url.com',
        }],
      },
    },
    getters: {
      'serviceJourneyRules/messagingEnabled': sjrMessagingEnabled,
      'serviceJourneyRules/im1MessagingEnabled': sjrIm1MessagingEnabled,
      'serviceJourneyRules/silverIntegrationEnabled': () => (context),
      'session/isProxying': isProxying,
      'session/isProofLevel9': isProofLevel9,
    },
  });
  wrapper = mount(Messages, { $store, $router });
};

describe('messages page', () => {
  describe('no messaging services available', () => {
    mountPage({
      practiceIm1MessagingEnabled: false,
      sjrIm1MessagingEnabled: false,
      context: false,
      sjrMessagingEnabled: false,
    });
    const im1MessagingLink = wrapper.find('#btn_im1_messaging');
    const noMessagesText = wrapper.find('[data-purpose="no-messages-available"]');

    expect(im1MessagingLink.exists()).toBe(false);
    expect(noMessagesText.exists()).toBe(true);
    expect(noMessagesText.text()).toEqual('translate_messagesHub.noMessages');
  });

  describe('im1 messaging services', () => {
    describe('practice disabled or sjr disabled', () => {
      each([
        { practice: true, sjr: false },
        { practice: false, sjr: true },
        { practice: false, sjr: false },
      ]).it('will not show link', ({ practice, sjr }) => {
        mountPage({
          practiceIm1MessagingEnabled: practice,
          sjrIm1MessagingEnabled: sjr,
        });
        expect(wrapper.find('#btn_im1_messaging').exists()).toBe(false);
      });
    });

    describe('messaging link', () => {
      each([
        { sjr: true, expected: true },
        { sjr: false, expected: false },
      ]).it('will not show link', ({ sjr, expected }) => {
        mountPage({
          sjrMessagingEnabled: sjr,
        });
        expect(wrapper.find('#btn_appMessaging').exists()).toBe(expected);
      });
    });

    describe('practice and sjr enabled', () => {
      it('will show link', () => {
        mountPage();
        const im1MessagingLink = wrapper.find('#btn_im1_messaging');
        const im1MessagingLinkSubHeader = im1MessagingLink.find('h2');
        const im1MessagingLinkBody = im1MessagingLink.find('p');

        expect(im1MessagingLink.exists()).toBe(true);
        expect(im1MessagingLinkSubHeader.exists()).toBe(true);
        expect(im1MessagingLinkBody.exists()).toBe(true);
        expect(im1MessagingLinkSubHeader.text()).toEqual('translate_messagesHub.im1Messaging.subheader');
        expect(im1MessagingLinkBody.text()).toEqual('translate_messagesHub.im1Messaging.body');
      });
    });
  });

  describe('pkb messages link', () => {
    describe('pkb messaging enabled', () => {
      beforeEach(() => {
        mountPage();
      });
      it('will show link', () => {
        expect(wrapper.find('#btn_pkb_messages_and_consultations').exists()).toBe(true);
      });
    });

    describe('pkb messaging enabled, but is proxying', () => {
      beforeEach(() => {
        mountPage({
          isProxying: true,
        });
      });
      it('will not show link', () => {
        expect(wrapper.find('#btn_pkb_messages_and_consultations').exists()).toBe(false);
      });
    });

    describe('pkb enabled but is proxying', () => {
      beforeEach(() => {
        mountPage({
          isProxying: true,
        });
      });
      it('will not show link', () => {
        expect(wrapper.find('#btn_pkb_messages_and_consultations').exists()).toBe(false);
      });
    });

    describe('pkb enabled but proof level is p5', () => {
      beforeEach(() => {
        mountPage({
          isProofLevel9: false,
        });
      });
      it('will not show link', () => {
        expect(wrapper.find('#btn_pkb_messages_and_consultations').exists()).toBe(false);
      });
    });

    describe('pkb messaging disabled', () => {
      beforeEach(() => {
        mountPage({
          context: false,
        });
      });
      it('will not show link', () => {
        expect(wrapper.find('#btn_pkb_messages_and_consultations').exists()).toBe(false);
      });
    });
  });
});
