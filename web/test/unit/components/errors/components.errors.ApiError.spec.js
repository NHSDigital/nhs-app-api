import ApiError from '@/components/errors/ApiError';
import each from 'jest-each';
import i18n from '@/plugins/i18n';
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
  EventBus: { $emit: jest.fn() },
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
        errorOverrideStyles: {},
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

  describe('updated', () => {
    beforeEach(() => {
      state = createState({
        action: { 504: '/test' },
        isNativeApp: false,
        path: '/prescriptions/view-orders',
        status: 504,
      });
      EventBus.$emit.mockClear();
    });

    it('will not emit to EventBus when updated and no api error shown', () => {
      getters = { 'errors/showApiError': true };
      $store = createStore({ getters, state });
      wrapper = mountApiError();
      getters['errors/showApiError'] = false;

      expect(EventBus.$emit).not.toHaveBeenCalled();
    });

    it('will update header and title when updated and api error shown', () => {
      getters = { 'errors/showApiError': false };
      $store = createStore({ getters, state });
      wrapper = mountApiError();
      getters['errors/showApiError'] = true;

      expect(EventBus.$emit).toHaveBeenCalledTimes(2);
      expect(EventBus.$emit).toHaveBeenNthCalledWith(1, UPDATE_HEADER, 'Prescription data error', true, true);
      expect(EventBus.$emit).toHaveBeenNthCalledWith(2, UPDATE_TITLE, 'Prescription data error', true);
    });
  });

  describe('standard error', () => {
    beforeEach(() => {
      getters = {
        'errors/showApiError': true,
        'errors/isStandardError': true,
      };
    });

    describe('additional info', () => {
      const getAdditionalInfo = () => wrapper.find(`.${$style.additionalInformation}`);

      describe('path has additional info', () => {
        let additionalInfo;

        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/account/notifications', status: 500, error: 10002 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
          additionalInfo = getAdditionalInfo();
        });

        it('will exist', () => {
          expect(additionalInfo.exists()).toBe(true);
        });

        it('will have text', () => {
          expect(additionalInfo.text()).toBe('Go to your device settings and check notifications are turned on, then try again.');
        });

        it('will not have label', () => {
          expect(additionalInfo.attributes('aria-label')).toBeUndefined();
        });
      });

      describe('path does not have additional info', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/account/notifications', status: 500, error: 10001 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
        });

        it('will not exist', () => {
          expect(getAdditionalInfo().exists()).toBe(false);
        });
      });
    });

    describe('message', () => {
      let message;
      const getMessage = () => wrapper.find('*[data-purpose="msg-text"]');

      describe('path has message text', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/organ_donation', status: 500 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
          message = getMessage();
        });

        it('will have text', () => {
          expect(message.text()).toBe('You can contact NHS Blood and Transplant to get help with this.');
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
          $store = createStore({ getters, state });
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
          $store = createStore({ getters, state });
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
          $store = createStore({ getters, state });
          wrapper = mountApiError();

          expect(wrapper.find(ReportAProblem).exists()).toBe(true);
        });

        it('will have three message texts', () => {
          expect(wrapper.findAll(MessageText).length).toBe(3);
        });

        it('will contain the header', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('We cannot complete this order');
        });

        it('will contain the subheader', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('You previously ordered at least one of these medications in the last 30 days');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('If you need more medication sooner, contact your GP.');
        });
      });

      describe('is not native app', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: false, path: '/prescriptions/confirm_prescription_details', status: 466 });
          $store = createStore({ getters, state });
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
          $store = createStore({ getters, state });
          wrapper = mountApiError();

          expect(wrapper.find(ReportAProblem).exists()).toBe(true);
        });

        it('will have three message texts', () => {
          expect(wrapper.findAll(MessageText).length).toBe(3);
        });

        it('will contain the header', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('We cannot complete this order');
        });

        it('will contain the subheader', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('You previously ordered at least one of these medications in the last 30 days');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('If you need more medication sooner, contact your GP.');
        });
      });
    });

    describe('retry button', () => {
      describe('no retry text', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/', status: 404 });
          $store = createStore({ getters, state });
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
            $store = createStore({ getters, state });
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
              $store = createStore({ getters, state });
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
              $store = createStore({ getters, state });
              wrapper = mountApiError();
              EventBus.$emit.mockClear();
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
        $store = createStore({ getters, state });
        wrapper = mountApiError();
      });

      it('will have a slim header', () => {
        expect(wrapper.find('h1').exists()).toBe(true);
      });
    });

    describe('is not native app', () => {
      beforeEach(() => {
        state = createState({ isNativeApp: false, path: '/', status: 404 });
        $store = createStore({ getters, state });
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
        $store = createStore({ getters, state });
        wrapper = mountApiError();
        expect(wrapper.vm.component).toEqual(expectedComponent);
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
        $store = createStore({ getters, state });
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
        expect(wrapper.vm.getText('apiErrors.pageHeader')).toEqual('Server error');
      });
    });
  });
});
