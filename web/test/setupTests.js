// initialise console.error handler
let consoleError;
beforeEach(() => {
  consoleError = jest.spyOn(global.console, 'error');
});

afterEach(() => {
  expect(consoleError).not.toHaveBeenCalled();
});

// initialise scrollTo
global.scrollTo = jest.fn();
