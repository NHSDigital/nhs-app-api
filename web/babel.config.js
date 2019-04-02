module.exports = function (api) {
  if (api) {
    api.cache(true);
  }

  const ignore = [
    '**/.transpiled',
  ];

  const presets = [
    [
      '@babel/preset-env',
      {
        targets: {
          browsers: [
            'last 1 version',
            'ie >= 11',
          ],
          firefox: '60',
          chrome: '67',
          safari: '11.1',
        },
        useBuiltIns: 'usage',
      },
    ],
  ];

  const plugins = [
    ['@babel/plugin-transform-runtime',
      {
        helpers: true,
        regenerator: true,
        useESModules: false,
      },
    ],
  ];

  return {
    ignore,
    presets,
    plugins,
  };
};
