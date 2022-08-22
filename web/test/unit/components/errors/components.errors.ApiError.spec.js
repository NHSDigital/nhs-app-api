import ApiError from '@/components/errors/ApiError';
import each from 'jest-each';
import i18n from '@/plugins/i18n';
import NativeApp from '@/services/native-app';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import ReportAProblem from '@/components/errors/ReportAProblem';
import Vue from 'vue';
import { initialState as initialDeviceState } from '@/store/modules/device/mutation-types';
import { initialState as initialErrorsState } from '@/store/modules/errors/mutation-types';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

Vue.mixin({
  methods: {
    correctUrl(url) {
      return url;
    },
  },
});

const testMessages = {
  400: 'Bad Request',
  403: 'Forbidden',
  409: 'Conflict',
  460: 'Limit reached',
  461: 'Too late',
  464: 'ODS code not supported or no nhs number',
  465: 'Underage',
  500: 'Internal Server Error',
  502: 'Bad Gateway',
  504: 'Gateway Timeout',
};

const createState = ({
  action,
  additionalInfoComponentName,
  error = '',
  isNativeApp = false,
  path,
  status,
  userSessionCreateReferenceCode,
  backLinks = {},
}) => ({
  session: {
    userSessionCreateReferenceCode,
  },
  errors: {
    ...initialErrorsState(),
    ...{
      apiErrors: [{
        error,
        status,
        message: testMessages[status],
      }],
      pageSettings: {
        action,
        additionalInfoComponentName,
        errorOverrideStyles: { 401: 'plain', 403: 'none' },
        backLinks,
      },
      routePath: path,
    },
  },
  device: {
    ...initialDeviceState(),
    ...{
      isNativeApp,
    },
  },
});

