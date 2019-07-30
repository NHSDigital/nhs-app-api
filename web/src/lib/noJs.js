export const noJsParameterName = 'nojs';

export const createUri = ({ path, noJs }) => {
  const noJsJson = JSON.stringify(noJs);
  return `${path}?${noJsParameterName}=${encodeURIComponent(noJsJson)}`;
};

export const ensureNoJsPostedValueIsArray = (value) => {
  if (Array.isArray(value)) {
    return value;
  }

  if (value) {
    return [value];
  }

  return [];
};
