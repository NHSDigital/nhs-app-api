import actions from '@/store/modules/gpMessages/actions';

describe('gpMessages store actions', () => {
  const getMessageResponse = {
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
  };
  const mockMessages = { messageSummaries: [{ id: '1', subject: 'subject' }] };
  const mockUpdateStatus = { messageReadStateUpdateStatus: 'Updated' };
  const mockDelete = { isDeleted: true };
  let commit;
  let dispatch;
  const id = '1';
  const recipient = 'test';

  const state = {
    selectedMessageId: '1',
  };

  const store = () => ({
    app: { $http: { getV1PatientMessages: jest.fn().mockResolvedValue(mockMessages),
      getV1PatientMessagesById: jest.fn()
        .mockImplementation(() => Promise.resolve(getMessageResponse)),
      putV1PatientMessagesUpdateReadStatus: jest.fn()
        .mockImplementation(() => Promise.resolve(mockUpdateStatus)),
      deleteV1PatientMessagesById: jest.fn()
        .mockImplementation(() => Promise.resolve(mockDelete)) } },
    dispatch,
    state,
  });

  beforeEach(() => {
    commit = jest.fn();
    dispatch = jest.fn();
  });

  describe('loadMessages', () => {
    beforeEach(async () => {
      await actions.loadMessages.call(store(), { commit });
    });

    it('will not clear api errors and commit the set summaries mutation passing the api response', () => {
      expect(dispatch).not.toHaveBeenCalledWith('errors/clearAllApiErrors');
      expect(commit).not.toHaveBeenCalledWith('LOADED_MESSAGES', false);
      expect(commit).toHaveBeenCalledWith('SET_SUMMARIES', [{ id: '1', subject: 'subject' }]);
      expect(commit).toHaveBeenCalledWith('LOADED_MESSAGES', true);
    });

    describe('clearApiErrors param is set to true', () => {
      beforeEach(async () => {
        await actions.loadMessages.call(store(), { commit }, { clearApiError: true });
      });

      it('will clear api errors and set loaded false then set summaries with the api response', () => {
        expect(dispatch).toHaveBeenCalledWith('errors/clearAllApiErrors');
        expect(commit).toHaveBeenCalledWith('LOADED_MESSAGES', false);
        expect(commit).toHaveBeenCalledWith('SET_SUMMARIES', [{ id: '1', subject: 'subject' }]);
        expect(commit).toHaveBeenCalledWith('LOADED_MESSAGES', true);
      });
    });
  });

  describe('loadMessage', () => {
    beforeEach(async () => {
      await actions.loadMessage.call(store(), { commit }, id, false);
    });

    it('will set loaded message to true`', () => {
      expect(commit).toBeCalledWith('LOADED_MESSAGE', true);
    });

    it('will commit endpoint response to `SET_DETAILS`', () => {
      expect(commit).toHaveBeenCalledWith('SET_DETAILS', getMessageResponse);
    });
  });

  describe('loadMessage clearApiErrors set to true', () => {
    beforeEach(async () => {
      await actions.loadMessage.call(store(), { commit }, { id, clearApiError: true });
    });

    it('will clear errors and reload details', () => {
      expect(dispatch).toHaveBeenCalledWith('errors/clearAllApiErrors');
      expect(commit).toBeCalledWith('LOADED_MESSAGE', false);
      expect(commit).toHaveBeenCalledWith('SET_DETAILS', getMessageResponse);
      expect(commit).toBeCalledWith('LOADED_MESSAGE', true);
    });
  });

  describe('setSelectedMessageID', () => {
    beforeEach(async () => {
      await actions.setSelectedMessageID({ commit }, id);
    });

    it('will commit endpoint response to `SET_SELECTED_MESSAGE_ID`', () => {
      expect(commit).toHaveBeenCalledWith('SET_SELECTED_MESSAGE_ID', '1');
    });
  });

  describe('setSelectedRecipient', () => {
    beforeEach(async () => {
      await actions.setSelectedRecipient({ commit }, recipient);
    });

    it('will commit endpoint response to `SET_SELECTED_MESSAGE_RECIPIENT`', () => {
      expect(commit).toHaveBeenCalledWith('SET_SELECTED_MESSAGE_RECIPIENT', 'test');
    });
  });

  describe('clear', () => {
    beforeEach(async () => {
      await actions.clear({ commit });
    });

    it('will commit clear on destroy of page', () => {
      expect(commit).toHaveBeenCalledWith('CLEAR');
    });
  });

  describe('clearErrorsAndLoadDetails', () => {
    beforeEach(async () => {
      await actions.clearErrorsAndLoadDetails.call(store(), { state });
    });

    it('will dispatch to load message', () => {
      expect(dispatch).toHaveBeenCalledWith('gpMessages/loadMessage', { id, clearApiError: true });
    });
  });

  describe('updateReadStatusAsRead', () => {
    let currentStore;

    beforeEach(async () => {
      currentStore = store();

      await actions.updateReadStatusAsRead.call(currentStore, { commit, state });
    });

    it('will call API to mark message as read', () => {
      expect(currentStore.app.$http.putV1PatientMessagesUpdateReadStatus).toHaveBeenCalledWith({
        updateMessageReadStatusRequestBody: {
          messageId: '1',
          messageReadState: 'Read',
        },
      });
    });
  });

  describe('deleteMessage', () => {
    beforeEach(async () => {
      await actions.deleteMessage.call(store(), { commit }, '1');
    });

    it('will dispatch to set deleted', () => {
      expect(commit).toHaveBeenCalledWith('SET_DELETED', true);
    });
  });

  describe('clearSelectedRetainingId', () => {
    beforeEach(async () => {
      await actions.clearSelectedRetainingId.call(store(), { commit }, '1');
    });

    it('will dispatch to clear but retain id', () => {
      expect(commit).toHaveBeenCalledWith('CLEAR_SELECTED_MESSAGE_DETAILS');
    });
  });

  describe('retryMessageDelete', () => {
    beforeEach(async () => {
      await actions.retryMessageDelete.call(store(), { commit, state }, '1');
    });

    it('will dispatch to retry to delete message', () => {
      expect(dispatch).toHaveBeenCalledWith('gpMessages/deleteMessage', '1');
    });
  });
});
