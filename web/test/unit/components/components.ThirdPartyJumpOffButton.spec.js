/* eslint-disable object-curly-newline */
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import { EMPTY_PATH } from '@/router/paths';
import { createStore, mount } from '../helpers';

const EXPECTED_URI = '/redirector?redirect_to=http%3A%2F%2Ffoo.com%2FnhsRedirectPath';
describe('ThirdParty Jumpoff Button', () => {
  let $router;
  let $store;
  let link;
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
    },
  }) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: native,
        },
        knownServices: {
          knownServices: [{
            id: providerId,
            url: providerUrl,
          }],
        },
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
    });
  };

  beforeEach(() => {
    $router = [];
  });

  describe('link', () => {
    describe('is running in native app', () => {
      beforeEach(() => {
        wrapper = mountAs({ native: true });
        link = wrapper.find('#btn_jumpoff');
      });

      it('will have the expected uri', () => {
        expect(link.attributes().href)
          .toEqual(EXPECTED_URI);
      });

      it('will begin with a / to enable proper navigation on desktop', () => {
        expect(link.attributes().href.startsWith(EMPTY_PATH)).toBe(true);
      });

      it('will have no target set', () => {
        expect(link.attributes().target)
          .toEqual(undefined);
      });
    });

    describe('is running on desktop', () => {
      beforeEach(() => {
        wrapper = mountAs({ native: false });
        link = wrapper.find('#btn_jumpoff');
      });

      it('will have the target set to "_blank"', () => {
        expect(link.attributes().target)
          .toEqual('_blank');
      });
    });
  });
});
