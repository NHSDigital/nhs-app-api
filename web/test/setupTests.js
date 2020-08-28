// initialise console.error handler
let consoleError;
let consoleWarn;
beforeEach(() => {
  consoleError = jest.spyOn(global.console, 'error');
  consoleWarn = jest.spyOn(global.console, 'warn');
});

afterEach(() => {
  expect(consoleError).not.toHaveBeenCalled();
  expect(consoleWarn).not.toHaveBeenCalled();
});

// initialise scrollTo
global.scrollTo = jest.fn();
