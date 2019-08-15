import Orchestrator from '@/components/online-consultations/Orchestrator';
import { initialState } from '@/store/modules/onlineConsultations/mutation-types';
import { mount, shallowMount } from '../../helpers';
import each from 'jest-each';

jest.mock('@/services/event-bus');
jest.mock('@/services/native-app');

let orchestrator;

const dispatch = jest.fn();

const baseQuestion = type => ({
  id: `${type}-id`,
  tag: 'p',
  text: `${type} question`,
  isLegend: false,
  type,
  name: `${type}-name`,
  required: true,
});

const store = {
  app: {
    $env: {},
  },
  state: {
    device: {
      isNativeApp: true,
    },
    onlineConsultations: initialState(),
    serviceJourneyRules: {
      rules: {
        cdssAdmin: {
          provider: 'stubs',
          serviceDefinition: 'testId',
        },
        cdssAdvice: {
          provider: 'stubs',
          serviceDefinition: 'testId',
        },
      },
    },
  },
  dispatch,
};

const mountOrchestrator = ({ stubbed = true, methods = {} } = {}) => {
  orchestrator = (stubbed ? shallowMount : mount)(Orchestrator, {
    propsData: {
      provider: 'stubs',
      serviceDefinitionId: 'NHS_ADMIN',
    },
    $store: store,
    methods,
    $style: {
      container: 'container',
      errorDialog: 'errorDialog',
      button: 'button',
    },
  });
};

