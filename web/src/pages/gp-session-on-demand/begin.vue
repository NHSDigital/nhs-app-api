<template>
  <div/>
</template>

<script>
import AuthorisationService from '@/services/authorisation-service';
import NativeApp from '@/services/native-app';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  layout: 'nhsuk-layout',
  async mounted() {
    const authorisationService = new AuthorisationService(this.$store.$env);
    const redirectTo = this.$router.currentRoute.query.targetPage;
    const cookies = this.$store.$cookies;

    const { token } = await this.$store.app.$http
      .postV1PatientAssertedLoginIdentity({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: window.location.hostname,
        },
      });

    if (NativeApp.supportsCreateOnDemandGpSession()) {
      authorisationService.setAuthCookieForNativeOnDemandGpSession(cookies);
      NativeApp.createOnDemandGpSession(JSON.stringify({
        redirectTo,
        assertedLoginIdentity: token,
      }));
      return;
    }

    EventBus.$emit(UPDATE_TITLE, this.$t('navigation.pages.titles.pageLoading'));
    const { gpSessionConnectUrl } = authorisationService.generateGpSessionUrl({
      redirectTo,
      cookies,
    });

    const fullGpSessionConnectUrl = `${gpSessionConnectUrl}&asserted_login_identity=${token}`;
    this.$store.dispatch('http/isLoadingExternalSite');
    window.location = fullGpSessionConnectUrl;
  },
};
</script>
