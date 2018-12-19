import actions from '@/store/modules/errors/actions';
import { SET_ROUTE_PATH } from '@/store/modules/errors/mutation-types';

describe('errors actions', () => {
  const app = {};
  beforeEach(() => {
    app.commit = jest.fn();
  });

  describe('setRoutePath', () => {
    describe('route object without params', () => {
      it('will commit the route path', () => {
        const route = { path: '/foo/bar' };
        actions.setRoutePath(app, route);
        expect(app.commit).toHaveBeenCalledWith(SET_ROUTE_PATH, '/foo/bar');
      });
    });

    describe('route object with matched param', () => {
      it('will commit the route path without the param placeholder', () => {
        const route = {
          path: '/some/route/with/param/12345',
          params: { id: '12345' },
        };
        actions.setRoutePath(app, route);
        expect(app.commit).toHaveBeenCalledWith(SET_ROUTE_PATH, '/some/route/with/param');
      });
    });

    describe('route path string', () => {
      it('will commit the route path', () => {
        actions.setRoutePath(app, '/boo/hoo');
        expect(app.commit).toHaveBeenCalledWith(SET_ROUTE_PATH, '/boo/hoo');
      });
    });
  });
});
