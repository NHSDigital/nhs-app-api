import actions from '@/store/modules/onlineConsultations/actions';
import {
  CLEAR,
  SET_STATUS,
  SET_DATA_REQUIREMENTS,
  SET_SESSION_ID,
  SET_QUESTION,
  SET_CARE_PLANS,
  SET_REFERRAL_REQUESTS,
  SET_ADMIN_PROVIDER_NAME,
  SET_ADVICE_PROVIDER_NAME,
  SET_CONDITIONS_LIST,
} from '@/store/modules/onlineConsultations/mutation-types';
import getParameters from '@/lib/online-consultations/mappers/parameters';
import { getDataRequirements, getSessionId, getQuestionnaire, getQuestionnaireId, getQuestionnaireItem, getCarePlansAndReferralRequests } from '@/lib/online-consultations/mappers/response';
import { getQuestion, getConditionsList } from '@/lib/online-consultations/mappers/item';
import getTCsAnswerForProvider from '@/lib/online-consultations/constants/termsConditionsAnswers';
import each from 'jest-each';

jest.mock('@/lib/online-consultations/mappers/parameters');
jest.mock('@/lib/online-consultations/mappers/response');
jest.mock('@/lib/online-consultations/mappers/item');
jest.mock('@/lib/online-consultations/constants/termsConditionsAnswers');

const { getServiceDefinition, evaluateServiceDefinition, setProviderNames } = actions;

