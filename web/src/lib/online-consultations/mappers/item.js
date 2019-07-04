import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import mapHtmlTags from '@/lib/online-consultations/mappers/html-tags';

function getQuestion(item) {
  try {
    let { type } = item;

    if (type === QuestionTypes.CHOICE && item.repeats) {
      type = QuestionTypes.MULTIPLE_CHOICE;
    }

    if (type === QuestionTypes.STRING && item.initialAttachment !== undefined) {
      type = QuestionTypes.IMAGE;
    }

    const question = {
      id: item.linkId,
      isLegend: false,
      name: `${item.linkId}-${type}`,
      repeats: item.repeats,
      required: item.required,
      text: mapHtmlTags(item.text),
      type,
    };

    if (type === QuestionTypes.ATTACHMENT) {
      question.accept = ['image/png', 'image/jpeg'];
      question.maxSize = 1048576;
    }

    if (type === QuestionTypes.BOOLEAN) {
      question.isLegend = true;
    }

    if (item.type === QuestionTypes.CHOICE) {
      question.isLegend = true;
      question.options = item.option.map(o => ({
        code: o.valueCoding.code,
        label: o.valueCoding.display,
        selected: item.repeats ? false : undefined,
      }));
      if (!question.options.length) {
        return undefined;
      }
      question.validCodes = question.options.map(o => (o.code));
    }

    if (type === QuestionTypes.INTEGER || type === QuestionTypes.DECIMAL) {
      question.tag = 'label';
      const isInteger = type === QuestionTypes.INTEGER;
      if (Array.isArray(item.extension)) {
        item.extension.forEach((extensionItem) => {
          if (extensionItem.url.includes('minValue')) {
            question.min = isInteger ? extensionItem.valueInteger : extensionItem.valueDecimal;
          }
          if (extensionItem.url.includes('maxValue')) {
            question.max = isInteger ? extensionItem.valueInteger : extensionItem.valueDecimal;
          }
        });
      }
    }

    if (type === QuestionTypes.IMAGE) {
      question.source = item.initialAttachment.url;
    }

    if (type === QuestionTypes.QUANTITY) {
      const unitCodes = item.extension
        .filter(c => c.valueCoding !== undefined);
      question.options = unitCodes.map(o => ({
        code: o.valueCoding.code,
        label: o.valueCoding.display,
      }));
      question.validCodes = question.options.map(o => (o.code));
      if (Array.isArray(item.extension)) {
        const extensionValues = item.extension
          .filter(c => c.valueCoding === undefined);
        extensionValues.forEach((extensionItem) => {
          if (extensionItem.url.includes('minValue')) {
            question.min = extensionItem.valueInteger;
          }
          if (extensionItem.url.includes('maxValue')) {
            question.max = extensionItem.valueInteger;
          }
        });
      }
    }

    if (type === QuestionTypes.STRING || type === QuestionTypes.TEXT) {
      question.tag = 'label';
      if (Array.isArray(item.extension)) {
        item.extension.forEach((extensionItem) => {
          if (extensionItem.url.includes('maxLength')) {
            question.maxLength = extensionItem.valueInteger.toString();
          }
        });
      }
    }

    return question;
  } catch (e) {
    return undefined;
  }
}

export default getQuestion;
