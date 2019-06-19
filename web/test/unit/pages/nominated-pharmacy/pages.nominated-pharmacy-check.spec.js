import * as dependency from '@/lib/utils';
import NominatedPharmacyCheck from '@/pages/nominated-pharmacy/check';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import { create$T, createStore, mount } from '../../helpers';
import { PRESCRIPTION_REPEAT_COURSES, PRESCRIPTIONS } from '../../../../src/lib/routes';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $style;
  let $router;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: undefined,
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyCheck, { $store, $style, $t, $router });

  describe('no nominated pharmacy warning', () => {
    let noNominatedPharmacyWarning;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {};
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      noNominatedPharmacyWarning = wrapper.find(NoNominatedPharmacyWarning);
    });

    it('will exist', () => {
      expect(noNominatedPharmacyWarning.exists()).toBe(true);
    });
  });

  describe('continue to repeat prescriptions', () => {
    let continueButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        button: 'button',
        green: 'green',
      };
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
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

  describe('back to prescriptions link present on desktop', () => {
    let backLink;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      backLink = wrapper.find('#back-link');
    });

    it('will exist', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyNotFound.backButton" for text', () => {
      expect(backLink.text())
        .toEqual('translate_nominatedPharmacyNotFound.backButton');
    });

    it('will navigate back to prescriptions page', async () => {
      dependency.redirectTo = jest.fn();
      await backLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS.path, null);
    });
  });
});

describe('back button present on mobile app', () => {
  let backButton;
  let $store;
  let $style;
  let wrapper;

  const createStateForApp = (state = {
    device: {
      source: 'ios',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'boots',
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyCheck, { $store, $style, $t });

  beforeEach(() => {
    $store = createStore({
      dispatch: jest.fn(() => Promise.resolve()),
      state: createStateForApp(),
    });
    $style = {
      button: 'button',
      grey: 'grey',
    };
    $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
    wrapper = mountPage();
    backButton = wrapper.find('#back-link');
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

describe('community pharmacy is nominated', () => {
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
        pharmacyType: 'P1',
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyCheck, { $store, $style, $t });

  describe('continue to repeat prescriptions', () => {
    let continueButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        button: 'button',
        green: 'green',
      };
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button-found');
    });

    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will use "nominated_pharmacy.continueButton" for text', () => {
      expect(continueButton.text())
        .toEqual('translate_nominated_pharmacy.continueButton');
    });

    it('will navigate to the repeat prescriptions page on click', async () => {
      dependency.redirectTo = jest.fn();
      await continueButton.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_REPEAT_COURSES.path, null);
    });
  });

  describe('show pharmacy details', () => {
    let pharmacyDetails;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacyDetails = wrapper.find('#pharmacy-details');
    });

    it('will exist', () => {
      expect(pharmacyDetails.exists()).toBe(true);
    });
  });
});
