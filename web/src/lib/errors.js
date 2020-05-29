function getText({ $i18n }, key) {
  return $i18n.te(key) ? $i18n.t(key) : '';
}

function getComponentErrorCodeKey({ $i18n }, showError, component, type,
  errorCode, statusCode) {
  if (showError) {
    return (errorCode && getText({ $i18n }, `${component}.errors.${statusCode}.${errorCode}.${type}`))
      || (errorCode && getText({ $i18n }, `${component}.errors.${errorCode}.${type}`))
      || getText({ $i18n }, `${component}.errors.${statusCode}.${type}`);
  }
  return '';
}

function getComponentKey({ $i18n }, component, type, domain) {
  return getText({ $i18n }, `${component}.${domain}.${type}`);
}

export const getMessage = ({ $store, $i18n }, type) => {
  const { status, error } = $store.state.errors.apiErrors[0];
  const showError = $store.getters['errors/showApiError'];
  const domain = showError ? 'errors' : 'noConnection';
  const component = $store.state.errors.routePath.substring(1).replace(/\//g, '.').replace(/-/g, '_');
  if (showError) {
    return getComponentErrorCodeKey({ $i18n }, showError, component, type, error, status)
    || getText({ $i18n }, `${component}.${domain}.${type}`)
    || getText({ $i18n }, `errors.${status}.${type}`)
    || getText({ $i18n }, `${domain}.${type}`);
  }
  return '';
};

export {
  getComponentKey,
  getComponentErrorCodeKey,
  getText,
};
