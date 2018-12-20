import { Router } from 'express';
import confirmation from './confirmation';

export default () => {
  const router = Router();
  confirmation(router);

  return router;
};
