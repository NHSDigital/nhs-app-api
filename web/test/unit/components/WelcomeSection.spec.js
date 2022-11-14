import WelcomeSection from '@/components/WelcomeSection';
import { createStore, mount } from '../helpers';

describe('WelcomeSection.vue', () => {
  let wrapper;

  const mountWelcomeSection = ({ displayName, nhsNumber }) => mount(WelcomeSection, {
    $store: createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    }),
    propsData: {
      displayName,
      nhsNumber,
    },
  });

  describe('props data', () => {
    describe('nhs number has value', () => {
      let nhsNumberSection;

      beforeEach(() => {
        wrapper = mountWelcomeSection({ displayName: 'name', nhsNumber: '123456789' });
        nhsNumberSection = wrapper.find('[data-sid="user-nhs-number"]');
      });

      it('will display nhs number section', () => {
        expect(nhsNumberSection.exists()).toBe(true);
      });

      it('will display the passed in nhs number', () => {
        expect(nhsNumberSection.text().replace(/\n|\r/g, '').replace(/  +/g, ' ')).toBe('123456789');
      });
    });

    describe('nhs number does not have value', () => {
      beforeEach(() => {
        wrapper = mountWelcomeSection({ displayName: 'name', nhsNumber: undefined });
      });

      it('will not display nhs number section', () => {
        expect(wrapper.find('[data-sid="user-nhs-number"]').exists()).toBe(false);
      });
    });

    describe('User name has value', () => {
      beforeEach(() => {
        wrapper = mountWelcomeSection({ displayName: 'USER DISPLAY NAME', nhsNumber: '123456789' });
      });

      it('will render displayName', () => {
        expect(wrapper.find('[data-sid="user-name"]').text()).toBe('USER DISPLAY NAME');
      });
    });

    describe('Display name does not have value', () => {
      beforeEach(() => {
        wrapper = mountWelcomeSection({ displayName: '', nhsNumber: '123456789' });
      });

      it('will render empty displayName', () => {
        expect(wrapper.find('[data-sid="user-name"]').text()).toBe('');
      });
    });
  });
});
