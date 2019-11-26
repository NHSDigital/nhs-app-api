import actions from '@/store/modules/log/actions';

const { onError } = actions;

describe('onError', () => {
  it('will call postV1ApiLog with correct parameter', async () => {
    // arrange
    const that = {
      $store: {
        dispatch: jest.fn(),
      },
      app: {
        $http: {
          postV1ApiLog: jest.fn().mockImplementation(() => Promise.resolve()),
        },
      },
      dispatch: jest.fn(),
    };

    const errorMessage = 'something went wrong';

    // act
    await onError.call(that, { commit: jest.fn() }, errorMessage);

    // assert
    expect(that.app.$http.postV1ApiLog).toBeCalled();
    expect(that.dispatch).toHaveBeenNthCalledWith(1, 'spinner/prevent', true);
    expect(that.dispatch).toHaveBeenNthCalledWith(2, 'spinner/prevent', false);
  });
});
