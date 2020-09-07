import actions from '@/store/modules/onlineConsultations/actions';
import each from 'jest-each';
import getParameters from '@/lib/online-consultations/mappers/parameters';
import getTCsAnswerForProvider from '@/lib/online-consultations/constants/termsConditionsAnswers';
import i18n from '@/plugins/i18n';
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
  SET_IS_AVAILABLE,
} from '@/store/modules/onlineConsultations/mutation-types';
import { getDataRequirements, getSessionId, getQuestionnaire, getQuestionnaireId, getQuestionnaireItem, getCarePlansAndReferralRequests } from '@/lib/online-consultations/mappers/response';
import { getQuestion, getConditionsList } from '@/lib/online-consultations/mappers/item';

jest.mock('@/lib/online-consultations/mappers/parameters');
jest.mock('@/lib/online-consultations/mappers/response');
jest.mock('@/lib/online-consultations/mappers/item');
jest.mock('@/lib/online-consultations/constants/termsConditionsAnswers');

const {
  getServiceDefinition,
  evaluateServiceDefinition,
  setProviderNames,
  serviceDefinitionIsValid,
} = actions;

const commit = jest.fn();
const store = {
  app: {
    $httpV2: {
      postV2CdssServiceDefinitionByProviderEvaluate: jest.fn(),
      postV2CdssServiceDefinitionByProvider: jest.fn(),
      getV2CdssServiceDefinitionByProviderDetails: jest.fn(),
      getV2CdssServiceDefinitionByProviderIsValid: jest.fn(),
    },
    $options: {
      i18n,
    },
  },
  dispatch: jest.fn(),
  state: {
    errors: {
      pageSettings: {
        ignoredErrors: [480],
      },
    },
  },
};
const state = {
  test: 'state',
  errors: {
    pageSettings: {
      ignoredErrors: [480],
    },
  },
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
  const error = {
    response: {
      status: 480,
    },
  };
  beforeEach(() => {
    commit.mockClear();
    store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate.mockClear();
    store.app.$httpV2.postV2CdssServiceDefinitionByProvider.mockClear();
    store.app.$httpV2.getV2CdssServiceDefinitionByProviderDetails.mockClear();
    store.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid.mockClear();
    store.dispatch.mockClear();
    error.response.status = 400;
  });

  describe('getServiceDefinition', () => {
    let serviceDefinitionId;
    let provider;
    let actionParameters;
    let apiParameters;

    beforeAll(() => {
      serviceDefinitionId = 'NHS_ADMIN';
      provider = 'stubs';
      apiParameters = {
        serviceDefinitionMetaData: {
          id: serviceDefinitionId,
          type: 'AdminHelp',
        },
        provider,
      };
      actionParameters = {
        serviceDefinitionId,
        provider,
      };
    });

    each([
      ['NHS_ADMIN', 'AdminHelp'],
      ['NHS_ADVICE', 'GeneralAdvice'],
      ['NHS_CONDITION_LIST', 'ConditionList'],
      ['ANOTHER_SERVICE_DEFINITION', 'ConditionAdvice'],
    ]).it('will map service definition id %s to %s type for api request', (
      id, type,
    ) => {
      // Arrange
      store.app.$httpV2.postV2CdssServiceDefinitionByProvider
        .mockImplementation(
          () => Promise.resolve(),
        );

      actionParameters.serviceDefinitionId = id;
      apiParameters.serviceDefinitionMetaData = {
        id,
        type,
      };

      // Act
      return getServiceDefinition
        .call(store, { rootState, commit }, actionParameters)
        .then(() => {
          // Assert
          expect(store.app.$httpV2.postV2CdssServiceDefinitionByProvider)
            .toHaveBeenCalledWith(apiParameters);
          expect(store.app.$httpV2.postV2CdssServiceDefinitionByProvider)
            .toHaveBeenCalledTimes(1);
        });
    });

    describe('attempted get service definition is rejected', () => {
      it('will dispatch clearAndSetError', () => {
        // Arrange
        store.app.$httpV2.postV2CdssServiceDefinitionByProvider
          .mockImplementation(
            () => Promise.reject(error),
          );

        // Act
        return getServiceDefinition
          .call(store, { rootState, commit }, actionParameters)
          .then(() => {
            // Assert
            const { postV2CdssServiceDefinitionByProvider } = store.app.$httpV2;
            expect(postV2CdssServiceDefinitionByProvider)
              .toHaveBeenCalledWith(apiParameters);
            expect(postV2CdssServiceDefinitionByProvider)
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
          store.app.$httpV2.postV2CdssServiceDefinitionByProvider
            .mockImplementation(
              () => Promise.resolve(undefined),
            );

          // Act
          return getServiceDefinition
            .call(store, { rootState, commit }, actionParameters)
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
          store.app.$httpV2.postV2CdssServiceDefinitionByProvider
            .mockImplementation(
              () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
            );
          getDataRequirements.mockReturnValue(undefined);

          // Act
          return getServiceDefinition
            .call(store, { rootState, commit }, actionParameters)
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

          store.app.$httpV2.postV2CdssServiceDefinitionByProvider
            .mockImplementation(
              () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
            );

          // Act
          return getServiceDefinition
            .call(store, { rootState, commit }, actionParameters)
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
                .call(store, { rootState, commit }, actionParameters)
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

              store.app.$httpV2.postV2CdssServiceDefinitionByProvider
                .mockImplementation(
                  () => Promise.resolve({ resourceType: 'ServiceDefinition' }),
                );

              const expectedQuestion = { type: 'text' };
              getQuestion.mockReturnValue(expectedQuestion);

              // Act
              return getServiceDefinition
                .call(store, { rootState, commit }, actionParameters)
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
        expect(getParameters).toHaveBeenCalledWith(state, rootState, undefined, 'NHS_ADMIN', 'AdminHelp');
        expect(getParameters).toHaveBeenCalledTimes(1);
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
        expect(store.dispatch).toHaveBeenCalledTimes(1);
        expect(store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate)
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
          provider: 'stubs',
          addJavascriptDisabledHeader: true,
        };
        actionParams = {
          provider: 'stubs',
          serviceDefinitionId: 'NHS_ADMIN',
          addJavascriptDisabledHeader: true,
          answeringConditionsQuestion: false,
        };

        getParameters.mockClear();
        getParameters.mockReturnValue(parameters);
      });

      describe('action called with addJavascriptDisabledHeader set to true', () => {
        it('will include addJavascriptDisabledHeader in post parameter', () => {
          // Arrange
          store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
            .mockImplementation(
              () => Promise.reject(error),
            );

          // Act
          return evaluateServiceDefinition
            .call(store, { commit, state, rootState }, actionParams)
            .then(() => {
              // Assert
              const {
                postV2CdssServiceDefinitionByProviderEvaluate,
              } = store.app.$httpV2;
              expect(postV2CdssServiceDefinitionByProviderEvaluate)
                .toHaveBeenCalledWith(request);
              expect(postV2CdssServiceDefinitionByProviderEvaluate)
                .toHaveBeenCalledTimes(1);
              expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
              expect(store.dispatch).toHaveBeenCalledTimes(1);
            });
        });
      });

      describe('response is session end', () => {
        it('will not dispatch clearAndSetError', () => {
          // Arrange
          error.response.status = 480;
          store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
            .mockImplementation(
              () => Promise.reject(error),
            );

          // Act
          return evaluateServiceDefinition
            .call(store, { commit, state, rootState }, actionParams)
            .then(() => {
              // Assert
              expect(store.dispatch).toHaveBeenCalledTimes(0);
            });
        });
      });

      describe('attempted evaluation is rejected', () => {
        it('will dispatch clearAndSetError', () => {
          // Arrange
          store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
            .mockImplementation(
              () => Promise.reject(error),
            );

          // Act
          return evaluateServiceDefinition
            .call(store, { commit, state, rootState }, actionParams)
            .then(() => {
              // Assert
              const {
                postV2CdssServiceDefinitionByProviderEvaluate,
              } = store.app.$httpV2;
              expect(postV2CdssServiceDefinitionByProviderEvaluate)
                .toHaveBeenCalledWith(request);
              expect(postV2CdssServiceDefinitionByProviderEvaluate)
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
            store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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
            store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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
                store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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
                store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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
                store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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
                    expect(store.app.$httpV2
                      .postV2CdssServiceDefinitionByProviderEvaluate)
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
                store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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
                store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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
                store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
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

      describe('non terms and conditions screen', () => {
        beforeEach(async () => {
          store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
            .mockImplementation(
              () => Promise.resolve(undefined),
            );

          parameters = {
            parameter: [
              {
                name: 'inputData',
                resource: {
                  item: [
                    {
                      linkId: 'SOME_OTHER_PLACE_OUT_THERE',
                    },
                  ],
                },
              },
            ],
          };

          getParameters.mockClear();
          getParameters.mockReturnValue(parameters);

          await evaluateServiceDefinition.call(store, { commit, state, rootState }, actionParams);
        });

        it('will not set should skip leaving warning', async () => {
          expect(store.dispatch).not.toHaveBeenCalledWith('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
          expect(store.dispatch).not.toHaveBeenCalledWith('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);
        });

        it('will not set the window onbeforeunload event handler', async () => {
          expect(typeof window.onbeforeunload).not.toBe('function');
        });
      });

      describe('terms and conditions screen', () => {
        beforeEach(async () => {
          store.app.$httpV2.postV2CdssServiceDefinitionByProviderEvaluate
            .mockImplementation(
              () => Promise.resolve(undefined),
            );

          parameters = {
            parameter: [
              {
                name: 'inputData',
                resource: {
                  item: [
                    {
                      linkId: 'GLO_PRE_DISCLAIMERS_NHS_2',
                    },
                  ],
                },
              },
            ],
          };

          getParameters.mockClear();
          getParameters.mockReturnValue(parameters);

          await evaluateServiceDefinition.call(store, { commit, state, rootState }, actionParams);
        });

        it('will set should skip leaving warning to false', async () => {
          expect(store.dispatch).toHaveBeenCalledWith('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);
        });

        it('will set the window onbeforeunload event handler', async () => {
          expect(typeof window.onbeforeunload).toBe('function');
        });

        describe('when window onbeforeunload event handler is called', () => {
          it('prevent default is called', async () => {
            const preventDefault = jest.fn();

            window.onbeforeunload({
              preventDefault,
            });

            expect(preventDefault).toHaveBeenCalled();
          });

          it('page leaving warning is returned', async () => {
            const returnValue = window.onbeforeunload({
              preventDefault: jest.fn(),
            });

            expect(returnValue).toBe('If you have entered any information, it will not be saved.');
          });
        });
      });
    });
  });
  describe('setProviderNames', () => {
    describe('sets provider names correctly', () => {
      it('will set names when they exist', () => {
        store.app.$httpV2.getV2CdssServiceDefinitionByProviderDetails
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
            expect(store.app.$httpV2.getV2CdssServiceDefinitionByProviderDetails)
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
            expect(store.app.$httpV2.getV2CdssServiceDefinitionByProviderDetails)
              .toHaveBeenCalledTimes(0);
            expect(commit)
              .toHaveBeenCalledTimes(0);
          });
      });
    });
  });
  describe('serviceDefinitionIsValid', () => {
    it('will commit isValid true to store when successful', () => {
      store.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid
        .mockImplementation((params) => {
          switch (params.provider) {
            case 'test-provider':
              return Promise.resolve({
                status: 204,
              });
            default:
              return undefined;
          }
        });
      serviceDefinitionIsValid.call(store, { commit }, 'test-provider')
        .then(() => {
          // Assert
          expect(store.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid)
            .toHaveBeenCalledWith({
              provider: 'test-provider',
              returnResponse: true,
            });
          expect(commit)
            .toHaveBeenCalledWith(SET_IS_AVAILABLE, true);
        });
    });

    it('will not set isValid when api call throws non 580 error', () => {
      store.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid
        .mockImplementation(() => Promise.reject(error));
      serviceDefinitionIsValid.call(store, { commit }, 'test-provider')
        .then(() => {
          // Assert
          expect(store.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid)
            .toHaveBeenCalledWith({
              provider: 'test-provider',
              returnResponse: true,
            });
          expect(commit).not.toHaveBeenCalledWith(SET_IS_AVAILABLE);
        });
    });

    it('will set isValid to false when api call throws custom 580 error', () => {
      error.response.status = 580;
      store.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid
        .mockImplementation(() => Promise.reject(error));
      serviceDefinitionIsValid.call(store, { commit }, 'test-provider')
        .then(() => {
          // Assert
          expect(store.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid)
            .toHaveBeenCalledWith({
              provider: 'test-provider',
              returnResponse: true,
            });
          expect(commit)
            .toHaveBeenCalledWith(SET_IS_AVAILABLE, false);
        });
    });
  });
});
