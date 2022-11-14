import ProxyWelcomeSection from '@/components/ProxyWelcomeSection';
import { createStore, mount } from '../helpers';

describe('ProxyWelcomeSection.vue', () => {
  let wrapper;

  const mountProxyWelcomeSection = ({ proxyAge, proxyDetails, displayName }) => mount(ProxyWelcomeSection, {
    $store: createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    }),
    propsData: {
      proxyDetails,
      proxyAge,
      displayName,
    },
  });

  describe('props data', () => {
    describe('Proxy Details does have a value', () => {
      describe('Surgery does have value', () => {
        beforeEach(() => {
          wrapper = mountProxyWelcomeSection({ proxyAge: '21', proxyDetails: { fullName: 'proxy name', gpPracticeName: 'surgery' }, displayName: 'USER DISPLAY NAME' });
        });

        it('will display user surgery section', () => {
          expect(wrapper.find('[data-sid="proxy-user-surgery"]').exists()).toBe(true);
        });
      });
    });

    describe('Age does have value', () => {
      beforeEach(() => {
        wrapper = mountProxyWelcomeSection({ proxyAge: 'age', proxyDetails: { fullName: 'proxy name', gpPracticeName: 'surgery' }, displayName: 'USER DISPLAY NAME' });
      });

      it('will display user age section', () => {
        expect(wrapper.find('[data-sid="proxy-user-age"]').exists()).toBe(true);
      });
    });

    describe('User name has value', () => {
      beforeEach(() => {
        wrapper = mountProxyWelcomeSection({ proxyAge: 'age', proxyDetails: { fullName: 'proxy name', gpPracticeName: 'surgery' }, displayName: 'USER DISPLAY NAME' });
      });

      it('will render sanitised displayName prop value not the proxy fullName value', () => {
        expect(wrapper.find('[data-sid="proxy-name"]').text()).toBe('USER DISPLAY NAME');
      });
    });
  });
});
