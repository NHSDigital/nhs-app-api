export const css = value => ({ value });

export const text = ({ value, element = '*' }) => ({
  value: `${element}[text() = '${value}']`,
  using: 'xpath',
});

export const textContains = ({ value, element = '*' }) => ({
  value: `*//${element}[text()[contains(., '${value}')]]`,
  using: 'xpath',
});
