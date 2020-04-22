import mutations from '@/store/modules/patientPracticeMessaging/mutations';
import buildMessageMetadata from '@/lib/patient-practice-messaging/build-message-metadata';

jest.mock('@/lib/patient-practice-messaging/build-message-metadata');

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

  describe('SET_DETAILS', () => {
    describe('with data', () => {
      const data = 'test state';

      beforeEach(() => {
        mutations.LOADED_MESSAGE(state, false);
        mutations.SET_DETAILS(state, data);
      });

      it('will set the message summaries state to the received value', () => {
        expect(state.selectedMessageDetails).toBe(data);
      });

      it('will call buildMessageMetadata with the current state', () => {
        expect(buildMessageMetadata).toHaveBeenCalledWith(state);
      });
    });

    describe('no data', () => {
      beforeEach(() => {
        mutations.LOADED_MESSAGE(state, false);
        mutations.SET_DETAILS(state, null);
      });

      it('will set the message details state to undefined', () => {
        expect(state.selectedMessageDetails).toEqual(undefined);
      });
    });
  });

  describe('SET_SELECTED_MESSAGE_ID', () => {
    describe('with data', () => {
      const id = '1';

      beforeEach(() => {
        mutations.SET_SELECTED_MESSAGE_ID(state, id);
      });

      it('will set the message summaries state to the received value', () => {
        expect(state.selectedMessageId).toBe(id);
      });
    });
  });
  describe('SET_SELECTED_MESSAGE_ID', () => {
    describe('with data', () => {
      const id = '1';

      beforeEach(() => {
        mutations.SET_SELECTED_MESSAGE_ID(state, id);
      });

      it('will set the message summaries state to the received value', () => {
        expect(state.selectedMessageId).toBe(id);
        mutations.CLEAR(state, id);
        expect(state.selectedMessageId).toBe(undefined);
      });
    });
  });
});
