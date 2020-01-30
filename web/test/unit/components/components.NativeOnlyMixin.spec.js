import NativeOnlyMixin from '@/components/NativeOnlyMixin';
import { INDEX } from '@/lib/routes';

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
      expect(redirect).toBeCalledWith(INDEX.path);
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
