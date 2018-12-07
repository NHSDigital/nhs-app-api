export const noJsParameterName = 'nojs';

export const createUri = ({ path, noJs }) => {
  const noJsJson = JSON.stringify(noJs);
  return `${path}?${noJsParameterName}=${encodeURIComponent(noJsJson)}`;
};
