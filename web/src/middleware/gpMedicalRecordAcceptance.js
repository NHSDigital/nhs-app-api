import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { createRoutePathObject } from '@/lib/utils';
import sessionStorageGet from '@/lib/sessionStorage';

export default ({ store, next }) => {
  if (!store.state.myRecord.hasAcceptedTerms && !sessionStorageGet('agreedToMedicalWarning')) {
    next(createRoutePathObject({ path: GP_MEDICAL_RECORD_PATH, store }));
  } else {
    next();
  }
};
