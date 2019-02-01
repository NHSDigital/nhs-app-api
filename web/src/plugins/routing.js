/* eslint-disable no-param-reassign */
import get from 'lodash/fp/get';

export default ({ app }) => {
  let navigatingBack = false;

  app.router.previousPaths = [];
  app.router.goBack = () => {
    navigatingBack = true;
    if (app.router.previousPaths) {
      app.router.push(app.router.previousPaths.pop());
    } else {
      app.router.go(-1);
    }
  };

  app.router.afterEach((_, from) => {
    if (navigatingBack) {
      navigatingBack = false;
    } else {
      if (app.router.previousPaths.length >= 15) {
        app.router.previousPaths = app.router.previousPaths.splice(1);
      }
      app.router.previousPaths.push(get('path')(from));
    }
  });
};
