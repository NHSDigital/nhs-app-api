import getters from '@/store/modules/serviceJourneyRules/getters';
import { initialState, IM1_PROVIDER, INFORMATICA } from '@/store/modules/serviceJourneyRules/mutation-types';

describe('getters', () => {
  let currentState;

  beforeEach(() => {
    currentState = initialState();
  });

  describe('im1Enabled', () => {
    const { im1Enabled } = getters;

    it('will be true if the appointments provider is im1', () => {
      currentState.rules.appointments.provider = IM1_PROVIDER;
      expect(im1Enabled(currentState)).toBe(true);
    });

    it('will be false if the appointments provider is Informatica', () => {
      currentState.rules.appointments.provider = INFORMATICA;
      expect(im1Enabled(currentState)).toBe(false);
    });
  });

  describe('informaticaEnabled', () => {
    const { informaticaEnabled } = getters;

    it('will be true if the appointments provider is Informatica', () => {
      currentState.rules.appointments.provider = INFORMATICA;
      expect(informaticaEnabled(currentState)).toBe(true);
    });

    it('will be false if the appointments provider is im1', () => {
      currentState.rules.appointments.provider = IM1_PROVIDER;
      expect(informaticaEnabled(currentState)).toBe(false);
    });
  });

  describe('nominatedPharmacyEnabled', () => {
    const { nominatedPharmacyEnabled } = getters;

    it('will be true if nominated pharmacy is true', () => {
      currentState.rules.nominatedPharmacy = true;
      expect(nominatedPharmacyEnabled(currentState)).toBe(true);
    });

    it('will be false if nominated pharmacy is false', () => {
      currentState.rules.nominatedPharmacy = false;
      expect(nominatedPharmacyEnabled(currentState)).toBe(false);
    });
  });
});
