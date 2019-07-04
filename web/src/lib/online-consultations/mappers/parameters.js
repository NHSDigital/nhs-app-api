import { SESSION_ID, INPUT_DATA, ORGANIZATION as ORGANIZATION_PARAMETER } from '@/lib/online-consultations/constants/parameter-names';
import { COMPLETED, DATA_REQUIRED } from '@/lib/online-consultations/constants/status-types';
import { PARAMETERS, QUESTIONNAIRE_RESPONSE, ORGANIZATION as ORGANIZATION_RESOURCE } from '@/lib/online-consultations/constants/resource-types';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';

function prependZero(value) {
  return (value < 10 && `${value}`.length === 1) ? `0${value}` : value;
}

function getSessionIdParameter(sessionId) {
  return {
    name: SESSION_ID,
    valueString: sessionId,
  };
}

function getOrganizationParameter() {
  return {
    name: ORGANIZATION_PARAMETER,
    resource: {
      resourceType: ORGANIZATION_RESOURCE,
      identifier: {
        value: 'A29928',
      },
    },
  };
}

function getAnswerItem(question, answer) {
  const item = {
    linkId: question.id,
  };

  switch (question.type) {
    case QuestionTypes.ATTACHMENT:
      item.answer = [{
        valueAttachment: {
          size: answer.size,
          title: answer.name,
          data: answer.base64,
          contentType: answer.type,
        },
      }];
      break;
    case QuestionTypes.BOOLEAN:
      item.answer = [{
        valueBoolean: answer,
      }];
      break;
    case QuestionTypes.CHOICE:
      item.answer = [{
        valueCoding: {
          code: answer,
        },
      }];
      break;
    case QuestionTypes.DATE:
      item.answer = [{
        valueDate: `${answer.year}-${prependZero(answer.month)}-${prependZero(answer.day)}`,
      }];
      break;
    case QuestionTypes.DATETIME:
      item.answer = [{
        valueDateTime: `${answer.year}-${prependZero(answer.month)}-${prependZero(answer.day)}T${prependZero(answer.hour)}:${prependZero(answer.minute)}:00.000Z`,
      }];
      break;
    case QuestionTypes.DECIMAL:
      item.answer = [{
        valueDecimal: answer,
      }];
      break;
    case QuestionTypes.INTEGER:
      item.answer = [{
        valueInteger: answer,
      }];
      break;
    case QuestionTypes.IMAGE:
      item.answer = [{
        valueString: `Point:${answer.x},${answer.y}`,
      }];
      break;
    case QuestionTypes.MULTIPLE_CHOICE:
      item.answer = answer.map(code => ({
        valueCoding: {
          code,
        },
      }));
      break;
    case QuestionTypes.QUANTITY:
      item.answer = [{
        valueQuantity: {
          value: answer.quantity,
          unit: answer.unit,
        },
      }];
      break;
    case QuestionTypes.STRING:
    case QuestionTypes.TEXT:
      item.answer = [{
        valueString: answer,
      }];
      break;
    case QuestionTypes.TIME:
      item.answer = [{
        valueTime: `${prependZero(answer.hour)}:${prependZero(answer.minute)}`,
      }];
      break;
    default:
      break;
  }
  return item;
}

function getInputDataParameter({ question, answer, answerIsValid, answerIsEmpty }) {
  let inputData;

  if (answerIsValid) {
    inputData = {
      name: INPUT_DATA,
      resource: {
        resourceType: QUESTIONNAIRE_RESPONSE,
        status: COMPLETED,
        item: [],
        questionnaire: {
          reference: `Questionnaire/${question.id}`,
        },
      },
    };
    if (!answerIsEmpty) {
      inputData.resource.item.push(getAnswerItem(question, answer));
    } else {
      inputData.resource.item.push({
        linkId: question.id,
      });
    }
  }

  return inputData;
}

function getParameters(state, rootState) {
  try {
    const parameters = {
      resourceType: PARAMETERS,
      parameter: [],
    };

    // todo: will be undefined if this is before the initial evaluation request
    // soon to be done when data requirement for CareConnectOrganization present
    if (state.status === undefined) {
      parameters.parameter.push(getOrganizationParameter(rootState.session.gpOdsCode));
    }

    if (state.status === DATA_REQUIRED) {
      parameters.parameter.push(...[
        getSessionIdParameter(state.sessionId),
        getInputDataParameter(state),
      ]);
    }

    return parameters;
  } catch (e) {
    return undefined;
  }
}

export default getParameters;
