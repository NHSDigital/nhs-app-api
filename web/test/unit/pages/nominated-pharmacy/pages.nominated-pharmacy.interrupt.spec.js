import { create$T, createStore, mount } from '../../helpers';
import NominatedPharmacyInterrupt from '@/pages/nominated-pharmacy/interrupt';
import { NOMINATED_PHARMACY } from '@/lib/routes';
import * as dependency from '@/lib/utils';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $router;
  let wrapper;
  let continueButton;
  let nominatedPharmacyFoundWarning;
  let nominatedPharmacyFoundWarningLine1;
  let nominatedPharmacyFoundWarningLine2;
  let backLink;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {},
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyInterrupt, { $store, $t, $router });

  describe('interrupt page', () => {
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button');
    });

    describe('continue-button', () => {
      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      it('will use "nominated_pharmacy.interrupt.continueButton" for text', () => {
        expect(continueButton.text())
          .toEqual('translate_nominated_pharmacy.interrupt.continueButton');
      });
    });
  });

  describe('nominated pharmacy not found', () => {
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button');
      nominatedPharmacyFoundWarning = wrapper.find('#nominated-pharmacy-prescriptions-warning');
    });

    describe('correct content is present', () => {
      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
        expect(nominatedPharmacyFoundWarning.exists()).toBe(false);
      });
    });
  });

  describe('nominated pharmacy found', () => {
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      dependency.redirectTo = jest.fn();
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button');
      nominatedPharmacyFoundWarning = wrapper.find('#nominated-pharmacy-prescriptions-warning');
      nominatedPharmacyFoundWarningLine1 = wrapper.find('#prescriptions-warning-line-one');
      nominatedPharmacyFoundWarningLine2 = wrapper.find('#prescriptions-warning-line-two');
      backLink = wrapper.find('#back-link').find('a');
    });

    describe('correct content', () => {
      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
        expect(nominatedPharmacyFoundWarning.exists()).toBe(true);
        expect(nominatedPharmacyFoundWarningLine1.exists()).toBe(true);
        expect(nominatedPharmacyFoundWarningLine2.exists()).toBe(true);
        expect(backLink.exists()).toBe(true);
      });

      it('will use the correct locale data for the p tags ', () => {
        expect(nominatedPharmacyFoundWarningLine1.text())
          .toEqual('translate_nominated_pharmacy.interrupt.nominatedPharmacyFoundLine1');
        expect(nominatedPharmacyFoundWarningLine2.text())
          .toEqual('translate_nominated_pharmacy.interrupt.nominatedPharmacyFoundLine2');
      });
    });

    describe('back link', () => {
      it('will exist', () => {
        expect(backLink.exists()).toBe(true);
      });

      it('will navigate to nominated pharmacy page when clicked ', () => {
        backLink.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY.path);
      });
    });
  });
});
