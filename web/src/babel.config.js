// eslint-disable-line unexpected-unnamed-function
module.exports = function (api) {
  if (api) {
    api.cache(true);
  }

  const plugins = ['babel-plugin-root-import', {
    rootPathPrefix: '@',
    rootPathSuffix: 'src',
  }];

  return {
    plugins,
  };
};
