import { createStore, mount } from '../helpers';
import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';

describe('calculate age in years and months mixin', () => {
  let $store;
  let component;
  let wrapper;
  let result;

  beforeEach(() => {
    $store = createStore({
      state: {
        linkedAccounts: {
          items: [
            {
              id: 'user-id-0',
              name: 'user 0',
              ageYears: 0,
              ageMonths: 0,
            },
            {
              id: 'user-id-1',
              name: 'user 1',
              ageYears: 0,
              ageMonths: 1,
            },
            {
              id: 'user-id-2',
              name: 'user 2',
              ageYears: 0,
              ageMonths: 8,
            },
            {
              id: 'user-id-3',
              name: 'user 3',
              ageYears: 1,
              ageMonths: 0,
            },
            {
              id: 'user-id-4',
              name: 'user 4',
              ageYears: 20,
              ageMonths: 5,
            },
            {
              id: 'user-id-5',
              name: 'user 5',
              ageYears: -10,
              ageMonths: -10,
            },
          ],
        },
      },
    });
    component = {
      template: '<div></div>',
      mixins: [CalculateAgeInMonthsAndYears],
    };
    wrapper = mount(component, { $store });
  });

  it('mixin returns the correct label for the age of the user less than one month old', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText($store.state.linkedAccounts.items[0]);

    // assert
    expect(result).toBe('translate_linkedProfiles.ageLabels.lessThanOneMonth');
  });

  it('mixin returns the correct label for the age of the user one month old', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText($store.state.linkedAccounts.items[1]);

    // assert
    expect(result).toBe('1translate_linkedProfiles.ageLabels.oneMonth');
  });

  it('mixin returns the correct label for the age of the user more than one month old but less than one year', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText($store.state.linkedAccounts.items[2]);

    // assert
    expect(result).toBe('8translate_linkedProfiles.ageLabels.greaterThanOneMonthLessThan1Year');
  });

  it('mixin returns the correct label for the age of the user that is one year old', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText($store.state.linkedAccounts.items[3]);

    // assert
    expect(result).toBe('1translate_linkedProfiles.ageLabels.oneYear');
  });

  it('mixin returns the correct label for the age of the user that is more than 1 years old', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText($store.state.linkedAccounts.items[4]);

    // assert
    expect(result).toBe('20translate_linkedProfiles.ageLabels.greaterThanOneYearOld');
  });

  it('mixin returns the correct label for the age of the user that is invalid', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText($store.state.linkedAccounts.items[5]);

    // assert
    expect(result).toBe('');
  });
});
