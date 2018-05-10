import {
  UPDATE_IS_NATIVE_APP,
} from './mutation-types';

export const updateIsNativeApp = ({ commit }, isNativeApp) => {
  commit(UPDATE_IS_NATIVE_APP, isNativeApp);
};

export default {
  updateIsNativeApp,
};
