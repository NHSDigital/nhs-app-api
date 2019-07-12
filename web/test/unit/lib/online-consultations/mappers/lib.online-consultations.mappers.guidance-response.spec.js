import { getSessionId, getQuestionnaireItem, getCarePlansAndReferralRequests } from '@/lib/online-consultations/mappers/guidance-response';
import { CARE_PLAN, PARAMETERS, QUESTIONNAIRE, QUESTIONNAIRE_RESPONSE, REFERRAL_REQUEST, REQUEST_GROUP } from '@/lib/online-consultations/constants/resource-types';
import { SESSION_ID } from '@/lib/online-consultations/constants/parameter-names';
import { ACTIVE } from '@/lib/online-consultations/constants/status-types';
import each from 'jest-each';

describe('online consultations mappers guidance response', () => {
  let guidanceResponse;

  describe('getSessionId', () => {
    describe('exception is thrown', () => {
      it('will return undefined', () => {
        // Act
        const sessionId = getSessionId(guidanceResponse);

        // Assert
        expect(sessionId).toBeUndefined();
      });
    });

    describe('guidance response contained array has parameters object with an id matching outputparams reference with a sessionId parameter', () => {
      it('will return that sessionId', () => {
        // Arrange
        const validSessionId = 'valid-session-id';
        const invalidSessionId = 'invalid-session-id';
        const outputParamsId = 'output-params-id';
        const invalidOutputParamsId = 'invalid-output-params-id';

        guidanceResponse = {
          contained: [{
            resourceType: PARAMETERS,
            id: outputParamsId,
            parameter: [{
              name: SESSION_ID,
              valueString: validSessionId,
            }],
          }, {
            resourceType: PARAMETERS,
            id: invalidOutputParamsId,
            parameter: [{
              name: SESSION_ID,
              valueString: invalidSessionId,
            }],
          }],
          outputParameters: {
            reference: `#${outputParamsId}`,
          },
        };

        // Act
        const sessionId = getSessionId(guidanceResponse);

        // Assert
        expect(sessionId).toEqual(validSessionId);
      });
    });
  });

  describe('getQuestionnaireItem', () => {
    describe('exception is thrown', () => {
      it('will return undefined', () => {
        // Act
        const item = getQuestionnaireItem(guidanceResponse);

        // Assert
        expect(item).toBeUndefined();
      });
    });

    describe('guidance response has a data requirement of type questionnaire response, with a contained active questionnaire with a matching id and at least one item', () => {
      it('will return the first item in the matching contained questionnaire', () => {
        // Arrange
        const validQuestionnaireId = 'valid-questionnaire-id';
        const invalidQuestionnaireId = 'invalid-questionnaire-id';
        const expectedItem = { expected: true };
        const unexpectedItem = { expected: false };

        guidanceResponse = {
          contained: [{
            resourceType: QUESTIONNAIRE,
            id: validQuestionnaireId,
            status: ACTIVE,
            item: [expectedItem, unexpectedItem],
          }, {
            resourceType: QUESTIONNAIRE,
            id: invalidQuestionnaireId,
            status: ACTIVE,
            item: [unexpectedItem],
          }],
          dataRequirement: [{
            type: QUESTIONNAIRE_RESPONSE,
            id: validQuestionnaireId,
          }],
        };

        // Act
        const item = getQuestionnaireItem(guidanceResponse);

        // Assert
        expect(item).toEqual(expectedItem);
      });
    });
  });

  describe('getCarePlansAndReferralRequests', () => {
    describe('exception is thrown', () => {
      it('will return undefined', () => {
        // Act
        const result = getCarePlansAndReferralRequests(guidanceResponse);

        // Assert
        expect(result).toBeUndefined();
      });
    });

    describe('guidance response has a result reference matching a contained request group', () => {
      let validRequestGroup;
      let validReferralRequest1;
      let validReferralRequest2;
      let validCarePlan1;
      let validCarePlan2;

      let invalidRequestGroup;
      let invalidReferralRequest;
      let invalidCarePlan;

      let expectedActions;

      beforeEach(() => {
        validReferralRequest1 = {
          resourceType: REFERRAL_REQUEST,
          id: 'rr1',
          description: 'This is referral request 1',
        };
        validReferralRequest2 = {
          resourceType: REFERRAL_REQUEST,
          id: 'rr2',
          description: 'This is referral request 2',
        };
        validCarePlan1 = {
          resourceType: CARE_PLAN,
          id: 'cp1',
          title: 'Care Plan 1',
          activity: [{
            detail: {
              description: 'Do this 1',
            },
          }, {
            detail: {
              description: 'Do that 1',
            },
          }],
        };
        validCarePlan2 = {
          resourceType: CARE_PLAN,
          id: 'cp2',
          title: 'Care Plan 2',
          activity: [{
            detail: {
              description: 'Do this 2',
            },
          }, {
            detail: {
              description: 'Do that 2',
            },
          }],
        };
        validRequestGroup = {
          resourceType: REQUEST_GROUP,
          id: 'rg1',
          action: [{
            resource: {
              reference: '#rr1',
            },
          }, {
            resource: {
              reference: '#rr2',
            },
          }, {
            resource: {
              reference: '#cp1',
            },
          }, {
            resource: {
              reference: '#cp2',
            },
          }],
        };

        invalidReferralRequest = {
          resourceType: REFERRAL_REQUEST,
          id: 'rr3',
          description: 'This is referral request 3',
        };
        invalidCarePlan = {
          resourceType: CARE_PLAN,
          id: 'cp3',
          title: 'Care Plan 3',
          activity: [{
            detail: {
              description: 'Do this 3',
            },
          }, {
            detail: {
              description: 'Do that 3',
            },
          }],
        };
        invalidRequestGroup = {
          resourceType: REQUEST_GROUP,
          id: 'rg2',
          action: [{
            resource: {
              reference: '#rr3',
            },
          }, {
            resource: {
              reference: '#cp3',
            },
          }],
        };

        expectedActions = {
          carePlans: [{
            id: 'cp1',
            title: 'Care Plan 1',
            activities: ['Do this 1', 'Do that 1'],
          }, {
            id: 'cp2',
            title: 'Care Plan 2',
            activities: ['Do this 2', 'Do that 2'],
          }],
          referralRequests: [{
            id: 'rr1',
            description: 'This is referral request 1',
          }, {
            id: 'rr2',
            description: 'This is referral request 2',
          }],
        };

        guidanceResponse = {
          contained: [
            validRequestGroup,
            validReferralRequest1,
            validReferralRequest2,
            validCarePlan1,
            validCarePlan2,
            invalidRequestGroup,
            invalidReferralRequest,
            invalidCarePlan,
          ],
          result: {
            reference: '#rg1',
          },
        };
      });

      describe('no care plans or referral requests can be mapped', () => {
        it('will return undefined', () => {
          // Arrange
          guidanceResponse.contained[0].action = [];

          // Act
          const actions = getCarePlansAndReferralRequests(guidanceResponse);

          // Assert
          expect(actions).toBeUndefined();
        });
      });

      describe('care plans and referral requests can be mapped and are fully populated', () => {
        it('will map contained care plans and referral requests mentioned in the request group', () => {
          // Act
          const actions = getCarePlansAndReferralRequests(guidanceResponse);

          // Assert
          expect(actions).toEqual(expectedActions);
        });
      });

      describe('care plans have activities that have no detail or description', () => {
        each([{
          carePlanActivities: [{}],
        }, {
          carePlanActivities: [{ details: { anotherProp: 'something' } }],
        }]).it('will have no activities in resulting care plan', ({ carePlanActivities }) => {
          // Arrange
          validCarePlan1.activity = carePlanActivities;
          expectedActions.carePlans[0].activities = [];

          // Act
          const actions = getCarePlansAndReferralRequests(guidanceResponse);

          // Assert
          expect(actions).toEqual(expectedActions);
        });
      });

      describe('referral request has no description', () => {
        it('will not add the referral request to the actions', () => {
          // Arrange
          validReferralRequest1.description = undefined;
          expectedActions.referralRequests.shift();

          // Act
          const actions = getCarePlansAndReferralRequests(guidanceResponse);

          // Assert
          expect(actions).toEqual(expectedActions);
        });
      });
    });
  });
});
