import NativeOnlyMixin from '@/components/NativeOnlyMixin';
import { INDEX } from '@/lib/routes';

describe('NativeOnlyMixin', () => {
  let redirect;

  const fetch = ({ source, isNativeApp }) => NativeOnlyMixin.fetch({
    redirect,
    route: {
      query: {
        source,
      },
    },
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
      fetch({ source: 'web', isNativeApp: false });
    });

    it('will redirect to the index page', () => {
      expect(redirect).toBeCalledWith(INDEX.path);
    });
  });

  describe('native', () => {
    beforeEach(() => {
      fetch({ source: 'ios', isNativeApp: true });
    });

    it('will not redirect', () => {
      expect(redirect).not.toBeCalled();
    });
  });

  describe('native source', () => {
    beforeEach(() => {
      fetch({ source: 'ios', isNativeApp: false });
    });

    it('will not redirect', () => {
      expect(redirect).not.toBeCalled();
    });
  });

  describe('native state', () => {
    beforeEach(() => {
      fetch({ source: 'web', isNativeApp: true });
    });

    it('will not redirect', () => {
      expect(redirect).not.toBeCalled();
    });
  });
});
