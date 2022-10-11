import actions from '@/store/modules/pageLeaveWarning/actions';
import { INDEX_PATH } from '@/router/paths';
import {
  SHOULD_BYPASS_ROUTE_GUARD,
  SHOW_LEAVING_PAGE_WARNING,
  SET_ATTEMPTED_REDIRECT_ROUTE,
  RESET,
} from '@/store/modules/pageLeaveWarning/mutation-types';
import NativeApp from '@/services/native-app';
import LeavingPageWarningModal from '@/components/modal/content/LeavingPageWarningModal';
import KeywordReplyLeavingPageWarningModal from '@/components/modal/content/KeywordReplyLeavingPageWarningModal';
import { redirectTo } from '@/lib/utils';
import { createRouter } from '../../../helpers';

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
        attemptedRedirectRoute: INDEX_PATH,
      },
    };
  });

  describe('setAttemptedRedirectRoute', () => {
    it('will call commit for the HIDE_EXPIRY_MESSAGE, passing through the route', () => {
      const route = { path: INDEX_PATH };
      actions.setAttemptedRedirectRoute(mutation, route);
      expect(mutation.commit).toHaveBeenCalledWith(SET_ATTEMPTED_REDIRECT_ROUTE, route);
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
        spy = jest.spyOn(NativeApp, 'displayPageLeaveWarning').mockImplementation(() => true);
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
        expect(NativeApp.displayPageLeaveWarning).toBeCalled();
      });
    });
    describe('not using native app', () => {
      let app;

      beforeEach(() => {
        window.nativeApp = false;
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

  describe('showKeywordReplyLeavingModal', () => {
    describe('using native app', () => {
      let spy;

      beforeEach(() => {
        window.nativeApp = true;
        spy = jest.spyOn(NativeApp, 'displayKeywordReplyPageLeaveWarning').mockImplementation(() => true);
        actions.showKeywordReplyLeavingModal(mutation);
      });

      afterEach(() => {
        (spy || {}).mockRestore();
        window.nativeApp = undefined;
      });

      it('will call commit for the SHOW_LEAVING_PAGE_WARNING', () => {
        expect(mutation.commit).toHaveBeenCalledWith(SHOW_LEAVING_PAGE_WARNING);
      });
      it('will call native function for showing the modal', () => {
        expect(NativeApp.displayKeywordReplyPageLeaveWarning).toBeCalled();
      });
    });

    describe('not using native app', () => {
      let app;

      beforeEach(() => {
        window.nativeApp = false;
        app = {
          dispatch: jest.fn(),
          showKeywordReplyLeavingModal: actions.showKeywordReplyLeavingModal,
          app: {
            $env: {},
          },
        };
        app.showKeywordReplyLeavingModal(mutation);
      });

      it('will call commit for the SHOW_LEAVING_PAGE_WARNING', () => {
        expect(mutation.commit).toHaveBeenCalledWith(SHOW_LEAVING_PAGE_WARNING);
      });
      it('will dispatch call for showing the modal', () => {
        expect(app.dispatch).toBeCalledWith('modal/show', { content: KeywordReplyLeavingPageWarningModal });
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
          NativeApp, 'dismissPageLeaveWarningDialogue',
        ).mockImplementation(() => true);

        app.stayOnPage(mutation);
      });
      it('will call commit for the RESET', () => {
        expect(mutation.commit).toHaveBeenCalledWith(RESET);
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
    let $store;
    const $router = createRouter('online-consultations');

    beforeEach(() => {
      redirectTo.mockClear();
      window.nativeApp = true;
      $store = {
        app: {
          $router,
          $env: jest.fn(),
        },
      };
      actions.leavePage.call($store, mutation);
    });
    it('will call commit for the RESET', () => {
      expect(mutation.commit).toHaveBeenCalledWith(RESET);
    });

    it('will call commit for the SHOULD_BYPASS_ROUTE_GUARD passing in true', () => {
      expect(mutation.commit).toHaveBeenCalledWith(SHOULD_BYPASS_ROUTE_GUARD, true);
    });

    it('will call redirect to the INDEX page', () => {
      expect(redirectTo).toHaveBeenCalledWith({ $router, $store }, INDEX_PATH);
    });
  });
});
