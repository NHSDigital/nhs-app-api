/* eslint-disable no-underscore-dangle */
import LogoutPage from '@/pages/logout';
import { createStore, mount } from '../helpers';

jest.mock('@/services/authorisation-service');
jest.mock('@/lib/utils');

const createLogoutPage = ($store, query) => mount(LogoutPage, {
  $store,
  $route: {
    query,
  },
});
describe('logout.vue', () => {
  it('will call auth/logout', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: '',
        },
      },
      getters: {
        'session/isLoggedIn': () => true,
      },
    };

    createLogoutPage($store);
    expect($store.dispatch).toHaveBeenCalledWith('auth/logout');
  });

  describe('metaInfo', () => {
    let head;

    beforeEach(() => {
      head = LogoutPage.metaInfo.call();
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
    let wrapper;

    const $store = createStore({
      state: {
        getters: {
          'session/isLoggedIn': () => true,
        },
      },
    });

    beforeEach(() => {
      wrapper = createLogoutPage($store, query);
      button = wrapper.find('#loginButton');
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
    });
  });
});
