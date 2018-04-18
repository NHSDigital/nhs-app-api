import {
  SET_TITLE,
} from './mutation-types';

export const setTitle = ({ commit }, title) => {
  commit(SET_TITLE, title);
};

export default {
  setTitle,
};
