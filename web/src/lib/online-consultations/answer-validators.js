import QuestionTypes from '@/lib/online-consultations/constants/question-types';

const integerRegExp = /^-?\d+$/;
const decimalRegExp = /^-?\d+(\.\d+)?$/;
const baseMessagePath = 'appointments.admin_help.errors.validation.message.';
const booleanOptions = ['true', 'false'];

export function questionAttachmentAnswerValid(answer = {}, required, accept = [], maxSize) {
  const { base64, type, size } = answer;

  const isEmpty = !type && !base64 && !size;

  if (!required && isEmpty) {
    return {
      isValid: true,
      isEmpty,
    };
  }

  const isCorrectType = accept.includes(type);
  const lessThanOrEqualToMaxSize = maxSize === undefined || size <= maxSize;

  return {
    isValid: !isEmpty && isCorrectType && lessThanOrEqualToMaxSize,
    message: `${baseMessagePath}attachment`,
    isEmpty,
  };
}

export function questionBooleanAnswerValid(answer, required) {
  const isEmpty = answer === undefined;
  return {
    isValid: (!required && isEmpty) || booleanOptions.includes(answer),
    message: `${baseMessagePath}boolean`,
    isEmpty,
  };
}

export function questionChoiceAnswerValid(answer, required, validCodes = []) {
  const isEmpty = answer === undefined;
  return {
    isValid: (!required && isEmpty) || validCodes.includes(answer),
    message: `${baseMessagePath}choice`,
    isEmpty,
  };
}

export function questionDateAnswerValid(answer = {}, required) {
  const { day, month, year } = answer;

  const dayEmpty = day === undefined || day === '';
  const monthEmpty = month === undefined || month === '';
  const yearEmpty = year === undefined || year === '';

  const isEmpty = dayEmpty && monthEmpty && yearEmpty;

  if (!required && isEmpty) {
    return { isValid: true, isEmpty };
  }

  if (dayEmpty || monthEmpty || yearEmpty || year < 1000 || year > 9999) {
    return {
      isValid: false,
      message: `${baseMessagePath}date`,
      isEmpty,
    };
  }

  return {
    isValid: !Number.isNaN(new Date(`${year}-${month}-${day}`).getTime()),
    message: `${baseMessagePath}date`,
    isEmpty,
  };
}

export function questionDateTimeAnswerValid(answer = {}, required) {
  const { day, month, year, hour, minute } = answer;

  const dayEmpty = day === undefined || day === '';
  const monthEmpty = month === undefined || month === '';
  const yearEmpty = year === undefined || year === '';
  const hourEmpty = hour === undefined || hour === '';
  const minuteEmpty = minute === undefined || minute === '';

  const isEmpty = dayEmpty && monthEmpty && yearEmpty && hourEmpty && minuteEmpty;

  if (!required && isEmpty) {
    return { isValid: true, isEmpty };
  }

  if (dayEmpty || monthEmpty || yearEmpty || hourEmpty || minuteEmpty ||
      year < 1000 || year > 9999 || hour < 0 || hour > 23 || minute < 0 || minute > 59) {
    return {
      isValid: false,
      message: `${baseMessagePath}dateTime`,
      isEmpty,
    };
  }

  return {
    isValid: !Number.isNaN(new Date(`${year}-${month}-${day}`).getTime()),
    message: `${baseMessagePath}dateTime`,
    isEmpty,
  };
}

export function questionImageAnswerValid(answer = {}, required) {
  const isEmpty = answer.x === undefined && answer.y === undefined;

  if (!required && isEmpty) {
    return { isValid: true, isEmpty };
  }

  return {
    isValid: !isEmpty && answer.x > 0 && answer.y > 0,
    message: `${baseMessagePath}image`,
    isEmpty,
  };
}

export function questionNumberAnswerValid(
  answer,
  required,
  type,
  min = undefined,
  max = undefined,
) {
  const isEmpty = answer === '' || answer === undefined;
  if (!required && isEmpty) {
    return { isValid: true, isEmpty };
  }

  const isInteger = type === QuestionTypes.INTEGER;
  const regExp = isInteger ? integerRegExp : decimalRegExp;

  if (Number.isNaN(answer) ||
      (max !== undefined && answer > max) ||
      (min !== undefined && answer < min) ||
      regExp.exec(answer) === null) {
    return {
      isValid: false,
      message: isInteger ? `${baseMessagePath}integer` : `${baseMessagePath}decimal`,
      isEmpty,
    };
  }

  return {
    isValid: true,
    isEmpty,
  };
}

