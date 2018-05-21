import createPersistedState from 'vuex-persistedstate';
import * as Cookies from 'js-cookie';

const localStorage = () => {
  createPersistedState({
    storage: {
      getItem: key => Cookies.get(key),
      setItem: (key, value) => Cookies.set(key, value, { secure: true }),
      removeItem: key => Cookies.remove(key),
    },
  });
};

export default [localStorage];
