import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';
import i18n from '@/plugins/i18n';
import { createStore, mount } from '../../helpers';
import each from 'jest-each';

describe('calculate age in years and months mixin', () => {
  let $store;
  let component;
  let wrapper;
  let result;

  beforeEach(() => {
    $store = createStore();
    component = {
      template: '<div></div>',
      mixins: [CalculateAgeInMonthsAndYears],
    };
    wrapper = mount(component, { $store, mountOpts: { i18n } });
  });

  it('mixin returns the correct label for the age of the user less than one month old', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText({ ageYears: 0, ageMonths: 0 });

    // assert
    expect(result).toBe('Less than 1 month old');
  });

  it('mixin returns the correct label for the age of the user one month old', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText({ ageYears: 0, ageMonths: 1 });

    // assert
    expect(result).toBe('1 month old');
  });

  each([
    [{ ageYears: 0, ageMonths: 2 }],
    [{ ageYears: 0, ageMonths: 3 }],
    [{ ageYears: 0, ageMonths: 4 }],
    [{ ageYears: 0, ageMonths: 5 }],
    [{ ageYears: 0, ageMonths: 6 }],
    [{ ageYears: 0, ageMonths: 7 }],
    [{ ageYears: 0, ageMonths: 8 }],
    [{ ageYears: 0, ageMonths: 9 }],
    [{ ageYears: 0, ageMonths: 10 }],
    [{ ageYears: 0, ageMonths: 11 }],
    [{ ageYears: 0, ageMonths: 12 }],
    [{ ageYears: 0, ageMonths: 13 }],
    [{ ageYears: 0, ageMonths: 14 }],
    [{ ageYears: 0, ageMonths: 15 }],
    [{ ageYears: 0, ageMonths: 16 }],
    [{ ageYears: 0, ageMonths: 17 }],
    [{ ageYears: 0, ageMonths: 18 }],
    [{ ageYears: 0, ageMonths: 19 }],
    [{ ageYears: 0, ageMonths: 20 }],
    [{ ageYears: 0, ageMonths: 21 }],
    [{ ageYears: 0, ageMonths: 22 }],
    [{ ageYears: 0, ageMonths: 23 }],
  ]).describe('age text less than two years old', (linkedAccount) => {
    it('mixin returns label showing age in months', () => {
      result = wrapper.vm.getDisplayedAgeText(linkedAccount);
      expect(result).toBe(`${linkedAccount.ageMonths} months old`);
    });
  });

  each([
    [{ ageYears: 2, ageMonths: 0 }],
    [{ ageYears: 2, ageMonths: 1 }],
    [{ ageYears: 7, ageMonths: 7 }],
    [{ ageYears: 19, ageMonths: 4 }],
    [{ ageYears: 68, ageMonths: 11 }],
  ]).describe('age text over two years old ', (linkedAccount) => {
    it('mixin returns label only showing age in years', () => {
      result = wrapper.vm.getDisplayedAgeText(linkedAccount);
      expect(result).toBe(`${linkedAccount.ageYears} years old`);
    });
  });

  it('mixin returns the correct label for the age of the user that is invalid', () => {
    // act
    result = wrapper.vm.getDisplayedAgeText({ ageYears: -10, ageMonths: -10 });

    // assert
    expect(result).toBe('');
  });
});
