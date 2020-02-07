import { create$T, createStore, mount } from '../../helpers';
import DspInterrupt from '@/pages/nominated-pharmacy/dsp-interrupt';
import { NOMINATED_PHARMACY_CHOOSE_TYPE, NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES } from '@/lib/routes';
import * as dependency from '@/lib/utils';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $router;
  let wrapper;
  let continueButton;
  let backLink;

  const createState = (state = {
    device: {
      source: 'web',
    },
  }) => state;

  const mountPage = () => mount(DspInterrupt, { $store, $t, $router });

  describe('dsp interrupt page', () => {
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      dependency.redirectTo = jest.fn();
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button');
      backLink = wrapper.find('#back-link').find('a');
    });

    describe('continue-button', () => {
      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });
      it('will use "nominated_pharmacy.interrupt.continueButton" for text', () => {
        expect(continueButton.text())
          .toEqual('translate_nominated_pharmacy.dspInterrupt.continueButton');
      });
      it('will navigate to online only choices page when clicked ', () => {
        continueButton.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.path);
      });
    });

    describe('back link', () => {
      it('will exist', () => {
        expect(backLink.exists()).toBe(true);
      });
      it('will navigate to choose type page when clicked ', () => {
        backLink.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_CHOOSE_TYPE.path);
      });
    });
  });
});
