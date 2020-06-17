import NativeOnlyMixin from '@/components/NativeOnlyMixin';
import { INDEX_PATH } from '@/router/paths';

describe('NativeOnlyMixin', () => {
  let redirect;

  const fetch = isNativeApp => NativeOnlyMixin.fetch({
    redirect,
    store: {
      state: {
        device: {
          isNativeApp,
        },
      },
    },
  });

  beforeEach(() => {
    redirect = jest.fn();
  });

  describe('not native', () => {
    beforeEach(() => {
      fetch(false);
    });

    it('will redirect to the index page', () => {
      expect(redirect).toBeCalledWith(INDEX_PATH);
    });
  });

  describe('native', () => {
    beforeEach(() => {
      fetch(true);
    });

    it('will not redirect', () => {
      expect(redirect).not.toBeCalled();
    });
  });
});
