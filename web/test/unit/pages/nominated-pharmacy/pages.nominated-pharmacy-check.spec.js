import * as dependency from '@/lib/utils';
import NominatedPharmacyCheck from '@/pages/nominated-pharmacy/check';
import { $t, createStore, mount } from '../../helpers';
import { PRESCRIPTION_REPEAT_COURSES, PRESCRIPTIONS, NOMINATED_PHARMACY_SEARCH } from '../../../../src/lib/routes';

describe('nominated pharmacy not found', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: undefined,
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyCheck, { $store, $style, $t });

  describe('warning', () => {
    let warningText;
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      warningText = wrapper.find('#warning-text');
    });

    it('will exist', () => {
      expect(warningText.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.warningText" for text', () => {
      expect(warningText.text())
        .toEqual('translate_nominatedPharmacyNotFound.warningText');
    });
  });

  describe('instruction', () => {
    let instruction;
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      instruction = wrapper.find('#instruction');
    });

    it('will exist', () => {
      expect(instruction.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.line" for text', () => {
      expect(instruction.text())
        .toEqual('translate_nominatedPharmacyNotFound.line');
    });
  });

  describe('link-to-add-nominated-pharmacy', () => {
    let link;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        link: 'link',
      };
      wrapper = mountPage();
      link = wrapper.find('#link-to-nominate-pharmacy');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.nominatedPharmacyLink" for text', () => {
      expect(link.text())
        .toEqual('translate_nominatedPharmacyNotFound.nominatedPharmacyLink');
    });

    it('will redirect to search nominated pharmacy page', async () => {
      dependency.redirectTo = jest.fn();
      await link.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH.path, null);
    });
  });

  describe('continue-to-repeat-prescriptions', () => {
    let continueButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        button: 'button',
        green: 'green',
      };
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button-found');
    });

    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.continueButton" for text', () => {
      expect(continueButton.text())
        .toEqual('translate_nominatedPharmacyNotFound.continueButton');
    });

    it('will navigate to the repeat prescriptions page on click', async () => {
      dependency.redirectTo = jest.fn();
      await continueButton.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_REPEAT_COURSES.path, null);
    });
  });

  describe('back-to-prescriptions', () => {
    let backButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        button: 'button',
        grey: 'grey',
      };
      wrapper = mountPage();
      backButton = wrapper.find('#back-button');
    });

    it('will exist', () => {
      expect(backButton.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.backButton" for text', () => {
      expect(backButton.text())
        .toEqual('translate_nominatedPharmacyNotFound.backButton');
    });

    it('will navigate back to prescriptions page', async () => {
      dependency.redirectTo = jest.fn();
      await backButton.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS.path, null);
    });
  });
});


describe('nominated pharmacy found', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'Boots',
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyCheck, { $store, $style, $t });

  describe('continue-to-repeat-prescriptions', () => {
    let continueButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        button: 'button',
        green: 'green',
      };
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button-found');
    });

    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will use "nominatedPharmacy.continueButton" for text', () => {
      expect(continueButton.text())
        .toEqual('translate_nominatedPharmacy.continueButton');
    });

    it('will navigate to the repeat prescriptions page on click', async () => {
      dependency.redirectTo = jest.fn();
      await continueButton.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_REPEAT_COURSES.path, null);
    });
  });

  describe('show-pharmacy-details', () => {
    let pharmacyDetails;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      pharmacyDetails = wrapper.find('#pharmacy-details');
    });

    it('will exist', () => {
      expect(pharmacyDetails.exists()).toBe(true);
    });
  });
});

