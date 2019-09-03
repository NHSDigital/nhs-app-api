/* eslint-disable */
import NHSOnlineApi from '@/services/v1nhsonlineapi';
import CDSApi from '@/services/fhircdsapi';

export default ({ app, store, res, req }) => {

  if (!app) {
    return;
  }

  const api = new NHSOnlineApi({
    store,
    res,
    req,
    cookies: app.$cookies,
  });

  const cdsApi = new CDSApi({
    store,
    res,
    req,
    cookies: app.$cookies,
  });

  if(process.server && app.context && app.context.req) {
    api.cookie = app.context.req.headers.cookie;
    cdsApi.cookie = app.context.req.headers.cookie;
  }

  app.$http = api;
  app.$cdsApi = cdsApi;
};
