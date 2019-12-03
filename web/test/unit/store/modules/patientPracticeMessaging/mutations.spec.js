import mutations from '@/store/modules/patientPracticeMessaging/mutations';


describe('patient practice messaging store mutations', () => {
  let state;

  beforeEach(() => {
    state = {};
  });

  describe('SET_SUMMARIES', () => {
    describe('data is undefined', () => {
      it('will set messageSummaries equal to an empty array', () => {
        mutations.SET_SUMMARIES(state, undefined);

        expect(state.messageSummaries).toEqual([]);
      });
    });

    describe('data is not undefined', () => {
      it('will set messageSummaries equal to the data parameter', () => {
        const expectedMessageSummaries = [{ id: '1', subject: 'subject' }];

        mutations.SET_SUMMARIES(state, expectedMessageSummaries);

        expect(state.messageSummaries).toEqual(expectedMessageSummaries);
      });
    });
  });
});
