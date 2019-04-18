import getters from '@/store/modules/nominatedPharmacy/getters';

describe('getters', () => {
  describe('hasNoNominatedPharmacy', () => {
    const { hasNoNominatedPharmacy } = getters;

    describe('nominated pharmacy is present', () => {
      let currentState;

      beforeEach(() => {
        currentState = {
          pharmacy: {
            pharmacyName: 'Boots',
          },
        };
      });

      it('will be false if pharmacy is nominated', () => {
        expect(hasNoNominatedPharmacy(currentState)).toEqual(false);
      });
    });

    describe('nominated pharmacy is not present', () => {
      let currentState;

      beforeEach(() => {
        currentState = {
          pharmacy: {
            pharmacyName: undefined,
          },
        };
      });

      it('will be true if pharmacy is not nominated', () => {
        expect(hasNoNominatedPharmacy(currentState)).toEqual(true);
      });
    });
  });
});
