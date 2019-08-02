import { get, isString } from 'lodash/fp';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';

function getAnswer(body, question) {
  return get(question.name, body);
}

function getDateAnswer(body, question) {
  return {
    day: get(`${question.name}-day`, body),
    month: get(`${question.name}-month`, body),
    year: get(`${question.name}-year`, body),
  };
}

function getDateTimeAnswer(body, question) {
  return {
    day: get(`${question.name}-day`, body),
    month: get(`${question.name}-month`, body),
    year: get(`${question.name}-year`, body),
    hour: get(`${question.name}-hour`, body),
    minute: get(`${question.name}-minute`, body),
  };
}

function getMultipleChoiceAnswer(body, question) {
  const answer = getAnswer(body, question);
  return isString(answer) ? [answer] : answer;
}

function getQuantityAnswer(body, question) {
  return {
    quantity: get(`${question.name}-quantity`, body),
    unit: get(`${question.name}-unit`, body),
  };
}

function getTimeAnswer(body, question) {
  return {
    hour: get(`${question.name}-hour`, body),
    minute: get(`${question.name}-minute`, body),
  };
}

function getAnswerFromRequestBody(body, question) {
  try {
    switch (question.type) {
      case QuestionTypes.ATTACHMENT:
      case QuestionTypes.BOOLEAN:
      case QuestionTypes.CHOICE:
      case QuestionTypes.DECIMAL:
      case QuestionTypes.INTEGER:
      case QuestionTypes.STRING:
      case QuestionTypes.TEXT:
        return getAnswer(body, question);
      case QuestionTypes.MULTIPLE_CHOICE:
        return getMultipleChoiceAnswer(body, question);
      case QuestionTypes.DATE:
        return getDateAnswer(body, question);
      case QuestionTypes.DATETIME:
        return getDateTimeAnswer(body, question);
      case QuestionTypes.TIME:
        return getTimeAnswer(body, question);
      case QuestionTypes.QUANTITY:
        return getQuantityAnswer(body, question);
      default:
        return undefined;
    }
  } catch (e) {
    return undefined;
  }
}

export default getAnswerFromRequestBody;
