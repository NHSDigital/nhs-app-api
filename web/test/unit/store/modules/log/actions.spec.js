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
          postV1ApiLog: jest.fn().mockResolvedValue(),
        },
      },
      dispatch: jest.fn(),
    };

    const errorMessage = 'something went wrong';

    // act
    await onError.call(that, { commit: jest.fn() }, errorMessage);

    // assert
    expect(that.app.$http.postV1ApiLog).toBeCalled();
  });
});
