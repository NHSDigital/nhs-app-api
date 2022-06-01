import FeatureNotAvailable from '@/pages/silver-integration/feature-not-available';
import i18n from '@/plugins/i18n';
import { RouterLinkStub } from '@vue/test-utils';
import { mount } from '../../helpers';

jest.mock('@/services/event-bus');
jest.mock('@/services/native-app');

describe('feature not available page', () => {
  const mountFeatureNotAvailable = () => mount(FeatureNotAvailable, {
    mountOpts: {
      i18n,
    },
    $store: {
      state: {
        device: {
          isNativeApp: true,
        },
      },
    },
    $router: {
      currentRoute: {
        query: {
          providerName: 'Substrakt Health',
          featureName: 'View appointments',
        },
      },
    },
    stubs: { 'router-link': RouterLinkStub },
  });
  it('feature not available page is displayed with provider name and feature name from route', async () => {
    const wrapper = mountFeatureNotAvailable();
    await wrapper.vm.$nextTick();

    expect(wrapper.find('span').html()).toContain('Substrakt Health');
    expect(wrapper.find('h1').html()).toContain('View appointments');
  });
});
