import responseHeaders from '@/middleware/server/responseHeaders';

describe('responseHeaders', () => {
  it('will not set cache headers when url is login', () => {
    const response = {
      setHeader: jest.fn(),
    };
    const next = jest.fn();
    const req = {
      url: '/login',
    };
    responseHeaders(req, response, next);
    expect(response.setHeader).not.toHaveBeenCalled();
    expect(next).toBeCalled();
  });

  it('will set cache headers when url is not login', () => {
    const response = {
      setHeader: jest.fn(),
    };
    const next = jest.fn();
    const req = {
      url: '/other',
    };
    responseHeaders(req, response, next);
    expect(response.setHeader).toHaveBeenCalledWith('Cache-Control', 'no-cache, no-store, no-transform, private, must-revalidate');
    expect(next).toBeCalled();
  });
});
