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
    };

    createLogoutPage($store);
    expect($store.dispatch).toHaveBeenCalledWith('auth/logout');
  });
});
