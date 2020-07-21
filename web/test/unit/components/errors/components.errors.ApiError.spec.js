/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import ApiError from '@/components/errors/ApiError';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import ReportAProblem from '@/components/errors/ReportAProblem';
import { initialState as initialDeviceState } from '@/store/modules/device/mutation-types';
import { initialState as initialErrorsState } from '@/store/modules/errors/mutation-types';
import { createStore, locale, mount } from '../../helpers';

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
          expect(additionalInfo.text()).toBe('translate_account.notifications.errors.500.10002.additionalInfo');
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
          expect(message.text()).toBe('translate_organ_donation.errors.500.message');
        });

        it('will not have label', () => {
          expect(message.attributes('aria-label')).toBeUndefined();
        });
      });

      describe('path has message text and label', () => {
        const messageText = locale.prescriptions.errors.message.text;
        const messageLabel = locale.prescriptions.errors.message.label;

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
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.466.header');
        });

        it('will contain the subheader', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.466.subheader');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.466.message');
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
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.466.header');
        });

        it('will contain the subheader', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.466.subheader');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.466.message');
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
            state = createState({ isNativeApp: true, path: '/prescriptions', status: 504 });
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
            });

            it('will exist', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(true);
            });

            it('will have the retry text', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').text())
                .toEqual('translate_prescriptions.view_orders.errors.504.retryButtonText');
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

      it('will not display if there is no retry text', () => {

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
        expect(wrapper.find('h2').text()).toEqual('translate_errors.404.subheader');
      });
    });
  });
});
