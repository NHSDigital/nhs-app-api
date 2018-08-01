module.exports = (req, res, next) => {
  res.setHeader('X-Xss-Protection', '1; mode=block');
  res.setHeader('X-Frame-Options', 'DENY');
  res.setHeader('X-Content-Type-Options', 'nosniff');
  res.setHeader('Referrer-Policy', 'no-referrer');
  res.setHeader('Content-Security-Policy', 'frame-src \'self\' https://nhs.demdex.net/; frame-ancestors \'self\'; object-src \'none\'');
  res.setHeader('Strict-Transport-Security', 'max-age=31536000; includeSubDomains');
  if (req.url === '/apple-app-site-association') {
    res.setHeader('content-type', 'application/json');
    res.writeHead(200);
    const links = {
      applinks: {
        apps: [],
        details: [
          {
            appID: '5Y58TN94AP.com.nhs.online',
            paths: ['*'],
          },
        ],
      },
    };
    res.write(JSON.stringify(links));
    res.end();
  } else {
    next();
  }
};
