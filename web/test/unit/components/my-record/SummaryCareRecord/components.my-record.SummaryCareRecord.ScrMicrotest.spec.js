import ScrMicrotest from '@/components/my-record/SummaryCareRecord/ScrMICROTEST';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, shallowMount } from '../../../helpers';

const createPropsData = () => ({
  record: {
    medications: {
      data: {},
    },
  },
});

const createState = () => ({
  myRecord: initialState(),
  device: {
    isNativeApp: true,
  },
});

const createComponent = ({ $store = createStore({ state: createState() }) } = {}) =>
  shallowMount(ScrMicrotest, { $store, propsData: createPropsData() });

describe('ScrMicrotest', () => {
  describe('running on server', () => {
    let $store;
    let component;

    beforeEach(() => {
      process.client = false;
      $store = createStore({ state: createState() });
      component = createComponent({ $store });
    });

    it('will have an isAllergiesAndAdverseReactionsCollapsed value of false', () => {
      expect(component.vm.isAllergiesAndAdverseReactionsCollapsed).toBe(false);
    });
  });

  describe('running on client', () => {
    let component;
    let $store;

    beforeEach(() => {
      process.client = true;
      $store = createStore({ state: createState() });
      component = createComponent({ $store });
    });

    it('will have an isAllergiesAndAdverseReactionsCollapsed value of true', () =>
      expect(component.vm.isAllergiesAndAdverseReactionsCollapsed).toBe(true));
  });
});
