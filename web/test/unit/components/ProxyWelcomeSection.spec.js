import ProxyWelcomeSection from '@/components/ProxyWelcomeSection';
import { createStore, mount } from '../helpers';

describe('ProxyWelcomeSection.vue', () => {
  let wrapper;

  const mountProxyWelcomeSection = ({ proxyAge, proxyDetails }) => mount(ProxyWelcomeSection, {
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
    },
  });

  describe('props data', () => {
    describe('Proxy Details does have a value', () => {
      describe('Surgery does have value', () => {
        beforeEach(() => {
          wrapper = mountProxyWelcomeSection({ proxyAge: '21', proxyDetails: { fullName: 'name', gpPracticeName: 'surgery' } });
        });

        it('will display user surgery section', () => {
          expect(wrapper.find('[data-sid="proxy-user-surgery"]').exists()).toBe(true);
        });
      });
    });

    describe('Age does have value', () => {
      beforeEach(() => {
        wrapper = mountProxyWelcomeSection({ proxyAge: 'age', proxyDetails: { fullName: 'name', gpPracticeName: 'surgery' } });
      });

      it('will display user age section', () => {
        expect(wrapper.find('[data-sid="proxy-user-age"]').exists()).toBe(true);
      });
    });
  });
});
