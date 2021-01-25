import each from 'jest-each';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import { EMPTY_PATH } from '@/router/paths';
import { createStore, mount } from '../helpers';

const EXPECTED_URI = '/redirector?redirect_to=http%3A%2F%2Ffoo.com%2FnhsRedirectPath';

describe('ThirdParty Jumpoff Button', () => {
  const goToUrl = jest.fn();
  let $router;
  let $store;
  let menuItem;
  let propsData;
  let wrapper;

  const mountAs = ({
    id = 'btn_jumpoff',
    native = true,
    providerId = 'pkb',
    providerUrl = 'http://foo.com/',
    providerConfiguration = {
      redirectPath: 'nhsRedirectPath',
      jumpOffType: 'messages',
      provider: 'pkb',
      jumpOffId: 'messages',
    },
    loggingEnabled = false,
    routePath = '/',
  } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: native,
        },
      },
      getters: {
        'knownServices/matchOneById': serviceId => ({
          id: serviceId,
          url: providerUrl,
        }),
      },
      $env: {
        THIRD_PARTY_JUMP_OFF_LOGGING_ENABLED: loggingEnabled,
      },
    });
    propsData = {
      id,
      providerConfiguration,
      providerId,
    };

    return mount(ThirdPartyJumpOffButton, {
      $store,
      $router,
      propsData,
      $route: { path: routePath },
      methods: { goToUrl },
    });
  };

  beforeEach(() => {
    $router = [];
    goToUrl.mockClear();
  });

  describe('menu item', () => {
    describe('is running in native app', () => {
      beforeEach(() => {
        wrapper = mountAs();
        menuItem = wrapper.find('#btn_jumpoff');
      });

      it('will have the expected uri', () => {
        expect(menuItem.attributes().href)
          .toEqual(EXPECTED_URI);
      });

      it('will begin with a / to enable proper navigation on desktop', () => {
        expect(menuItem.attributes().href.startsWith(EMPTY_PATH)).toBe(true);
      });

      it('will have no target set', () => {
        expect(menuItem.attributes().target)
          .toEqual(undefined);
      });
    });

    describe('is running on desktop', () => {
      beforeEach(() => {
        wrapper = mountAs({ native: false });
        menuItem = wrapper.find('#btn_jumpoff');
      });

      it('will have the target set to "_blank"', () => {
        expect(menuItem.attributes().target)
          .toEqual('_blank');
      });
    });

    describe('onClick', () => {
      describe('goToUrl', () => {
        it('will invoke goToUrl when on native', () => {
          wrapper = mountAs();

          wrapper.vm.onClick('/jump-off/path');

          expect(goToUrl).toHaveBeenCalledWith('/jump-off/path');
        });

        it('will not invoke goToUrl when not on native', () => {
          wrapper = mountAs({ native: false });

          wrapper.vm.onClick('/jump-off/path');

          expect(goToUrl).not.toHaveBeenCalledWith('/jump-off/path');
        });
      });

      describe('logging', () => {
        each([
          ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835', '/'],
          ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835/somewhere', '/somewhere'],
          ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835/somewhere/else', '/somewhere/else'],
        ]).it('will log info on where the jump off is and the linked service when enabled in env vars', (routePath, outputPath) => {
          wrapper = mountAs({ loggingEnabled: true, routePath });

          wrapper.vm.onClick('/jump-off/path');

          expect($store.dispatch).toHaveBeenCalledWith('log/onInfo', `Jump-off from ${outputPath} to pkb - messages`);
        });

        it('will not log info when disabled in env vars', () => {
          wrapper = mountAs();

          wrapper.vm.onClick('/jump-off/path');

          expect($store.dispatch).not.toHaveBeenCalledWith('log/onInfo', expect.anything());
        });
      });
    });
  });
});