export function questionMultipleChoiceAnswerValid(answer = [], required, validCodes = []) {
  const isEmpty = answer.length === 0;

  return {
    isValid: (!required && isEmpty) || (!isEmpty && answer.every(o => validCodes.includes(o))),
    message: `${baseMessagePath}multiple_choice`,
    isEmpty,
  };
}

export function questionQuantityAnswerValid(
  answer = {},
  required,
  max = undefined,
  validCodes = [],
) {
  const { quantity, unit } = answer;

  const quantityEmpty = quantity === '' || quantity === undefined;
  const unitEmpty = unit === '' || unit === undefined;
  const isEmpty = quantityEmpty && unitEmpty;

  if (!required && isEmpty) {
    return { isValid: true, isEmpty };
  }

  const isValidUnit = validCodes.includes(unit);
  const lessThanOrEqualToMax = max === undefined || quantity <= max;
  const greaterThanOrEqualToMinValue = quantity >= 0;

  return {
    isValid: !quantityEmpty &&
             isValidUnit &&
             lessThanOrEqualToMax &&
             greaterThanOrEqualToMinValue,
    message: `${baseMessagePath}quantity`,
    isEmpty,
  };
}

export function questionStringAnswerValid(answer, required) {
  const isEmpty = answer === undefined || answer === '' || answer.trim() === '';

  return {
    isValid: (!required && isEmpty) || !isEmpty,
    message: `${baseMessagePath}string`,
    isEmpty,
  };
}

export function questionTextAnswerValid(answer, required, maxLength) {
  const isEmpty = answer === undefined || answer === '' || answer.trim() === '';
  const lessThanMaxLength = !isEmpty && (maxLength === undefined || answer.length <= maxLength);

  return {
    isValid: (!required && isEmpty) || lessThanMaxLength,
    message: `${baseMessagePath}text`,
    isEmpty,
  };
}

export function questionTimeAnswerValid(answer = {}, required) {
  const { hour, minute } = answer;

  const hourIsEmpty = hour === undefined || hour === '';
  const timeIsEmpty = minute === undefined || minute === '';
  const isEmpty = hourIsEmpty && timeIsEmpty;

  if (!required && isEmpty) {
    return { isValid: true, isEmpty };
  }

  return {
    isValid: !hourIsEmpty && !timeIsEmpty && hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59,
    message: `${baseMessagePath}time`,
    isEmpty,
  };
}

export function isAnswerValid(answer, question = {}) {
  switch (question.type) {
    case QuestionTypes.ATTACHMENT:
      return questionAttachmentAnswerValid(answer, question.required);
    case QuestionTypes.BOOLEAN:
      return questionBooleanAnswerValid(answer, question.required);
    case QuestionTypes.CHOICE:
      return questionChoiceAnswerValid(answer, question.required, question.validCodes);
    case QuestionTypes.DATE:
      return questionDateAnswerValid(answer, question.required);
    case QuestionTypes.DATETIME:
      return questionDateTimeAnswerValid(answer, question.required);
    case QuestionTypes.DECIMAL:
    case QuestionTypes.INTEGER:
      return questionNumberAnswerValid(
        answer,
        question.required,
        question.type,
        question.min,
        question.max,
      );
    case QuestionTypes.IMAGE:
      return questionImageAnswerValid(answer, question.required);
    case QuestionTypes.MULTIPLE_CHOICE:
      return questionMultipleChoiceAnswerValid(answer, question.required, question.validCodes);
    case QuestionTypes.QUANTITY:
      return questionQuantityAnswerValid(
        answer,
        question.required,
        question.max,
        question.validCodes,
      );
    case QuestionTypes.STRING:
      return questionStringAnswerValid(answer, question.required);
    case QuestionTypes.TEXT:
      return questionTextAnswerValid(answer, question.required, question.maxLength);
    case QuestionTypes.TIME:
      return questionTimeAnswerValid(answer, question.required);
    default:
      return { isValid: false, message: `${baseMessagePath}default` };
  }
}
