import Vue from 'vue';
import Vuex from 'vuex';
import createLogger from 'vuex/dist/logger';

// Add our Modules here
import auth from './modules/auth';
import http from './modules/http';

Vue.use(Vuex);

const debug = process.env.NODE_ENV !== 'production';

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

  plugins: debug ? [createLogger()] : [],
});
