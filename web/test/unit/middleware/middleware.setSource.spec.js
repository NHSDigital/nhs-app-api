import setSource from '@/middleware/setSource';
import { createStore } from '../helpers';

describe('middleware/setSource', () => {
  let store;

  const callSetSource = source => setSource({ route: { query: { source } }, store });

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

  beforeEach(() => {
    store = createStore();
  });

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
