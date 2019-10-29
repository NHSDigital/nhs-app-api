import { getQuestion, getConditionsList } from '@/lib/online-consultations/mappers/item';
import mapHtmlTags from '@/lib/online-consultations/mappers/html-tags';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';

jest.mock('@/lib/online-consultations/mappers/html-tags');

describe('online consultations mappers item', () => {
  describe('getQuestion', () => {
    let item;

    describe('exception occurs', () => {
      it('will return undefined', () => {
        // Act
        const question = getQuestion(item);

        // Assert
        expect(question).toBeUndefined();
      });
    });

    describe('item is defined', () => {
      const defaultLinkId = 'test-link-id';
      const defaultRepeats = false;
      const defaultRequired = true;
      const defaultText = 'This is a question!';

      let expectedQuestion;

      beforeEach(() => {
        mapHtmlTags.mockClear();
        mapHtmlTags.mockReturnValue('mapped-html');

        item = {
          linkId: defaultLinkId,
          repeats: defaultRepeats,
          required: defaultRequired,
          text: defaultText,
        };

        expectedQuestion = {
          id: item.linkId,
          isLegend: false,
          repeats: item.repeats,
          required: item.required,
          text: 'mapped-html',
        };
      });

      describe('item type group', () => {
        beforeEach(() => {
          item.type = QuestionTypes.GROUP;
        });

        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.item = [{
            linkId: 'code-1',
            text: 'Option 1',
            required: true,
          }, {
            linkId: 'code-2',
            text: 'Option 2',
            required: true,
          }];
          expectedQuestion.type = QuestionTypes.MULTIPLE_CHOICE;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.MULTIPLE_CHOICE}`;
          expectedQuestion.repeats = true;
          expectedQuestion.isLegend = true;
          expectedQuestion.options = [{
            code: 'code-1',
            label: 'mapped-html',
            selected: false,
            required: true,
          }, {
            code: 'code-2',
            label: 'mapped-html',
            selected: false,
            required: true,
          }];
          expectedQuestion.validCodes = ['code-1', 'code-2'];
          expectedQuestion.allOptionsRequired = true;

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(3);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });

        describe('item has no options', () => {
          it('will return undefined', () => {
            // Arrange
            item.item = [];

            // Act
            const question = getQuestion(item);

            // Assert
            expect(question).toBeUndefined();
          });
        });

        describe('item has no text property', () => {
          it('will return undefined', () => {
            // Arrange
            item.text = undefined;

            // Act
            const question = getQuestion(item);

            // Assert
            expect(question).toBeUndefined();
          });
        });
      });

      describe('item type attachment', () => {
        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.type = QuestionTypes.ATTACHMENT;
          expectedQuestion.type = QuestionTypes.ATTACHMENT;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.ATTACHMENT}`;
          expectedQuestion.maxSize = 1048576;
          expectedQuestion.accept = ['image/png', 'image/jpeg'];

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(1);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });
      });

      describe('item type boolean', () => {
        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.type = QuestionTypes.BOOLEAN;
          expectedQuestion.type = QuestionTypes.BOOLEAN;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.BOOLEAN}`;
          expectedQuestion.isLegend = true;

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(1);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });
      });

      describe('item type choice', () => {
        beforeEach(() => {
          item.type = QuestionTypes.CHOICE;
        });

        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.option = [{
            valueCoding: {
              code: 'code-1',
              display: 'Option 1',
            },
          }, {
            valueCoding: {
              code: 'code-2',
              display: 'Option 2',
            },
          }];
          expectedQuestion.type = QuestionTypes.CHOICE;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.CHOICE}`;
          expectedQuestion.isLegend = true;
          expectedQuestion.options = [{
            code: 'code-1',
            label: 'mapped-html',
          }, {
            code: 'code-2',
            label: 'mapped-html',
          }];
          expectedQuestion.validCodes = ['code-1', 'code-2'];

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(3);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });

        describe('item has no options', () => {
          it('will return undefined', () => {
            // Arrange
            item.option = [];

            // Act
            const question = getQuestion(item);

            // Assert
            expect(question).toBeUndefined();
          });
        });
      });

      describe('item type decimal', () => {
        beforeEach(() => {
          item.type = QuestionTypes.DECIMAL;
          expectedQuestion.type = QuestionTypes.DECIMAL;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.DECIMAL}`;
          expectedQuestion.tag = 'label';
        });

        it('will return a question with appropriate properties set', () => {
          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(1);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });

        describe('extensions are available', () => {
          describe('minValue extension present', () => {
            it('will set min property on question from minValue.valueDecimal', () => {
              // Arrange
              item.extension = [{
                url: 'http://fhir.extensions.somewhere/minValue',
                valueDecimal: 1.23,
              }];
              expectedQuestion.min = 1.23;

              // Act
              const question = getQuestion(item);

              // Assert
              expect(question).toEqual(expectedQuestion);
              expect(mapHtmlTags).toBeCalledTimes(1);
              expect(mapHtmlTags).toBeCalledWith(item.text);
            });
          });
          describe('maxValue extension present', () => {
            it('will set max property on question from maxValue.valueDecimal', () => {
              // Arrange
              item.extension = [{
                url: 'http://fhir.extensions.somewhere/maxValue',
                valueDecimal: 124.32,
              }];
              expectedQuestion.max = 124.32;

              // Act
              const question = getQuestion(item);

              // Assert
              expect(question).toEqual(expectedQuestion);
              expect(mapHtmlTags).toBeCalledTimes(1);
              expect(mapHtmlTags).toBeCalledWith(item.text);
            });
          });
        });
      });

      describe('item type image', () => {
        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.type = QuestionTypes.STRING;
          item.initialAttachment = {
            url: 'http://fhir.attachment.somewhere/something.jpg',
          };
          expectedQuestion.type = QuestionTypes.IMAGE;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.IMAGE}`;
          expectedQuestion.source = item.initialAttachment.url;

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(1);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });
      });

      describe('item type integer', () => {
        beforeEach(() => {
          item.type = QuestionTypes.INTEGER;
          expectedQuestion.type = QuestionTypes.INTEGER;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.INTEGER}`;
          expectedQuestion.tag = 'label';
        });

        it('will return a question with appropriate properties set', () => {
          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(1);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });

        describe('extensions are available', () => {
          describe('minValue extension present', () => {
            it('will set min property on question from minValue.valueInteger', () => {
              // Arrange
              item.extension = [{
                url: 'http://fhir.extensions.somewhere/minValue',
                valueInteger: 4,
              }];
              expectedQuestion.min = 4;

              // Act
              const question = getQuestion(item);

              // Assert
              expect(question).toEqual(expectedQuestion);
              expect(mapHtmlTags).toBeCalledTimes(1);
              expect(mapHtmlTags).toBeCalledWith(item.text);
            });
          });
          describe('maxValue extension present', () => {
            it('will set max property on question from maxValue.valueInteger', () => {
              // Arrange
              item.extension = [{
                url: 'http://fhir.extensions.somewhere/maxValue',
                valueInteger: 125,
              }];
              expectedQuestion.max = 125;

              // Act
              const question = getQuestion(item);

              // Assert
              expect(question).toEqual(expectedQuestion);
              expect(mapHtmlTags).toBeCalledTimes(1);
              expect(mapHtmlTags).toBeCalledWith(item.text);
            });
          });
        });
      });

      describe('item type multiple choice', () => {
        beforeEach(() => {
          item.type = QuestionTypes.CHOICE;
          item.repeats = true;
        });

        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.option = [{
            valueCoding: {
              code: 'code-1',
              display: 'Option 1',
            },
          }, {
            valueCoding: {
              code: 'code-2',
              display: 'Option 2',
            },
          }];
          expectedQuestion.type = QuestionTypes.MULTIPLE_CHOICE;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.MULTIPLE_CHOICE}`;
          expectedQuestion.repeats = true;
          expectedQuestion.isLegend = true;
          expectedQuestion.options = [{
            code: 'code-1',
            label: 'mapped-html',
            selected: false,
          }, {
            code: 'code-2',
            label: 'mapped-html',
            selected: false,
          }];
          expectedQuestion.validCodes = ['code-1', 'code-2'];

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(3);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });

        describe('item has no options', () => {
          it('will return undefined', () => {
            // Arrange
            item.option = [];

            // Act
            const question = getQuestion(item);

            // Assert
            expect(question).toBeUndefined();
          });
        });
      });

      describe('item type quantity', () => {
        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.type = QuestionTypes.QUANTITY;
          item.extension = [{
            valueCoding: {
              code: 'm',
              display: 'min',
            },
          }, {
            valueCoding: {
              code: 's',
              display: 'sec',
            },
          }, {
            url: 'minValue',
            valueInteger: 0,
          }, {
            url: 'minValue',
            valueInteger: 0,
          }];
          expectedQuestion.options = [{
            code: 'm',
            label: 'min',
          }, {
            code: 's',
            label: 'sec',
          }];
          expectedQuestion.validCodes = ['m', 's'];
          expectedQuestion.type = QuestionTypes.QUANTITY;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.QUANTITY}`;
          expectedQuestion.min = 0;

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(1);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });
      });

      describe('item type string', () => {
        it('will return a question with appropriate properties set', () => {
          // Arrange
          item.type = QuestionTypes.STRING;
          expectedQuestion.type = QuestionTypes.STRING;
          expectedQuestion.name = `${item.linkId}-${QuestionTypes.STRING}`;
          expectedQuestion.tag = 'label';

          // Act
          const question = getQuestion(item);

          // Assert
          expect(question).toEqual(expectedQuestion);
          expect(mapHtmlTags).toBeCalledTimes(1);
          expect(mapHtmlTags).toBeCalledWith(item.text);
        });
      });
    });
  });

  describe('getConditionsList', () => {
    it('will map the given questionnaire items and subitems into categories', () => {
      // Arrange
      const expectedConditionsList = [{
        category: 'Allergies',
        items: [{ title: 'Hayfever', id: 'HFV' }, { title: 'Urticaria', id: 'URT' }],
      }, {
        category: 'Breathing Problems',
        items: [{ title: 'Asthma', id: 'AST' }, { title: 'Bronchitis', id: 'BRC' }],
      }];
      const questionnaire = {
        item: [{
          text: 'Allergies',
          item: [{ linkId: 'HFV', text: 'Hayfever' }, { linkId: 'URT', text: 'Urticaria' }],
        }, {
          text: 'Breathing Problems',
          item: [{ linkId: 'AST', text: 'Asthma' }, { linkId: 'BRC', text: 'Bronchitis' }],
        }],
      };

      // Act
      const conditionsList = getConditionsList(questionnaire);

      // Assert
      expect(conditionsList).toEqual(expectedConditionsList);
    });
  });
});
