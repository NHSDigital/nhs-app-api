module.exports = (key, def) => {
  if (process.env[key]) {
    return `"${process.env[key]}"`;
  }

  if (def) {
    return `"${def}"`;
  }

  return def;
};
