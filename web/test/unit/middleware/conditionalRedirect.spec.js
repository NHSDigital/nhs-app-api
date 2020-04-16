import conditionalRedirect from '@/middleware/conditionalRedirect';
import { findByName } from '@/lib/routes';

jest.mock('@/lib/routes');

describe('middleware/conditionalRedirect', () => {
  const currentPath = '/current-path';
  let getters;
  let gettersSpy;
  let redirect;
  let store;

  const callConditionalRedirect = () => conditionalRedirect({ redirect, route: { name: 'foo' }, store });

  beforeEach(() => {
    getters = {};
    store = {
      get getters() { return getters; },
    };
    gettersSpy = jest.spyOn(store, 'getters', 'get');
    redirect = jest.fn();
  });

  describe('route detail does not exists', () => {
    beforeEach(() => {
      findByName.mockReturnValue(undefined);
      callConditionalRedirect();
    });

    it('will not call getters', () => {
      expect(gettersSpy).not.toBeCalled();
    });

    it('will not redirect', () => {
      expect(redirect).not.toBeCalled();
    });
  });

  describe('has route details', () => {
    let routeDetails;

    beforeEach(() => {
      routeDetails = { path: currentPath };
      findByName.mockReturnValue(routeDetails);
    });

    describe.each([
      undefined,
      { invalid: true },
    ])('redirect rules is `%o` (must be an array)', (redirectRules) => {
      beforeEach(() => {
        routeDetails.redirectRules = redirectRules;
        callConditionalRedirect();
      });

      it('will not call getters', () => {
        expect(gettersSpy).not.toBeCalled();
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
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
        routeDetails.redirectRules = [
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

          describe('redirect path is the same as current path', () => {
            beforeEach(() => {
              rule.url = currentPath;
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('will not redirect', () => {
              expect(redirect).not.toBeCalled();
            });
          });

          describe('redirect path is different than the current path', () => {
            beforeEach(() => {
              rule.url = '/different_path';
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('will redirect to rule url', () => {
              expect(redirect).toBeCalledWith('302', rule.url);
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

          it('will not redirect', () => {
            expect(redirect).not.toBeCalled();
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

          describe('redirect path is the same as current path', () => {
            beforeEach(() => {
              rule.url = currentPath;
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('will not redirect', () => {
              expect(redirect).not.toBeCalled();
            });
          });

          describe('redirect path is different than the current path', () => {
            beforeEach(() => {
              rule.url = '/different_path';
              callConditionalRedirect();
            });

            it('will call rule condition getter', () => {
              expect(conditionGetterSpy).toBeCalled();
            });

            it('will redirect to rule url', () => {
              expect(redirect).toBeCalledWith('302', rule.url);
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

          it('will not redirect', () => {
            expect(redirect).not.toBeCalled();
          });
        });
      });

      describe('rule with context', () => {
        beforeEach(() => {
          returnValue = jest.fn().mockImplementation(() => true);
          rule.url = '/context-path';
          rule.context = { foo: 'test' };
          callConditionalRedirect();
        });

        it('will call rule condition getter', () => {
          expect(conditionGetterSpy).toBeCalled();
        });

        it('will pass in the context to getter result', () => {
          expect(returnValue).toBeCalledWith(rule.context);
        });

        it('will redirect to rule url', () => {
          expect(redirect).toBeCalledWith('302', rule.url);
        });
      });
    });
  });
});
