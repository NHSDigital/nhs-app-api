import actions from '@/store/modules/messaging/actions';
import {
  ADD_ERROR,
  CLEAR,
  INIT,
  LOADED,
  LOADED_MESSAGE,
  LOADED_SENDERS,
  SET_HAS_UNREAD,
} from '@/store/modules/messaging/mutation-types';

describe('messaging actions', () => {
  const getResponse = 'get test response';
  let $http;
  let $httpV2;
  let commit;
  let dispatch;

  beforeEach(() => {
    commit = jest.fn();
    dispatch = jest.fn();
    $http = {
      getV1ApiUsersMeMessages:
        jest.fn().mockImplementation(() => Promise.resolve(getResponse)),
      getV1ApiUsersMeMessagesByMessageid:
        jest.fn().mockImplementation(() => Promise.resolve(getResponse)),
      patchV1ApiUsersMeMessagesByMessageid:
        jest.fn().mockImplementation(() => Promise.resolve(getResponse)),
    };
    $httpV2 = {};
    actions.app = {
      get $http() {
        return $http;
      },
      get $httpV2() {
        return $httpV2;
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
      await actions.load({ commit }, { senderId: 'fedcba-123124bcc-edaadc', sender: 'Test Sender' });
    });

    it('will call the `getV1ApiUsersMeMessages` endpoint adding ignoreError to the given request', () => {
      expect($http.getV1ApiUsersMeMessages).toBeCalledWith({ senderId: 'fedcba-123124bcc-edaadc', sender: 'Test Sender', ignoreError: true });
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

    beforeEach(() => {
      $httpV2.getV2ApiUsersMeMessagesSenders =
        jest.fn().mockImplementation(() => Promise.resolve(response));
    });

    describe('on success', () => {
      beforeEach(async () => {
        await actions.loadSenders({ commit });
      });

      it('will call the `getV2ApiusersMeMessagesSenders` endpoint', () => {
        expect($httpV2.getV2ApiUsersMeMessagesSenders).toBeCalledWith({ ignoreError: true });
      });

      it('will commit endpoint response to `LOADED_SENDERS`', () => {
        expect(commit).toBeCalledWith(LOADED_SENDERS, 'test sender');
      });
    });

    describe('on error', () => {
      beforeEach(async () => {
        const error = { response: { status: 502 } };
        $httpV2.getV2ApiUsersMeMessagesSenders =
          jest.fn().mockImplementation(() => Promise.reject(error));

        await actions.loadSenders({ commit });
      });

      it('will call the `getV2ApiUsersMeMessagesSenders` endpoint', () => {
        expect($httpV2.getV2ApiUsersMeMessagesSenders).toBeCalledWith({ ignoreError: true });
      });

      it('will commit `ADD_ERROR`', () => {
        expect(commit).toBeCalledWith(ADD_ERROR, { status: 502, serviceDeskReference: '' });
      });

      it('will not commit `LOADED_SENDERS`', () => {
        expect(commit).not.toBeCalledWith(LOADED_SENDERS, expect.any(String));
      });
    });
  });

  describe('recordMessageResponse', () => {
    it('will call the `patchV1ApiUsersMeMessagesByMessageid` endpoint when recordMessageResponse is invoked', async () => {
      const expectedPayload = {
        messageId: 123,
        patchMessageRequest: [{
          op: 'add',
          path: '/reply/response',
          value: 'user_response',
        }],
        ignoreError: true,
        ignoreLoading: true,
      };

      await actions.recordMessageResponse({ commit }, { messageId: 123, response: 'user_response' });

      expect($http.patchV1ApiUsersMeMessagesByMessageid).toBeCalledWith(expectedPayload);
    });
  });
});
