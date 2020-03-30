import { create$T, createStore, mount } from '../../helpers';
import DspInterrupt from '@/pages/nominated-pharmacy/dsp-interrupt';
import { PRESCRIPTIONS } from '@/lib/routes';
import * as dependency from '@/lib/utils';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $router;
  let wrapper;
  let dspLink;
  let prescriptionHomeLink;
  const NOM_PHARMA_DSP_LINK = 'bazz';

  const createState = (state = {
    device: {
      source: 'web',
    },
  }) => state;

  const mountPage = () => mount(DspInterrupt, {
    $store,
    $t,
    $router,
  });

  describe('dsp interrupt page', () => {
    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
        $env: {
          NOM_PHARMA_DSP_LINK,
        },
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
      it('will use "nominated_pharmacy.dspInterrupt.visitOnlineListText" for text', () => {
        expect(dspLink.text())
          .toEqual('translate_nominated_pharmacy.dspInterrupt.visitOnlineListText');
      });
      it('will navigate to online only choices page when clicked ', () => {
        expect(dspLink.attributes().href).toEqual(NOM_PHARMA_DSP_LINK);
      });
    });

    describe('prescriptions home link', () => {
      it('will exist', () => {
        expect(prescriptionHomeLink.exists()).toBe(true);
      });
      it('will use "nominated_pharmacy.dspInterrupt.returnToPrescriptionsText" for text', () => {
        expect(prescriptionHomeLink.text())
          .toEqual('translate_nominated_pharmacy.dspInterrupt.returnToPrescriptionsText');
      });
      it('will navigate to prescriptions home page when clicked ', () => {
        prescriptionHomeLink.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS.path);
      });
    });
  });
});
