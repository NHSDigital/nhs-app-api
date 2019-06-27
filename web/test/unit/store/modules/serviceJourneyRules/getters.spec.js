import getters from '@/store/modules/serviceJourneyRules/getters';
import { IM1_PROVIDER, INFORMATICA } from '@/store/modules/serviceJourneyRules/mutation-types';

describe('getters', () => {
  describe('im1Enabled', () => {
    const { im1Enabled } = getters;
    let currentState;

    beforeEach(() => {
      currentState = {
        rules: {
          appointments: {
            provider: 'im1',
          },
        },
      };
    });

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
    let currentState;

    beforeEach(() => {
      currentState = {
        rules: {
          appointments: {
            provider: 'informatica',
          },
        },
      };
    });

    it('will be true if the appointments provider is Informatica', () => {
      currentState.rules.appointments.provider = INFORMATICA;
      expect(informaticaEnabled(currentState)).toBe(true);
    });

    it('will be false if the appointments provider is im1', () => {
      currentState.rules.appointments.provider = IM1_PROVIDER;
      expect(informaticaEnabled(currentState)).toBe(false);
    });
  });
});
