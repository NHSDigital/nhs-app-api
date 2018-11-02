const cacheUrls = ['/login', '/Login'];

module.exports = (req, res, next) => {
  if (!cacheUrls.includes(req.url)) {
    res.setHeader('Cache-Control', 'no-cache, no-store, no-transform, private, must-revalidate');
    res.setHeader('Pragma', 'no-cache');
  }
  next();
};
