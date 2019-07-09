/* eslint-disable max-len */
/* eslint-disable no-undef */
import { getParameters, getAnswerFromItem } from '@/lib/online-consultations/mappers/parameters';
import { initialState } from '@/store/modules/onlineConsultations/mutation-types';
import { SESSION_ID, INPUT_DATA, ORGANIZATION as ORGANIZATION_PARAMETER } from '@/lib/online-consultations/constants/parameter-names';
import { QUESTIONNAIRE_RESPONSE, ORGANIZATION as ORGANIZATION_RESOURCE } from '@/lib/online-consultations/constants/resource-types';
import { COMPLETED, DATA_REQUIRED } from '@/lib/online-consultations/constants/status-types';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import each from 'jest-each';

describe('online consultations mappers parameters', () => {
  let state;
  let rootState;

  const defaultQuestionId = 'TEST_QUESTION';
  const defaultInputData = {
    name: INPUT_DATA,
    resource: {
      resourceType: QUESTIONNAIRE_RESPONSE,
      status: COMPLETED,
      item: [{
        linkId: 'TEST_QUESTION',
      }],
      questionnaire: {
        reference: `Questionnaire/${defaultQuestionId}`,
      },
    },
  };

  beforeEach(() => {
    state = initialState();
    rootState = {
      session: {
        gpOdsCode: 'A29928',
      },
    };
  });

  describe('answer is valid', () => {
    describe('organization is a data requirement', () => {
      it('will add organization parameter', () => {
        // Arrange
        state.answerIsValid = true;
        state.dataRequirements = { organization: true };

        // Act
        const parameters = getParameters(state, rootState);

        // Assert
        const organizationParameter = parameters.parameter.filter(p => p.name === ORGANIZATION_PARAMETER)[0];
        expect(organizationParameter.resource.resourceType).toEqual(ORGANIZATION_RESOURCE);
        expect(organizationParameter.resource.identifier.value).toEqual(rootState.session.gpOdsCode);
      });
    });

    describe('organization is not a data requirement', () => {
      it('will not add organization parameter', () => {
        // Arrange
        state.answerIsValid = true;
        state.dataRequirements = { organization: false };

        // Act
        const parameters = getParameters(state, rootState);

        // Assert
        const organizationParameters = parameters.parameter.filter(p => p.name === ORGANIZATION_PARAMETER);
        expect(organizationParameters).toHaveLength(0);
      });
    });
  });

  describe('answer is invalid', () => {
    each([true, false]).it('will not add organization parameter', (organizationRequired) => {
      // Arrange
      state.answerIsValid = false;
      state.dataRequirements = { organization: organizationRequired };

      // Act
      const parameters = getParameters(state, rootState);

      // Assert
      const organizationParameters = parameters.parameter.filter(p => p.name === ORGANIZATION_PARAMETER);
      expect(organizationParameters).toHaveLength(0);
    });
  });

  describe('status is data-required', () => {
    beforeEach(() => {
      state.status = DATA_REQUIRED;
    });

    it('will add the current consultation session id as a parameter', () => {
      // Arrange
      const sessionId = 'test-session-id';
      state.sessionId = sessionId;

      // Act
      const parameters = getParameters(state, rootState);

      // Assert
      const sessionIdParameter = parameters.parameter.filter(p => p && p.name === SESSION_ID)[0];
      expect(sessionIdParameter.valueString).toEqual(sessionId);
    });

    it('will not add an inputData if the current answer is invalid', () => {
      // Arrange
      state.status = DATA_REQUIRED;

      // Act
      const parameters = getParameters(state, rootState);

      // Assert
      const inputDataParameters = parameters.parameter.filter(p => p && p.name === INPUT_DATA);
      expect(inputDataParameters).toHaveLength(0);
    });

    describe('answer is valid', () => {
      beforeEach(() => {
        state.answerIsValid = true;
        state.question = { id: defaultQuestionId };
      });

      it('will add an answerless inputData containing a questionnaire response if the answer is valid but empty as of last validate', () => {
        // Arrange
        state.answerIsEmpty = true;
        state.question = { id: defaultQuestionId };

        // Act
        const parameters = getParameters(state, rootState);

        // Assert
        const inputDataParameter = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0];
        expect(inputDataParameter).toEqual(defaultInputData);
      });

      describe('answer is not empty', () => {
        beforeEach(() => {
          state.answerIsEmpty = false;
        });

        it('will return undefined if validated to be valid and not empty, but answer is actually undefined resulting in exception', () => {
          // Arrange
          state.question = { type: QuestionTypes.MULTIPLE_CHOICE };

          // Act
          const parameters = getParameters(state, rootState);

          // Assert
          expect(parameters).toBeUndefined();
        });

        describe('valid and non empty as of last validate', () => {
          describe('known question types', () => {
            describe('attachment question response item', () => {
              it('attachment attachment response item', () => {
                // Arrange
                const question = {
                  type: 'attachment',
                };
                const answer = {
                  answer: [{
                    valueAttachment: {
                      title: 'title',
                      contentType: 'image/png',
                      size: 9999,
                      data: 'data',
                    },
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual({
                  base64: 'data',
                  name: 'title',
                  size: 9999,
                  type: 'image/png',
                });
              });
            });
            describe('attachment question', () => {
              it('will add a valueAttachment to item.answer with a size, title, data and contentType', () => {
                // Arrange
                state.question.type = QuestionTypes.ATTACHMENT;
                const answer = {
                  size: 100,
                  name: 'test-title',
                  base64: 'base64:some-encoded-stuff-here',
                  type: 'image/png',
                };
                state.answer = answer;
                const expectedAnswer = [{
                  valueAttachment: {
                    size: answer.size,
                    title: answer.name,
                    data: answer.base64,
                    contentType: answer.type,
                  },
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('boolean question response item', () => {
              it('boolean question response item', () => {
                // Arrange
                const question = {
                  type: 'boolean',
                };
                const answer = {
                  answer: [{
                    valueBoolean: true,
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual('true');
              });
            });
            describe('boolean question', () => {
              each([true, false]).it('will add a valueBoolean to item.answer with a value of true or false', (answer) => {
                // Arrange
                state.question.type = QuestionTypes.BOOLEAN;
                state.answer = answer;
                const expectedAnswer = [{
                  valueBoolean: answer,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('choice question response item', () => {
              it('choice attachment response item', () => {
                // Arrange
                const question = {
                  type: 'choice',
                };
                const answer = {
                  answer: [{
                    valueCoding: {
                      code: 'Test',
                    },
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual('Test');
              });
            });
            describe('choice question', () => {
              each(['choice-1', 'another-choice']).it('will add a valueCoding to item.answer with a code equal to answer', (answer) => {
                // Arrange
                state.question.type = QuestionTypes.CHOICE;
                state.answer = answer;
                const expectedAnswer = [{
                  valueCoding: {
                    code: answer,
                  },
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('date question response item', () => {
              it('date attachment response item', () => {
                // Arrange
                const question = {
                  type: 'date',
                };
                const answer = {
                  answer: [{
                    valueDate: '1990-10-12',
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual({
                  year: '1990',
                  month: '10',
                  day: '12',
                });
              });
            });
            describe('date question', () => {
              each([{
                answer: {
                  day: 1,
                  month: 2,
                  year: 1999,
                },
                valueDate: '1999-02-01',
              }, {
                answer: {
                  day: 12,
                  month: 11,
                  year: 2000,
                },
                valueDate: '2000-11-12',
              }, {
                answer: {
                  day: '02',
                  month: '01',
                  year: 2000,
                },
                valueDate: '2000-01-02',
              }, {
                answer: {
                  day: '1',
                  month: '1',
                  year: '2000',
                },
                valueDate: '2000-01-01',
              }]).it('will add a valueDate to item.answer with a date in the format yyyy-MM-dd', ({ answer, valueDate }) => {
                // Arrange
                state.question.type = QuestionTypes.DATE;
                state.answer = answer;
                const expectedAnswer = [{
                  valueDate,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('dateTime question response item', () => {
              it('dateTime response item', () => {
                // Arrange
                const question = {
                  type: 'dateTime',
                };
                const answer = {
                  answer: [{
                    valueDateTime: '2015-03-25T12:00:00Z',
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual({
                  year: '2015',
                  month: '03',
                  day: '25',
                  hour: '12',
                  minute: '00',
                });
              });
            });
            describe('dateTime question', () => {
              each([{
                answer: {
                  day: 1,
                  month: 2,
                  year: 1999,
                  hour: 1,
                  minute: 1,
                },
                valueDateTime: '1999-02-01T01:01:00.000Z',
              }, {
                answer: {
                  day: 12,
                  month: 11,
                  year: 2000,
                  hour: 13,
                  minute: 44,
                },
                valueDateTime: '2000-11-12T13:44:00.000Z',
              }, {
                answer: {
                  day: '02',
                  month: '01',
                  year: 2000,
                  hour: '01',
                  minute: '01',
                },
                valueDateTime: '2000-01-02T01:01:00.000Z',
              }, {
                answer: {
                  day: '1',
                  month: '1',
                  year: '2000',
                  hour: '1',
                  minute: '1',
                },
                valueDateTime: '2000-01-01T01:01:00.000Z',
              }]).it('will add a valueDateTime to item.answer with a date time in the format yyyy-MM-ddTHH:mm:ss.SSSZ', ({ answer, valueDateTime }) => {
                // Arrange
                state.question.type = QuestionTypes.DATETIME;
                state.answer = answer;
                const expectedAnswer = [{
                  valueDateTime,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('decimal question', () => {
              it('will add a valueDecimal to item.answer with a value equal to the answer', () => {
                // Arrange
                state.question.type = QuestionTypes.DECIMAL;
                const answer = 12.3;
                state.answer = answer;
                const expectedAnswer = [{
                  valueDecimal: answer,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('decimal response item', () => {
              it('decimal response item', () => {
                // Arrange
                const question = {
                  type: 'decimal',
                };
                const answer = {
                  answer: [{
                    valueDecimal: 5.5,
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual(5.5);
              });
            });
            describe('image question', () => {
              it('will add a valueString to item.answer containing selected point', () => {
                // Arrange
                state.question.type = QuestionTypes.IMAGE;
                const x = 12;
                const y = 200;
                const answer = `Point:${x},${y}`;
                state.answer = { x, y };
                const expectedAnswer = [{
                  valueString: answer,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('image response item', () => {
              it('image response item', () => {
                // Arrange
                const question = {
                  type: 'image',
                };
                const answer = {
                  answer: [{
                    valueString: '10,6',
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual({
                  x: '10',
                  y: '6',
                });
              });
            });
            describe('integer question', () => {
              it('will add a valueInteger to item.answer with a value equal to answer', () => {
                // Arrange
                state.question.type = QuestionTypes.INTEGER;
                const answer = 1304;
                state.answer = answer;
                const expectedAnswer = [{
                  valueInteger: answer,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('integer response item', () => {
              it('integer response item', () => {
                // Arrange
                const question = {
                  type: 'integer',
                };
                const answer = {
                  answer: [{
                    valueInteger: '10',
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual('10');
              });
            });
            describe('multiple choice question', () => {
              each([
                { answer: ['choice-1', 'choice-2', 'choice-5'] },
                { answer: ['choice-1'] },
                { answer: [] },
              ]).it('will add a valueCoding to item.answer with a code for each value in answer', ({ answer }) => {
                // Arrange
                state.question.type = QuestionTypes.MULTIPLE_CHOICE;
                state.answer = answer;
                const expectedAnswer = answer.map(code => ({
                  valueCoding: { code },
                }));

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('multiple choice response item', () => {
              it('multiple choice response item', () => {
                // Arrange
                const question = {
                  type: QuestionTypes.MULTIPLE_CHOICE,
                };
                const answer = {
                  answer: [{
                    valueCoding: {
                      code: 'TEST',
                    },
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual(['TEST']);
              });
            });
            describe('quantity question', () => {
              it('will add a valueQuantity to item.answer with a quantity and unit from answer', () => {
                // Arrange
                state.question.type = QuestionTypes.QUANTITY;
                const answer = {
                  quantity: 12,
                  unit: 'm',
                };
                state.answer = answer;
                const expectedAnswer = [{
                  valueQuantity: {
                    value: answer.quantity,
                    unit: answer.unit,
                  },
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('quantity response item', () => {
              it('quantity response item', () => {
                // Arrange
                const question = {
                  type: QuestionTypes.QUANTITY,
                };
                const answer = {
                  answer: [{
                    valueQuantity: {
                      value: '5',
                      unit: 'm',
                    },
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual({
                  quantity: '5',
                  unit: 'm',
                });
              });
            });
            describe('string question', () => {
              it('will add a valueString to item.answer equal to answer', () => {
                // Arrange
                state.question.type = QuestionTypes.STRING;
                const answer = 'this is my answer';
                state.answer = answer;
                const expectedAnswer = [{
                  valueString: answer,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('string response item', () => {
              it('string response item', () => {
                // Arrange
                const question = {
                  type: QuestionTypes.STRING,
                };
                const answer = {
                  answer: [{
                    valueString: 'TEST',
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual('TEST');
              });
            });
            describe('text question', () => {
              it('will add a valueString to item.answer equal to answer', () => {
                // Arrange
                state.question.type = QuestionTypes.TEXT;
                const answer = 'this is my answer.\n\rWith multiple lines';
                state.answer = answer;
                const expectedAnswer = [{
                  valueString: answer,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('text response item', () => {
              it('text response item', () => {
                // Arrange
                const question = {
                  type: QuestionTypes.TEXT,
                };
                const answer = {
                  answer: [{
                    valueString: 'TEST',
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual('TEST');
              });
            });
            describe('time question', () => {
              each([{
                answer: {
                  hour: 1,
                  minute: 2,
                },
                valueTime: '01:02',
              }, {
                answer: {
                  hour: 12,
                  minute: 11,
                },
                valueTime: '12:11',
              }, {
                answer: {
                  hour: '02',
                  minute: '01',
                },
                valueTime: '02:01',
              }, {
                answer: {
                  hour: '1',
                  minute: '1',
                },
                valueTime: '01:01',
              }]).it('will add a valueTime to item.answer with a time in the format HH:mm', ({ answer, valueTime }) => {
                // Arrange
                state.question.type = QuestionTypes.TIME;
                state.answer = answer;
                const expectedAnswer = [{
                  valueTime,
                }];

                // Act
                const parameters = getParameters(state, rootState);
                const actualAnswer = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0].resource.item[0].answer;

                // Assert
                expect(actualAnswer).toEqual(expectedAnswer);
              });
            });
            describe('time response item', () => {
              it('time response item', () => {
                // Arrange
                const question = {
                  type: QuestionTypes.TIME,
                };
                const answer = {
                  answer: [{
                    valueTime: '11:11',
                  }],
                };

                // Act
                const previousAnswer = getAnswerFromItem(question, answer);

                // Assert
                expect(previousAnswer).toEqual({
                  hour: '11',
                  minute: '11',
                });
              });
            });
          });
        });
      });
    });
  });
});
