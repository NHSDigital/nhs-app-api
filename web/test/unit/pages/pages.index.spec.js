import Index from '@/pages/index';
import { createStore, mount, createRouter } from '../helpers';

describe('index', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({ isProxying = false } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
    $store.getters['session/isProxying'] = isProxying;
    $store.getters['session/currentProfile'] = {
      name: '',
    };
    return mount(Index, { $store, $router, stubs: ['BiometricBanner'] });
  };

  it('will display the navigation items when not proxying', () => {
    wrapper = mountAs({ isProxying: false });
    const navMenu = wrapper.find('[data-sid="navigation-list-menu"]');
    expect(navMenu.exists()).toBe(true);
  });

  it('will not display the navigation items when proxying', () => {
    wrapper = mountAs({ isProxying: true });
    const navMenu = wrapper.find('[data-sid="navigation-list-menu"]');
    expect(navMenu.exists()).toBe(false);
  });
});
