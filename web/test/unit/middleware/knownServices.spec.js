import knownServices from '@/middleware/knownServices';
import { initialState } from '@/store/modules/knownServices/mutation-types';

describe('middleware/knownServices', () => {
  let store;
  const next = jest.fn();

  const callKnownServices
   = async ({ isLoggedIn, isLoaded }) => {
     store = {
       getters: {
         'session/isLoggedIn': () => isLoggedIn,
       },
       dispatch: jest.fn(),
       state: {
         knownServices: initialState(),
       },
     };

     store.state.knownServices.isLoaded = isLoaded;
     await knownServices({ next, store });
   };

  describe('not logged in', () => {
    beforeEach(async () => {
      await callKnownServices({
        isLoggedIn: false,
        isLoaded: false,
      });
    });

    it('will not dispatch `knownServices/load`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });

  describe('already loaded', () => {
    beforeEach(async () => {
      await callKnownServices({
        isLoggedIn: true,
        isLoaded: true,
      });
    });

    it('will not dispatch `knownServices/load`', () => {
      expect(store.dispatch).not.toBeCalled();
    });
  });

  describe('logged in and not loaded', () => {
    beforeEach(async () => {
      await callKnownServices({
        isLoggedIn: true,
        isLoaded: false,
      });
    });

    it('will dispatch `knownServices/load`', () => {
      expect(store.dispatch).toHaveBeenCalledWith('knownServices/load');
    });
  });
});
