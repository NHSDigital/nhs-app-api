import actions from '@/store/modules/onlineConsultations/actions';
import {
  CLEAR,
  UPDATE_REQUEST_ID,
  SET_STATUS,
  SET_DATA_REQUIREMENTS,
  SET_SESSION_ID,
  SET_QUESTION,
  SET_CARE_PLANS,
  SET_REFERRAL_REQUESTS,
} from '@/store/modules/onlineConsultations/mutation-types';
import getParameters from '@/lib/online-consultations/mappers/parameters';
import { getDataRequirements, getSessionId, getQuestionnaireItem, getCarePlansAndReferralRequests } from '@/lib/online-consultations/mappers/response';
import getQuestion from '@/lib/online-consultations/mappers/item';
import each from 'jest-each';

jest.mock('@/lib/online-consultations/mappers/parameters');
jest.mock('@/lib/online-consultations/mappers/response');
jest.mock('@/lib/online-consultations/mappers/item');

const { getServiceDefinition, evaluateServiceDefinition } = actions;

const commit = jest.fn();
const store = {
  app: {
    $cdsApi: {
      postFhirServiceDefinitionEvaluate: jest.fn(),
      getFhirServiceDefinition: jest.fn(),
    },
  },
  dispatch: jest.fn(),
};
const state = {
  test: 'state',
};
const rootState = {
  session: {
    gpOdsCode: 'A29928',
  },
};

