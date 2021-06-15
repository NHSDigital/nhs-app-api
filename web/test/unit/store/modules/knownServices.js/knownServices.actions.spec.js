import actions from '@/store/modules/knownServices/actions';
import { LOADSERVICES } from '@/store/modules/knownServices/mutation-types';

const createApp = urls => ({
  $httpV3: {
    getV3KnownServices: jest.fn().mockResolvedValue(urls),
  },
});

describe('known serivces actions', () => {
  describe('load', () => {
    const urls = { knownServices:
      [{
        id: 'pkb',
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        url: 'test.test.com',
        domains: [
          'subdomain1.test.test.com',
          'subdomain2.test.test.com',
        ],
      }],
    };

    let app;

    beforeEach(() => {
      app = createApp(urls);

      actions.app = app;
    });

    describe('load services', () => {
      let commit;
      beforeEach(async () => {
        commit = jest.fn();
        actions.load({ commit });
      });

      it('will request known services', async () => {
        expect(app.$httpV3.getV3KnownServices).toHaveBeenCalled();
      });

      it('will commit the loaded services mutation with the urls', async () => {
        expect(commit).toHaveBeenCalledWith(LOADSERVICES, [{
          id: 'pkb',
          requiresAssertedLoginIdentity: true,
          showThirdPartyWarning: true,
          url: 'test.test.com',
          domains: [
            'subdomain1.test.test.com',
            'subdomain2.test.test.com',
          ] }]);
      });
    });
  });
});
