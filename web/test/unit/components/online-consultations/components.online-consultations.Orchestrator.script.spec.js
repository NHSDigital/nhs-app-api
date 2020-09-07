import each from 'jest-each';
import Orchestrator from '@/components/online-consultations/Orchestrator';
import { initialState } from '@/store/modules/onlineConsultations/mutation-types';
import QuestionAttachment from '@/components/online-consultations/QuestionAttachment';
import QuestionBoolean from '@/components/online-consultations/QuestionBoolean';
import QuestionChoice from '@/components/online-consultations/QuestionChoice';
import QuestionDate from '@/components/online-consultations/QuestionDate';
import QuestionDateTime from '@/components/online-consultations/QuestionDateTime';
import QuestionImage from '@/components/online-consultations/QuestionImage';
import QuestionMultipleChoice from '@/components/online-consultations/QuestionMultipleChoice';
import QuestionNumber from '@/components/online-consultations/QuestionNumber';
import QuestionQuantity from '@/components/online-consultations/QuestionQuantity';
import QuestionString from '@/components/online-consultations/QuestionString';
import QuestionText from '@/components/online-consultations/QuestionText';
import QuestionTime from '@/components/online-consultations/QuestionTime';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import NativeApp from '@/services/native-app';
import { redirectTo } from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';
import { mount, shallowMount, createScrollTo } from '../../helpers';

jest.mock('@/services/event-bus');
jest.mock('@/services/native-app');
jest.mock('@/lib/utils');

let orchestrator;

const testCarePlan = {
  title: 'careplan',
};
const testReferralRequest = {
  description: 'referralrequest',
};

const scrollTo = createScrollTo();
const dispatch = jest.fn();

