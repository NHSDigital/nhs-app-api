export default (req, res, next) => {
  if (req.url === '/apple-app-site-association' || req.url === '/.well-known/apple-app-site-association') {
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