describe('online consultations store actions', () => {
  beforeEach(() => {
    commit.mockClear();
    store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockClear();
    store.app.$cdsApi.getFhirServiceDefinition.mockClear();
    store.dispatch.mockClear();
  });

  describe('getServiceDefinition', () => {
    let request;

    beforeEach(() => {
      request = { serviceDefinitionId: 'GEC_ADM' };
    });

    afterEach(() => {
      expect(commit).toHaveBeenCalledWith(UPDATE_REQUEST_ID);
    });

    describe('attempted get service definition is rejected', () => {
      it('will dispatch clearAndSetError', () => {
        // Arrange
        store.app.$cdsApi.getFhirServiceDefinition.mockImplementation(
          () => Promise.reject(),
        );

        // Act
        return getServiceDefinition
          .call(store, { commit })
          .then(() => {
            // Assert
            const { getFhirServiceDefinition } = store.app.$cdsApi;
            expect(getFhirServiceDefinition).toHaveBeenCalledWith(request);
            expect(getFhirServiceDefinition).toHaveBeenCalledTimes(1);
            expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
            expect(store.dispatch).toHaveBeenCalledTimes(1);
          });
      });
    });

    describe('attempted get service definition is resolved', () => {
      afterEach(() => {
        expect(commit).toHaveBeenNthCalledWith(1, CLEAR);
      });

      describe('response is undefined', () => {
        it('will dispatch clearAndSetError', () => {
          // Arrange
          store.app.$cdsApi.getFhirServiceDefinition.mockImplementation(
            () => Promise.resolve(undefined),
          );

          // Act
          return getServiceDefinition
            .call(store, { commit })
            .then(() => {
              // Assert
              expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
              expect(store.dispatch).toHaveBeenCalledTimes(1);
            });
        });
      });

      describe('getDataRequirements returns undefined', () => {
        it('will dispatch clearAndSetError', () => {
          // Arrange
          store.app.$cdsApi.getFhirServiceDefinition.mockImplementation(
            () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
          );
          getDataRequirements.mockReturnValue(undefined);

          // Act
          return getServiceDefinition
            .call(store, { commit })
            .then(() => {
              // Assert
              expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
              expect(store.dispatch).toHaveBeenCalledTimes(1);
            });
        });
      });

      describe('getDataRequirements returns value', () => {
        it('will commit data requirements to store', () => {
          // Arrange
          const expectedDataRequirements = {
            questionnaireResponse: false,
            patient: false,
            organization: false,
          };
          getDataRequirements.mockReturnValue(expectedDataRequirements);

          store.app.$cdsApi.getFhirServiceDefinition.mockImplementation(
            () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
          );

          // Act
          return getServiceDefinition
            .call(store, { commit })
            .then(() => {
              // Assert
              expect(commit).toHaveBeenCalledWith(SET_DATA_REQUIREMENTS, expectedDataRequirements);
              expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
              expect(store.dispatch).toHaveBeenCalledTimes(1);
            });
        });

        describe('getDataRequirements.questionnaireResponse is true', () => {
          describe('question cannot be retrieved from response', () => {
            it('will dispatch clearAndSetError', () => {
              // Arrange
              const expectedDataRequirements = {
                questionnaireResponse: true,
                patient: false,
                organization: false,
              };
              getDataRequirements.mockReturnValue(expectedDataRequirements);
              getQuestion.mockReturnValue(undefined);

              // Act
              return getServiceDefinition
                .call(store, { commit })
                .then(() => {
                  // Assert
                  expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                  expect(store.dispatch).toHaveBeenCalledTimes(1);
                });
            });
          });

          describe('question successfully retrieved from response', () => {
            it('will commit question to store and set status to data-required', () => {
              // Arrange
              const expectedDataRequirements = {
                questionnaireResponse: true,
                patient: false,
                organization: false,
              };
              getDataRequirements.mockReturnValue(expectedDataRequirements);

              store.app.$cdsApi.getFhirServiceDefinition.mockImplementation(
                () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
              );

              const expectedQuestion = { type: 'text' };
              getQuestion.mockReturnValue(expectedQuestion);

              // Act
              return getServiceDefinition
                .call(store, { commit })
                .then(() => {
                  // Assert
                  expect(commit).toHaveBeenCalledWith(SET_STATUS, 'data-required');
                  expect(commit).toHaveBeenCalledWith(SET_QUESTION, expectedQuestion);
                  expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                });
            });
          });
        });
      });
    });
  });

  describe('evaluateServiceDefinition', () => {
    describe('unable to get parameters from state', () => {
      it('will return undefined, dispatch clearAndSetError and not request evaluation', () => {
        // Arrange
        getParameters.mockReturnValue(undefined);

        // Act
        const result = evaluateServiceDefinition.call(store, { commit, state, rootState });

        // Assert
        expect(result).toBeUndefined();
        expect(getParameters).toHaveBeenCalledWith(state, rootState);
        expect(getParameters).toHaveBeenCalledTimes(1);
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
        expect(store.dispatch).toHaveBeenCalledTimes(1);
        expect(store.app.$cdsApi.postFhirServiceDefinitionEvaluate).not.toHaveBeenCalled();
      });
    });

    describe('successfully get parameters from state', () => {
      let parameters;
      let request;

      beforeAll(() => {
        parameters = {
          parameters: 'test',
        };
        request = {
          parameters,
          serviceDefinitionId: 'GEC_ADM',
        };
        getParameters.mockClear();
        getParameters.mockReturnValue(parameters);
      });

      afterEach(() => {
        expect(commit).toHaveBeenCalledWith(UPDATE_REQUEST_ID);
      });

      describe('attempted evaluation is rejected', () => {
        it('will dispatch clearAndSetError', () => {
          // Arrange
          store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockImplementation(
            () => Promise.reject(),
          );

          // Act
          return evaluateServiceDefinition
            .call(store, { commit, state, rootState })
            .then(() => {
              // Assert
              const { postFhirServiceDefinitionEvaluate } = store.app.$cdsApi;
              expect(postFhirServiceDefinitionEvaluate).toHaveBeenCalledWith(request);
              expect(postFhirServiceDefinitionEvaluate).toHaveBeenCalledTimes(1);
              expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
              expect(store.dispatch).toHaveBeenCalledTimes(1);
            });
        });
      });

      describe('attempted evaluation is resolved', () => {
        afterEach(() => {
          expect(commit).toHaveBeenNthCalledWith(1, CLEAR);
        });

        describe('response is undefined', () => {
          it('will dispatch clearAndSetError', () => {
            // Arrange
            store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockImplementation(
              () => Promise.resolve(undefined),
            );

            // Act
            return evaluateServiceDefinition
              .call(store, { commit, state, rootState })
              .then(() => {
                // Assert
                expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                expect(store.dispatch).toHaveBeenCalledTimes(1);
              });
          });
        });

        describe('response status is undefined or not recognised', () => {
          each([
            undefined,
            'unknown-status',
          ]).it('will dispatch clearAndSetError', (status) => {
            // Arrange
            store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockImplementation(
              () => Promise.resolve({ status }),
            );

            // Act
            return evaluateServiceDefinition
              .call(store, { commit, state, rootState })
              .then(() => {
                // Assert
                expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                expect(store.dispatch).toHaveBeenCalledTimes(1);
                expect(commit).toHaveBeenCalledWith(SET_STATUS, status);
              });
          });
        });

        describe('response status is recognised', () => {
          describe('data-required', () => {
            beforeEach(() => {
              getSessionId.mockClear();
              getQuestion.mockClear();
              getQuestionnaireItem.mockClear();
            });

            describe('cannot retrieve question or session id from response', () => {
              each([{
                sessionId: undefined,
                question: {},
              }, {
                sessionId: 'session-id',
                question: undefined,
              }, {
                sessionId: undefined,
                question: undefined,
              }]).it('will dispatch clearAndSetError', ({ sessionId, question }) => {
                // Arrange
                const expectedQuestionnaireItem = { item: 'value' };
                const expectedResponse = { status: 'data-required' };
                store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockImplementation(
                  () => Promise.resolve(expectedResponse),
                );
                getSessionId.mockReturnValue(sessionId);
                getQuestion.mockReturnValue(question);
                getQuestionnaireItem.mockReturnValue(expectedQuestionnaireItem);

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState })
                  .then(() => {
                    // Assert
                    expect(getSessionId).toHaveBeenCalledWith(expectedResponse);
                    expect(getQuestionnaireItem).toHaveBeenCalledWith(expectedResponse);
                    expect(getQuestion).toHaveBeenCalledWith(expectedQuestionnaireItem);
                    expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                    expect(store.dispatch).toHaveBeenCalledTimes(1);
                  });
              });
            });

            describe('retrieves session id and question from response', () => {
              it('will commit session id and question to store', () => {
                // Arrange
                const expectedSessionId = 'session-id';
                const expectedQuestion = { text: 'question' };
                const expectedResponse = { status: 'data-required' };
                const expectedQuestionnaireItem = { item: 'value' };
                store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockImplementation(
                  () => Promise.resolve(expectedResponse),
                );
                getSessionId.mockReturnValue(expectedSessionId);
                getQuestion.mockReturnValue(expectedQuestion);
                getQuestionnaireItem.mockReturnValue(expectedQuestionnaireItem);

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState })
                  .then(() => {
                    // Assert
                    expect(getSessionId).toHaveBeenCalledWith(expectedResponse);
                    expect(getQuestionnaireItem).toHaveBeenCalledWith(expectedResponse);
                    expect(getQuestion).toHaveBeenCalledWith(expectedQuestionnaireItem);
                    expect(commit).toHaveBeenCalledWith(SET_SESSION_ID, expectedSessionId);
                    expect(commit).toHaveBeenCalledWith(SET_QUESTION, expectedQuestion);
                    expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                  });
              });
            });
          });

          describe('success', () => {
            beforeEach(() => {
              getCarePlansAndReferralRequests.mockClear();
            });

            describe('retrieved actions from response is undefined', () => {
              it('will dispatch clearAndSetError', () => {
                // Arrange
                const expectedResponse = { status: 'success' };
                store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockImplementation(
                  () => Promise.resolve(expectedResponse),
                );
                getCarePlansAndReferralRequests.mockReturnValue(undefined);

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState })
                  .then(() => {
                    // Assert
                    expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                    expect(store.dispatch).toHaveBeenCalledTimes(1);
                  });
              });
            });

            describe('retrieves actions from response', () => {
              each([{
                carePlans: [],
                referralRequests: [],
              }, {
                carePlans: ['care', 'plan'],
                referralRequests: ['referral', 'request'],
              }, {
                carePlans: undefined,
                referralRequests: undefined,
              }]).it('will commit care plans and referral requests to store', ({ carePlans, referralRequests }) => {
                // Arrange
                const expectedResponse = { status: 'success' };
                store.app.$cdsApi.postFhirServiceDefinitionEvaluate.mockImplementation(
                  () => Promise.resolve(expectedResponse),
                );
                getCarePlansAndReferralRequests.mockReturnValue({ carePlans, referralRequests });

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState })
                  .then(() => {
                    // Assert
                    expect(getCarePlansAndReferralRequests).toHaveBeenCalledWith(expectedResponse);
                    expect(commit).toHaveBeenCalledWith(SET_CARE_PLANS, carePlans);
                    expect(commit).toHaveBeenCalledWith(SET_REFERRAL_REQUESTS, referralRequests);
                    expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                  });
              });
            });
          });
        });
      });
    });
  });
});
