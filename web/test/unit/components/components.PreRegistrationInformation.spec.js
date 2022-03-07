import i18n from '@/plugins/i18n';
import PreRegistrationInformation from '@/components/PreRegistrationInformation';
import { mount } from '../helpers';

describe('Pre registration information', () => {
  const createWrapper = shouldShowFullContent => mount(PreRegistrationInformation, {
    propsData: {
      shouldShowFullContent,
    },
    mountOpts: {
      i18n,
    },
  });

  describe('Should show full content prop', () => {
    describe('true', () => {
      let wrapper;

      beforeEach(() => {
        wrapper = createWrapper(true);
      });

      it('header will be visible on page', () => {
        expect(wrapper.find('h2').exists()).toBe(true);
      });

      it('header will have the expected text to translate', () => {
        expect(wrapper.find('h2').text()).toEqual('Who can have an NHS account');
      });

      it('list will be visible on page', () => {
        expect(wrapper.find('ul').exists()).toBe(true);
      });
    });

    describe('false', () => {
      let wrapper;

      beforeEach(() => {
        wrapper = createWrapper(false);
      });

      it('header will not be visible on page', () => {
        expect(wrapper.find('h2').exists()).toBe(false);
      });

      it('list will not be visible on page', () => {
        expect(wrapper.find('ul').exists()).toBe(false);
      });
    });
  });
});
