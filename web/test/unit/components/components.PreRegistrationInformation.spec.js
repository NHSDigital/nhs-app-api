import PreRegistrationInformation from '@/components/PreRegistrationInformation';
import { mount } from '../helpers';

describe('Pre registration information', () => {
  const createWrapper = (shouldShowHeader = true) => mount(PreRegistrationInformation, {
    propsData: {
      shouldShowHeader,
    },
  });

  describe('header text', () => {
    describe('visible', () => {
      let wrapper;
      beforeEach(() => {
        wrapper = createWrapper();
      });

      it('will be visible on page', async () => {
        expect(wrapper.find('h2').exists()).toBe(true);
      });

      it('will have the expected text to translate', async () => {
        expect(wrapper.find('h2').text()).toEqual('translate_web.home.beforeYouStartTitle');
      });
    });

    describe('not visible', () => {
      let wrapper;
      beforeEach(() => {
        wrapper = createWrapper(false);
      });

      it('will not be visible on page', async () => {
        expect(wrapper.find('h2').exists()).toBe(false);
      });
    });
  });
});
