import Vue from 'vue';
import Vuex from 'vuex';
import createLogger from 'vuex/dist/logger';
import VuexPersist from 'vuex-persist';

// Add our Modules here
import auth from './modules/auth';
import http from './modules/http';

Vue.use(Vuex);

const debug = process.env.NODE_ENV !== 'production';

const vuexLocalStorage = new VuexPersist({
  key: 'vuex', // The key to store the state on in the storage provider.
  storage: window.localStorage, // or window.sessionStorage
});
export default new Vuex.Store({
  /**
   * Assign the modules to the store
   */

  modules: {
    auth,
    http,
  },
  /**
   * If strict mode should be enables
   */

  strict: debug,

  /**
   * Plugins used in the store
   */

  plugins: debug ? [createLogger(), vuexLocalStorage.plugin] : [vuexLocalStorage.plugin],
});
