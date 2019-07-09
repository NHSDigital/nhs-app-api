import { get, isEmpty } from 'lodash/fp';
import {
  CARE_PLAN,
  ORGANIZATION,
  PATIENT,
  PARAMETERS,
  QUESTIONNAIRE,
  QUESTIONNAIRE_RESPONSE,
  REFERRAL_REQUEST,
  REQUEST_GROUP,
  OPERATION_OUTCOME,
} from '@/lib/online-consultations/constants/resource-types';
import { SESSION_ID } from '@/lib/online-consultations/constants/parameter-names';
import { ACTIVE } from '@/lib/online-consultations/constants/status-types';

function getQuestionnaireId(response) {
  const questionnaireIds = response.dataRequirement
    .filter(d => d.type === QUESTIONNAIRE_RESPONSE);

  return questionnaireIds[0].extension[0].valueReference.reference.substring(1);
}

function getQuestionnaireResponse(guidanceResponse) {
  const questionnaireResponse = guidanceResponse.contained
    .filter(c => c.resourceType === QUESTIONNAIRE_RESPONSE);

  return questionnaireResponse[0];
}
function getQuestionnaire(response, questionnaireId) {
  const activeQuestionnaires = response.contained
    .filter(c => c.resourceType === QUESTIONNAIRE &&
                 c.id === questionnaireId &&
                 c.status === ACTIVE);

  return activeQuestionnaires[0];
}

function getOperationOutcome(guidanceResponse) {
  const operationOutcomes = guidanceResponse.contained
    .filter(c => c.resourceType === OPERATION_OUTCOME);

  return operationOutcomes;
}

function getRequestGroupId(guidanceResponse) {
  const requestGroupId = guidanceResponse.result.reference;

  return requestGroupId.substring(1);
}

function getRequestGroupActionIds(guidanceResponse, requestGroupId) {
  const requestGroup = guidanceResponse.contained
    .filter(c => c.resourceType === REQUEST_GROUP &&
                 c.id === requestGroupId)[0];

  return requestGroup.action.map(a => a.resource.reference.substring(1));
}

function getRequestGroupActions(guidanceResponse, actionIds) {
  const carePlans = [];
  const referralRequests = [];

  guidanceResponse.contained
    .filter(c => actionIds.indexOf(c.id) !== -1)
    .forEach((c) => {
      const { id, title, activity = [], description } = c;

      if (c.resourceType === CARE_PLAN) {
        const activities = activity
          .map(a => get('detail.description', a))
          .filter(a => a !== undefined);

        carePlans.push({ id, title, activities });
      }

      if (c.resourceType === REFERRAL_REQUEST && !isEmpty(description)) {
        referralRequests.push({ id, description });
      }
    });

  return {
    carePlans,
    referralRequests,
  };
}

export function getSessionId(guidanceResponse) {
  try {
    const outputParamsId = guidanceResponse.outputParameters.reference;
    const outputParams = guidanceResponse.contained
      .filter(c => c.resourceType === PARAMETERS &&
                   c.id === outputParamsId.substring(1));
    const parameters = outputParams[0].parameter;

    return parameters.filter(p => p.name === SESSION_ID)[0].valueString;
  } catch (e) {
    return undefined;
  }
}

export function getQuestionnaireItem(response) {
  try {
    const questionnaireId = getQuestionnaireId(response);
    const questionnaire = getQuestionnaire(response, questionnaireId);
    let questionReturn;

    if (questionnaire.item[0].item !== undefined && !questionnaire.item[0].text) {
      questionReturn = questionnaire.item[0].item.filter(c => c.linkId === questionnaireId);
      return questionReturn[0];
    }
    return questionnaire.item[0];
  } catch (e) {
    return undefined;
  }
}

export function getQuestionnaireResponseAnswers(response) {
  try {
    const questionnaireResponse = getQuestionnaireResponse(response);
    const questionnaireId = getQuestionnaireId(response);
    let questionAnswerReturn;

    if (questionnaireResponse.item !== undefined) {
      questionAnswerReturn = questionnaireResponse.item.filter(c => c.linkId === questionnaireId);
      return questionAnswerReturn[0];
    }
    return undefined;
  } catch (e) {
    return undefined;
  }
}

/**
 * @param {{valueCodeableConcept:string}} object
 * @param guidanceResponse
 */

export function getPreviousQuestion(guidanceResponse) {
  try {
    const questionnaireId = getQuestionnaireId(guidanceResponse);
    const questionnaire = getQuestionnaire(guidanceResponse, questionnaireId);
    let questionReturn;

    if (questionnaire.item[0].item !== undefined) {
      const questionReturnWithExtension =
        questionnaire.item[0].item.filter(c => c.extension !== undefined);
      questionReturn = questionReturnWithExtension.filter(c => c.linkId.includes('_PREV') &&
        c.type === 'boolean' && c.extension[0].valueCodeableConcept.text === 'back');
      return questionReturn[0];
    }
    return undefined;
  } catch (e) {
    return undefined;
  }
}

/**
 * @param {{issue:string}} object
 * @param guidanceResponse
 */
export function getAllIssues(guidanceResponse) {
  try {
    const operationOutcomeIssues = [];
    const operationOutcome = getOperationOutcome(guidanceResponse);
    if (isEmpty(operationOutcome)) {
      return undefined;
    }

    operationOutcome.forEach((c) => {
      c.issue.forEach((issue) => {
        operationOutcomeIssues.push(issue.details.text);
      });
    });
    return operationOutcomeIssues;
  } catch (e) {
    return undefined;
  }
}

export function getCarePlansAndReferralRequests(guidanceResponse) {
  try {
    const requestGroupId = getRequestGroupId(guidanceResponse);
    const requestGroupActionIDs = getRequestGroupActionIds(guidanceResponse, requestGroupId);
    const actions = getRequestGroupActions(guidanceResponse, requestGroupActionIDs);

    if (isEmpty(actions.carePlans) && isEmpty(actions.referralRequests)) {
      return undefined;
    }

    return actions;
  } catch (e) {
    return undefined;
  }
}

export function getDataRequirements(response) {
  try {
    const result = {
      questionnaireResponse: false,
      patient: false,
      organization: false,
    };

    response.dataRequirement.forEach((dr) => {
      if (dr.type === ORGANIZATION) {
        result.organization = true;
      } else if (dr.type === PATIENT) {
        result.patient = true;
      } else if (dr.type === QUESTIONNAIRE_RESPONSE) {
        result.questionnaireResponse = true;
      }
    });

    return result;
  } catch (e) {
    return undefined;
  }
}
