import actions from '@/store/modules/messaging/actions';
import {
  ADD_ERROR,
  CLEAR,
  INIT,
  LOADED,
  LOADED_MESSAGE,
  LOADED_SENDERS,
  SET_HAS_UNREAD,
  SET_SENDER,
} from '@/store/modules/messaging/mutation-types';

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
      getV1ApiUsersMeMessagesByMessageid:
        jest.fn().mockImplementation(() => Promise.resolve(getResponse)),
    };
    actions.app = {
      get $http() {
        return $http;
      },
    };
    actions.dispatch = dispatch;
  });

  describe('clear', () => {
    beforeEach(() => {
      actions.clear({ commit });
    });

    it('will commit `CLEAR`', () => {
      expect(commit).toBeCalledWith(CLEAR);
    });
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
      expect($http.getV1ApiUsersMeMessages).toBeCalledWith({ summary: true, ignoreError: true });
    });

    it('will commit endpoint response to `LOADED`', () => {
      expect(commit).toBeCalledWith(LOADED, getResponse);
    });

    it('will commit endpoint response to `SET_HAS_UNREAD`', () => {
      expect(commit).toBeCalledWith(SET_HAS_UNREAD, getResponse);
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
        expect($http.getV1ApiUsersMeMessages).toBeCalledWith({ sender, ignoreError: true });
      });
    });

    describe('on error', () => {
      beforeEach(async () => {
        const error = { response: { status: 502 } };
        commit.mockClear();
        dispatch.mockClear();
        $http.getV1ApiUsersMeMessages = jest.fn().mockImplementation(() => Promise.reject(error));
        await actions.load({ commit });
      });

      it('will call the `getV1ApiUsersMeMessages`', () => {
        expect($http.getV1ApiUsersMeMessages).toBeCalled();
      });

      it('will commit `ADD_ERROR`', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, { status: 502, serviceDeskReference: '' });
      });

      it('will not commit `LOADED`', () => {
        expect(commit).not.toBeCalledWith(LOADED, expect.any(String));
      });

      it('will not commit `SET_HAS_UNREAD`', () => {
        expect(commit).not.toBeCalledWith(SET_HAS_UNREAD, expect.any(String));
      });

      it('will dispatch `device/unlockNavBar` event', () => {
        expect(dispatch).toBeCalledWith('device/unlockNavBar');
      });
    });
  });

  describe('loadMessage', () => {
    const messageId = 'messageId';

    beforeEach(async () => {
      await actions.loadMessage({ commit }, { messageId });
    });

    it('will call the `getV1ApiUsersMeMessagesByMessageid` endpoint with message id', () => {
      expect($http.getV1ApiUsersMeMessagesByMessageid)
        .toBeCalledWith({ messageId, ignoreError: true });
    });

    it('will commit endpoint response to `LOADED_MESSAGE`', () => {
      expect(commit).toBeCalledWith(LOADED_MESSAGE, getResponse);
    });

    describe('on error', () => {
      beforeEach(async () => {
        const error = { response: { status: 502 } };
        commit.mockClear();
        $http.getV1ApiUsersMeMessagesByMessageid =
          jest.fn().mockImplementation(() => Promise.reject(error));
        await actions.loadMessage({ commit }, { messageId });
      });

      it('will call the `getV1ApiUsersMeMessagesByMessageid` endpoint with message id', () => {
        expect($http.getV1ApiUsersMeMessagesByMessageid)
          .toBeCalledWith({ messageId, ignoreError: true });
      });

      it('will commit `ADD_ERROR`', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, { status: 502, serviceDeskReference: '' });
      });

      it('will not commit `LOADED_MESSAGE`', () => {
        expect(commit).not.toBeCalledWith(LOADED_MESSAGE, expect.any(String));
      });
    });
  });

  describe('loadSenders', () => {
    const response = { senders: 'test sender' };

    beforeEach(async () => {
      $http.getV1ApiUsersMeMessagesSenders =
        jest.fn().mockImplementation(() => Promise.resolve(response));
      await actions.loadSenders({ commit });
    });

    it('will call the `getV1ApiUsersMeMessagesSenders` endpoint', () => {
      expect($http.getV1ApiUsersMeMessagesSenders).toBeCalledWith({ ignoreError: true });
    });

    it('will commit endpoint response to `LOADED_SENDERS`', () => {
      expect(commit).toBeCalledWith(LOADED_SENDERS, 'test sender');
    });

    describe('on error', () => {
      beforeEach(async () => {
        const error = { response: { status: 502 } };
        commit.mockClear();
        $http.getV1ApiUsersMeMessagesSenders =
          jest.fn().mockImplementation(() => Promise.reject(error));
        await actions.loadSenders({ commit });
      });

      it('will call the `getV1ApiUsersMeMessagesSenders` endpoint', () => {
        expect($http.getV1ApiUsersMeMessagesSenders).toBeCalledWith({ ignoreError: true });
      });

      it('will commit `ADD_ERROR`', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, { status: 502, serviceDeskReference: '' });
      });

      it('will not commit `LOADED_SENDERS`', () => {
        expect(commit).not.toBeCalledWith(LOADED_SENDERS, expect.any(String));
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