const commit = jest.fn();
const store = {
  app: {
    $cdsApi: {
      postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate: jest.fn(),
      getFhirServiceDefinitionByProviderByServicedefinitionid: jest.fn(),
      getFhirServiceDefinitionProviderNameByProvider: jest.fn(),
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
  serviceJourneyRules: {
    rules: {
      cdssAdmin: {
        serviceDefinition: 'NHS_ADMIN',
        provider: 'stubs',
      },
      cdssAdvice: {
        serviceDefinition: 'NHS_ADVICE',
        conditionsServiceDefinition: 'NHS_CONDITION_LIST',
        provider: 'stubs',
      },
    },
  },
};

describe('online consultations store actions', () => {
  beforeEach(() => {
    commit.mockClear();
    store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate.mockClear();
    store.app.$cdsApi.getFhirServiceDefinitionByProviderByServicedefinitionid.mockClear();
    store.app.$cdsApi.getFhirServiceDefinitionProviderNameByProvider.mockClear();
    store.dispatch.mockClear();
  });

  describe('getServiceDefinition', () => {
    let serviceDefinitionId;
    let provider;
    let parameters;

    beforeAll(() => {
      serviceDefinitionId = 'NHS_ADMIN';
      provider = 'stubs';
      parameters = {
        serviceDefinitionId,
        provider,
      };
    });

    describe('attempted get service definition is rejected', () => {
      it('will dispatch clearAndSetError', () => {
        // Arrange
        store.app.$cdsApi.getFhirServiceDefinitionByProviderByServicedefinitionid
          .mockImplementation(
            () => Promise.reject(),
          );

        // Act
        return getServiceDefinition
          .call(store, { rootState, commit }, parameters)
          .then(() => {
            // Assert
            const { getFhirServiceDefinitionByProviderByServicedefinitionid } = store.app.$cdsApi;
            expect(getFhirServiceDefinitionByProviderByServicedefinitionid)
              .toHaveBeenCalledWith(parameters);
            expect(getFhirServiceDefinitionByProviderByServicedefinitionid)
              .toHaveBeenCalledTimes(1);
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
          store.app.$cdsApi.getFhirServiceDefinitionByProviderByServicedefinitionid
            .mockImplementation(
              () => Promise.resolve(undefined),
            );

          // Act
          return getServiceDefinition
            .call(store, { rootState, commit }, parameters)
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
          store.app.$cdsApi.getFhirServiceDefinitionByProviderByServicedefinitionid
            .mockImplementation(
              () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
            );
          getDataRequirements.mockReturnValue(undefined);

          // Act
          return getServiceDefinition
            .call(store, { rootState, commit }, parameters)
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

          store.app.$cdsApi.getFhirServiceDefinitionByProviderByServicedefinitionid
            .mockImplementation(
              () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
            );

          // Act
          return getServiceDefinition
            .call(store, { rootState, commit }, parameters)
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
                .call(store, { rootState, commit }, parameters)
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

              store.app.$cdsApi.getFhirServiceDefinitionByProviderByServicedefinitionid
                .mockImplementation(
                  () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
                );

              const expectedQuestion = { type: 'text' };
              getQuestion.mockReturnValue(expectedQuestion);

              // Act
              return getServiceDefinition
                .call(store, { rootState, commit }, parameters)
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
        const result = evaluateServiceDefinition.call(store, { commit, state, rootState }, {
          provider: 'stubs',
          serviceDefinitionId: 'NHS_ADMIN',
        });

        // Assert
        expect(result).toBeUndefined();
        expect(getParameters).toHaveBeenCalledWith(state, rootState, undefined);
        expect(getParameters).toHaveBeenCalledTimes(1);
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
        expect(store.dispatch).toHaveBeenCalledTimes(1);
        expect(store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate)
          .not.toHaveBeenCalled();
      });
    });

    describe('successfully get parameters from state', () => {
      let parameters;
      let request;
      let actionParams;

      beforeEach(() => {
        parameters = {
          parameter: ['test'],
        };
        request = {
          parameters,
          serviceDefinitionId: 'NHS_ADMIN',
          provider: 'stubs',
          addJavascriptDisabledHeader: true,
        };
        actionParams = { provider: 'stubs', serviceDefinitionId: 'NHS_ADMIN', addJavascriptDisabledHeader: true, answeringConditionsQuestion: false };

        getParameters.mockClear();
        getParameters.mockReturnValue(parameters);
      });

      describe('action called with addJavascriptDisabledHeader set to true', () => {
        it('will include addJavascriptDisabledHeader in post parameter', () => {
          // Arrange
          store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
            .mockImplementation(
              () => Promise.reject(),
            );

          // Act
          return evaluateServiceDefinition
            .call(store, { commit, state, rootState }, actionParams)
            .then(() => {
              // Assert
              const {
                postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate,
              } = store.app.$cdsApi;
              expect(postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate)
                .toHaveBeenCalledWith(request);
              expect(postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate)
                .toHaveBeenCalledTimes(1);
              expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
              expect(store.dispatch).toHaveBeenCalledTimes(1);
            });
        });
      });

      describe('attempted evaluation is rejected', () => {
        it('will dispatch clearAndSetError', () => {
          // Arrange
          store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
            .mockImplementation(
              () => Promise.reject(),
            );

          // Act
          return evaluateServiceDefinition
            .call(store, { commit, state, rootState }, actionParams)
            .then(() => {
              // Assert
              const {
                postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate,
              } = store.app.$cdsApi;
              expect(postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate)
                .toHaveBeenCalledWith(request);
              expect(postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate)
                .toHaveBeenCalledTimes(1);
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
            store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
              .mockImplementation(
                () => Promise.resolve(undefined),
              );

            // Act
            return evaluateServiceDefinition
              .call(store, { commit, state, rootState }, actionParams)
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
            store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
              .mockImplementation(
                () => Promise.resolve({ status }),
              );

            // Act
            return evaluateServiceDefinition
              .call(store, { commit, state, rootState }, actionParams)
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
              getQuestionnaire.mockClear();
              getConditionsList.mockClear();
              getQuestionnaireId.mockClear();
              getTCsAnswerForProvider.mockClear();
            });

            describe('cannot retrieve either question or session id from response, as well as a list of conditions', () => {
              each([{
                sessionId: undefined,
                question: {},
                conditionList: [],
              }, {
                sessionId: 'session-id',
                question: undefined,
                conditionList: [],
              }, {
                sessionId: undefined,
                question: undefined,
                conditionList: undefined,
              }]).it('will dispatch clearAndSetError', ({ sessionId, question, conditionList }) => {
                // Arrange
                const expectedQuestionnaireItem = { item: 'value' };
                const expectedResponse = { status: 'data-required' };
                store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
                  .mockImplementation(
                    () => Promise.resolve(expectedResponse),
                  );
                getSessionId.mockReturnValue(sessionId);
                getQuestion.mockReturnValue(question);
                getConditionsList.mockReturnValue(conditionList);
                getQuestionnaireItem.mockReturnValue(expectedQuestionnaireItem);

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState }, actionParams)
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

            describe('guidance response contains questionnaire matching conditionsServiceDefinition in SJR', () => {
              it('will store list of service definitions in conditionList', () => {
                // Arrange
                const expectedConditionsList = [{
                  category: 'Allergies',
                  items: [{
                    id: 'HAY',
                    title: 'Hayfever',
                  }, {
                    id: 'URT',
                    title: 'Urticaria',
                  }],
                }];
                store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
                  .mockImplementation(
                    () => Promise.resolve({ status: 'data-required' }),
                  );
                getQuestionnaireId.mockReturnValue('NHS_CONDITION_LIST');
                getQuestionnaire.mockReturnValue({ id: 'NHS_CONDITION_LIST' });
                getConditionsList.mockReturnValue(expectedConditionsList);

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState }, actionParams)
                  .then(() => {
                    // Assert
                    expect(commit).toHaveBeenCalledWith(
                      SET_CONDITIONS_LIST,
                      expectedConditionsList,
                    );
                    expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
                  });
              });
            });

            describe('evaluating a new service definition selected from conditionList', () => {
              it('will add the terms and conditions answer to the fhir parameters and set the appropriate value for demographics consent', () => {
                // Arrange
                const expectedTCsAnswer = { termsAccepted: true };
                const expectedRequest = { ...request, demographicsConsentGiven: false };
                store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
                  .mockImplementation(
                    () => Promise.resolve({ status: 'data-required' }),
                  );
                getTCsAnswerForProvider.mockReturnValue(expectedTCsAnswer);
                actionParams.answeringConditionsQuestion = true;

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState }, actionParams)
                  .then(() => {
                    // Assert
                    expect(getTCsAnswerForProvider).toHaveBeenCalledWith(actionParams.provider);
                    expect(store.app.$cdsApi
                      .postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate)
                      .toHaveBeenCalledWith(expectedRequest);
                    expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
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
                store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
                  .mockImplementation(
                    () => Promise.resolve(expectedResponse),
                  );
                getQuestionnaire.mockReturnValue({});
                getSessionId.mockReturnValue(expectedSessionId);
                getQuestion.mockReturnValue(expectedQuestion);
                getQuestionnaireItem.mockReturnValue(expectedQuestionnaireItem);

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState }, actionParams)
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
                store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
                  .mockImplementation(
                    () => Promise.resolve(expectedResponse),
                  );
                getCarePlansAndReferralRequests.mockReturnValue(undefined);

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState }, actionParams)
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
                store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate
                  .mockImplementation(
                    () => Promise.resolve(expectedResponse),
                  );
                getCarePlansAndReferralRequests.mockReturnValue({ carePlans, referralRequests });

                // Act
                return evaluateServiceDefinition
                  .call(store, { commit, state, rootState }, actionParams)
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
  describe('setProviderNames', () => {
    describe('sets provider names correctly', () => {
      it('will set names when they exist', () => {
        store.app.$cdsApi.getFhirServiceDefinitionProviderNameByProvider
          .mockImplementation(
            () => Promise.resolve({ response: 'test' }),
          );
        setProviderNames.call(store, { commit, state, rootState },
          {
            adminProviderName: 'test',
            adviceProviderName: 'test',
          })
          .then(() => {
            // Assert
            expect(store.app.$cdsApi.getFhirServiceDefinitionProviderNameByProvider)
              .toHaveBeenCalledTimes(2);
            expect(commit)
              .toHaveBeenCalledWith(SET_ADMIN_PROVIDER_NAME, { response: 'test' });
            expect(commit)
              .toHaveBeenCalledWith(SET_ADVICE_PROVIDER_NAME, { response: 'test' });
          });
      });
    });
    describe('setProviderNames none', () => {
      it('will not set names when they are none', () => {
        setProviderNames.call(store, { commit, state, rootState },
          {
            adminProviderName: 'none',
            adviceProviderName: 'none',
          })
          .then(() => {
            // Assert
            expect(store.app.$cdsApi.getFhirServiceDefinitionProviderNameByProvider)
              .toHaveBeenCalledTimes(0);
            expect(commit)
              .toHaveBeenCalledTimes(0);
          });
      });
    });
  });
});
