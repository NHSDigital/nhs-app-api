import { INDEX } from '@/lib/routes';

export default ({ route, store, app }) => {
  if (store.getters['linkedAccounts/isRecoveringFromProxyLoss']) {
    store.dispatch('flashMessage/addError', app.i18n.tc('linkedProfiles.lossProxyError'));
    store.dispatch('flashMessage/show');
  }

  if (route.name === INDEX.name) {
    store.dispatch('linkedAccounts/proxyRecoveryComplete');
  }
};
