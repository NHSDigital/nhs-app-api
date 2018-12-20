/* eslint-disable class-methods-use-this */
const createApp = router => ({
  use: jest.fn(),
  Router: jest.fn(() => router),
});

const createRouter = jest.fn(() => ({
  post: jest.fn(),
}));

let router = createRouter();

class ExpressMock {
  constructor() {
    this.app = createApp(this.router);
  }

  get router() {
    return router;
  }
}

export const expressMock = new ExpressMock();

export const Router = jest.fn(() => expressMock.router).mockReturnValue(router);
export default jest.fn(() => expressMock.app);
export const reset = () => {
  router = createRouter();
};

