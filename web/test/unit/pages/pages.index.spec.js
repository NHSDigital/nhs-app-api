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
        practiceSettings: {
          im1MessagingEnabled: false,
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

  it('will display the proxy users information when proxying', () => {
    wrapper = mountAs({ isProxying: true });
    const proxyWelcomeSection = wrapper.find('[data-sid="welcome-info-proxy"]');
    const proxyAge = wrapper.find('[data-sid="proxy-user-age"]');
    const proxyGpSurgery = wrapper.find('[data-sid="proxy-user-surgery"]');

    expect(proxyWelcomeSection.exists()).toBe(true);
    expect(proxyAge.exists()).toBe(true);
    expect(proxyGpSurgery.exists()).toBe(true);
  });
});
