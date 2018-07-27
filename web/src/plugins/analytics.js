export default ({ app, store }) => {
  /* eslint-disable-next-line no-param-reassign */
  app.$analytics = {
    trackButtonClick: (target) => {
      const action = {
        type: 'page_view',
        senderType: 'button',
        target,
      };
      store.dispatch('analytics/track', action);
    },
  };
};

