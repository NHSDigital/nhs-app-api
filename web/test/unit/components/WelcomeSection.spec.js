import WelcomeSection from '@/components/WelcomeSection';
import { createStore, mount } from '../helpers';

describe('WelcomeSection.vue', () => {
  let wrapper;

  const mountWelcomeSection = nhsNumber => mount(WelcomeSection, {
    $store: createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    }),
    propsData: {
      nhsNumber,
    },
  });

  describe('props data', () => {
    describe('nhs number has value', () => {
      let nhsNumberSection;

      beforeEach(() => {
        wrapper = mountWelcomeSection('123456789');
        nhsNumberSection = wrapper.find('#welcomeSectionNhsNumber');
      });

      it('will display nhs number section', () => {
        expect(nhsNumberSection.exists()).toBe(true);
      });

      it('will display the passed in nhs number', () => {
        expect(nhsNumberSection.text().replace(/\n|\r/g, '').replace(/  +/g, ' ')).toBe('NHS number: 123456789');
      });
    });

    describe('nhs number does not have value', () => {
      beforeEach(() => {
        wrapper = mountWelcomeSection(undefined);
      });

      it('will not display nhs number section', () => {
        expect(wrapper.find('#welcomeSectionNhsNumber').exists()).toBe(false);
      });
    });
  });
});
