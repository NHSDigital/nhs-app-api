import UserResearch from '@/pages/user-research';
import * as LibUtils from '@/lib/utils';
import { NOTIFICATIONS_PATH } from '@/router/paths';
import { createRouter, createStore, mount } from '../helpers';

LibUtils.redirectTo = jest.fn();
LibUtils.redirectByName = jest.fn();

const mountUserResearch = ({ methods, $http, $router }) => mount(UserResearch, {
  $store: createStore({
    $http,
    state: {
      device: {
        isNativeApp: false,
      },
    },
  }),
  $style: {
    error: 'error',
  },
  methods,
  stubs: {
    'terms-and-conditions-layout': '<div><slot/></div>',
  },
  $router,
});

describe('user research', () => {
  let wrapper;
  let conditionalRedirect;
  let $http;
  let $router;

  beforeEach(() => {
    $router = createRouter();
    conditionalRedirect = jest.fn();
    $http = {
      postV1ApiUsersMeInfoUserresearch: jest.fn(() => Promise.resolve()),
    };

    wrapper = mountUserResearch({
      $http,
      methods: {
        conditionalRedirect,
      },
      $router,
    });
  });

  describe('continue button', () => {
    let button;

    beforeEach(() => {
      button = wrapper.find('button');
    });

    it('will exist', () => {
      expect(button.exists()).toBe(true);
    });

    describe('click', () => {
      const clickButton = () => button.trigger('click');

      describe('selection not made', () => {
        beforeEach(() => {
          clickButton();
        });

        it('will show error dialog', () => {
          expect(wrapper.find('.error').exists()).toBe(true);
        });

        it('will not call conditional redirect', () => {
          expect(conditionalRedirect).not.toBeCalled();
        });

        it('will show inline errors', () => {
          expect(wrapper.find('.error-message').exists()).toBe(true);
        });
      });

      describe.each([
        ['Yes', 'optIn'],
        ['No', 'optOut'],
      ])('`%s` radio button is selected', (_, value) => {
        beforeEach(() => {
          const radioButton = wrapper.find(`#radioButton-${value}`);
          radioButton.trigger('click');
          clickButton();
        });

        it('will not show error dialog', () => {
          expect(wrapper.find('.error').exists()).toBe(false);
        });

        it('will not show inline errors', () => {
          expect(wrapper.find('.error-message').exists()).toBe(false);
        });

        it('will post user research preference', () => {
          expect($http.postV1ApiUsersMeInfoUserresearch).toBeCalledWith({
            userResearchRequest: { preference: value },
            ignoreError: true,
          });
        });

        it('will call conditional redirect', () => {
          expect($router.push).toHaveBeenCalledWith({ path: NOTIFICATIONS_PATH });
        });
      });

      describe('post user research fails', () => {
        beforeEach(() => {
          const error = { response: { status: 500 } };
          $http.postV1ApiUsersMeInfoUserresearch.mockImplementation(() => Promise.reject(error));

          const radioButton = wrapper.find('#radioButton-optIn');
          radioButton.trigger('click');
          clickButton();
        });

        it('will still call conditional redirect', () => {
          expect($router.push).toHaveBeenCalledWith({ path: NOTIFICATIONS_PATH });
        });
      });
    });
  });
});
