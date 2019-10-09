import actions from '@/store/modules/messaging/actions';
import { INIT, LOADED } from '@/store/modules/messaging/mutation-types';

describe('messaging actions', () => {
  const getResponse = 'get test response';
  let $http;
  let commit;
  let dispatch;

  beforeEach(() => {
    commit = jest.fn();
    dispatch = jest.fn();
    $http = {
      getV1ApiUsersMeMessages: jest.fn().mockImplementation(() => Promise.resolve(getResponse)),
    };
    actions.app = {
      get $http() {
        return $http;
      },
    };
    actions.dispatch = dispatch;
  });

  describe('init', () => {
    beforeEach(() => {
      actions.init({ commit });
    });

    it('will commit `INIT`', () => {
      expect(commit).toBeCalledWith(INIT);
    });
  });

  describe('load', () => {
    beforeEach(async () => {
      await actions.load({ commit });
    });

    it('will call the `getV1ApiUsersMeMessages` endpoint', () => {
      expect($http.getV1ApiUsersMeMessages).toBeCalled();
      expect(dispatch).toHaveBeenCalledWith('device/unlockNavBar');
    });

    it('will commit endpoint response to `LOADED`', () => {
      expect(commit).toBeCalledWith(LOADED, getResponse);
      expect(dispatch).toHaveBeenCalledWith('device/unlockNavBar');
    });
  });
});
