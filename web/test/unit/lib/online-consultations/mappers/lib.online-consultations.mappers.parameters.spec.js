/* eslint-disable max-len */
/* eslint-disable no-undef */
import getParameters from '@/lib/online-consultations/mappers/parameters';
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
      item: [],
      questionnaire: {
        reference: `Questionnaire/${defaultQuestionId}`,
      },
    },
  };
  const defaultItem = {
    linkId: defaultQuestionId,
  };

  beforeEach(() => {
    state = initialState();
    rootState = {
      session: {
        gpOdsCode: 'A29928',
      },
    };
  });

  describe('status is undefined - initial request', () => {
    it('will add a care connect organization parameter', () => {
      // Act
      const parameters = getParameters(state, rootState);

      // Assert
      const organizationParameter = parameters.parameter.filter(p => p.name === ORGANIZATION_PARAMETER)[0];
      expect(organizationParameter.resource.resourceType).toEqual(ORGANIZATION_RESOURCE);
      expect(organizationParameter.resource.identifier.value).toEqual(rootState.session.gpOdsCode);
    });
  });

  describe('status is not undefined', () => {
    it('will not add a care connect organization parameter', () => {
      // Arrange
      state.status = 'an-unknown-status';

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
          describe('unknown question type', () => {
            it('will add an item to the questionnaire response containing only a link id matching the question id', () => {
              // Arrange
              state.question.type = 'unknown-question-type';
              const expectedInputData = defaultInputData;
              expectedInputData.resource.item.push(defaultItem);

              // Act
              const parameters = getParameters(state, rootState);

              // Assert
              const inputDataParameter = parameters.parameter.filter(p => p && p.name === INPUT_DATA)[0];
              expect(inputDataParameter).toEqual(expectedInputData);
            });
          });

          describe('known question types', () => {
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
          });
        });
      });
    });
  });
});
