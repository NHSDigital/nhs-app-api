import AuthReturnLayout from '@/layouts/authReturn';
import { shallowMount } from '../helpers';

import each from 'jest-each';

describe('back to home links', () => {
  each([{
    errorStatusCode: 400,
    buttonText: 'auth_return.errors.400.backButtonText',
  }, {
    errorStatusCode: 403,
    buttonText: 'auth_return.errors.403.backButtonText',
  }, {
    errorStatusCode: 500,
    buttonText: 'auth_return.errors.500.backButtonText',
  }, {
    errorStatusCode: 502,
    buttonText: 'auth_return.errors.502.backButtonText',
  }, {
    errorStatusCode: 504,
    buttonText: 'auth_return.errors.504.backButtonText',
  }, {
    errorStatusCode: 999, // for v-else
    buttonText: 'auth_return.errors.default.backButtonText',
  }]).it('will link to /login', ({ errorStatusCode, buttonText }) => {
    // Arrange
    const layout = shallowMount(AuthReturnLayout, {
      $route: {
        query: {
          source: 'android',
        },
      },
      state: {
        device: {
          isNativeApp: true,
          source: 'android',
        },
        errors: {
          pageSettings: { errorOverrideStyles: [] },
          routePath: '/auth-return',
          apiErrors: [{ status: errorStatusCode }],
          hasConnectionProblem: true,
        },
      },
    });

    // Act
    const backToHomeActions = layout.findAll('api-error-button-stub').wrappers.filter(button => button.vm.from === buttonText).map(button => button.vm.action);

    // Assert
    backToHomeActions.forEach((action) => {
      expect(action).toBe('/login');
    });
  });
});
