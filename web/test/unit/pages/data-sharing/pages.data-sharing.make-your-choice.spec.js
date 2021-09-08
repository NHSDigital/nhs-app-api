import GenericButton from '@/components/widgets/GenericButton';
import MakeYourChoice from '@/pages/data-sharing/make-your-choice';
import NativeApp from '@/services/native-app';
import { NDOP_HELP_PATH } from '@/router/externalLinks';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/native-app');

describe('Make your choice', () => {
  let wrapper;
  let $store;
  let $http;

  const jwtToken = 'NdopToken';
  const dataPreferencesUrl = 'NdopUrl';

  beforeEach(() => {
    NativeApp.openPostWebIntegration = jest.fn();

    $http = {
      getV1PatientNdop: jest.fn().mockImplementation(() => Promise.resolve({ token: jwtToken })),
    };

    $store = createStore({
      $http,
      $env: {
        DATA_PREFERENCES_URL: dataPreferencesUrl,
      },
    });
  });

  describe('is native app', () => {
    beforeEach(() => {
      jest.clearAllMocks();
      wrapper = mount(MakeYourChoice, { $store });

      NativeApp.supportsNativeWebPostIntegration = jest.fn().mockImplementation(() => true);

      wrapper.find(GenericButton).trigger('click');
    });

    it('will call getV1PatientNdop to get the token', () => {
      expect($store.app.$http.getV1PatientNdop).toBeCalled();
    });

    it('will get call NativeApp.openPostWebIntegration with the token', () => {
      expect(NativeApp.openPostWebIntegration).toBeCalledWith(
        dataPreferencesUrl, { token: jwtToken }, [], NDOP_HELP_PATH,
      );
    });
  });

  describe('is not native app', () => {
    beforeEach(() => {
      jest.clearAllMocks();
      wrapper = mount(MakeYourChoice, { $store });

      NativeApp.supportsNativeWebPostIntegration = jest.fn().mockImplementation(() => false);
      wrapper.vm.$refs.ndopTokenForm = {
        submit: jest.fn(),
      };

      wrapper.find(GenericButton).trigger('click');
    });

    it('will call getV1PatientNdop to get the token', () => {
      expect($store.app.$http.getV1PatientNdop).toBeCalled();
    });

    it('will submit the form with the token', () => {
      expect(wrapper.vm.$refs.ndopTokenForm.submit).toBeCalled();
    });
  });
});