describe('orchestrator', () => {
  afterEach(() => {
    dispatch.mockClear();
  });

  describe('template', () => {
    describe('question is defined and status is data-required', () => {
      beforeEach(() => {
        store.state.onlineConsultations.status = 'data-required';
      });

      describe('validation error dialog', () => {
        it('will display validationErrorMessage when isValidationError is true', () => {
          // Arrange
          const validationHeader = 'appointments.admin_help.errors.validation.header';
          const validationErrorMessage = 'test.validation.error.message';
          const expectedValidationHeader = `translate_${validationHeader}`;
          const expectedValidationErrorMessage = `translate_${validationErrorMessage}`;

          store.state.onlineConsultations.validationError = true;
          store.state.onlineConsultations.validationErrorMessage = validationErrorMessage;
          store.state.onlineConsultations.question = { type: 'integer', text: 'text' };

          // Act
          mountOrchestrator();

          // Assert
          const messageDialog = orchestrator.find('message-dialog-stub');
          const dialogHeader = orchestrator.find('message-text-stub');
          const dialogMessage = orchestrator.find('message-list-stub > li');

          expect(messageDialog.vm.$props.messageType).toEqual('error');
          expect(messageDialog.vm.$attrs.role).toEqual('alert');
          expect(dialogHeader.element.innerHTML.trim()).toEqual(expectedValidationHeader);
          expect(dialogMessage.element.innerHTML).toEqual(expectedValidationErrorMessage);
        });
      });

      describe('question components', () => {
        const questionSelector = 'question-stub';
        let question;

        let id;
        let questionTag;
        let text;
        let error;
        let isLegend;
        let name;
        let required;
        let errorText;
        let accept;
        let maxSize;
        let options;
        let min;
        let max;
        let source;
        let maxValue;
        let maxLength;
        let allOptionsRequired;

        let questionVm;
        let questionInputVm;

        afterEach(() => {
          ({ id, questionTag, text, error, isLegend } = questionVm);
          ({ name, required, errorText } = questionInputVm);

          const questionInputId = questionInputVm.id;
          const questionInputError = questionInputVm.error;

          // Assert
          if (!['boolean', 'choice', 'multiple_choice', 'quantity'].includes(question.type)) {
            expect(questionInputId).toEqual(question.name);
          }
          expect(questionInputError).toEqual(true);
          expect(id).toEqual(question.id);
          expect(questionTag).toEqual(question.tag);
          expect(text).toEqual(question.text);
          expect(error).toEqual(true);
          expect(isLegend).toEqual(question.isLegend);
          expect(name).toEqual(question.name);
          expect(required).toEqual(question.required);
          expect(errorText).toEqual(['translate_test.validation.error.message']);
        });

        describe('attachment', () => {
          it('will render the question input assigning appropriate values to props', () => {
            // Arrange
            question = baseQuestion('attachment');
            question.accept = ['image/png'];
            question.maxSize = 1024;

            const questionInputSelector = 'questionattachment-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;

            ({ accept, maxSize } = questionInputVm);

            // Assert
            expect(accept).toEqual(question.accept);
            expect(maxSize).toEqual(question.maxSize);
          });
        });

        describe('boolean', () => {
          it('will render the question input assigning appropriate values to props', () => {
            // Arrange
            question = baseQuestion('boolean');
            const questionInputSelector = 'questionboolean-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;
          });
        });

        describe('choice', () => {
          it('will render the question input assigning appropriate values to props', () => {
            question = baseQuestion('choice');
            question.isLegend = true;
            question.options = ['choice-1'];

            const questionInputSelector = 'questionchoice-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;

            ({ options } = questionInputVm);

            // Assert
            expect(options).toEqual(question.options);
          });
        });

        describe('date and datetime', () => {
          each([{
            type: 'date',
            selector: 'questiondate-stub',
          }, {
            type: 'dateTime',
            selector: 'questiondatetime-stub',
          }]).it('will render the question input assigning appropriate values to props', ({ type, selector }) => {
            // Arrange
            question = baseQuestion(type);
            const questionInputSelector = selector;
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;
          });
        });

        describe('decimal and integer', () => {
          each([
            'decimal',
            'integer',
          ]).it('will render the question input assigning appropriate values to props', (type) => {
            // Arrange
            question = baseQuestion(type);
            question.min = 20;
            question.max = 200;

            const questionInputSelector = 'questionnumber-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;

            ({ min, max } = questionInputVm);

            // Assert
            expect(min).toEqual(question.min);
            expect(max).toEqual(question.max);
          });
        });

        describe('image', () => {
          it('will render the question input assigning appropriate values to props', () => {
            // Arrange
            question = baseQuestion('image');
            question.source = 'image source';

            const questionInputSelector = 'questionimage-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;

            ({ source } = questionInputVm);

            // Assert
            expect(source).toEqual(question.source);
          });
        });

        describe('multiple_choice', () => {
          each([
            true,
            false,
          ]).it('will render the question input assigning appropriate values to props', (allRequired) => {
            question = baseQuestion('multiple_choice');
            question.isLegend = true;
            question.options = ['choice-1'];
            question.allOptionsRequired = allRequired;

            const questionInputSelector = 'questionmultiplechoice-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;

            ({ options, allOptionsRequired } = questionInputVm);

            // Assert
            expect(options).toEqual(question.options);
            expect(allOptionsRequired).toEqual(question.allOptionsRequired);
          });
        });

        describe('quantity', () => {
          it('will render the question input assigning appropriate values to props', () => {
            // Arrange
            question = baseQuestion('quantity');
            question.maxValue = 100;
            question.options = ['m', 's'];

            const questionInputSelector = 'questionquantity-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;

            ({ options, maxValue } = questionInputVm);

            // Assert
            expect(options).toEqual(question.options);
            expect(maxValue).toEqual(question.maxValue);
          });
        });

        describe('string', () => {
          it('will render the question input assigning appropriate values to props', () => {
            // Arrange
            question = baseQuestion('string');

            const questionInputSelector = 'questionstring-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;
          });
        });

        describe('text', () => {
          it('will render the question input assigning appropriate values to props', () => {
            // Arrange
            question = baseQuestion('text');
            question.maxLength = '240';

            const questionInputSelector = 'questiontext-stub';
            store.state.onlineConsultations.question = question;

            // Act
            mountOrchestrator();

            questionVm = orchestrator.find(questionSelector).vm;
            questionInputVm = orchestrator.find(questionInputSelector).vm;

            ({ maxLength } = questionInputVm);

            // Assert
            expect(maxLength).toEqual(question.maxLength);
          });
        });
      });

      describe('continue button', () => {
        it('will have continue text', () => {
          // Arrange
          store.state.onlineConsultations.question = baseQuestion('integer');

          // Act
          mountOrchestrator();

          // Assert
          const continueButton = orchestrator.find('generic-button-stub');
          expect(continueButton.exists()).toBe(true);
          expect(continueButton.text()).toEqual('translate_appointments.admin_help.orchestrator.continueButton');
        });

        describe('when clicked', () => {
          it('will call continueClicked method', async () => {
            // Arrange
            store.state.onlineConsultations.question = baseQuestion('integer');
            const continueClicked = jest.fn();
            mountOrchestrator({ stubbed: false, methods: { continueClicked } });
            const continueButton = orchestrator.find('button');

            // Act
            continueButton.trigger('click');

            // Assert
            expect(continueClicked).toHaveBeenCalled();
          });
        });
      });

      describe('desktop back link', () => {
        it('will not be shown', () => {
          // Arrange
          store.state.device.isNativeApp = false;

          // Act
          mountOrchestrator();
          const backLink = orchestrator.find('desktopgenericbacklink-stub');

          // Assert
          expect(backLink.exists()).toBe(false);
        });
      });

      describe('back button', () => {
        it('will be shown', () => {
          // Act
          mountOrchestrator();

          // Assert
          const backButton = orchestrator.find('back-button-stub');
          expect(backButton.exists()).toBe(true);
          expect(backButton.vm.text).toEqual('translate_appointments.admin_help.orchestrator.endMyConsultationButton');
          expect(backButton.vm.gotoPath).toEqual('/');
        });
      });

      describe('nojs', () => {
        it('will wrap question/input in a post no-js-form with a value bound to noJsState', () => {
          // Arrange
          store.state.onlineConsultations.question = {
            type: 'integer',
            text: 'no js question text',
          };
          const expectedNoJsState = {
            onlineConsultations: store.state.onlineConsultations,
          };

          // Act
          mountOrchestrator();

          // Assert
          const noJsForm = orchestrator.find('no-js-form-stub');
          const questionComponent = noJsForm.find('question-stub');

          expect(noJsForm.exists()).toBe(true);
          expect(noJsForm.vm.value).toEqual(expectedNoJsState);
          expect(noJsForm.vm.method).toEqual('post');
          expect(questionComponent.exists()).toBe(true);
          expect(questionComponent.vm.text).toEqual(store.state.onlineConsultations.question.text);
        });

        it('will wrap back button in a form with an action bound to /', () => {
          // Arrange
          store.state.onlineConsultations.question = {
            type: 'integer',
            text: 'no js question text',
          };

          // Act
          mountOrchestrator();

          // Assert
          const form = orchestrator.find('form[action=\\/]');
          const backButton = form.find('back-button-stub');

          expect(form.exists()).toBe(true);
          expect(backButton.exists()).toBe(true);
        });
      });
    });

    describe('question is defined and status is success', () => {
      beforeEach(() => {
        store.state.onlineConsultations = initialState();
        store.state.onlineConsultations.status = 'success';
      });

      describe('referralRequests present', () => {
        it('will render a question for each request setting text to the referral request description', () => {
          // Arrange
          const referralRequests = [{ description: 'rr1' }, { description: 'rr2' }];
          const expectedDescriptions = ['rr1', 'rr2'];
          store.state.onlineConsultations.referralRequests = referralRequests;
          mountOrchestrator();

          // Act
          const renderedDescriptions = orchestrator.findAll('#result-container div question-stub').wrappers.map(w => w.vm.text);

          // Assert
          expect(renderedDescriptions).toEqual(expectedDescriptions);
        });
      });

      describe('carePlans present', () => {
        it('will render a question for each care plan title and care plan activity', () => {
          // Arrange
          const carePlans = [{
            title: 'careplan1',
            activities: [
              'activity1',
              'activity2',
            ],
          }, {
            title: 'careplan2',
            activities: [
              'activity3',
              'activity4',
            ],
          }];
          const expectedTitlesAndActivities = ['careplan1', 'activity1', 'activity2', 'careplan2', 'activity3', 'activity4'];
          store.state.onlineConsultations.status = 'success';
          store.state.onlineConsultations.carePlans = carePlans;
          mountOrchestrator();

          // Act
          const renderedTitlesAndActivities = orchestrator.findAll('#result-container div question-stub').wrappers.map(w => w.vm.text);

          // Assert
          expect(renderedTitlesAndActivities).toEqual(expectedTitlesAndActivities);
        });
      });

      describe('desktop generic back link', () => {
        each([{
          isNativeApp: true,
          status: 'success',
          isShown: false,
        }, {
          isNativeApp: false,
          status: 'success',
          isShown: true,
        }, {
          isNativeApp: false,
          status: 'data-required',
          isShown: false,
        }]).it('will be shown if on desktop and status is success', ({ isNativeApp, status, isShown }) => {
          // Arrange
          store.state.onlineConsultations.referralRequests = [{ description: 'rr' }];
          store.state.onlineConsultations.status = status;
          store.state.device.isNativeApp = isNativeApp;
          mountOrchestrator();

          // Act
          const desktopBackLink = orchestrator.find('desktopgenericbacklink-stub');

          // Assert
          expect(desktopBackLink.exists()).toBe(isShown);
        });
      });
    });
  });
});
