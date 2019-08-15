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
        expect(store.dispatch).toBeCalledWith('device/updateIsNativeApp', true);
      });

      it(`will dispatch \`device/setSourceDevice\` with \`${source}\``, () => {
        expect(store.dispatch).toBeCalledWith('device/setSourceDevice', source);
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
      expect(store.dispatch).not.toBeCalled();
    });
  });
});
