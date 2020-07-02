import getters from '@/store/modules/loginSettings/getters';
import each from 'jest-each';

describe('getters', () => {
  const { getBiometricWarningText,
    getBiometricInformation,
    retrieveCannotFindErrorText,
    getDeviceBiometricNameString } = getters;
  describe('biometricInformation', () => {
    each(['fingerPrint', 'face', 'touch'])
      .it('will return the correct biometricInformation based on the biometric type',
        (biometricType) => {
          const currentState = { biometricType };
          expect(getBiometricInformation(currentState)).toEqual(`loginSettings.biometrics.biometricInformation.${biometricType}`);
        });
  });

  describe('biometricWarning', () => {
    each(['fingerPrint', 'face', 'touch'])
      .it('will return the correct biometricWarning based on the biometric type',
        (biometricType) => {
          const currentState = { biometricType };
          expect(getBiometricWarningText(currentState)).toEqual(`loginSettings.biometrics.warningText.${biometricType}`);
        });
  });

  describe('getDeviceBiometricNameString', () => {
    each(['fingerPrint', 'face', 'touch'])
      .it('will return the correct biometricNameString based on the biometric type if not undefined',
        (biometricType) => {
          const currentState = { biometricType };
          expect(getDeviceBiometricNameString(currentState)).toEqual(`loginSettings.biometrics.biometricType.${biometricType}`);
        });

    it('will return undefined for the biometricNameString based on the biometric type if undefined', () => {
      const currentState = { biometricType: undefined };
      expect(getDeviceBiometricNameString(currentState)).toEqual(undefined);
    });
  });

  describe('retrieveCannotFindErrorText', () => {
    each(['fingerPrint', 'face', 'touch'])
      .it('will return the correct biometricNameString based on the biometric type if not undefined',
        (biometricType) => {
          const currentState = { biometricType };
          expect(retrieveCannotFindErrorText(currentState)).toEqual(`loginSettings.biometrics.errors.cannotFindBiometricType.errorText.${biometricType}`);
        });
  });
});
