import {
  UPDATE_HEADER_TEXT,
} from './mutation-types';

export const updateHeaderText = ({ commit }, header) => {
  commit(UPDATE_HEADER_TEXT, header);
};

export default {
  updateHeaderText,
};
