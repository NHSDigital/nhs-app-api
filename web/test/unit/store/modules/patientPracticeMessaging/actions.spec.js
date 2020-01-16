import actions from '@/store/modules/patientPracticeMessaging/actions';

describe('patient practice messaging store actions', () => {
  const getMessageResponse = {
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
  };
  const mockMessages = { messageSummaries: [{ id: '1', subject: 'subject' }] };
  const mockUpdateStatus = { messageReadStateUpdateStatus: 'Updated' };
  let commit;
  let dispatch;
  const id = 1;
  const recipient = 'test';

  const state = {
    selectedMessageId: 1,
  };

  const store = () => ({
    app: { $http: { getV1PatientMessages: jest.fn().mockResolvedValue(mockMessages),
      getV1PatientMessagesById: jest.fn()
        .mockImplementation(() => Promise.resolve(getMessageResponse)),
      postV1PatientMessagesUpdateReadStatus: jest.fn()
        .mockImplementation(() => Promise.resolve(mockUpdateStatus)) } },
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
        await actions.loadMessages.call(store(), { commit }, true);
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
      expect(commit).toHaveBeenCalledWith('SET_SELECTED_MESSAGE_ID', 1);
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

    it('will dispath to load message', () => {
      expect(dispatch).toHaveBeenCalledWith('patientPracticeMessaging/loadMessage', { id, clearApiError: true });
    });
  });

  describe('updateReadStatusAsRead', () => {
    beforeEach(async () => {
      await actions.updateReadStatusAsRead.call(store(), { commit, state });
    });

    it('will dispatch to load message', () => {
      expect(commit).toHaveBeenCalledWith('SET_STATUS_STATE', 'Updated');
    });
  });
});
