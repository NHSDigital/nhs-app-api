import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import { ORGAN_DONATION_WITHDRAW_REASON_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createRouter, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

describe('OtherThingsToDo', () => {
  const externalLink = 'www.foo.com';
  let wrapper;

  const mountOtherThingsToDo = ({ canWithdraw = true, $router = undefined } = {}) =>
    mount(OtherThingsToDo, {
      $env: {
        BLOOD_DONATION_URL: externalLink,
      },
      $router,
      propsData: { canWithdraw },
    });

  beforeEach(() => {
    redirectTo.mockClear();
    wrapper = mountOtherThingsToDo();
  });

  describe('blood donation link', () => {
    let link;

    beforeEach(() => {
      link = wrapper.find('#btn_blood');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will have link to blood donation site', () => {
      expect(link.attributes().href).toEqual(externalLink);
    });
  });

  describe('cannot withdraw', () => {
    beforeEach(() => {
      wrapper = mountOtherThingsToDo({ canWithdraw: false });
    });

    it('will not show withdraw link', () => {
      expect(wrapper.find('#btn_withdraw').exists()).toBe(false);
    });
  });

  describe('can withdraw', () => {
    let $router;

    beforeEach(() => {
      $router = createRouter();
      wrapper = mountOtherThingsToDo({ canWithdraw: true, $router });
    });

    describe('withdraw link', () => {
      let link;

      beforeEach(() => {
        link = wrapper.find('#btn_withdraw');
      });

      it('will exist', () => {
        expect(link.exists()).toBe(true);
      });

      describe('click', () => {
        beforeEach(() => {
          global.digitalData = {};
          link.trigger('click');
        });

        it('will push ORGAN_DONATION_WITHDRAW_REASON to the router', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_WITHDRAW_REASON_PATH);
        });
      });
    });
  });
});
