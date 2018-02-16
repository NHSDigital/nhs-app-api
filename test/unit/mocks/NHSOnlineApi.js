const DEFAULT_VALUES = ['One', 'Two', 'Three'];

export {
  DEFAULT_VALUES,
};

export default class NHSOnlineApi {
  constructor() {
    this.mockValues = DEFAULT_VALUES;
    this.getValues = jest.fn().mockReturnValue(Promise.resolve(this.mockValues));
  }
}
