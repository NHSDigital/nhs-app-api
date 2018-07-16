/* eslint-disable */
import NHSOnlineApi from '../services/nhsonlineapi';

export default ({ app, store, res }) => {
  debugger;
  const api = new NHSOnlineApi({
    domain: process.server ? process.env.API_HOST_SERVER : process.env.API_HOST,
    store,
    res,
    app,
  });

  if(process.server) {
    api.cookie = app.context.req.headers.cookie;
  }
  app.$http = api;
};
