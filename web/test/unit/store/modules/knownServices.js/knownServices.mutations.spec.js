import mutations from '@/store/modules/knownServices/mutations';
import {
  initialState,
  LOADSERVICES,
} from '@/store/modules/knownServices/mutation-types';

describe('known services mutations', () => {
  let state;
  let urls;

  beforeEach(() => {
    urls = ['url'];

    state = {
      record: initialState(),
    };
  });

  describe('LOADSERVICES', () => {
    beforeEach(() => {
      mutations[LOADSERVICES](state, urls);
    });

    it('will set the known services', () => {
      expect(state.knownServices).toEqual(urls);
    });

    it('will set the loaded state', () => {
      expect(state.isLoaded).toEqual(true);
    });
  });
});
