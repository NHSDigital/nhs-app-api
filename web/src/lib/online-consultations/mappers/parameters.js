import { SESSION_ID, INPUT_DATA, ORGANIZATION as ORGANIZATION_PARAMETER } from '@/lib/online-consultations/constants/parameter-names';
import { COMPLETED, DATA_REQUIRED } from '@/lib/online-consultations/constants/status-types';
import { PARAMETERS, QUESTIONNAIRE_RESPONSE, ORGANIZATION as ORGANIZATION_RESOURCE } from '@/lib/online-consultations/constants/resource-types';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import moment from 'moment';

const pointRegExp = /^Point:(\d*),(\d*)$/;

function prependZero(value) {
  return (value < 10 && `${value}`.length === 1) ? `0${value}` : value;
}

function getSessionIdParameter(sessionId) {
  return {
    name: SESSION_ID,
    valueString: sessionId,
  };
}

function getOrganizationParameter(odsCode) {
  return {
    name: ORGANIZATION_PARAMETER,
    resource: {
      resourceType: ORGANIZATION_RESOURCE,
      identifier: {
        value: odsCode,
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

export function getAnswerFromItem(question, answer) {
  const answerFromResponse = [];

  switch (question.type) {
    case QuestionTypes.ATTACHMENT:
      answer.answer.forEach((c) => {
        answerFromResponse.push({
          name: c.valueAttachment.title,
          type: c.valueAttachment.contentType,
          size: c.valueAttachment.size,
          base64: c.valueAttachment.data,
        });
      });
      break;
    case QuestionTypes.BOOLEAN:
      answer.answer.forEach((c) => {
        answerFromResponse.push(c.valueBoolean.toString());
      });
      break;
    case QuestionTypes.CHOICE:
      answer.answer.forEach((c) => {
        answerFromResponse.push(c.valueCoding.code);
      });
      break;
    case QuestionTypes.DATE:
      answer.answer.forEach((c) => {
        const formattedDate = moment(c.valueDate, 'YYYY-MM-DD');
        answerFromResponse.push({
          year: formattedDate.format('YYYY'),
          month: prependZero(formattedDate.format('MM')),
          day: prependZero(formattedDate.format('DD')),
        });
      });
      break;
    case QuestionTypes.DATETIME:
      answer.answer.forEach((c) => {
        const formattedDate = moment(c.valueDateTime, 'YYYY-MM-DDThh:mm');
        answerFromResponse.push({
          year: formattedDate.format('YYYY'),
          month: prependZero(formattedDate.format('MM')),
          day: prependZero(formattedDate.format('DD')),
          hour: prependZero(formattedDate.format('hh')),
          minute: prependZero(formattedDate.format('mm')),
        });
      });
      break;
    case QuestionTypes.DECIMAL:
      answer.answer.forEach((c) => {
        answerFromResponse.push(c.valueDecimal);
      });
      break;
    case QuestionTypes.INTEGER:
      answer.answer.forEach((c) => {
        answerFromResponse.push(c.valueInteger);
      });
      break;
    case QuestionTypes.IMAGE:
      answer.answer.forEach((c) => {
        const match = pointRegExp.exec(c.valueString);
        if (match !== null) {
          answerFromResponse.push({
            x: match[1],
            y: match[2],
          });
        }
      });
      break;
    case QuestionTypes.MULTIPLE_CHOICE:
      answer.answer.forEach((c) => {
        answerFromResponse.push(c.valueCoding.code);
      });
      break;
    case QuestionTypes.QUANTITY:
      answer.answer.forEach((c) => {
        answerFromResponse.push({
          quantity: c.valueQuantity.value,
          unit: c.valueQuantity.unit,
        });
      });
      break;
    case QuestionTypes.STRING:
    case QuestionTypes.TEXT:
      answer.answer.forEach((c) => {
        answerFromResponse.push(c.valueString);
      });
      break;
    case QuestionTypes.TIME:
      answer.answer.forEach((c) => {
        const splitTime = c.valueTime.split(':');
        answerFromResponse.push({
          hour: prependZero(splitTime[0]),
          minute: prependZero(splitTime[1]),
        });
      });
      break;
    default:
      break;
  }

  if (question.type === QuestionTypes.MULTIPLE_CHOICE) {
    return answerFromResponse;
  }
  return answerFromResponse[0];
}

function getInputDataParameter({ question,
  answer,
  answerIsValid,
  answerIsEmpty,
  previousQuestion,
  previousSelected }) {
  let inputData;
  if (answerIsValid || previousSelected === true) {
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
    if (previousSelected === true) {
      inputData.resource.item.push(getAnswerItem(previousQuestion, true));
    } else if (!answerIsEmpty) {
      inputData.resource.item.push(getAnswerItem(question, answer));
    } else {
      inputData.resource.item.push({
        linkId: question.id,
      });
    }
  }

  return inputData;
}

export function getParameters(state, rootState) {
  try {
    const parameters = {
      resourceType: PARAMETERS,
      parameter: [],
    };

    // Adding patient and organization parameters when valid answer is given and
    // data requirements present - assumed to be Ts&Cs.
    if (state.answerIsValid && state.dataRequirements) {
      if (state.dataRequirements.organization) {
        parameters.parameter.push(getOrganizationParameter(rootState.session.gpOdsCode));
      }
    }

    if (state.status === DATA_REQUIRED) {
      if (state.sessionId) {
        parameters.parameter.push(getSessionIdParameter(state.sessionId));
      }
      parameters.parameter.push(getInputDataParameter(state));
    }

    return parameters;
  } catch (e) {
    return undefined;
  }
}

export default getParameters;
