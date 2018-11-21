import ScrEmis from '@/components/my-record/SummaryCareRecord/ScrEMIS';
import { shallowMount } from '../../../../helpers';

const createPropsData = () => ({
  record: {
    medications: {
      data: {},
    },
  },
});

describe('ScrEmis', () => {
  describe('running on server', () => {
    let component;

    beforeEach(() => {
      process.client = false;
      component = shallowMount(ScrEmis, { propsData: createPropsData() });
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

    beforeEach(() => {
      process.client = true;
      component = shallowMount(ScrEmis, { propsData: createPropsData() });
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
