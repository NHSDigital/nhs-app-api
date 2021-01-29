import viewedPreRegInstructions from '@/middleware/viewedPreRegInstructions';
import { BEGINLOGIN_NAME } from '@/router/names';
import { createRouteByNameObject } from '@/lib/utils';
import { createStore } from '../helpers';

jest.mock('@/lib/utils');

describe('viewedPreRegInstructions', () => {
  const query = 'example';
  const params = 'params';
  const createRouteByNameObjectResult = 'Return route';
  let next;
  let store;

  const callViewedPreRegInstructions = ({ isNativeApp, seen }) => {
    next = jest.fn();
    store = createStore({
      state: {
        device: {
          isNativeApp,
        },
        preRegistrationInformation: {
          seen,
        },
      },
    });

    viewedPreRegInstructions({ to: { query, params }, next, store });
  };

  beforeAll(() => {
    createRouteByNameObject.mockReturnValue(createRouteByNameObjectResult);
  });

  afterEach(() => {
    createRouteByNameObject.mockClear();
  });

  describe.each([
    ['native and seen previously', true, true],
    ['non-native and seen previously', false, true],
    ['non-native and not seen previously', false, true],
  ])('%s', (_, isNativeApp, seen) => {
    beforeEach(() => {
      callViewedPreRegInstructions({ isNativeApp, seen });
    });

    it('will dispatch `preRegistrationInformation/sync`', () => {
      expect(store.dispatch).toBeCalledWith('preRegistrationInformation/sync');
    });

    it('will create begin login route', () => {
      expect(createRouteByNameObject).toBeCalledWith({
        name: BEGINLOGIN_NAME,
        query,
        params,
        store,
      });
    });

    it('will call next with create begin login route result', () => {
      expect(next).toBeCalledWith(createRouteByNameObjectResult);
    });
  });

  describe('native and not seen previously', () => {
    beforeEach(() => {
      callViewedPreRegInstructions({ isNativeApp: true, seen: false });
    });

    it('will dispatch `preRegistrationInformation/sync`', () => {
      expect(store.dispatch).toBeCalledWith('preRegistrationInformation/sync');
    });

    it('will call next', () => {
      expect(next).toBeCalled();
    });
  });
});
