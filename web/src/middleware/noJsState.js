/* eslint-disable no-param-reassign */
import qs from 'qs';
import find from 'lodash/fp/find';
import mapKeys from 'lodash/fp/mapKeys';

const findNoJsKey = find(x => x.toLowerCase() === 'nojs');

const parseNoJs = (queryString) => {
  const key = findNoJsKey(Object.keys(queryString));
  if (!key) return undefined;
  try {
    return JSON.parse(queryString[key]);
  } catch (e) {
    // eslint-disable-next-line no-console
    console.log(`Error parsing JSON from the "${key}" query string parameter.`);
    return undefined;
  }
};

const getQueryString = ({ url }) => {
  const index = url.indexOf('?');
  return index === -1 ? undefined : url.substr(index + 1);
};

export default ({ req, store }) => {
  if (process.client) return;
  const queryString = qs.parse(getQueryString(req));
  const noJs = parseNoJs(queryString);

  if (noJs) {
    const merged = { ...store.state, ...noJs };
    mapKeys((key) => {
      store.state[key] = merged[key];
    })(merged);

    store.state.noJs = noJs;
  }
};
