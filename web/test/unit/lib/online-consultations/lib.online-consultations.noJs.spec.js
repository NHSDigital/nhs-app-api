import getAnswerFromRequestBody from '@/lib/online-consultations/noJs';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import each from 'jest-each';

describe('online consultations noJs', () => {
  const questionId = 'test-id';
  let question;
  let body;

  describe('getAnswerFromRequestBody', () => {
    describe('exception occurs', () => {
      it('will return undefined', () => {
        // Arrange
        question = undefined;

        // Act
        const answer = getAnswerFromRequestBody(question, body);

        // Assert
        expect(answer).toBeUndefined();
      });
    });

    describe('if unmatched', () => {
      it('will return undefined', () => {
        // Arrange
        const noJsKey = `${questionId}-${QuestionTypes.ATTACHMENT}`;

        question = {
          id: questionId,
          type: QuestionTypes.CHOICE,
        };
        body = {
          [noJsKey]: { data: 'stuff' },
        };

        // Act
        const answer = getAnswerFromRequestBody(question, body);

        // Assert
        expect(answer).toBeUndefined();
      });
    });

    describe('if matched', () => {
      describe('requires no manipulation', () => {
        each([
          QuestionTypes.ATTACHMENT,
          QuestionTypes.BOOLEAN,
          QuestionTypes.CHOICE,
          QuestionTypes.DECIMAL,
          QuestionTypes.INTEGER,
          QuestionTypes.STRING,
          QuestionTypes.TEXT,
        ]).it('will simply return the value', (questionType) => {
          // Arrange
          const name = `${questionId}-${questionType}`;
          const expectedAnswer = 'stuff';

          question = {
            name,
            id: questionId,
            type: questionType,
          };
          body = {
            [name]: expectedAnswer,
          };

          // Act
          const answer = getAnswerFromRequestBody(body, question);

          // Assert
          expect(answer).toEqual(expectedAnswer);
        });
      });

      describe('question type is date', () => {
        each([{}, {
          day: 1,
          year: 1993,
        }, {
          day: 1,
          month: 3,
        }, {
          month: 23,
          year: 1993,
        }, {
          day: 1,
          month: 23,
          year: 1993,
        }]).it('will return a date object populated with values from request body', ({ day, month, year }) => {
          // Arrange
          const questionName = `${questionId}-${QuestionTypes.DATE}`;
          const noJsDayKey = `${questionName}-day`;
          const noJsMonthKey = `${questionName}-month`;
          const noJsYearKey = `${questionName}-year`;
          const expectedAnswer = { day, month, year };

          question = {
            name: questionName,
            type: QuestionTypes.DATE,
          };
          body = {
            [noJsDayKey]: day,
            [noJsMonthKey]: month,
            [noJsYearKey]: year,
          };

          // Act
          const answer = getAnswerFromRequestBody(body, question);

          // Assert
          expect(answer).toEqual(expectedAnswer);
        });
      });

      describe('question type is datetime', () => {
        each([{}, {
          day: 1,
          year: 1993,
          hour: 1,
          minute: 59,
        }, {
          day: 1,
          month: 3,
          hour: 1,
          minute: 59,
        }, {
          month: 23,
          year: 1993,
          hour: 1,
          minute: 59,
        }, {
          day: 1,
          month: 23,
          year: 1993,
          hour: 1,
        }, {
          day: 1,
          month: 23,
          year: 1993,
          minute: 23,
        }, {
          day: 1,
          month: 23,
          year: 1993,
          hour: 11,
          minute: 23,
        }]).it('will return a date object populated with values from request body', ({ day, month, year, hour, minute }) => {
          // Arrange
          const questionName = `${questionId}-${QuestionTypes.DATE}`;
          const noJsDayKey = `${questionName}-day`;
          const noJsMonthKey = `${questionName}-month`;
          const noJsYearKey = `${questionName}-year`;
          const noJsHourKey = `${questionName}-hour`;
          const noJsMinuteKey = `${questionName}-minute`;
          const expectedAnswer = { day, month, year, hour, minute };

          question = {
            name: questionName,
            type: QuestionTypes.DATETIME,
          };
          body = {
            [noJsDayKey]: day,
            [noJsMonthKey]: month,
            [noJsYearKey]: year,
            [noJsHourKey]: hour,
            [noJsMinuteKey]: minute,
          };

          // Act
          const answer = getAnswerFromRequestBody(body, question);

          // Assert
          expect(answer).toEqual(expectedAnswer);
        });
      });

      describe('question type is multiple choice', () => {
        each([{}, {
          noJsAnswer: 'choice-1',
          expectedAnswer: ['choice-1'],
        }, {
          noJsAnswer: ['choice-1', 'choice-3'],
          expectedAnswer: ['choice-1', 'choice-3'],
        }]).it('will return an array of values from the request body, even if only one option was selected', ({ noJsAnswer, expectedAnswer }) => {
          // Arrange
          const name = `${questionId}-${QuestionTypes.MULTIPLE_CHOICE}`;

          question = {
            name,
            id: questionId,
            type: QuestionTypes.MULTIPLE_CHOICE,
          };
          body = {
            [name]: noJsAnswer,
          };

          // Act
          const answer = getAnswerFromRequestBody(body, question);

          // Assert
          expect(answer).toEqual(expectedAnswer);
        });
      });

      describe('question type is quantity', () => {
        each([{}, {
          quantity: 1,
        }, {
          unit: 'm',
        }, {
          quantity: 2,
          unit: 's',
        }]).it('will return a quantity object populated with values from request body', ({ quantity, unit }) => {
          // Arrange
          const questionName = `${questionId}-${QuestionTypes.QUANTITY}`;
          const noJsQuantityKey = `${questionName}-quantity`;
          const noJsUnitKey = `${questionName}-unit`;
          const expectedAnswer = { quantity, unit };

          question = {
            name: questionName,
            type: QuestionTypes.QUANTITY,
          };
          body = {
            [noJsQuantityKey]: quantity,
            [noJsUnitKey]: unit,
          };

          // Act
          const answer = getAnswerFromRequestBody(body, question);

          // Assert
          expect(answer).toEqual(expectedAnswer);
        });
      });

      describe('question type is time', () => {
        each([{}, {
          hour: 1,
        }, {
          minute: 40,
        }, {
          hour: 2,
          minute: 59,
        }]).it('will return a time object populated with values from request body', ({ hour, minute }) => {
          // Arrange
          const questionName = `${questionId}-${QuestionTypes.TIME}`;
          const noJsHourKey = `${questionName}-hour`;
          const noJsMinuteKey = `${questionName}-minute`;
          const expectedAnswer = { hour, minute };

          question = {
            name: questionName,
            type: QuestionTypes.TIME,
          };
          body = {
            [noJsHourKey]: hour,
            [noJsMinuteKey]: minute,
          };

          // Act
          const answer = getAnswerFromRequestBody(body, question);

          // Assert
          expect(answer).toEqual(expectedAnswer);
        });
      });
    });
  });
});
