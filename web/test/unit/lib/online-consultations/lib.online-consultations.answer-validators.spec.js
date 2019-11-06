import {
  questionAttachmentAnswerValid,
  questionBooleanAnswerValid,
  questionChoiceAnswerValid,
  questionDateAnswerValid,
  questionDateTimeAnswerValid,
  questionImageAnswerValid,
  questionNumberAnswerValid,
  questionMultipleChoiceAnswerValid,
  questionQuantityAnswerValid,
  questionStringAnswerValid,
  questionTextAnswerValid,
  questionTimeAnswerValid,
  isAnswerValid,
} from '@/lib/online-consultations/answer-validators';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import each from 'jest-each';

describe('online consultations answer validators', () => {
  const baseMessage = 'onlineConsultations.validationErrors.message.';
  let validator;
  let message;
  let additionalValue;

  describe('attachment validator', () => {
    beforeAll(() => {
      validator = questionAttachmentAnswerValid;
      message = `${baseMessage}${QuestionTypes.ATTACHMENT}`;
    });

    describe('answer is empty and not required', () => {
      each([
        undefined,
        {},
        { anotherProp: 'something' },
      ]).it('will return is valid and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: true,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is empty and required', () => {
      each([
        undefined,
        {},
        { anotherProp: 'something' },
      ]).it('will return is valid false and is empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is not empty', () => {
      describe('answer type is not accepted', () => {
        it('will return is valid false', () => {
          // Arrange
          const answer = {
            type: 'application/json',
          };
          const accept = ['image/png'];
          const expectedValidation = {
            isValid: false,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, accept);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('max size is defined', () => {
        each([{
          size: 1004,
          maxSize: 1005,
          isValid: true,
        }, {
          size: 1004,
          maxSize: 1004,
          isValid: true,
        }, {
          size: 1004,
          maxSize: 1003,
          isValid: false,
        }]).it('will return is valid true if size less than max size else false', ({ size, maxSize, isValid }) => {
          // Arrange
          const answer = {
            type: 'image/png',
            size,
          };
          const accept = ['image/png'];
          const expectedValidation = {
            isValid,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, accept, maxSize);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });
  });

  describe('boolean validator', () => {
    beforeAll(() => {
      validator = questionBooleanAnswerValid;
      message = `${baseMessage}${QuestionTypes.BOOLEAN}`;
    });

    describe('answer is empty and not required', () => {
      it('will return is valid and empty true', () => {
        // Arrange
        const answer = undefined;
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is empty and required', () => {
      it('will return is valid false and is empty true', () => {
        // Arrange
        const answer = undefined;
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is not empty', () => {
      describe('answer is not true or false', () => {
        each([
          true,
          false,
        ]).it('will return is valid false', (required) => {
          // Arrange
          const answer = 'nottrueorfalse';
          const expectedValidation = {
            isValid: false,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, required);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('answer is true or false', () => {
        each([
          'true',
          'false',
        ]).it('will return is valid true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: true,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });
  });

  describe('choice validator', () => {
    beforeAll(() => {
      validator = questionChoiceAnswerValid;
      message = `${baseMessage}${QuestionTypes.CHOICE}`;
    });

    describe('answer is empty and not required', () => {
      it('will return is valid and empty true', () => {
        // Arrange
        const answer = undefined;
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is empty and required', () => {
      it('will return is valid false and is empty true', () => {
        // Arrange
        const answer = undefined;
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is is not empty', () => {
      describe('answer is not a valid option', () => {
        it('will return is valid false', () => {
          // Arrange
          const validCodes = ['choice-1', 'choice-2'];
          const answer = 'choice-3';
          const expectedValidation = {
            isValid: false,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, validCodes);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('answer is a valid option', () => {
        it('will return is valid true', () => {
          // Arrange
          const validCodes = ['choice-1', 'choice-2'];
          const answer = 'choice-1';
          const expectedValidation = {
            isValid: true,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, validCodes);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });
  });

  describe('date validator', () => {
    beforeAll(() => {
      validator = questionDateAnswerValid;
    });

    describe('answer is empty', () => {
      describe('answer is not required', () => {
        each([
          { day: '', month: '', year: '' },
          undefined,
        ]).it('will return is valid and empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: true,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, false);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('answer is required', () => {
        each([
          { day: '', month: '', year: '' },
          undefined,
        ]).it('will return is valid and empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: false,
            message: `${baseMessage}dateEmpty`,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, true);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });

    describe('any part of the date is invalid or outside accepted range', () => {
      each([
        { day: '32', month: '12', year: '1999' },
        { day: '0', month: '12', year: '1999' },
        { day: '31', month: '13', year: '1999' },
        { day: '31', month: '0', year: '1999' },
        { day: '31', month: '12', year: '10000' },
        { day: '31', month: '12', year: '999' },
        { day: 'day', month: '12', year: '1999' },
        { day: '31', month: 'month', year: '1999' },
        { day: '31', month: '12', year: 'year' },
        { day: '10.2', month: '12', year: '1999' },
        { day: '31', month: '2.54', year: '1999' },
        { day: '31', month: '12', year: '45.6' },
      ]).it('will return is valid false', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message: `${baseMessage}date`,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is is a valid date', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = {
          day: '1',
          month: '3',
          year: '1993',
        };
        const expectedValidation = {
          isValid: true,
          message: `${baseMessage}date`,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });
  });

  describe('datetime validator', () => {
    beforeAll(() => {
      validator = questionDateTimeAnswerValid;
    });

    describe('answer is empty', () => {
      message = `${baseMessage}dateTimeEmpty`;
      describe('answer is not required', () => {
        each([
          { day: '', month: '', year: '', hour: '', minute: '' },
          undefined,
        ]).it('will return is valid and empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: true,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, false);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('answer is required', () => {
        each([
          { day: '', month: '', year: '', hour: '', minute: '' },
          undefined,
        ]).it('will return is valid and empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: false,
            message: `${baseMessage}dateTimeEmpty`,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, true);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });

    describe('any part of the datetime is invalid or outside accepted range', () => {
      each([
        { day: '32', month: '12', year: '1999', hour: '1', minute: '2' },
        { day: '0', month: '12', year: '1999', hour: '1', minute: '2' },
        { day: '31', month: '13', year: '1999', hour: '1', minute: '2' },
        { day: '31', month: '0', year: '1999', hour: '1', minute: '2' },
        { day: '31', month: '12', year: '10000', hour: '1', minute: '2' },
        { day: '31', month: '12', year: '999', hour: '1', minute: '2' },
        { day: '31', month: '12', year: '1999', hour: '24', minute: '20' },
        { day: '31', month: '12', year: '1999', hour: '-1', minute: '20' },
        { day: '31', month: '12', year: '1999', hour: '1', minute: '60' },
        { day: '31', month: '12', year: '1999', hour: '1', minute: '-1' },
        { day: 'day', month: '12', year: '1999', hour: '10', minute: '40' },
        { day: '31', month: 'month', year: '1999', hour: '10', minute: '40' },
        { day: '31', month: '12', year: 'year', hour: '10', minute: '40' },
        { day: '31', month: '12', year: '1999', hour: 'hour', minute: '40' },
        { day: '31', month: '12', year: '1999', hour: '10', minute: 'minute' },
        { day: '00.1', month: '12', year: '1999', hour: '10', minute: '40' },
        { day: '31', month: '11.5', year: '1999', hour: '10', minute: '40' },
        { day: '31', month: '12', year: '199.45', hour: '10', minute: '40' },
        { day: '31', month: '12', year: '1999', hour: '10.2', minute: '40' },
        { day: '31', month: '12', year: '1999', hour: '10', minute: '59.5' },
      ]).it('will return is valid false', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message: `${baseMessage}dateTime`,
          isEmpty: false,
        };
        message = `${baseMessage}dateTime`;
        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is partially empty', () => {
      each([
        { day: '', month: '12', year: '1999', hour: '1', minute: '2' },
        { day: '31', month: '', year: '1999', hour: '1', minute: '2' },
        { day: '31', month: '12', year: '', hour: '1', minute: '2' },
        { day: '31', month: '12', year: '1999', hour: '', minute: '2' },
        { day: '31', month: '12', year: '1999', hour: '1', minute: '' },
      ]).it('will return is valid false', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message: `${baseMessage}dateTimeEmpty`,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is is a valid datetime', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = {
          day: '1',
          month: '3',
          year: '1993',
          hour: '11',
          minute: '23',
        };
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });
  });

  describe('image validator', () => {
    beforeAll(() => {
      validator = questionImageAnswerValid;
      message = `${baseMessage}${QuestionTypes.IMAGE}`;
    });

    describe('answer is empty and not required', () => {
      each([
        {},
        undefined,
      ]).it('will return is valid and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: true,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is empty and required', () => {
      each([
        {},
        undefined,
      ]).it('will return is valid false and is empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer x or y is missing, or less than 0', () => {
      each([{
        x: 1,
      }, {
        y: 1,
      }, {
        x: -1,
        y: 1,
      }, {
        x: 1,
        y: -1,
      }]).it('will return is valid false', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is valid', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = {
          x: 100,
          y: 20,
        };
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });
  });

  describe('number validator', () => {
    let numberQuestionType;

    beforeAll(() => {
      validator = questionNumberAnswerValid;
    });

    describe('integer validation', () => {
      beforeAll(() => {
        numberQuestionType = QuestionTypes.INTEGER;
        additionalValue = 100;
      });

      describe('answer is empty and not required', () => {
        each([
          '',
          undefined,
        ]).it('will return is valid and empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: true,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, false, numberQuestionType);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('answer is not a number', () => {
        each([
          'not a number',
          '1203notanumber',
          '4 3 23',
        ]).it('will return is valid false', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: false,
            message: `${baseMessage}integer`,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, numberQuestionType);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('max is defined', () => {
        describe('answer is greater than max', () => {
          it('will return is valid false', () => {
            // Arrange
            const answer = 200;
            const max = 100;
            const expectedValidation = {
              isValid: false,
              isEmpty: false,
              additionalValue,
              message: `${baseMessage}overMaxValueNumber`,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, undefined, max);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });

        describe('answer is less than or equal to max', () => {
          each([
            200,
            199,
            0,
            -1,
          ]).it('will return is valid false', (answer) => {
            // Arrange
            const max = 200;
            const expectedValidation = {
              isValid: true,
              isEmpty: false,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, undefined, max);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });
      });

      describe('min is defined', () => {
        describe('answer is less than min', () => {
          it('will return is valid false', () => {
            // Arrange
            const answer = 99;
            const min = 100;
            const expectedValidation = {
              isValid: false,
              isEmpty: false,
              message: `${baseMessage}underMinValueNumber`,
              additionalValue,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, min);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });

        describe('answer is greater than or equal to min', () => {
          each([
            -200,
            -199,
            0,
            1,
          ]).it('will return is valid false', (answer) => {
            // Arrange
            const min = -200;
            const expectedValidation = {
              isValid: true,
              isEmpty: false,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, min);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });
      });

      describe('answer is not in an integer format', () => {
        it('will return is valid false', () => {
          // Arrange
          const answer = 12.3;
          const expectedValidation = {
            isValid: false,
            message: `${baseMessage}integer`,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, numberQuestionType);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });

    describe('decimal validation', () => {
      beforeAll(() => {
        numberQuestionType = QuestionTypes.DECIMAL;
      });

      describe('answer is empty and not required', () => {
        each([
          '',
          undefined,
        ]).it('will return is valid and empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: true,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, false, numberQuestionType);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('answer is not a number', () => {
        each([
          'not a number',
          '1203notanumber',
          '4 3 23',
        ]).it('will return is valid false', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: false,
            message: `${baseMessage}decimal`,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, numberQuestionType);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('max is defined', () => {
        describe('answer is greater than max', () => {
          it('will return is valid false', () => {
            // Arrange
            const answer = 200.2;
            const max = 100.2;
            const expectedValidation = {
              isValid: false,
              message: `${baseMessage}overMaxValueNumber`,
              additionalValue: 100.2,
              isEmpty: false,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, undefined, max);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });

        describe('answer is less than or equal to max', () => {
          each([
            200.5,
            199.9,
            0,
            -1.24,
            -50,
          ]).it('will return is valid false', (answer) => {
            // Arrange
            const max = 200.5;
            const expectedValidation = {
              isValid: true,
              isEmpty: false,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, undefined, max);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });
      });

      describe('min is defined', () => {
        describe('answer is less than min', () => {
          it('will return is valid false', () => {
            // Arrange
            const answer = 100.05;
            const min = 100.1;
            const expectedValidation = {
              isValid: false,
              message: `${baseMessage}underMinValueNumber`,
              additionalValue: 100.1,
              isEmpty: false,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, min);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });

        describe('answer is greater than or equal to min', () => {
          each([
            -200.5,
            -199.94,
            0,
            1.123,
            150,
          ]).it('will return is valid false', (answer) => {
            // Arrange
            const min = -200.5;
            const expectedValidation = {
              isValid: true,
              isEmpty: false,
            };

            // Act
            const validation = validator(answer, true, numberQuestionType, min);

            // Assert
            expect(validation).toEqual(expectedValidation);
          });
        });
      });
    });
  });

  describe('multiple choice validator', () => {
    beforeAll(() => {
      validator = questionMultipleChoiceAnswerValid;
      message = `${baseMessage}multiple_choiceAtLeastOneRequired`;
    });

    describe('answer is empty and not required', () => {
      each([
        undefined,
        [],
      ]).it('will return is valid and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: true,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is not a valid option', () => {
      each([{
        answer: ['choice-1'],
        validCodes: [],
      }, {
        answer: ['choice-1'],
        validCodes: ['choice-2'],
      }, {
        answer: ['choice-1'],
      }]).it('will return is valid false', ({ answer, validCodes }) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true, false, validCodes);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('not all answers are in valid options', () => {
      it('will return is valid false', () => {
        // Arrange
        const answer = ['choice-1', 'choice-2'];
        const validCodes = ['choice-2'];
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true, false, validCodes);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('all answers are in valid options', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = ['choice-1', 'choice-2'];
        const validCodes = ['choice-2', 'choice-1', 'choice-3'];
        const options = [
          {
            code: 'choice-1',
            required: false,
          },
          {
            code: 'choice-2',
            required: false,
          },
          {
            code: 'choice-3',
            required: false,
          },
        ];
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true, false, options, validCodes);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('all options are required', () => {
      beforeEach(() => {
        message = `${baseMessage}multiple_choiceAllRequired`;
      });

      describe('all options are selected', () => {
        it('will return is valid true', () => {
          // Arrange
          const answer = ['choice-1', 'choice-2', 'choice-3'];
          const validCodes = ['choice-2', 'choice-1', 'choice-3'];
          const expectedValidation = {
            isValid: true,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, true, validCodes);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
      describe('all options are not selected', () => {
        it('will return is valid false', () => {
          // Arrange
          const answer = ['choice-1', 'choice-3'];
          const validCodes = ['choice-2', 'choice-1', 'choice-3'];
          const options = [
            {
              code: 'choice-1',
              required: false,
            },
            {
              code: 'choice-2',
              required: false,
            },
            {
              code: 'choice-3',
              required: false,
            },
          ];
          const expectedValidation = {
            isValid: false,
            message,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, true, options, validCodes);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });
  });

  describe('quantity validator', () => {
    beforeAll(() => {
      validator = questionQuantityAnswerValid;
    });

    describe('answer is empty and not required', () => {
      each([{
        quantity: '',
        unit: '',
      }, undefined]).it('will return is valid and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: true,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer unit is not a valid option', () => {
      it('will return is valid false', () => {
        // Arrange
        const answer = {
          quantity: 20,
          unit: 's',
        };
        const validCodes = ['m'];
        const expectedValidation = {
          isValid: false,
          message: `${baseMessage}quantityUnit`,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true, undefined, undefined, validCodes);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('max quantity is defined and answer quantity is greater than max', () => {
      it('will return is valid false', () => {
        // Arrange
        const answer = {
          quantity: 20,
          unit: 'm',
        };
        const validCodes = ['m'];
        const max = 15;
        const expectedValidation = {
          additionalValue: 15,
          isValid: false,
          message: `${baseMessage}overMaxValueNumber`,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true, undefined, max, validCodes);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer quantity is less than min (0)', () => {
      it('will return is valid false', () => {
        // Arrange
        const answer = {
          quantity: -20,
          unit: 'm',
        };
        const validCodes = ['m'];
        const max = 15;
        const min = 0;
        const expectedValidation = {
          additionalValue: 0,
          isValid: false,
          message: `${baseMessage}underMinValueNumber`,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true, min, max, validCodes);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is valid', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = {
          quantity: 20,
          unit: 'm',
        };
        const validCodes = ['m'];
        const max = 20;
        const expectedValidation = {
          isValid: true,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true, undefined, max, validCodes);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });
  });

  describe('string validator', () => {
    beforeAll(() => {
      validator = questionStringAnswerValid;
      message = `${baseMessage}${QuestionTypes.STRING}`;
    });

    describe('answer is empty or white space and not required', () => {
      each([
        '',
        '    ',
        '\t\r\n',
        undefined,
      ]).it('will return is valid and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is empty or white space and required', () => {
      each([
        '',
        '    ',
        '\t\r\n',
        undefined,
      ]).it('will return is valid false and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is not empty ', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = 'This a test answer';
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });
  });

  describe('text validator', () => {
    beforeAll(() => {
      validator = questionTextAnswerValid;
      message = `${baseMessage}${QuestionTypes.TEXT}`;
    });

    describe('answer is empty or white space and not required', () => {
      each([
        '',
        '    ',
        '\t\r\n',
        undefined,
      ]).it('will return is valid and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: true,
          additionalValue: undefined,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, false);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is empty or white space and required', () => {
      each([
        '',
        '    ',
        '\t\r\n',
        undefined,
      ]).it('will return is valid false and empty true', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message: `${baseMessage}text`,
          additionalValue: undefined,
          isEmpty: true,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is not empty and max length is undefined', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = 'This is a test answer';
        const expectedValidation = {
          isValid: true,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('max length is defined', () => {
      describe('answer is greater than max length', () => {
        it('will return is valid false', () => {
          // Arrange
          const answer = 'This is a test answer';
          const maxLength = 5;
          const expectedValidation = {
            isValid: false,
            message: `${baseMessage}textTooLong`,
            additionalValue: 5,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, maxLength);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
      describe('answer is less than or equal to max length', () => {
        each([{
          answer: 'T',
          maxLength: 5,
        }, {
          answer: 'This!',
          maxLength: 5,
        }]).it('will return is valid true', ({ answer, maxLength }) => {
          // Arrange
          const expectedValidation = {
            isValid: true,
            isEmpty: false,
          };

          // Act
          const validation = validator(answer, true, maxLength);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });
  });

  describe('time validator', () => {
    beforeAll(() => {
      validator = questionTimeAnswerValid;
      message = `${baseMessage}${QuestionTypes.TIME}`;
    });

    describe('answer is empty', () => {
      describe('answer is not required', () => {
        each([
          { hour: '', minute: '' },
          undefined,
        ]).it('will return is valid and empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: true,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, false);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });

      describe('answer is required', () => {
        each([
          { hour: '', minute: '' },
          undefined,
        ]).it('will return is valid false and is empty true', (answer) => {
          // Arrange
          const expectedValidation = {
            isValid: false,
            message,
            isEmpty: true,
          };

          // Act
          const validation = validator(answer, true);

          // Assert
          expect(validation).toEqual(expectedValidation);
        });
      });
    });

    describe('any part of the time is invalid or outside accepted range', () => {
      each([
        { hour: '24', minute: '20' },
        { hour: '-1', minute: '20' },
        { hour: '1', minute: '60' },
        { hour: '1', minute: '-1' },
        { hour: 'hour', minute: '40' },
        { hour: '10', minute: 'minute' },
        { hour: '01.02', minute: '40' },
        { hour: '10', minute: '10.20' },
      ]).it('will return is valid false', (answer) => {
        // Arrange
        const expectedValidation = {
          isValid: false,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });

    describe('answer is is a valid time', () => {
      it('will return is valid true', () => {
        // Arrange
        const answer = {
          hour: '11',
          minute: '23',
        };
        const expectedValidation = {
          isValid: true,
          message,
          isEmpty: false,
        };

        // Act
        const validation = validator(answer, true);

        // Assert
        expect(validation).toEqual(expectedValidation);
      });
    });
  });

  describe('isAnswerValid validator', () => {
    beforeAll(() => {
      validator = isAnswerValid;
    });

    describe('all question types', () => {
      each([
        QuestionTypes.ATTACHMENT,
        QuestionTypes.BOOLEAN,
        QuestionTypes.CHOICE,
        QuestionTypes.DATE,
        QuestionTypes.DATETIME,
        QuestionTypes.DECIMAL,
        QuestionTypes.IMAGE,
        QuestionTypes.INTEGER,
        QuestionTypes.MULTIPLE_CHOICE,
        QuestionTypes.QUANTITY,
        QuestionTypes.STRING,
        QuestionTypes.TEXT,
        QuestionTypes.TIME,
      ]).it('will call the appropriate validation method for each type', (questionType) => {
        // Arrange
        const question = {
          type: questionType,
          required: true,
        };

        // Act
        const validation = isAnswerValid(undefined, question);

        // Assert
        expect(validation).toBeDefined();
        expect(validation.message.includes(questionType)).toBe(true);
      });

      describe('unknown question type', () => {
        it('will return is valid false with default error message', () => {
          // Act
          const validation = isAnswerValid();

          // Assert
          expect(validation).toBeDefined();
          expect(validation.isValid).toBe(false);
          expect(validation.message).toEqual(`${baseMessage}default`);
        });
      });
    });
  });
});
