/* eslint-disable no-underscore-dangle */
import { mount } from '@vue/test-utils';
import LogoutPage from '@/pages/logout';

const createLogoutPage = $store =>
  mount(LogoutPage, {
    mocks: {
      $store,
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
});
