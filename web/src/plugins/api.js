/* eslint-disable no-param-reassign */
import store from '@/store';
import NHSOnlineApiV1 from '@/services/v1nhsonlineapi';
import NHSOnlineApiV2 from '@/services/v2nhsonlineapi';
import NHSOnlineApiV3 from '@/services/v3nhsonlineapi';

const ApiPlugin = {
  install(Vue) {
    const api = new NHSOnlineApiV1({
      store,
      cookies: Vue.$cookies,
    });

    const apiV2 = new NHSOnlineApiV2({
      store,
      cookies: Vue.$cookies,
    });

    const apiV3 = new NHSOnlineApiV3({
      store,
      cookies: Vue.$cookies,
    });

    Vue.$http = api;
    Vue.$httpV2 = apiV2;
    Vue.$httpV3 = apiV3;

    Vue.prototype.$http = api;
    Vue.prototype.$httpV2 = apiV2;
    Vue.prototype.$httpV3 = apiV3;

    store.$http = api;
    store.$httpV2 = apiV2;
    store.$httpV3 = apiV3;
  },
};

export default ApiPlugin;

