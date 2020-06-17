import gpMedicalRecordAcceptance from '@/middleware/gpMedicalRecordAcceptance';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { createRoutePathObject } from '@/lib/utils';

jest.mock('@/lib/utils');

const routePathObject = `redirect/${GP_MEDICAL_RECORD_PATH}`;
createRoutePathObject.mockReturnValue(routePathObject);

const store = { state: { myRecord: {} } };
const next = jest.fn();

describe('gp medical record acceptance middleware', () => {
  describe('accepted warning and agreed to terms', () => {
    it('will not redirect to main record page', () => {
      store.state.myRecord.hasAcceptedTerms = true;

      gpMedicalRecordAcceptance({ store, next });

      expect(createRoutePathObject).not.toHaveBeenCalled();
      expect(next).not.toHaveBeenCalledWith(expect.anything());
    });
  });

  describe('not agreed to terms', () => {
    it('will redirect to main record page', () => {
      store.state.myRecord.hasAcceptedTerms = false;

      gpMedicalRecordAcceptance({ store, next });

      expect(createRoutePathObject).toHaveBeenCalledWith({ path: GP_MEDICAL_RECORD_PATH, store });
      expect(next).toHaveBeenCalledWith(routePathObject);
    });
  });
});
