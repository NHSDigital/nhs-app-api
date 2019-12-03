import actions from '@/store/modules/patientPracticeMessaging/actions';

describe('patient practice messaging store actions', () => {
  const mockMessages = { messageSummaries: [{ id: '1', subject: 'subject' }] };
  let commit;
  let dispatch;

  const store = () => ({
    app: { $http: { getV1PatientMessages: jest.fn().mockResolvedValue(mockMessages) } },
    dispatch,
  });

  beforeEach(() => {
    commit = jest.fn();
    dispatch = jest.fn();
  });

  describe('load', () => {
    beforeEach(async () => {
      await actions.load.call(store(), { commit });
    });

    it('will not clear api errors and commit the set summaries mutation passing the api response', () => {
      expect(dispatch).not.toHaveBeenCalledWith('errors/clearAllApiErrors');
      expect(commit).not.toHaveBeenCalledWith('LOADED', false);
      expect(commit).toHaveBeenCalledWith('SET_SUMMARIES', [{ id: '1', subject: 'subject' }]);
      expect(commit).toHaveBeenCalledWith('LOADED', true);
    });

    describe('clearApiErrors param is set to true', () => {
      beforeEach(async () => {
        await actions.load.call(store(), { commit }, true);
      });

      it('will clear api errors and set loaded false then set summaries with the api response', () => {
        expect(dispatch).toHaveBeenCalledWith('errors/clearAllApiErrors');
        expect(commit).toHaveBeenCalledWith('LOADED', false);
        expect(commit).toHaveBeenCalledWith('SET_SUMMARIES', [{ id: '1', subject: 'subject' }]);
        expect(commit).toHaveBeenCalledWith('LOADED', true);
      });
    });
  });
});
