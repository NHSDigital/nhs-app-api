import * as dependency from '@/lib/utils';
import { PRESCRIPTIONS } from '@/lib/routes';
import NoNominatedPharmacyChange from '@/pages/nominated-pharmacy/cannot-change';
import { create$T, createStore, mount } from '../../helpers';

const $t = create$T();

describe('can not change nominated pharmacy', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'ios',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'Boots',
        pharmacyType: 'P3',
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
      },
    },
  }) => state;

  const mountPage = () => mount(NoNominatedPharmacyChange, { $store, $style, $t });

  describe('show-pharmacy-summary', () => {
    let pharmacySummary;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacySummary = wrapper.find('#pharmacy-summary');
    });

    it('will exist', () => {
      expect(pharmacySummary.exists()).toBe(true);
    });
  });

  describe('show-pharmacy-opening-times', () => {
    let pharmacyOpeningTimes;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacyOpeningTimes = wrapper.find('#pharmacy-opening-times');
    });

    it('will exist', () => {
      expect(pharmacyOpeningTimes.exists()).toBe(true);
    });
  });

  describe('back button', () => {
    let backButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      backButton = wrapper.find('#back-button').find('a');
      $style = {
        button: 'button',
        green: 'grey',
      };
    });

    it('will exist', () => {
      expect(backButton.exists()).toBe(true);
    });

    it('will use "nominatedPharmacyCannotChange.backButton.text" for text', () => {
      expect(backButton.text())
        .toEqual('translate_nominatedPharmacyCannotChange.backButton');
    });

    it('will redirect to the prescriptions page on click', async () => {
      dependency.redirectTo = jest.fn();
      await backButton.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS.path);
    });
  });
});
