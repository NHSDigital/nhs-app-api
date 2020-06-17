function middlewarePipeline(context, middleware, index, routerNext) {
  const nextMiddleware = middleware[index];

  if (!nextMiddleware) {
    return routerNext();
  }

  return nextMiddleware({
    ...context,
    next: (someVal) => {
      if (someVal) {
        routerNext(someVal);
      } else {
        middlewarePipeline(context, middleware, index + 1, routerNext);
      }
    },
  });
}

export default middlewarePipeline;
