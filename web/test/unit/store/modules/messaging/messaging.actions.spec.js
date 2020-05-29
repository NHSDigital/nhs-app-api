import actions from '@/store/modules/messaging/actions';
import { INIT, LOADED, SET_SENDER } from '@/store/modules/messaging/mutation-types';

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

    it('will call the `getV1ApiUsersMeMessages` endpoint with `summary=true`', () => {
      expect($http.getV1ApiUsersMeMessages).toBeCalledWith({ summary: true });
    });

    it('will commit endpoint response to `LOADED`', () => {
      expect(commit).toBeCalledWith(LOADED, getResponse);
    });

    it('will dispatch `device/unlockNavBar` event', () => {
      expect(dispatch).toBeCalledWith('device/unlockNavBar');
    });

    describe('with sender', () => {
      const sender = 'test sender';

      beforeEach(async () => {
        await actions.load({ commit }, { sender });
      });

      it('will call the `getV1ApiUsersMeMessages` endpoint with sender', () => {
        expect($http.getV1ApiUsersMeMessages).toBeCalledWith({ sender });
      });
    });
  });

  describe('selectSender', () => {
    const sender = 'test sender';

    beforeEach(() => {
      actions.selectSender({ commit }, sender);
    });

    it('will commit `SET_SENDER with passed value`', () => {
      expect(commit).toBeCalledWith(SET_SENDER, sender);
    });
  });
});