describe('api errors', () => {
  let $store;
  let getters;
  let state;
  let wrapper;

  const $style = { additionalInformation: 'additionalInformation' };

  const mountApiError = () => mount(ApiError, {
    $store,
    $style,
    mountOpts: {
      i18n,
    },
  });

  function createApiErrorStore(gettersParam, stateParam) {
    const store = createStore({ getters: gettersParam, state: stateParam });

    return store;
  }

  describe('updated', () => {
    beforeEach(() => {
      state = createState({
        action: { 504: '/test' },
        isNativeApp: false,
        path: '/prescriptions/view-orders',
        status: 504,
      });
      EventBus.$emit.mockClear();
      EventBus.$on.mockClear();
      EventBus.$off.mockClear();
    });

    it('will not emit to EventBus when updated and no api error shown', () => {
      getters = { 'errors/showApiError': true };
      $store = createApiErrorStore(getters, state);
      wrapper = mountApiError();
      getters['errors/showApiError'] = false;

      expect(EventBus.$emit).not.toHaveBeenCalled();
    });

    it('will update header and title when updated and api error shown', () => {
      getters = { 'errors/showApiError': false };
      $store = createApiErrorStore(getters, state);
      wrapper = mountApiError();
      getters['errors/showApiError'] = true;

      expect(EventBus.$emit).toHaveBeenCalledTimes(2);
      expect(EventBus.$emit).toHaveBeenNthCalledWith(1, UPDATE_HEADER, 'Cannot show prescription information', true, true);
      expect(EventBus.$emit).toHaveBeenNthCalledWith(2, UPDATE_TITLE, 'Cannot show prescription information', true);
    });
  });

  describe('standard error', () => {
    beforeEach(() => {
      getters = {
        'errors/showApiError': true,
        'errors/isStandardError': true,
      };
      NativeApp.openAppSettings = jest.fn();
    });

    describe('device settings', () => {
      const getDeviceSettings = () => wrapper.find('*[id="device-settings"]');

      describe('path has device settings', () => {
        let deviceSettings;

        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/account/notifications', status: 500, error: 10002 });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();
          deviceSettings = getDeviceSettings();
        });

        it('will exist', () => {
          expect(deviceSettings.exists()).toBe(true);
        });

        it('will have text', () => {
          expect(deviceSettings.text()).toBe('Go to device settings');
        });

        it('will call native app when clicked', () => {
          deviceSettings.trigger('click');
          expect(NativeApp.openAppSettings).toHaveBeenCalled();
        });
      });

      describe('path does not have additional info', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/account/notifications', status: 500, error: 10001 });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();
        });

        it('will not exist', () => {
          expect(getDeviceSettings().exists()).toBe(false);
        });
      });
    });

    describe('message', () => {
      let message;
      const getMessage = () => wrapper.find('*[data-purpose="msg-text"]');

      describe('path has message text', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/organ_donation', status: 500 });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();
          message = getMessage();
        });

        it('will have text', () => {
          expect(message.text()).toBe('You cannot register your decision or update your organ donation preferences right now.');
        });

        it('will not have label', () => {
          expect(message.attributes('aria-label')).toBeUndefined();
        });
      });

      describe('path has message text and label', () => {
        const messageText = 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.';
        const messageLabel = 'Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call one one one.';

        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/prescriptions', status: 500 });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();
          message = getMessage();
        });

        it('will have text', () => {
          expect(message.text()).toBe(messageText);
        });

        it('will have label', () => {
          expect(message.attributes('aria-label')).toBe(messageLabel);
        });
      });
    });

    describe('standard message text', () => {
      describe('is native app', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/prescriptions/confirm_prescription_details', status: 466 });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();
        });

        it('will have a message dialog', () => {
          expect(wrapper.find(MessageDialog).exists()).toBe(true);
        });

        it('will show the user session service desk reference code if it exists', () => {
          state = createState({
            isNativeApp: true,
            path: '/prescriptions/confirm_prescription_details',
            status: 466,
            userSessionCreateReferenceCode: 'xxxxxx' });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();

          expect(wrapper.find(ReportAProblem).exists()).toBe(true);
        });

        it('will have two message texts', () => {
          expect(wrapper.findAll(MessageText).length).toBe(2);
        });

        it('will contain the header', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('You previously ordered at least one of these medications in the last 30 days');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('If you need this medicine sooner, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.');
        });

        it('will contain no subheader', () => {
          expect(wrapper.find('[data-purpose="msg-subheader"').exists()).toBe(false);
        });
      });

      describe('is not native app', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: false, path: '/prescriptions/confirm_prescription_details', status: 466 });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();
        });

        it('will have a message dialog', () => {
          expect(wrapper.find(MessageDialog).exists()).toBe(true);
        });

        it('will show the user session service desk reference code if it exists', () => {
          state = createState({
            isNativeApp: true,
            path: '/prescriptions/confirm_prescription_details',
            status: 466,
            userSessionCreateReferenceCode: 'xxxxxx' });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();

          expect(wrapper.find(ReportAProblem).exists()).toBe(true);
        });

        it('will have two message texts', () => {
          expect(wrapper.findAll(MessageText).length).toBe(2);
        });

        it('will contain the subheader', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('You previously ordered at least one of these medications in the last 30 days');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('If you need this medicine sooner, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.');
        });
      });
    });

    describe('retry button', () => {
      describe('no retry text', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/', status: 404 });
          $store = createApiErrorStore(getters, state);
          wrapper = mountApiError();
        });

        it('will not exist', () => {
          expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(false);
        });
      });

      describe('retry text', () => {
        describe('is native app', () => {
          beforeEach(() => {
            state = createState({ isNativeApp: true, path: '/prescriptions/view-orders', status: 504 });
            $store = createApiErrorStore(getters, state);
            wrapper = mountApiError();
          });

          it('will exist', () => {
            expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(true);
          });
        });

        describe('is not native app', () => {
          describe('no retry action or text', () => {
            beforeEach(() => {
              state = createState({ isNativeApp: false, path: '/', status: 404 });
              $store = createApiErrorStore(getters, state);
              wrapper = mountApiError();
            });

            it('will not exist', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(false);
            });
          });

          describe('retry action and retry text', () => {
            beforeEach(() => {
              // This has retry text because View orders page from prescriptions hub
              // has an associated `retryButtonText`
              // in the locale file.
              state = createState({
                action: { 504: '/test' },
                isNativeApp: false,
                path: '/prescriptions/view-orders',
                status: 504,
              });
              $store = createApiErrorStore(getters, state);
              wrapper = mountApiError();
              EventBus.$emit.mockClear();
              EventBus.$on.mockClear();
              EventBus.$off.mockClear();
            });

            it('will exist', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(true);
            });

            it('will have the retry text', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').text())
                .toEqual('Try again');
            });

            it('will call setFocus method on update', () => {
              wrapper.vm.setFocus = jest.fn();
              getters['errors/showApiError'] = false;
              getters['errors/showApiError'] = true;

              expect(wrapper.vm.setFocus).toHaveBeenCalled();
            });

            it('will focus on the form when setFocus method is called', () => {
              wrapper.vm.$refs.retryFormRef = {
                focus: jest.fn(),
              };
              wrapper.vm.setFocus();

              expect(wrapper.vm.$refs.retryFormRef.focus).toHaveBeenCalled();
            });
          });

          describe('back link', () => {
            it('will show the back link if there is a back link url for the error', () => {
              state = createState({
                isNativeApp: false,
                path: '/prescriptions/confirm-prescription-details',
                status: 466,
                backLinks: { 466: 'prescriptions' },
              });

              $store = createApiErrorStore(getters, state);
              wrapper = mountApiError();

              expect(wrapper.find('#backLink').exists()).toBe(true);
              expect(wrapper.find('#retryButton').exists()).toBe(false);
            });

            it('will not show the back link if there is no back link url for the error', () => {
              state = createState({
                isNativeApp: false,
                path: '/prescriptions/repeat_courses',
                status: 504,
                backLinks: { 466: 'prescriptions' },
              });
              $store = createApiErrorStore(getters, state);
              wrapper = mountApiError();

              expect(wrapper.find('#backLink').exists()).toBe(false);
              expect(wrapper.find('#retryButton').exists()).toBe(true);
            });

            it('will show the prescriptions link if there is text for the link', () => {
              state = createState({
                isNativeApp: false,
                path: '/prescriptions/confirm-prescription-details',
              });
              $store = createApiErrorStore(getters, state);
              wrapper = mountApiError();

              expect(wrapper.find('#prescriptionsLink').exists()).toBe(true);
            });

            it('will not show the prescriptions link if there is no text for the link', () => {
              state = createState({
                isNativeApp: false,
                path: '/prescriptions/confirm-prescription-details',
                status: 466,
              });
              $store = createApiErrorStore(getters, state);
              wrapper = mountApiError();

              expect(wrapper.find('#prescriptionsLink').exists()).toBe(false);
            });
          });
        });
      });
    });
  });

  describe('is information error', () => {
    beforeEach(() => {
      getters = {
        'errors/showApiError': true,
        'errors/isStandardError': false,
      };
    });

    describe('is native app', () => {
      beforeEach(() => {
        state = createState({ isNativeApp: true, path: '/', status: 404 });
        $store = createApiErrorStore(getters, state);
        wrapper = mountApiError();
      });

      it('will have a slim header', () => {
        expect(wrapper.find('h1').exists()).toBe(true);
      });
    });

    describe('is not native app', () => {
      beforeEach(() => {
        state = createState({ isNativeApp: false, path: '/', status: 404 });
        $store = createApiErrorStore(getters, state);
        wrapper = mountApiError();
      });

      it('will not have a slim header', () => {
        expect(wrapper.find('h1').exists()).toBe(false);
      });

      it('will have an h2 set to the subheader', () => {
        expect(wrapper.find('h2').text()).toEqual('If you entered a web address, check it was correct.');
      });
    });
  });
  describe('computed', () => {
    describe('component', () => {
      each([
        ['will remove the leading slash', '/mypage', 'mypage'],
        ['will substitude "/" for "."', '/my/page', 'my.page'],
        ['will substitude "-" for "_"', '/my-page', 'my_page'],
      ]).it('%s', (_, routePath, expectedComponent) => {
        state = createState({ path: routePath });
        $store = createApiErrorStore(getters, state);
        wrapper = mountApiError();
        expect(wrapper.vm.component).toEqual(expectedComponent);
      });
    });
    describe('override style', () => {
      it('will not return an override style if not defined for given status code', () => {
        state = createState({ isNativeApp: true, path: '/', status: 500 });
        $store = createApiErrorStore(getters, state);
        wrapper = mountApiError();
        expect(wrapper.vm.overrideStyle).toBeUndefined();
      });

      it('will return an override style if defined for given status code', () => {
        state = createState({ isNativeApp: true, path: '/', status: 403 });
        $store = createApiErrorStore(getters, state);
        wrapper = mountApiError();
        expect(wrapper.vm.overrideStyle).toEqual('none');
      });
    });
  });

  describe('methods', () => {
    beforeEach(() => {
      getters = { 'errors/showApiError': true };
    });

    describe('get component error code key', () => {
      it('will be empty string when there is no API error', () => {
        wrapper = mountApiError();
        expect(wrapper.vm.getComponentErrorCodeKey('any')).toEqual('');
      });

      it('will be value of `[component].errors.[statusCode].[errorCode].[type] if it exists', () => {
        state = createState({ path: '/account/notifications', error: 10001, status: 500 });
        $store = createApiErrorStore(getters, state);
        wrapper = mountApiError();

        const x = wrapper.vm.getComponentErrorCodeKey('retryButtonText');
        expect(x).toEqual('Try again');
      });
    });

    describe('get message', () => {
      describe('not showing errors', () => {
        it('will be an empty string when not showing errors', () => {
          wrapper = mountApiError();
          expect(wrapper.vm.getMessage('appointments')).toEqual('');
        });
      });
    });

    describe('get text', () => {
      it('will return an empty string if the key does not exist in the locale file', () => {
        wrapper = mountApiError();
        expect(wrapper.vm.getText('mickey.mouse')).toEqual('');
      });

      it('will return the value if the key exists in the locale file', () => {
        wrapper = mountApiError();
        expect(wrapper.vm.getText('apiErrors.pageHeader')).toEqual('The service is unavailable');
      });
    });
  });
});
