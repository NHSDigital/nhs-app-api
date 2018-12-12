export const isFalsy = value => !(value && value !== 'false');
export const isTruthy = value => !isFalsy(value);
