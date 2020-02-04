import actions from '@/store/modules/knownServices/actions';
import { LOADSERVICES } from '@/store/modules/knownServices/mutation-types';

const createApp = urls => ({
  $httpV2: {
    getV2Configuration: jest.fn().mockResolvedValue(urls),
  },
});

describe('known serivces actions', () => {
  describe('load', () => {
    const urls = { knownServices:
      [{
        url: 'test.test.com',
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

      it('will request configuration', async () => {
        expect(app.$httpV2.getV2Configuration).toHaveBeenCalled();
      });

      it('will commit the loaded services mutation with the urls', async () => {
        expect(commit).toHaveBeenCalledWith(LOADSERVICES, ['test.test.com']);
      });
    });
  });
});
