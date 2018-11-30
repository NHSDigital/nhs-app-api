/* eslint-disable import/prefer-default-export */
export const createUri = ({ path, noJs }) => {
  const noJsJson = JSON.stringify(noJs);
  return `${path}?nojs=${encodeURIComponent(noJsJson)}`;
};
