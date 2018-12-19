import session from '@/store/modules/session';

const {
  state,
} = session;

describe('state', () => {
  describe('initial state', () => {
    it('will have a lastCalledAt property', () => {
      expect(state()).toHaveProperty('lastCalledAt');
    });

    it('will have a durationSeconds property', () => {
      expect(state()).toHaveProperty('durationSeconds');
    });

    it('will have a gpOdsCode property', () => {
      expect(state()).toHaveProperty('gpOdsCode');
    });

    it('will have lastCalledAt set to undefined', () => {
      expect(state().lastCalledAt).toEqual(undefined);
    });

    it('will have durationSeconds set to undefined', () => {
      expect(state().durationSeconds).toEqual(undefined);
    });

    it('will have gpOdsCode set to undefined', () => {
      expect(state().gpOdsCode).toEqual(undefined);
    });
  });
});
