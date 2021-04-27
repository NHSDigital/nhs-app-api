import each from 'jest-each';
import {
  getPathWithPatientIdPrefix,
  removePatientIdPrefixFromPath,
} from '@/lib/utils';

describe('util library patientId', () => {
  describe('getPathWithPatientIdPrefix', () => {
    const PATIENT_ID = 1234;
    let $store;
    beforeEach(() => {
      $store = {
        getters: {
          'linkedAccounts/getPatientId': PATIENT_ID,
          'linkedAccounts/isPatientIdNotEmpty': true,
        },
      };
    });

    it('will not add patient id prefix when url begins with patient/', () => {
      const param = { trimmedPath: 'patient/pathName', store: $store };
      // act
      const result = getPathWithPatientIdPrefix(param);
      expect(result).toBe('/patient/pathName');
    });

    it('will not add patient id prefix when url begins with patient/<id>/', () => {
      const patientId = '330b2795-e20f-427e-9699-7943dd31d4db';
      const param = { trimmedPath: `patient/${patientId}/pathName`, store: $store };
      // act
      const result = getPathWithPatientIdPrefix(param);
      expect(result).toBe(`/patient/${patientId}/pathName`);
    });

    it('will add patient id prefix when url does not begin with patient', () => {
      const patientId = '330b2795-e20f-427e-9699-7943dd31d4db';
      $store.getters['linkedAccounts/isPatientIdNotEmpty'] = true;
      $store.getters['linkedAccounts/getPatientId'] = patientId;
      const param = { trimmedPath: 'pathName', store: $store };
      // act
      const result = getPathWithPatientIdPrefix(param);
      expect(result).toBe(`/patient/${patientId}/pathName`);
    });
  });

  describe('removePatientIdPrefixFromPath', () => {
    each([
      [undefined, undefined],
      ['', ''],
      [null, null],
      [1, 1],
      [{}, {}],
      ['/', '/'],
      ['/patient', '/'],
      ['/patient/a', '/a'],
      ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835', '/'],
      ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835/a', '/a'],
      ['/patient/c5af7c34-b4ba-4f1a-bff8-476324e5f835/a/b', '/a/b'],
    ]).it('will remove the /patient/patientId prefix from the path', (path, expected) => {
      expect(removePatientIdPrefixFromPath(path)).toEqual(expected);
    });
  });
});
