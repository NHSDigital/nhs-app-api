/* eslint-disable no-param-reassign */

export default (context, inject) => {
  if (process.server) {
    const defaultValues = context.env;
    const environmentValues = process.env;

    Object.keys(context.env).forEach((key) => {
      context.env[key] = environmentValues[key] || defaultValues[key];
    });

    inject('env', context.env);

    context.beforeNuxtRender(({ nuxtState }) => {
      nuxtState.env = context.env;
    });
  } else {
    context.env = context.nuxtState.env;
    inject('env', context.env);
  }
};

/* eslint-enable no-param-reassign */
