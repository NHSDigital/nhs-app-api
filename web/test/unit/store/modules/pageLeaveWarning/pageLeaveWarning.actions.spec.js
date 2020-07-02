import actions from '@/store/modules/pageLeaveWarning/actions';
import { INDEX } from '@/lib/routes';
import {
  SHOULD_BYPASS_ROUTE_GUARD,
  SHOW_LEAVING_PAGE_WARNING,
  SET_ATTEMPTED_REDIRECT_ROUTE,
  RESET,
} from '@/store/modules/pageLeaveWarning/mutation-types';
import NativeCallbacks from '@/services/native-app';
import LeavingPageWarningModal from '@/components/modal/content/LeavingPageWarningModal';
import { redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils', () =>

  ({
    redirectTo: jest.fn(),
  }));

describe('actions', () => {
  let mutation;

  beforeEach(() => {
    actions.app = {
      $env: jest.fn(),
    };
    mutation = {
      commit: jest.fn(),
      state: {
        showLeavingWarning: false,
        attemptedRedirectRoute: INDEX,
      },
    };
  });

  describe('setAttemptedRedirectRoute', () => {
    it('will call commit for the HIDE_EXPIRY_MESSAGE, passing through the route', () => {
      actions.setAttemptedRedirectRoute(mutation, INDEX);
      expect(mutation.commit).toHaveBeenCalledWith(SET_ATTEMPTED_REDIRECT_ROUTE, INDEX);
    });
  });

  describe('reset', () => {
    it('will call commit for the RESET', () => {
      actions.reset(mutation);
      expect(mutation.commit).toHaveBeenCalledWith(RESET);
    });
  });

  describe('shouldSkipDisplayingLeavingWarning', () => {
    describe('setting to true', () => {
      it('will call commit for the SHOULD_BYPASS_ROUTE_GUARD, passing true', () => {
        actions.shouldSkipDisplayingLeavingWarning(mutation, true);
        expect(mutation.commit).toHaveBeenCalledWith(SHOULD_BYPASS_ROUTE_GUARD, true);
      });
    });
    describe('setting to false', () => {
      it('will call commit for the SHOULD_BYPASS_ROUTE_GUARD, passing false', () => {
        actions.shouldSkipDisplayingLeavingWarning(mutation, false);
        expect(mutation.commit).toHaveBeenCalledWith(SHOULD_BYPASS_ROUTE_GUARD, false);
      });
    });
  });

  describe('showLeavingModal', () => {
    describe('using native app', () => {
      let spy;

      beforeEach(() => {
        window.nativeApp = true;
        process.client = true;
        spy = jest.spyOn(NativeCallbacks, 'displayPageLeaveWarning').mockImplementation(() => true);
        actions.showLeavingModal(mutation);
      });

      afterEach(() => {
        (spy || {}).mockRestore();
        window.nativeApp = undefined;
      });

      it('will call commit for the SHOW_LEAVING_PAGE_WARNING', () => {
        expect(mutation.commit).toHaveBeenCalledWith(SHOW_LEAVING_PAGE_WARNING);
      });
      it('will call native function for showing the modal', () => {
        expect(NativeCallbacks.displayPageLeaveWarning).toBeCalled();
      });
    });
    describe('not using native app', () => {
      let app;

      beforeEach(() => {
        window.nativeApp = false;
        process.client = true;
        app = {
          dispatch: jest.fn(),
          showLeavingModal: actions.showLeavingModal,
          app: {
            $env: {},
          },
        };
        app.showLeavingModal(mutation);
      });

      it('will call commit for the SHOW_LEAVING_PAGE_WARNING', () => {
        expect(mutation.commit).toHaveBeenCalledWith(SHOW_LEAVING_PAGE_WARNING);
      });
      it('will dispatch call for showing the modal', () => {
        expect(app.dispatch).toBeCalledWith('modal/show', { content: LeavingPageWarningModal });
      });
    });
  });

  describe('stayOnPage', () => {
    describe('is using native app', () => {
      let app;
      let dismissPageLeaveWarningDialogueSpy;

      beforeEach(() => {
        window.nativeApp = true;

        app = {
          dispatch: jest.fn(),
          stayOnPage: actions.stayOnPage,
          app: {
            $env: {},
          },
        };

        dismissPageLeaveWarningDialogueSpy = jest.spyOn(
          NativeCallbacks, 'dismissPageLeaveWarningDialogue',
        ).mockImplementation(() => true);

        app.stayOnPage(mutation);
      });
      it('will call commit for the RESET', () => {
        expect(mutation.commit).toHaveBeenCalledWith(RESET);
      });

      it('will dispatch to set the menu item as previous', () => {
        expect(app.dispatch).toBeCalledWith('navigation/setPreviousMenuItem');
      });

      it('will call native app to dismiss the dialogue', () => {
        expect(dismissPageLeaveWarningDialogueSpy).toHaveBeenCalled();
      });
    });

    describe('is not using native app', () => {
      let store;
      beforeEach(() => {
        redirectTo.mockClear();
        window.nativeApp = false;
        store =
        {
          dispatch: jest.fn(),
          app: {
            $env: jest.fn(),
          },
        };
        actions.stayOnPage.call(store, mutation);
      });
      it('will call commit for the RESET', () => {
        expect(mutation.commit).toHaveBeenCalledWith(RESET);
      });

      it('will dispatch to set the menu item as previous', () => {
        expect(store.dispatch).toHaveBeenCalledWith('modal/hide');
      });
    });
  });

  describe('leavePage', () => {
    let store;
    beforeEach(() => {
      redirectTo.mockClear();
      window.nativeApp = true;
      store =
      {
        app: {
          $env: jest.fn(),
        },
      };
      actions.leavePage.call(store, mutation);
    });
    it('will call commit for the RESET', () => {
      expect(mutation.commit).toHaveBeenCalledWith(RESET);
    });

    it('will call commit for the SHOULD_BYPASS_ROUTE_GUARD passing in true', () => {
      expect(mutation.commit).toHaveBeenCalledWith(SHOULD_BYPASS_ROUTE_GUARD, true);
    });

    it('will call redirect to the INDEX page', () => {
      expect(redirectTo).toHaveBeenCalledWith(store, INDEX.path);
    });
  });
});
