import { mount } from '../../../helpers';
import each from 'jest-each';
import AdminHelpPage from '@/pages/appointments/admin-help/index';
import { INDEX } from '@/lib/routes';
import { noJsParameterName } from '@/lib/noJs';
import getAnswerFromRequestBody from '@/lib/online-consultations/noJs';
import { isAnswerValid } from '@/lib/online-consultations/answer-validators';

jest.mock('@/lib/online-consultations/noJs');
jest.mock('@/lib/online-consultations/answer-validators');

describe('Admin Help page', () => {
  let page;
  const dispatch = jest.fn(() => Promise.resolve());
  const redirect = jest.fn();

  const $store = {
    state: {
      device: {
        isNativeApp: true,
      },
      onlineConsultations: {},
      serviceJourneyRules: {
        rules: {
          cdssAdmin: {
            serviceDefinition: 'NHS_ADMIN',
            provider: 'stubs',
          },
          cdssAdvice: {
            serviceDefinition: 'NHS_ADVICE',
            provider: 'stubs',
          },
        },
      },
    },
    dispatch,
  };

  const $style = {
    desktopWeb: 'desktopWeb',
  };

  const mountPage = () => {
    page = mount(AdminHelpPage, {
      data: () => ({
        provider: 'stubs',
        serviceDefinitionId: 'NHS_ADMIN',
      }),
      $store,
      $style,
      showTemplate: () => true,
      stubs: {
        orchestrator: '<div class="orchestrator"></div>',
      },
    });
  };

  beforeEach(() => {
    dispatch.mockClear();
    redirect.mockClear();
  });

  describe('computed properties', () => {
    describe('isError', () => {
      each([true, false]).it('should get error state from store', (error) => {
        // Arrange
        $store.state.onlineConsultations.error = error;

        // Act
        mountPage();

        // Assert
        expect(page.vm.isError).toBe(error);
      });
    });
    describe('isNativeApp', () => {
      each([true, false]).it('should get is native app from store', (isNativeApp) => {
        // Arrange
        $store.state.device.isNativeApp = isNativeApp;

        // Act
        mountPage();

        // Assert
        expect(page.vm.isNativeApp).toBe(isNativeApp);
      });
    });
  });

  describe('lifecycle hooks', () => {
    describe('beforeDestroy', () => {
      it('should call the onlineConsultations clear action with true to reset request id', () => {
        // Arrange
        mountPage();

        // Act
        page.destroy();

        // Assert
        expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/clear', true);
      });
    });
  });

  describe('asyncData', () => {
    let req = {};

    describe('with online consultations disabled', () => {
      each([
        'none',
      ]).it('should redirect to logged in home page', async (provider) => {
        $store.state.serviceJourneyRules.rules.cdssAdmin.provider = provider;
        // Arrange
        mountPage();

        // Act
        await page.vm.$options.asyncData({ store: $store, redirect });

        // Assert
        expect(redirect).toHaveBeenCalledWith(302, INDEX.path, null);
      });
    });
    describe('with online consultations enabled', () => {
      each([
        'stubs',
      ]).it('should not redirect to logged in home page', async (provider) => {
        $store.state.serviceJourneyRules.rules.cdssAdmin.provider = provider;
        // Arrange
        mountPage();

        // Act
        await page.vm.$options.asyncData({ store: $store, redirect });

        // Assert
        expect(redirect).toHaveBeenCalledTimes(0);
      });
    });
    it('should return provider and serviceDefinitionId from SJR rules store', async () => {
      // Arrange
      const expectedResult = {
        provider: 'stubs',
        serviceDefinitionId: 'NHS_ADMIN',
        addJavascriptDisabledHeader: false,
      };

      // Act
      const result = await page.vm.$options.asyncData({ store: $store, req });

      // Assert
      expect(result).toEqual(expectedResult);
    });

    describe('nojs', () => {
      describe('with nojs body not present in request or question not present in store', () => {
        each([{
          request: {},
          question: {},
        }, {
          request: { body: {} },
          question: {},
        }, {
          request: {
            body: {
              [noJsParameterName]: {},
            },
          },
        }]).it('should not update store with answer or validation state', async ({ request, question }) => {
          // Arrange
          $store.state.onlineConsultations.question = question;
          mountPage();

          // Act
          await page.vm.$options.asyncData({ store: $store, req: request });

          // Assert
          expect($store.dispatch).not.toBeCalledWith('onlineConsultations/setAnswer');
          expect($store.dispatch).not.toBeCalledWith('onlineConsultations/setAnswerIsValid');
          expect($store.dispatch).not.toBeCalledWith('onlineConsultations/setValidationError');
        });
      });
      describe('with nojs body present in request and question in store', () => {
        const expectedAnswer = 'test answer';
        const expectedValidation = {
          isValid: true,
          isEmpty: false,
          message: 'test-error-message',
        };

        beforeEach(() => {
          getAnswerFromRequestBody.mockClear();
          getAnswerFromRequestBody.mockReturnValue(expectedAnswer);

          isAnswerValid.mockClear();
          isAnswerValid.mockReturnValue(expectedValidation);
        });

        it('should update store with answer and validation info', async () => {
          // Arrange
          const expectedBody = { [noJsParameterName]: '' };
          req = { body: expectedBody };

          const expectedQuestion = {
            text: 'What is this question?',
            type: 'string',
          };
          $store.state.onlineConsultations.question = expectedQuestion;

          mountPage();

          // Act
          await page.vm.$options.asyncData({ store: $store, req });

          // Assert
          expect(getAnswerFromRequestBody).toHaveBeenCalledWith(expectedBody, expectedQuestion);
          expect(isAnswerValid).toHaveBeenCalledWith(expectedAnswer, expectedQuestion);

          expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setAnswer', expectedAnswer);
          expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setAnswerIsValid', expectedValidation);
          expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/setValidationError');
        });
      });
    });
    describe('service definition evaluation', () => {
      describe('with question missing from the store', () => {
        it('should not dispatch evaluate action', async () => {
          // Arrange
          $store.state.onlineConsultations.question = undefined;
          mountPage();

          // Act
          await page.vm.$options.asyncData({ store: $store, req });

          // Assert
          expect($store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', {
            provider: 'stubs',
            serviceDefinitionId: 'NHS_ADMIN',
            addJavascriptDisabledHeader: false,
          });
        });
        it('should dispatch get action', async () => {
          // Arrange
          $store.state.onlineConsultations.question = undefined;
          mountPage();

          // Act
          await page.vm.$options.asyncData({ store: $store, req });

          // Assert
          expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/getServiceDefinition', {
            provider: 'stubs',
            serviceDefinitionId: 'NHS_ADMIN',
            addJavascriptDisabledHeader: false,
          });
        });
      });
      describe('with valid answer in store', () => {
        it('should dispatch evaluate action', async () => {
          // Arrange
          $store.state.onlineConsultations.question = {};
          $store.state.onlineConsultations.answerIsValid = true;
          mountPage();

          // Act
          await page.vm.$options.asyncData({ store: $store, req });

          // Assert
          expect($store.dispatch).toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', {
            provider: 'stubs',
            serviceDefinitionId: 'NHS_ADMIN',
          });
        });
      });
      describe('with a question and invalid answer', () => {
        it('should not dispatch evaluate action', async () => {
          // Arrange
          $store.state.onlineConsultations.question = {};
          $store.state.onlineConsultations.answerIsValid = false;
          mountPage();

          // Act
          await page.vm.$options.asyncData({ store: $store, req });

          // Assert
          expect($store.dispatch).not.toHaveBeenCalledWith('onlineConsultations/evaluateServiceDefinition', {
            provider: 'stubs',
            serviceDefinitionId: 'NHS_ADMIN',
          });
        });
      });
    });
  });
  describe('template', () => {
    describe('error dialog', () => {
      it('should not appear if onlineConsultations error state is false', () => {
        // Arrange
        $store.state.onlineConsultations.error = false;

        // Act
        mountPage();

        // Assert
        expect(page.find('[data-purpose=error-container]').exists()).toBe(false);
      });
      it('should appear if onlineConsultations error state is true', () => {
        // Arrange
        $store.state.onlineConsultations.error = true;

        // Act
        mountPage();

        // Assert
        expect(page.find('[data-purpose=error-container]').exists()).toBe(true);
      });
      it('should display the default error content for admin help', () => {
        // Arrange
        $store.state.onlineConsultations.error = true;

        // Act
        mountPage();
        const errorHeading = page.find('[data-purpose=error-heading]');
        const errorReason = page.find('[data-purpose=reason-error]');

        // Assert
        expect(errorHeading.exists()).toBe(true);
        expect(errorReason.exists()).toBe(true);
        expect(errorHeading.text()).toBe('translate_appointments.admin_help.errors.header');
        expect(errorReason.text()).toBe('translate_appointments.admin_help.errors.message.text');
      });
    });
    describe('orchestrator', () => {
      it('should appear if onlineConsultations error state is false', () => {
        // Arrange
        $store.state.onlineConsultations.error = false;

        // Act
        mountPage();

        // Assert
        expect(page.find('div.orchestrator').exists()).toBe(true);
      });
      it('should not appear if onlineConsultations error state is true', () => {
        // Arrange
        $store.state.onlineConsultations.error = true;

        // Act
        mountPage();

        // Assert
        expect(page.find('div.orchestrator').exists()).toBe(false);
      });
    });
  });
});
