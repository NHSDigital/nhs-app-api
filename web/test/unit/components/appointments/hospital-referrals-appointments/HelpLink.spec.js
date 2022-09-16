import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import { createStore, mount } from '../../../helpers';

const mountReferralBookable = ({ propsData = {}, $store }) => mount(HelpLink,
  {
    propsData,
    $store,
  });

const propsSetup = (id, hasBody) => {
  let bodyVal = '';

  if (hasBody) {
    bodyVal = 'This is the body';
  }

  return mountReferralBookable({
    propsData: {
      id,
      body: bodyVal,
      text: 'This is the main text',
      href: '#',
      clickFunc() {},
    },
    $store: createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    }),
  });
};

describe('Wayfinder help link', () => {
  describe('link with a body', () => {
    const wrapper = propsSetup('btn_linkWithBody', true);

    it('will display the help link with text', () => {
      const text = wrapper.find('#btn_linkWithBody').find('h2');

      expect(text.exists()).toBe(true);
      expect(text.text()).toBe('This is the main text');
    });

    it('will display the help link with a body', () => {
      const body = wrapper.find('#btn_linkWithBody').find('p');

      expect(body.exists()).toBe(true);
      expect(body.text()).toBe('This is the body');
    });
  });

  describe('link without a body', () => {
    const wrapper = propsSetup('btn_linkWithoutABody', false);

    it('will display the help link with text', () => {
      const text = wrapper.find('#btn_linkWithoutABody').find('h2');

      expect(text.exists()).toBe(true);
      expect(text.text()).toBe('This is the main text');
    });

    it('will display the help link without a body', () => {
      const body = wrapper.find('#btn_linkWithoutABody').find('p');

      expect(body.exists()).toBe(false);
    });
  });
});
