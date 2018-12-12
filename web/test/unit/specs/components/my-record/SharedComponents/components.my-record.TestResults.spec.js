/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import TestResults from '@/components/my-record/SharedComponents/TestResults';
import DCRErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { shallowMount } from '../../../../helpers';

Vue.filter('datePart', x => x.toString());

const createData = () => ([{
  date: { value: new Date(2018, 4, 1) },
}, {
  date: { value: new Date(2018, 4, 3) },
}]);

const createPropData = ({
  hasErrored = false,
  hasAccess = true,
  supplier = 'EMIS',
  data = createData(),
} = {}) => ({
  results: {
    data,
    hasAccess,
    hasErrored,
  },
  supplier,
});

describe('TestResults', () => {
  let component;
  let propsData;

  describe('success', () => {
    beforeEach(() => {
      propsData = createPropData();
      component = shallowMount(TestResults, {
        propsData,
      });
    });

    it('will not show the DCR error no access', () => {
      expect(component.find(DCRErrorNoAccess).exists()).toBe(false);
    });

    it('will display results in decending order', () => {
      const dates = component
        .findAll('[data-purpose="record-item"] > span')
        .wrappers
        .map(x => x.text());

      expect(dates[0]).toEqual(new Date(2018, 4, 3).toString());
      expect(dates[1]).toEqual(new Date(2018, 4, 1).toString());
    });
  });

  describe('has errored', () => {
    beforeEach(() => {
      propsData = createPropData({ hasErrored: true });
      component = shallowMount(TestResults, {
        propsData,
      });
    });

    it('will show the DCR error no access', () => {
      expect(component.find(DCRErrorNoAccess).exists()).toBe(true);
    });
  });

  describe('no data', () => {
    beforeEach(() => {
      propsData = createPropData({ data: [] });
      component = shallowMount(TestResults, {
        propsData,
      });
    });

    it('will show the DCR error no access', () => {
      expect(component.find(DCRErrorNoAccess).exists()).toBe(true);
    });
  });

  describe('no access', () => {
    beforeEach(() => {
      propsData = createPropData({ hasAccess: false });
      component = shallowMount(TestResults, {
        propsData,
      });
    });

    it('will show the DCR error no access', () => {
      expect(component.find(DCRErrorNoAccess).exists()).toBe(true);
    });
  });
});
