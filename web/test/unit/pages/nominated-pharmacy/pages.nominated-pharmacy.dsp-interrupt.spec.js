import DspInterrupt from '@/pages/nominated-pharmacy/dsp-interrupt';
import i18n from '@/plugins/i18n';
import { PRESCRIPTIONS_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';
import {
  NOMINATED_PHARMACY_DSP_URL,
} from '@/router/externalLinks';
import { create$T, createStore, mount } from '../../helpers';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $router;
  let wrapper;
  let dspLink;
  let prescriptionHomeLink;

  const createState = (state = {
    device: {
      source: 'web',
    },
  }) => state;

  const mountPage = () => mount(
    DspInterrupt,
    {
      $store,
      $t,
      $router,
      mountOpts: {
        i18n,
      },
    },
  );

  describe('dsp interrupt page', () => {
    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      dependency.redirectTo = jest.fn();
      wrapper = mountPage();
      dspLink = wrapper.find('#dsp-link').find('a');
      prescriptionHomeLink = wrapper.find('#prescriptions-home-link').find('a');
    });

    describe('continue-button', () => {
      it('will exist', () => {
        expect(dspLink.exists()).toBe(true);
      });
      it('will display list online pharmacies text', () => {
        expect(dspLink.text()).toEqual('View a list of online-only pharmacies');
      });
      it('will navigate to online only choices page when clicked ', () => {
        expect(dspLink.attributes().href).toEqual(NOMINATED_PHARMACY_DSP_URL);
      });
    });

    describe('prescriptions home link', () => {
      it('will exist', () => {
        expect(prescriptionHomeLink.exists()).toBe(true);
      });
      it('will display got back to your prescriptions text', () => {
        expect(prescriptionHomeLink.text()).toEqual('Go back to your prescriptions');
      });
      it('will navigate to prescriptions home page when clicked ', () => {
        prescriptionHomeLink.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_PATH);
      });
    });
  });
});
