const createApp = router => ({
  use: jest.fn(),
  Router: jest.fn(() => router),
});

const createRouter = jest.fn(() => ({
  post: jest.fn(),
}));

class ExpressMock {
  constructor() {
    this.router = createRouter();
    this.app = createApp(this.router);
  }
}

export const expressMock = new ExpressMock();

export const Router = jest.fn(() => expressMock.router);
export default jest.fn(() => expressMock.app);

