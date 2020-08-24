import * as dependency from '@/lib/utils';
import i18n from '@/plugins/i18n';
import NominatedPharmacyCheck from '@/pages/nominated-pharmacy/check';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import { PRESCRIPTION_REPEAT_COURSES_PATH, PRESCRIPTIONS_PATH } from '@/router/paths';
import { createStore, mount } from '../../helpers';

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
      hasLoaded: true,
      pharmacy: {
        pharmacyName: undefined,
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
      },
    },
  }) => state;

  const mountPage = () => mount(
    NominatedPharmacyCheck,
    {
      $store,
      $style,
      $router,
      mountOpts: {
        i18n,
      },
    },
  );

  describe('no nominated pharmacy warning', () => {
    let noNominatedPharmacyWarning;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {};
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      $store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
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
      $store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button-found');
    });

    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will display continue without nominating text', () => {
      expect(continueButton.text()).toEqual('Continue without nominating');
    });

    it('will navigate to the repeat prescriptions page on click', async () => {
      dependency.redirectTo = jest.fn();
      await continueButton.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_REPEAT_COURSES_PATH);
    });
  });

  describe('back to prescriptions link present on desktop', () => {
    let backLink;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      $store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
      wrapper = mountPage();
      backLink = wrapper.find('#back-link').find('a');
    });

    it('will exist', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('will display back text', () => {
      expect(backLink.text()).toEqual('Back');
    });

    it('will navigate back to prescriptions page', async () => {
      dependency.redirectTo = jest.fn();
      await backLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_PATH);
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

  const mountPage = () => mount(
    NominatedPharmacyCheck,
    {
      $store,
      $style,
      mountOpts: {
        i18n,
      },
    },
  );

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
    $store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
    wrapper = mountPage();
    backButton = wrapper.find('#back-link').find('a');
  });

  it('will exist', () => {
    expect(backButton.exists()).toBe(true);
  });

  it('will display back text', () => {
    expect(backButton.text()).toEqual('Back');
  });

  it('will navigate back to prescriptions page', async () => {
    dependency.redirectTo = jest.fn();
    await backButton.trigger('click');
    expect(dependency.redirectTo)
      .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_PATH);
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

  const mountPage = () => mount(
    NominatedPharmacyCheck,
    {
      $store,
      $style,
      mountOpts: {
        i18n,
      },
    },
  );

  describe('continue to repeat prescriptions', () => {
    let continueButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        button: 'button',
        green: 'green',
      };
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      $store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button-found');
    });

    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will display continue text', () => {
      expect(continueButton.text()).toEqual('Continue');
    });

    it('will navigate to the repeat prescriptions page on click', async () => {
      dependency.redirectTo = jest.fn();
      await continueButton.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_REPEAT_COURSES_PATH);
    });
  });

  describe('show pharmacy details', () => {
    let pharmacyDetails;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      $store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
      wrapper = mountPage();
      pharmacyDetails = wrapper.find('#pharmacy-details');
    });

    it('will exist', () => {
      expect(pharmacyDetails.exists()).toBe(true);
    });
  });
});
