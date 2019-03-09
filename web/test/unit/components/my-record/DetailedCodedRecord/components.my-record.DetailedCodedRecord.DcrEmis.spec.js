import DcrEmis from '@/components/my-record/DetailedCodedRecord/DcrEMIS';
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
  shallowMount(DcrEmis, { $store, propsData: createPropsData() });


describe('DcrEmis', () => {
  describe('running on server', () => {
    let $store;
    let component;

    beforeEach(() => {
      process.client = false;
      $store = createStore({ state: createState() });
      component = createComponent({ $store });
    });

    it('will have an isImmunisationsCollapsed value of false', () =>
      expect(component.vm.isImmunisationsCollapsed).toBe(false));

    it('will have an isTestResultsCollapsed value of false', () =>
      expect(component.vm.isTestResultsCollapsed).toBe(false));

    it('will have an isProblemsCollapsed value of false', () =>
      expect(component.vm.isProblemsCollapsed).toBe(false));

    it('will have an isConsultationsCollapsed value of false', () =>
      expect(component.vm.isConsultationsCollapsed).toBe(false));
  });

  describe('running on client', () => {
    let component;
    let $store;

    beforeEach(() => {
      process.client = true;
      $store = createStore({ state: createState() });
      component = createComponent({ $store });
    });

    it('will have an isImmunisationsCollapsed value of true', () =>
      expect(component.vm.isImmunisationsCollapsed).toBe(true));

    it('will have an isTestResultsCollapsed value of true', () =>
      expect(component.vm.isTestResultsCollapsed).toBe(true));

    it('will have an isProblemsCollapsed value of true', () =>
      expect(component.vm.isProblemsCollapsed).toBe(true));

    it('will have an isConsultationsCollapsed value of true', () =>
      expect(component.vm.isConsultationsCollapsed).toBe(true));
  });
});
