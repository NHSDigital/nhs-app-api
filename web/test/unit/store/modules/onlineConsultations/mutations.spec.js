import mutations from '@/store/modules/onlineConsultations/mutations';
import { initialState } from '@/store/modules/onlineConsultations/mutation-types';
import each from 'jest-each';

let state;

describe('online consultations store mutations', () => {
  beforeEach(() => {
    state = initialState();
  });

  describe('simple mutations', () => {
    each([{
      mutation: 'SET_SESSION_ID',
      stateProp: 'sessionId',
      parameter: 'session-id',
    }, {
      mutation: 'SET_ANSWER',
      stateProp: 'answer',
      parameter: 'this is an answer',
    }, {
      mutation: 'SET_QUESTION',
      stateProp: 'question',
      parameter: { text: 'question text' },
    }, {
      mutation: 'SET_STATUS',
      stateProp: 'status',
      parameter: 'success',
    }, {
      mutation: 'SET_PREVIOUS_ROUTE',
      stateProp: 'previousRoute',
      parameter: '/more',
    }, {
      mutation: 'SET_CARE_PLANS',
      stateProp: 'carePlans',
      parameter: [4, 5, 6],
    }, {
      mutation: 'SET_REFERRAL_REQUESTS',
      stateProp: 'referralRequests',
      parameter: [1, 2, 3],
    }, {
      mutation: 'SET_ERROR',
      stateProp: 'error',
      parameter: true,
    }, {
      mutation: 'SET_ANSWER_IS_EMPTY',
      stateProp: 'answerIsEmpty',
      parameter: false,
    }]).it('will assign parameter to appropriate store value', ({ mutation, stateProp, parameter }) => {
      // Act
      mutations[mutation](state, parameter);

      // Assert
      expect(state[stateProp]).toEqual(parameter);
    });
  });

  describe('CLEAR', () => {
    describe('clearDemographicsConsent parameter is true', () => {
      it('will reset state to initialState', () => {
        // Arrange
        const expectedState = initialState();

        state = {
          sessionId: 1,
          status: 'success',
          question: {},
          dataRequirements: {},
          answer: 'answer',
          answerIsValid: true,
          answerIsEmpty: false,
          latestErrorMessage: 'message',
          validationError: true,
          validationErrorMessage: 'message',
          error: true,
          carePlans: [],
          referralRequests: [],
          isLoadingFile: true,
          demographicsConsentGiven: true,
          demographicsQuestionAnswered: true,
        };

        // Act
        mutations.CLEAR(state, true);

        // Assert
        expect(state).toEqual(expectedState);
      });
    });

    describe('clearDemographicsConsent parameter is false', () => {
      it('will reset state to initialState except demographics properties', () => {
        // Arrange
        const expectedState = initialState();
        expectedState.demographicsConsentGiven = true;
        expectedState.demographicsQuestionAnswered = true;

        state = {
          sessionId: 1,
          status: 'success',
          question: {},
          dataRequirements: {},
          answer: 'answer',
          answerIsValid: true,
          answerIsEmpty: false,
          latestErrorMessage: 'message',
          validationError: true,
          validationErrorMessage: 'message',
          error: true,
          carePlans: [],
          referralRequests: [],
          isLoadingFile: true,
          demographicsConsentGiven: true,
          demographicsQuestionAnswered: true,
        };

        // Act
        mutations.CLEAR(state, false);

        // Assert
        expect(state).toEqual(expectedState);
      });
    });
  });

  describe('SET_ANSWER_IS_VALID', () => {
    it('will assign appropriate properties to error state', () => {
      // Arrange
      const expectedAnswerIsValid = true;
      const expectedLatestErrorMessage = 'message';
      const expectedAnswerIsEmpty = false;

      // Act
      mutations.SET_ANSWER_IS_VALID(state, {
        isValid: expectedAnswerIsValid,
        message: expectedLatestErrorMessage,
        isEmpty: expectedAnswerIsEmpty,
      });

      // Assert
      expect(state.answerIsValid).toEqual(expectedAnswerIsValid);
      expect(state.latestErrorMessage).toEqual(expectedLatestErrorMessage);
      expect(state.answerIsEmpty).toEqual(expectedAnswerIsEmpty);
    });
  });

  describe('SET_VALIDATION_ERROR', () => {
    it('will set validation state based on existing error state', () => {
      // Arrange
      state.answerIsValid = true;
      state.latestErrorMessage = 'message';
      const expectedValidationError = false;
      const expectedValidationErrorMessage = 'message';

      // Act
      mutations.SET_VALIDATION_ERROR(state);

      // Assert
      expect(state.validationError).toEqual(expectedValidationError);
      expect(state.validationErrorMessage).toEqual(expectedValidationErrorMessage);
    });
  });

  describe('FILE_LOADING', () => {
    it('will set isLoadingFile state to true', () => {
      // Arrange
      state.isLoadingFile = false;
      const expectedIsLoadingFile = true;

      // Act
      mutations.FILE_LOADING(state);

      // Assert
      expect(state.isLoadingFile).toEqual(expectedIsLoadingFile);
    });
  });

  describe('FILE_LOAD_COMPLETE', () => {
    it('will set isLoadingFile state to false', () => {
      // Arrange
      state.isLoadingFile = true;
      const expectedIsLoadingFile = false;

      // Act
      mutations.FILE_LOAD_COMPLETE(state);

      // Assert
      expect(state.isLoadingFile).toEqual(expectedIsLoadingFile);
    });
  });
});
