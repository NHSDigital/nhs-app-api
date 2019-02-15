import { isDefault } from '@/lib/organ-donation/registration-comparison';

describe('organ donation registration comparison', () => {
  describe('isDefault', () => {
    let path;
    let state;

    beforeEach(() => {
      state = {
        organDonation: {
          registration: {
            decisionDetails: {
              all: '',
            },
          },
        },
      };
    });

    describe('full path', () => {
      beforeEach(() => {
        path = 'organDonation.registration.decisionDetails.all';
      });

      it('will be true if the value at the specified absolute path matches the initial state', () => {
        expect(isDefault({ state, path })).toEqual(true);
      });

      it('will be false if the value at the specified absolute path matches the initial state', () => {
        state.organDonation.registration.decisionDetails.all = 'boo';
        expect(isDefault({ state, path })).toEqual(false);
      });
    });
  });
});
