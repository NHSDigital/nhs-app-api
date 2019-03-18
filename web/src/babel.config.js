// eslint-disable-next-line func-names
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
