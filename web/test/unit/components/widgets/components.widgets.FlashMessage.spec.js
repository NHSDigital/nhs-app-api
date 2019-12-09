/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import { mount, createLocalVue } from '@vue/test-utils';
import FlashMessage from '@/components/widgets/FlashMessage';

const $t = key => `translate_${key}`;

const createFlashMessage = ($store, $translate) => {
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const $style = {
    mainShowingSlots: 'mainShowingSlots',
    warning: 'warning',
    error: 'error',
  };

  return mount(FlashMessage, {
    localVue,
    mocks: {
      $store,
      $t: $translate || $t,
      $style,
      showTemplate: () => true,
    },
  });
};

describe('FlashMessage.vue', () => {
  it('will not show message', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        flashMessage: {
          show: false,
        },
      },
    };

    const component = createFlashMessage($store);

    expect(component.find('.warning').exists()).toBeFalsy();
  });

  it('will show warning message', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        flashMessage: {
          show: true,
          message: 'Warning!',
          type: 'warning',
        },
      },
    };

    const component = createFlashMessage($store);

    expect(component.find('.warning').exists()).toBeTruthy();
    expect(component.find('.warning p').text()).toEqual('Warning!');
  });

  it('will show warning message using key', () => {
    const key = 'message.key';
    const message = 'Warning!';
    const $translate = (messageKey) => {
      if (messageKey === key) return message;
      return $t(key);
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        flashMessage: {
          show: true,
          key,
          type: 'warning',
        },
      },
    };

    const component = createFlashMessage($store, $translate);

    expect(component.find('.warning').exists()).toBeTruthy();
    expect(component.find('.warning p').text()).toEqual('Warning!');
  });

  it('will show success message', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        flashMessage: {
          show: true,
          message: 'Success!',
          type: 'success',
        },
      },
    };

    const component = createFlashMessage($store);

    expect(component.find('.warning').exists()).toBeFalsy();
    expect(component.find('#success-dialog').exists()).toBeTruthy();
    expect(component.find('#success-dialog p').text()).toEqual('Success!');
  });

  it('will show success message using key', () => {
    const key = 'message.key';
    const message = 'Success!';
    const $translate = (messageKey) => {
      if (messageKey === key) return message;
      return $t(key);
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        flashMessage: {
          show: true,
          key,
          type: 'success',
        },
      },
    };

    const component = createFlashMessage($store, $translate);

    expect(component.find('.warning').exists()).toBeFalsy();
    expect(component.find('#success-dialog').exists()).toBeTruthy();
    expect(component.find('#success-dialog p').text()).toEqual(message);
  });

  it('will show error message', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        flashMessage: {
          show: true,
          message: 'Error!',
          type: 'error',
        },
      },
    };

    const component = createFlashMessage($store);

    expect(component.find('.warning').exists()).toBeFalsy();
    expect(component.find('.error').exists()).toBeTruthy();
    expect(component.find('.error p').text()).toEqual('Error!');
  });

  it('will show error message using key', () => {
    const key = 'message.key';
    const message = 'Error!';
    const $translate = (messageKey) => {
      if (messageKey === key) return message;
      return $t(key);
    };
    const $store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
        flashMessage: {
          show: true,
          key,
          type: 'error',
        },
      },
    };

    const component = createFlashMessage($store, $translate);

    expect(component.find('.error').exists()).toBeTruthy();
    expect(component.find('.error p').text()).toEqual('Error!');
  });
});
