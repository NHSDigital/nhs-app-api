import {
  SHOULD_BYPASS_ROUTE_GUARD,
  SHOW_LEAVING_PAGE_WARNING,
  SET_ATTEMPTED_REDIRECT_ROUTE,
  RESET,
} from './mutation-types';
import LeavingPageWarningModal from '@/components/modal/content/LeavingPageWarningModal';
import NativeCallbacks from '@/services/native-app';
import { redirectTo } from '@/lib/utils';

export default {
  setAttemptedRedirectRoute({ commit }, route) {
    commit(SET_ATTEMPTED_REDIRECT_ROUTE, route);
  },

  reset({ commit }) {
    commit(RESET);
  },

  showLeavingModal({ state, commit }) {
    if (process.client && !state.showLeavingWarning) {
      commit(SHOW_LEAVING_PAGE_WARNING);

      if (window.nativeApp) {
        NativeCallbacks.displayPageLeaveWarning();
      } else {
        this.dispatch('modal/show', { content: LeavingPageWarningModal });
      }
    }
  },

  shouldSkipDisplayingLeavingWarning({ commit }, shouldSkipModal) {
    commit(SHOULD_BYPASS_ROUTE_GUARD, shouldSkipModal);
  },

  stayOnPage({ commit }) {
    if (window.nativeApp) {
      this.dispatch('navigation/setPreviousMenuItem');

      NativeCallbacks.dismissPageLeaveWarningDialogue();
    } else {
      this.dispatch('modal/hide');
    }
    commit(RESET);
    commit(SHOULD_BYPASS_ROUTE_GUARD, false);
  },

  leavePage({ commit, state }) {
    if (!window.nativeApp) {
      this.dispatch('modal/hide');
    }
    commit(SHOULD_BYPASS_ROUTE_GUARD, true);
    redirectTo(this, state.attemptedRedirectRoute.path);
    commit(RESET);
  },

  sessionExpiryCheckAndCloseLeavePageWarningModal({ commit, state }) {
    if (state.showLeavingWarning) {
      if (window.nativeApp) {
        this.dispatch('navigation/setPreviousMenuItem');
        this.dispatch('device/unlockNavBar');
      }
      commit(RESET);
      commit(SHOULD_BYPASS_ROUTE_GUARD, true);
    }
  },
};
