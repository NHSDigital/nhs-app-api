import '@/static/js/v1/src/nhsapp';

describe('nhsapp third party API', () => {
  beforeEach(() => {
    global.nhsappNative = { goToPage: jest.fn() };
  });

  describe('navigation.goToHomePage', () => {
    describe('native app version < 1.36', () => {
      beforeEach(() => {
        global.nhsappNative.goToHomepage = jest.fn();
        window.nhsapp.navigation.goToHomePage();
      });

      it('will call `nhsappNative.goToHomepage`', () => {
        expect(global.nhsappNative.goToHomepage).toBeCalled();
      });
    });

    describe('native app version >= 1.36', () => {
      beforeEach(() => {
        window.nhsapp.navigation.goToHomePage();
      });

      it('will call `nhsappNative.goToPage` with `AppPage.HOME_PAGE`', () => {
        expect(global.nhsappNative.goToPage)
          .toBeCalledWith(window.nhsapp.navigation.AppPage.HOME_PAGE);
      });
    });
  });

  describe('navigation.goToPage', () => {
    const page = 'foo';

    beforeEach(() => {
      window.nhsapp.navigation.goToPage(page);
    });

    it('will call `nhsappNative.goToPage` with passed value', () => {
      expect(global.nhsappNative.goToPage).toBeCalledWith(page);
    });
  });
});
