/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import {
  IS_LOADING,
  LOADING_COMPLETE,
  ADD_CANCEL_REQUEST_HANDLER,
  CANCEL_REQUESTS,
  INIT_HTTP,
} from './mutation-types';

export default {
  [LOADING_COMPLETE](state) {
    state.loadingCounter -= 1;
  },
  [IS_LOADING](state) {
    state.loadingCounter += 1;
  },
  [ADD_CANCEL_REQUEST_HANDLER](state, handler) {
    state.cancelRequestHandlers.push(handler);
  },
  [CANCEL_REQUESTS](state) {
    let index = 0;
    for (index; index < state.cancelRequestHandlers.length; index += 1) {
      const handler = state.cancelRequestHandlers[index];
      if (typeof handler === 'function') handler();
    }
    state.cancelRequestHandlers = [];
    state.loadingCounter = 0;
  },
  [INIT_HTTP](state) {
    state = {
      loadingCounter: 0,
    };
  },
};
