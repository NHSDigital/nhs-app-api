import actions from '@/store/modules/biometricBanner/actions';
import { SYNC, DISMISS } from '@/store/modules/biometricBanner/mutation-types';

const commit = jest.fn();

describe('actions', () => {
  describe('dismiss', () => {
    beforeEach(() => {
      actions.dismiss({ commit });
    });

    it('will call commit with DISMISS', () => {
      expect(commit).toBeCalledWith(DISMISS);
    });

    it('will call commit with SYNC', () => {
      expect(commit).toBeCalledWith(SYNC);
    });
  });

  describe('sync', () => {
    beforeEach(() => {
      actions.sync({ commit });
    });

    it('will call commit with SYNC', () => {
      expect(commit).toBeCalledWith(SYNC);
    });
  });
});
