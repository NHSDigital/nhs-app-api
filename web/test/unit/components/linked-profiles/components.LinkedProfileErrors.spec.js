import LinkedProfileErrors from '@/components/linked-profiles/LinkedProfileErrors';
import each from 'jest-each';
import { createStore, mount } from '../../helpers';

describe('linked profiles error component', () => {
  let page;

  const mountComponent = ({
    hasRetried = false,
    isNativeApp = false,
    backLinkOverride,
  } = {}) =>
    mount(LinkedProfileErrors, {
      $store: createStore({
        state: {
          device: {
            isNativeApp,
          },
          navigation: {
            backLinkOverride,
          },
          linkedAccounts: {
            error: {
              status: 599,
            },
          },
        },
      }),
      computed: {
        hasRetried() {
          return hasRetried;
        },
      },
    });

  describe('errors', () => {
    each([
      ['not show', true, false],
      ['show', false, true],
    ])
      .it('will %s the temporary error when hasRetried is %s', (_, hasRetried, errorVisible) => {
        page = mountComponent({ hasRetried });
        expect(page.find('#linked-profiles-599-temporary-error').exists()).toBe(errorVisible);
      });

    each([
      ['show', true, true],
      ['not show', false, false],
    ])
      .it('will %s the permanent error when hasRetried is %s', (_, hasRetried, errorVisible) => {
        page = mountComponent({ hasRetried });
        expect(page.find('#linked-profiles-599-error').exists()).toBe(errorVisible);
      });
  });

  each([
    [undefined, 'more'],
    ['overridePath', 'overridePath'],
  ])
    .it('will set the appropriate backUrl', (backLinkOverride, expectedBackUrl) => {
      page = mountComponent({ backLinkOverride });
      expect(page.vm.backUrl).toBe(expectedBackUrl);
    });
});
