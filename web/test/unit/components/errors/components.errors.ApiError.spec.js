/* eslint-disable import/no-extraneous-dependencies */
import has from 'lodash/fp/has';
import Vue from 'vue';
import ApiError from '@/components/errors/ApiError';
import { initialState as initialErrorsState } from '@/store/modules/errors/mutation-types';
import { initialState as initialDeviceState } from '@/store/modules/device/mutation-types';
import { createStore, locale, mount } from '../../helpers';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';

Vue.mixin({
  computed: {
    showTemplate: {
      get() {
        return true;
      },
      set() {
      },
    },
  },
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
  isNativeApp = false,
  path,
  status,
}) => ({
  errors: {
    ...initialErrorsState(),
    ...{
      apiErrors: [{
        error: status,
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
  const $te = key => has(key)(locale);

  const mountApiError = () => mount(ApiError, {
    $store,
    $style,
    // $t,
    $te,
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
          state = createState({ isNativeApp: true, path: '/appointments/confirmation', status: 460 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
          additionalInfo = getAdditionalInfo();
        });

        it('will exist', () => {
          expect(additionalInfo.exists()).toBe(true);
        });

        it('will have text', () => {
          expect(additionalInfo.text()).toBe('translate_appointments.confirmation.errors.460.additionalInfo');
        });

        it('will not have label', () => {
          expect(additionalInfo.attributes('aria-label')).toBeUndefined();
        });
      });

      describe('path has additional info and label', () => {
        let additionalInfo;
        const additionalInfoText = locale.auth_return.errors.additionalInfo.text;
        const additionalInfoLabel = locale.auth_return.errors.additionalInfo.label;

        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/auth-return', status: 400 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
          additionalInfo = getAdditionalInfo();
        });

        it('will exist', () => {
          expect(additionalInfo.exists()).toBe(true);
        });

        it('will have text', () => {
          expect(additionalInfo.text()).toBe(additionalInfoText);
        });

        it('will have label', () => {
          expect(additionalInfo.attributes('aria-label')).toBe(additionalInfoLabel);
        });
      });

      describe('path does not have additional info', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/appointments', status: 400 });
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
          state = createState({ isNativeApp: true, path: '/appointments/cancelling', status: 403 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
          message = getMessage();
        });

        it('will have text', () => {
          expect(message.text()).toBe('translate_appointments.cancelling.errors.403.message');
        });

        it('will not have label', () => {
          expect(message.attributes('aria-label')).toBeUndefined();
        });
      });

      describe('path has message text and label', () => {
        const messageText = locale.appointments.errors.message.text;
        const messageLabel = locale.appointments.errors.message.label;

        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/appointments', status: 400 });
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
          state = createState({ isNativeApp: true, path: '/appointments/cancelling', status: 403 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
        });

        it('will have a message dialog', () => {
          expect(wrapper.find(MessageDialog).exists()).toBe(true);
        });

        it('will have three message texts', () => {
          expect(wrapper.findAll(MessageText).length).toBe(3);
        });

        it('will contain the header', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.403.header');
        });

        it('will contain the subheader', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.subheader');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.403.message');
        });
      });

      describe('is not native app', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: false, path: '/appointments/cancelling', status: 403 });
          $store = createStore({ getters, state });
          wrapper = mountApiError();
        });

        it('will have a message dialog', () => {
          expect(wrapper.find(MessageDialog).exists()).toBe(true);
        });

        it('will have three message texts', () => {
          expect(wrapper.findAll(MessageText).length).toBe(3);
        });

        it('will contain the header', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.403.header');
        });

        it('will contain the subheader', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.subheader');
        });

        it('will contain the message', () => {
          expect(wrapper.find(MessageDialog).text()).toContain('.errors.403.message');
        });
      });
    });

    describe('retry button', () => {
      describe('no retry text', () => {
        beforeEach(() => {
          state = createState({ isNativeApp: true, path: '/appointments', status: 400 });
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
            state = createState({ isNativeApp: true, path: '/appointments/cancelling', status: 400 });
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
              state = createState({ isNativeApp: false, path: '/appointments/cancelling', status: 400 });
              $store = createStore({ getters, state });
              wrapper = mountApiError();
            });

            it('will not exist', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(false);
            });
          });

          describe('retry action but no retry text', () => {
            beforeEach(() => {
              // This state does not have retry text because there is no `retryButtonText`
              // associated with appointments in the locale file
              state = createState({
                action: { 400: '/test' },
                isNativeApp: false,
                path: '/appointments',
                status: 400,
              });
              $store = createStore({ getters, state });
              wrapper = mountApiError();
            });

            it('will not exist', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(false);
            });
          });

          describe('retry action and retry text', () => {
            beforeEach(() => {
              // This has retry text because auth-return has an associated `retryButtonText`
              // in the locale file.
              state = createState({
                action: { 400: '/test' },
                isNativeApp: false,
                path: '/auth-return',
                status: 400,
              });
              $store = createStore({ getters, state });
              wrapper = mountApiError();
            });

            it('will exist', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').exists()).toBe(true);
            });

            it('will have the retry text', () => {
              expect(wrapper.find('[data-purpose="retry-or-back-button"').text())
                .toEqual('translate_auth_return.errors.retryButtonText');
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
        state = createState({ isNativeApp: true, path: '/appointments', status: 400 });
        $store = createStore({ getters, state });
        wrapper = mountApiError();
      });

      it('will have a slim header', () => {
        expect(wrapper.find('h1').exists()).toBe(true);
      });
    });

    describe('is not native app', () => {
      beforeEach(() => {
        state = createState({ isNativeApp: false, path: '/appointments', status: 400 });
        $store = createStore({ getters, state });
        wrapper = mountApiError();
      });

      it('will not have a slim header', () => {
        expect(wrapper.find('h1').exists()).toBe(false);
      });

      it('will have an h2 set to the subheader', () => {
        expect(wrapper.find('h2').text()).toEqual('translate_appointments.errors.subheader');
      });
    });
  });
});
