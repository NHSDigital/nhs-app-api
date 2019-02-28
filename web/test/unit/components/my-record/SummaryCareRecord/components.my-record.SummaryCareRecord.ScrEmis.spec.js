import ScrEmis from '@/components/my-record/SummaryCareRecord/ScrEMIS';
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
  shallowMount(ScrEmis, { $store, propsData: createPropsData() });

describe('ScrEmis', () => {
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

    it('will have an isAcuteMedicationsCollapsed value of false', () => {
      expect(component.vm.isAcuteMedicationsCollapsed).toBe(false);
    });
    it('will have an isCurrentRepeatMedicationsCollapsed value of false', () => {
      expect(component.vm.isCurrentRepeatMedicationsCollapsed).toBe(false);
    });

    it('will have an isDiscontinuedRepeatMedicationsCollapsed value of false', () => {
      expect(component.vm.isDiscontinuedRepeatMedicationsCollapsed).toBe(false);
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

    it('will have an isAcuteMedicationsCollapsed value of true', () =>
      expect(component.vm.isAcuteMedicationsCollapsed).toBe(true));

    it('will have an isCurrentRepeatMedicationsCollapsed value of true', () =>
      expect(component.vm.isCurrentRepeatMedicationsCollapsed).toBe(true));

    it('will have an isDiscontinuedRepeatMedicationsCollapsed value of true', () =>
      expect(component.vm.isDiscontinuedRepeatMedicationsCollapsed).toBe(true));
  });
});
