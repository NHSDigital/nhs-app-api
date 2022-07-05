/* eslint-disable no-underscore-dangle */
import LogoutPage from '@/pages/logout';
import { createStore, mount } from '../helpers';
import i18n from '@/plugins/i18n';

jest.mock('@/services/authorisation-service');
jest.mock('@/lib/utils');

const createLogoutPage = ($store, query) => mount(LogoutPage, {
  $store,
  $route: {
    query,
  },
  mountOpts: { i18n },
});

describe('logout.vue', () => {
  let $store;
  let wrapper;
  beforeEach(() => {
    $store = {
      dispatch: jest.fn(),
      state: {
        session: {
          showExpiryMessage: true,
        },
        device: {
          source: '',
        },
      },
      getters: {
        'session/isLoggedIn': () => true,
      },
    };

    wrapper = createLogoutPage($store);
  });
  it('will call auth/logout', () => {
    expect($store.dispatch).toHaveBeenCalledWith('auth/logout');
  });

  describe('metaInfo', () => {
    let head;

    beforeEach(() => {
      head = LogoutPage.metaInfo.call(wrapper.vm);
    });

    it('will have no scripts defined', () => {
      expect(head.script).toBeUndefined();
    });

    it('will disable sanitizers for noscript', () => {
      expect(head.__dangerouslyDisableSanitizers).toEqual(['noscript']);
    });

    it('will have a noscript to redirect to /', () => {
      expect(head.noscript[0]).toEqual({
        innerHTML: '<meta http-equiv="refresh" content="0;URL=\'/\'">',
        body: false,
      });
    });
  });

  describe('continue button', () => {
    const query = { REDIRECT_PARAMETER: 'foo' };
    let button;
    let forSecurityYouAreAutoLoggedOutText;
    let ifYouWereEnteringInfoText;

    $store = createStore({
      state: {
        session: { showExpiryMessage: false },
        getters: {
          'session/isLoggedIn': () => true,
        },
      },
    });

    beforeEach(() => {
      wrapper = createLogoutPage($store, query);
      button = wrapper.find('#loginButton');
      forSecurityYouAreAutoLoggedOutText = wrapper.find('#forSecurityYouAreAutoLoggedOutText');
      ifYouWereEnteringInfoText = wrapper.find('#ifYouWereEnteringInfoText');
    });

    it('will exist', () => {
      expect(button.exists()).toBe(true);
    });

    it('will be enabled', () => {
      expect(wrapper.vm.isButtonDisabled).toBe(false);
    });

    describe('on click', () => {
      beforeEach(() => {
        button.trigger('click');
      });

      it('will disable button', () => {
        expect(wrapper.vm.isButtonDisabled).toBe(true);
      });

      it('will dispatch `analytics/satelliteTrack`', () => {
        expect($store.dispatch).toBeCalledWith('analytics/satelliteTrack', 'login');
      });

      it('will exists paragraphs text for session expiry', () => {
        expect(forSecurityYouAreAutoLoggedOutText.exists()).toBe(true);
        expect(ifYouWereEnteringInfoText.exists()).toBe(true);
      });
    });
  });
});
