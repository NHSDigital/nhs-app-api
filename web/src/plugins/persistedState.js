import createPersistedState from 'vuex-persistedstate';
import * as Cookies from 'js-cookie';
import cookie from 'cookie';
import { get } from 'lodash/fp';

const excludedProperties = [
  'session/validationInterval',
];

const removeProperty = (obj, path) => {
  const parentPath = path.substring(0, path.lastIndexOf('/'));
  const propName = path.substring(path.lastIndexOf('/') + 1);
  const parent = get(parentPath)(obj);
  if (parent) {
    delete parent[propName];
  }
};

const removeExcluded = obj => excludedProperties.forEach(x => removeProperty(obj, x));

export default ({ store, req, isDev }) => {
  createPersistedState({
    key: 'nhso',
    storage: {
      getItem: key =>
        (process.client
          ? Cookies.getJSON(key)
          : cookie.parse(req.headers.cookie || '')[key]),
      setItem: (key, value) => {
        const state = JSON.parse(value);
        removeExcluded(state);

        Cookies.set(
          key, JSON.stringify(state),
          { secure: !isDev },
        );
      },
      removeItem: key => Cookies.remove(key),
    },
  })(store);
};
