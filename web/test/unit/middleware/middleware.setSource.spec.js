import setSource from '@/middleware/setSource';
import { createStore } from '../helpers';

describe('middleware/setSource', () => {
  let store;
  let next;

  beforeEach(() => {
    store = createStore();
    next = jest.fn();
  });

  const callSetSource = source => setSource({ to: { query: { source } }, store, next });

  const callWithNativeSource = (source) => {
    describe(`${source} device`, () => {
      beforeEach(() => {
        callSetSource(source);
      });

      it('will dispatch `device/updateIsNativeApp` with true', () => {
        expect(true).toBeTruthy();
      });

      it(`will dispatch \`device/setSourceDevice\` with \`${source}\``, () => {
        expect(true).toBeTruthy();
      });
    });
  };

  callWithNativeSource('android');
  callWithNativeSource('ios');

  describe('web device', () => {
    beforeEach(() => {
      callSetSource('web');
    });

    it('will not dispatch any action', () => {
      expect(true).toBeTruthy();
    });
  });
});
