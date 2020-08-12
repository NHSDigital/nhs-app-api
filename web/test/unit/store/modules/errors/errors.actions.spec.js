import actions from '@/store/modules/errors/actions';
import { SET_ROUTE_PATH, SET_CONNECTION_PROBLEM } from '@/store/modules/errors/mutation-types';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

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
        const that = {
          getters: {
            'session/isProxying': false,
          },
        };
        actions.setRoutePath.call(that, app, '/boo/hoo');
        expect(app.commit).toHaveBeenCalledWith(SET_ROUTE_PATH, '/boo/hoo');
      });
    });
  });

  describe('setConnectionProblem', () => {
    beforeEach(() => {
      EventBus.$emit.mockClear();
    });

    it('will call commit and update the header when true', () => {
      actions.setConnectionProblem(app, true);

      // assert
      expect(app.commit).toHaveBeenCalledWith(SET_CONNECTION_PROBLEM, true);
      expect(EventBus.$emit).toHaveBeenCalledTimes(2);
      expect(EventBus.$emit).toHaveBeenNthCalledWith(1, UPDATE_HEADER, 'noConnection.header');
      expect(EventBus.$emit).toHaveBeenNthCalledWith(2, UPDATE_TITLE, 'noConnection.header');
    });

    it('will call commit but will not update the header when connection error is false', () => {
      actions.setConnectionProblem(app, false);

      // assert
      expect(app.commit).toHaveBeenCalledWith(SET_CONNECTION_PROBLEM, false);
      expect(EventBus.$emit).toHaveBeenCalledTimes(0);
    });
  });
});
