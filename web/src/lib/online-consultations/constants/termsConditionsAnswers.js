const tcsAnswers = {
  eConsult: {
    name: 'inputData',
    resource: {
      resourceType: 'QuestionnaireResponse',
      status: 'completed',
      item: [
        {
          linkId: 'GLO_PRE_DISCLAIMERS',
          answer: [
            { valueCoding: { code: 'GLO_PRE_DISCLAIMERS_1' } },
            { valueCoding: { code: 'GLO_PRE_DISCLAIMERS_2' } },
          ],
        },
      ],
      questionnaire: { reference: 'Questionnaire/GLO_PRE_DISCLAIMERS' },
    },
  },
};

export default provider => (tcsAnswers[provider]);
