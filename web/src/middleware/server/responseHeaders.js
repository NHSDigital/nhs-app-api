module.exports = (req, res, next) => {
  res.setHeader('Cache-Control', 'no-cache, no-store, no-transform, private');
  next();
};
