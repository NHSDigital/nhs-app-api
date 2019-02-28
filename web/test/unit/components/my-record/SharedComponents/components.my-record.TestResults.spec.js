/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import TestResults from '@/components/my-record/SharedComponents/TestResults';
import DCRErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, shallowMount } from '../../../helpers';

Vue.filter('datePart', x => x.toString());

const createData = () => ([{
  date: { value: new Date(2018, 4, 1) },
}, {
  date: { value: new Date(2018, 4, 3) },
}]);

const createPropData = ({
  hasErrored = false,
  hasAccess = true,
  isCollapsed = false,
  supplier = 'EMIS',
  data = createData(),
} = {}) => ({
  results: {
    data,
    hasAccess,
    hasErrored,
  },
  supplier,
  isCollapsed,
});

const createState = () => ({
  myRecord: initialState(),
  device: {
    isNativeApp: false,
  },
});

const createComponent = ({ $store = createStore({ state: createState() }), propsData } = {}) =>
  shallowMount(TestResults, { $store, propsData });

describe('TestResults', () => {
  describe('success', () => {
    let $store;
    let component;
    let propsData;
    beforeEach(() => {
      propsData = createPropData();
      $store = createStore({ state: createState() });
      component = createComponent({ $store, propsData });
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
    let propsData;
    let $store;
    let component;
    beforeEach(() => {
      propsData = createPropData({ hasErrored: true });
      $store = createStore({ state: createState() });
      component = createComponent({ $store, propsData });
    });

    it('will show the DCR error no access', () => {
      expect(component.find(DCRErrorNoAccess).exists()).toBe(true);
    });
  });

  describe('no data', () => {
    let propsData;
    let $store;
    let component;
    beforeEach(() => {
      propsData = createPropData({ data: [] });
      $store = createStore({ state: createState() });
      component = createComponent({ $store, propsData });
    });

    it('will show the DCR error no access', () => {
      expect(component.find(DCRErrorNoAccess).exists()).toBe(true);
    });
  });

  describe('no access', () => {
    let propsData;
    let $store;
    let component;
    beforeEach(() => {
      propsData = createPropData({ hasAccess: false });
      $store = createStore({ state: createState() });
      component = createComponent({ $store, propsData });
    });

    it('will show the DCR error no access', () => {
      expect(component.find(DCRErrorNoAccess).exists()).toBe(true);
    });
  });
});
