import DcrEmis from '@/components/my-record/DetailedCodedRecord/DcrEMIS';
import { shallowMount } from '../../../helpers';

describe('DcrEmis', () => {
  describe('running on server', () => {
    let component;

    beforeEach(() => {
      process.client = false;
      component = shallowMount(DcrEmis);
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

    beforeEach(() => {
      process.client = true;
      component = shallowMount(DcrEmis);
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