const store = {
  $env: {},
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

const mountOrchestrator = (shallow = true) => {
  orchestrator = (shallow ? shallowMount : mount)(Orchestrator, {
    propsData: {
      provider: 'stubs',
      serviceDefinitionId: 'NHS_ADMIN',
    },
    $store: store,
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
    scrollTo.mockClear();
    NativeApp.resetPageFocus.mockClear();
    redirectTo.mockClear();
  });

  describe('computed', () => {
    afterEach(() => {
      store.state.device.isNativeApp = true;
      store.state.onlineConsultations = initialState();
    });

    describe('isNativeApp', () => {
      each([
        true,
        false,
      ]).it('will get from store', (isNativeApp) => {
        // Arrange
        store.state.device.isNativeApp = isNativeApp;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.isNativeApp).toBe(isNativeApp);
      });
    });

    describe('isValidationError', () => {
      each([
        true,
        false,
      ]).it('will get from store', (validationError) => {
        // Arrange
        store.state.onlineConsultations.validationError = validationError;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.isValidationError).toBe(validationError);
      });
    });

    describe('validationErrorMessage', () => {
      it('will get i18n key from store', () => {
        // Arrange
        const validationErrorMessage = 'onlineConsultations.validationErrors.message.attachment';
        const expectedValidationErrorMessage = 'Select a file';
        store.state.onlineConsultations.validationErrorMessage = validationErrorMessage;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.validationErrorMessage).toBe(expectedValidationErrorMessage);
      });
    });

    describe('isDataRequired', () => {
      each([{
        status: 'data-required',
        expectedValue: true,
      }, {
        status: 'success',
        expectedValue: false,
      }]).it('will get status from store', ({ status, expectedValue }) => {
        // Arrange
        store.state.onlineConsultations.status = status;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.isDataRequired).toBe(expectedValue);
      });
    });

    describe('question', () => {
      each([{
        text: 'something',
        type: 'choice',
      }, undefined]).it('will get question from store', (question) => {
        // Arrange
        store.state.onlineConsultations.question = question;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.question).toEqual(question);
      });
    });

    describe('questionComponent', () => {
      describe('is known type', () => {
        each([{
          questionType: 'attachment',
          component: QuestionAttachment,
        }, {
          questionType: 'boolean',
          component: QuestionBoolean,
        }, {
          questionType: 'choice',
          component: QuestionChoice,
        }, {
          questionType: 'date',
          component: QuestionDate,
        }, {
          questionType: 'dateTime',
          component: QuestionDateTime,
        }, {
          questionType: 'image',
          component: QuestionImage,
        }, {
          questionType: 'multiple_choice',
          component: QuestionMultipleChoice,
        }, {
          questionType: 'integer',
          component: QuestionNumber,
        }, {
          questionType: 'decimal',
          component: QuestionNumber,
        }, {
          questionType: 'quantity',
          component: QuestionQuantity,
        }, {
          questionType: 'string',
          component: QuestionString,
        }, {
          questionType: 'text',
          component: QuestionText,
        }, {
          questionType: 'time',
          component: QuestionTime,
        }]).it('will get appropriate question component based on question type', ({ questionType, component }) => {
          // Arrange
          store.state.onlineConsultations.question = {
            type: questionType,
          };

          // Act
          mountOrchestrator();

          // Assert
          expect(orchestrator.vm.questionComponent).toEqual(component);
        });
      });

      describe('is unknown type', () => {
        it('will dispatch clearAndSetError action', () => {
          // Arrange
          store.state.onlineConsultations.question = {
            type: 'unknown-type',
          };

          // Act
          mountOrchestrator();

          // Assert
          expect(orchestrator.vm.questionComponent).toBeUndefined();
          expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
          expect(store.dispatch).toHaveBeenCalledTimes(1);
        });
      });
    });

    describe('questionKey', () => {
      it('will combine question id and request id from store', () => {
        // Arrange
        const expectedQuestionKey = 'test-id-123';
        store.state.onlineConsultations.question = {
          id: 'test-id',
        };
        store.state.onlineConsultations.requestId = 123;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.questionKey).toEqual(expectedQuestionKey);
      });
    });

    describe('requestId', () => {
      it('will get request id from store', () => {
        // Arrange
        store.state.onlineConsultations.requestId = 123;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.requestId).toEqual(123);
      });
    });

    describe('isSuccess', () => {
      each([{
        status: 'data-required',
        expectedValue: false,
      }, {
        status: 'success',
        expectedValue: true,
      }]).it('will get status from store', ({ status, expectedValue }) => {
        // Arrange
        store.state.onlineConsultations.status = status;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.isSuccess).toEqual(expectedValue);
      });
    });

    describe('carePlans', () => {
      it('will get from store', () => {
        // Arrange
        const expectedCarePlans = [{ carePlan1: 'carePlan1', carePlan2: 'carePlan2' }];
        store.state.onlineConsultations.carePlans = expectedCarePlans;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.carePlans).toEqual(expectedCarePlans);
      });
    });

    describe('referralRequests', () => {
      it('will get from store', () => {
        // Arrange
        const expectedReferralRequests = [{ referralRequest1: 'referralRequest1', referralRequest2: 'referralRequest2' }];
        store.state.onlineConsultations.referralRequests = expectedReferralRequests;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.referralRequests).toEqual(expectedReferralRequests);
      });
    });

    describe('answer', () => {
      describe('get', () => {
        it('will get from store', () => {
          // Arrange
          const expectedAnswer = 'test answer.';
          store.state.onlineConsultations.answer = expectedAnswer;

          // Act
          mountOrchestrator();

          // Assert
          expect(orchestrator.vm.answer).toEqual(expectedAnswer);
        });
      });

      describe('set', () => {
        it('will dispatch setAnswer action with the answer as payload', () => {
          // Arrange
          const expectedAnswer = 'another test answer';
          mountOrchestrator();

          // Act
          orchestrator.vm.answer = expectedAnswer;

          // Assert
          expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/setAnswer', expectedAnswer);
          expect(store.dispatch).toHaveBeenCalledTimes(1);
        });
      });
    });

    describe('indexPath', () => {
      it('will return path of logged in home page', () => {
        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.indexPath).toEqual(INDEX_PATH);
      });
    });

    describe('showDesktopBackLink', () => {
      each([{
        isNativeApp: true,
        status: 'success',
        isShown: false,
      }, {
        isNativeApp: false,
        status: 'data-required',
        isShown: false,
      }, {
        isNativeApp: false,
        status: 'success',
        isShown: false,
      }]).it('will return true if not on native and status is success', ({ isNativeApp, status, isShown }) => {
        // Arrange
        store.state.device.isNativeApp = isNativeApp;
        store.state.onlineConsultations.status = status;

        // Act
        mountOrchestrator();
        const desktopBackLink = orchestrator.find('#desktopBackLink');

        // Assert
        expect(desktopBackLink.exists()).toBe(isShown);
      });
    });

    describe('backButtonText', () => {
      each([{
        status: 'success',
        expectedText: 'onlineConsultations.orchestrator.backToHomeButton',
      }, {
        status: 'data-required',
        expectedText: 'onlineConsultations.orchestrator.endMyConsultationButton',
      }]).it('will appropriate text for status', ({ status, expectedText }) => {
        // Arrange
        store.state.onlineConsultations.status = status;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.backButtonText).toBe(expectedText);
      });
    });

    describe('nothingToDisplay', () => {
      each([{
        carePlans: [],
        referralRequests: [],
        question: {},
        status: 'success',
        expectedValue: true,
      }, {
        carePlans: [testCarePlan],
        referralRequests: [],
        question: {},
        status: 'success',
        expectedValue: false,
      }, {
        carePlans: [],
        referralRequests: [testReferralRequest],
        question: {},
        status: 'success',
        expectedValue: false,
      }, {
        carePlans: [],
        question: {},
        status: 'success',
        expectedValue: true,
      }, {
        referralRequests: [],
        question: {},
        status: 'success',
        expectedValue: true,
      }, {
        carePlans: [testCarePlan],
        referralRequests: [testReferralRequest],
        status: 'data-required',
        expectedValue: true,
      }, {
        carePlans: [testCarePlan],
        referralRequests: [testReferralRequest],
        question: {},
        status: 'unknown-status',
        expectedValue: true,
      }, {
        carePlans: [testCarePlan],
        referralRequests: [testReferralRequest],
        status: 'success',
        expectedValue: false,
      }, {
        question: { text: '', type: 'integer' },
        status: 'data-required',
        expectedValue: false,
      }]).it('will return true if there is nothing to display', ({ carePlans, referralRequests, question, status, expectedValue }) => {
        // Arrange
        store.state.onlineConsultations.status = status;
        store.state.onlineConsultations.question = question;
        store.state.onlineConsultations.carePlans = carePlans;
        store.state.onlineConsultations.referralRequests = referralRequests;

        // Act
        mountOrchestrator();

        // Assert
        expect(orchestrator.vm.nothingToDisplay).toEqual(expectedValue);
      });
    });

    describe('watcher', () => {
      it('will dispatch clearAndSetError if to is true', () => {
        // Arrange
        store.state.onlineConsultations.status = 'data-required';
        store.state.onlineConsultations.question = { type: 'integer', text: 'text' };
        mountOrchestrator(false);

        // Act
        store.state.onlineConsultations.status = 'success';

        // Assert
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
      });

      it('will not dispatch clearAndSetError if to is false', () => {
        // Arrange
        store.state.onlineConsultations.status = 'data-required';
        store.state.onlineConsultations.question = { type: 'integer', text: 'text' };
        mountOrchestrator(false);

        // Act
        store.state.onlineConsultations.carePlans = [testCarePlan];
        store.state.onlineConsultations.status = 'success';

        // Assert
        expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/clearAndSetError');
      });
    });
  });

  describe('methods', () => {
    describe('onAnswerValidate', () => {
      it('will dispatch setAnswerIsValid action with validation object as payload', () => {
        // Arrange
        const expectedValidationObject = { valid: true };
        mountOrchestrator();

        // Act
        orchestrator.vm.onAnswerValidate(expectedValidationObject);

        // Assert
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/setAnswerIsValid', expectedValidationObject);
        expect(store.dispatch).toHaveBeenCalledTimes(1);
      });
    });

    describe('continueClicked', () => {
      let isValidationError;
      let isLoadingFile;

      describe('file is not loading and there was a validation error', () => {
        beforeEach(() => {
          isValidationError = true;
          isLoadingFile = false;
        });

        it('will dispatch setValidationError action and scroll to 0, 0', async () => {
          // Arrange
          store.state.onlineConsultations.validationError = isValidationError;
          store.state.onlineConsultations.isLoadingFile = isLoadingFile;
          mountOrchestrator();

          // Act
          await orchestrator.vm.continueClicked();

          // Assert
          expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/setValidationError');
          expect(scrollTo).toHaveBeenCalledWith(0, 0);
          expect(scrollTo).toHaveBeenCalledTimes(1);
        });
      });

      describe('file is not loading and there is no validation error', () => {
        beforeEach(() => {
          isValidationError = false;
          isLoadingFile = false;
        });

        describe('is native app', () => {
          it('will dispatch setValidationError action, dispatch evaluateServiceDefinition and natively reset page focus', async () => {
            // Arrange
            const blur = jest.fn();
            store.state.onlineConsultations.validationError = isValidationError;
            store.state.onlineConsultations.isLoadingFile = isLoadingFile;
            mountOrchestrator();
            const expectedParams = {
              provider: 'stubs',
              serviceDefinitionId: 'NHS_ADMIN',
            };
            document.activeElement.blur = blur;

            // Act
            await orchestrator.vm.continueClicked();

            // Assert
            expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/setValidationError');
            expect(blur).toHaveBeenCalled();
            expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', expectedParams);
            expect(EventBus.$emit).not.toHaveBeenCalled();
            expect(NativeApp.resetPageFocus).toHaveBeenCalled();
            expect(scrollTo).toHaveBeenCalledWith(0, 0);
            expect(scrollTo).toHaveBeenCalledTimes(1);
          });
        });

        describe('is not native app', () => {
          it('will dispatch setValidationError action, dispatch evaluateServiceDefinition and focus nhsapp root', async () => {
            // Arrange
            const blur = jest.fn();
            store.state.device.isNativeApp = false;
            store.state.onlineConsultations.validationError = isValidationError;
            store.state.onlineConsultations.isLoadingFile = isLoadingFile;
            mountOrchestrator();
            document.activeElement.blur = blur;
            const expectedParams = {
              provider: 'stubs',
              serviceDefinitionId: 'NHS_ADMIN',
            };

            // Act
            await orchestrator.vm.continueClicked();

            // Assert
            expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/setValidationError');
            expect(blur).toHaveBeenCalled();
            expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', expectedParams);
            expect(EventBus.$emit).toHaveBeenCalledWith(FOCUS_NHSAPP_ROOT);
            expect(NativeApp.resetPageFocus).not.toHaveBeenCalled();
            expect(scrollTo).toHaveBeenCalledWith(0, 0);
            expect(scrollTo).toHaveBeenCalledTimes(1);
          });
        });
      });

      describe('file load in progress (for question attachment)', () => {
        beforeEach(() => {
          isLoadingFile = true;
        });

        it('will not set validation error or attempt to evaluate', async () => {
          // Arrange
          store.state.onlineConsultations.isLoadingFile = isLoadingFile;
          mountOrchestrator();
          const unexpectedParams = {
            provider: 'stubs',
            serviceDefinitionId: 'NHS_ADMIN',
          };

          // Act
          await orchestrator.vm.continueClicked();

          // Assert
          expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/setValidationError');
          expect(store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', unexpectedParams);
          expect(store.dispatch).toHaveBeenCalledTimes(0);
        });
      });
    });

    describe('backToHomeClicked', () => {
      it('will redirect to logged in home page', () => {
        // Arrange
        mountOrchestrator();

        // Act
        orchestrator.vm.backToHomeClicked();

        // Assert
        expect(redirectTo).toHaveBeenCalledWith(orchestrator.vm, INDEX_PATH);
        expect(redirectTo).toHaveBeenCalledTimes(1);
      });
    });
  });
});
