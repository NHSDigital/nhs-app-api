import { v4 as uuid } from 'uuid';

module.exports = (req, res, next) => {
  const currentRequestLifetimeVariables = res.locals;
  currentRequestLifetimeVariables.nhsoRequestId = uuid();
  next();
};
