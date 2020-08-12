/* eslint-disable import/no-extraneous-dependencies */
/* eslint-disable no-param-reassign */
import get from 'lodash/fp/get';

const RoutingPlugin = {
  install(Vue, { router }) {
    let navigatingBack = false;

    router.previousPaths = [];
    router.goBack = () => {
      navigatingBack = true;
      if (router.previousPaths) {
        router.push(router.previousPaths.pop());
      } else {
        router.go(-1);
      }
    };

    router.afterEach((_, from) => {
      if (navigatingBack) {
        navigatingBack = false;
      } else {
        if (router.previousPaths.length >= 15) {
          router.previousPaths = router.previousPaths.splice(1);
        }
        router.previousPaths.push(get('path')(from));
      }
    });
  },
};

export default RoutingPlugin;
