import {
  LOADING_COMPLETE,
  IS_LOADING,
} from './mutation-types';

export const isLoading = ({ commit }) => {
  commit(IS_LOADING, true);
};

export const loadingCompleted = ({ commit }) => {
  commit(LOADING_COMPLETE, true);
};


export default {
  isLoading,
  loadingCompleted,
};
