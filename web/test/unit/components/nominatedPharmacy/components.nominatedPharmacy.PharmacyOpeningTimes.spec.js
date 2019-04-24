import PharmacyOpeningTimes from '@/components/nominatedPharmacy/PharmacyOpeningTimes';
import { initialState } from '@/store/modules/nominatedPharmacy/mutation-types';
import { createStore, mount } from '../../helpers';

describe('pharmacy summary', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    $store = createStore({
      state: {
        nominatedPharmacy: initialState(),
        device: {
          source: 'android',
        },
      },
    });

    wrapper = mount(PharmacyOpeningTimes, {
      $store,
      propsData: {
        pharmacyOpeningTime: [
          {
            day: 'Sunday',
            times: [],
          },
          {
            day: 'Monday',
            times: ['8am to 6pm'],
          },
          {
            day: 'Tuesday',
            times: ['8am to 1pm', '2pm to 6pm'],
          }],
      },
    });
  });

  describe('show nominated pharmacy opening times', () => {
    let openingTimeRows;

    beforeEach(() => {
      openingTimeRows = wrapper.findAll('[data-day-row]');
    });

    it('will exist', () => {
      expect(openingTimeRows.exists()).toBe(true);
    });

    it('will format the address', () => {
      expect(openingTimeRows.length).toBe(3);

      const sunday = openingTimeRows.at(0);
      expect(sunday.text()).toContain('Sunday translate_nominatedPharmacy.closed');

      const monday = openingTimeRows.at(1);
      expect(monday.text()).toContain('Monday');
      expect(monday.text()).toContain('8am to 6pm');

      const tuesday = openingTimeRows.at(2);
      expect(tuesday.text()).toContain('Tuesday');
      expect(tuesday.text()).toContain('8am to 1pm');
      expect(tuesday.text()).toContain('2pm to 6pm');
    });
  });
});
