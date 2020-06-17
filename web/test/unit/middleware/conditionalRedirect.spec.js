import conditionalRedirect from '@/middleware/conditionalRedirect';
import { isNhsAppRouteName } from '@/router/names';
import * as dependency from '@/lib/utils';

jest.mock('@/router/names');

describe('middleware/conditionalRedirect', () => {
  const currentName = 'current-name';
  let getters;
  let gettersSpy;
  let store;
  let to;
  const next = jest.fn();
  dependency.createRoutePathObject = jest.fn(x => ({ path: x.path }));

  const callConditionalRedirect = () => conditionalRedirect({ next, to, store });

  beforeEach(() => {
    getters = {
      'session/isLoggedIn': () => true,
    };
    store = {
      get getters() { return getters; },
    };
    gettersSpy = jest.spyOn(store, 'getters', 'get');
  });

  describe('route detail does not exists', () => {
    beforeEach(() => {
      to = { name: 'foo' };
      isNhsAppRouteName.mockReturnValue(undefined);
      callConditionalRedirect();
    });

    it('will not call getters', () => {
      expect(gettersSpy).not.toBeCalled();
    });

    it('next called with no parameters', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });

  describe('has route details', () => {
    let meta;

    beforeEach(() => {
      meta = {
        redirectRules: { name: currentName },
      };
      to = {
        name: 'foo',
        meta,
      };
      isNhsAppRouteName.mockReturnValue(true);
    });

    describe.each([
      undefined,
      { invalid: true },
    ])('redirect rules is `%o` (must be an array)', (redirectRules) => {
      beforeEach(() => {
        meta.redirectRules = redirectRules;
        callConditionalRedirect();
      });

      it('will not call getters', () => {
        expect(gettersSpy).not.toBeCalled();
      });

      it('next called with no parameters', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });

    describe('has redirect rules', () => {
      let rule;
      let conditionGetterSpy;
      let returnValue;

      beforeEach(() => {
        const secondRule = { condition: 'condition/getter2' };
        rule = { condition: 'condition/getter' };
        getters = {
          get [rule.condition]() { return returnValue; },
          get [secondRule]() { return returnValue; },
        };
        conditionGetterSpy = jest.spyOn(getters, rule.condition, 'get');
        meta.redirectRules = [
          { condition: 'invalid/getter' },
          rule,
          secondRule,
        ];
      });

      describe.each([
        undefined,
        true,
      ])('rule value is `%s`', (value) => {
        beforeEach(() => {
          rule.value = value;
        });

        describe('getter returns true', () => {
          beforeEach(() => {
            returnValue = true;
          });

          describe('redirect name is the same as current name', () => {
            beforeEach(() => {
              rule.route = { name: currentName };
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('next called with no parameters', () => {
              expect(next).not.toBeCalledWith(expect.anything);
              expect(next).toBeCalled();
            });
          });

          describe('redirect path is different than the current path', () => {
            beforeEach(() => {
              rule.route = { path: '/different_path' };
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('will next to becalled with rule path', () => {
              expect(next).toBeCalledWith({ path: rule.route.path });
            });
          });
        });

        describe('getter returns false', () => {
          beforeEach(() => {
            returnValue = false;
            callConditionalRedirect();
          });

          it('will call rule condition getter', () => {
            expect(conditionGetterSpy).toBeCalled();
          });

          it('next called with no parameters', () => {
            expect(next).not.toBeCalledWith(expect.anything);
            expect(next).toBeCalled();
          });
        });
      });

      describe('rule value is false', () => {
        beforeEach(() => {
          rule.value = false;
        });

        describe('getter returns false', () => {
          beforeEach(() => {
            returnValue = false;
          });

          describe('redirect name is the same as current name', () => {
            beforeEach(() => {
              rule.route = { name: currentName };
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('next called with no parameters', () => {
              expect(next).not.toBeCalledWith(expect.anything);
              expect(next).toBeCalled();
            });
          });

          describe('redirect path is different than the current path', () => {
            beforeEach(() => {
              rule.route = { url: '/different_path' };
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('will next with to rule path', () => {
              expect(next).toBeCalledWith({ path: rule.route.path });
            });
          });
        });

        describe('getter returns true', () => {
          beforeEach(() => {
            returnValue = true;
            callConditionalRedirect();
          });

          it('will call rule condition getter', () => {
            expect(conditionGetterSpy).toBeCalled();
          });

          it('next called with no parameters', () => {
            expect(next).not.toBeCalledWith(expect.anything);
            expect(next).toBeCalled();
          });
        });
      });

      describe('rule with context', () => {
        beforeEach(() => {
          returnValue = jest.fn().mockImplementation(() => true);
          rule.route = { url: '/context-path' };
          rule.context = { foo: 'test' };
          callConditionalRedirect();
        });

        it('will call rule condition getter', () => {
          expect(conditionGetterSpy).toBeCalled();
        });

        it('will pass in the context to getter result', () => {
          expect(returnValue).toBeCalledWith(rule.context);
        });

        it('will next with to rule path', () => {
          expect(next).toBeCalledWith({ path: rule.route.path });
        });
      });
    });
  });
});
