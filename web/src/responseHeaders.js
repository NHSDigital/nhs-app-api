const dontCacheUrls = ['/login', '/Login'];

module.exports = (req, res, next) => {
  if (!dontCacheUrls.includes(req.url)) {
    res.setHeader('Cache-Control', 'no-cache, no-store, no-transform, private, must-revalidate');
    res.setHeader('Pragma', 'no-cache');
  }
  next();
};
