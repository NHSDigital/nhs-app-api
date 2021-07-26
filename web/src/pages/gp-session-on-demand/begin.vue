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
    const redirectTo = this.$router.currentRoute.query.targetPage;
    const { token } = await this.$store.app.$http
      .postV1PatientAssertedLoginIdentity({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: window.location.hostname,
        },
      });

    if (NativeApp.supportsCreateOnDemandGpSession()) {
      NativeApp.createOnDemandGpSession(JSON.stringify({
        redirectTo,
        assertedLoginIdentity: token,
      }));
      return;
    }

    EventBus.$emit(UPDATE_TITLE, this.$t('navigation.pages.titles.pageLoading'));
    const authorisationService = new AuthorisationService(this.$store.$env);
    const { gpSessionConnectUrl } = authorisationService.generateGpSessionUrl({
      redirectTo,
      cookies: this.$store.$cookies,
    });
    const fullGpSessionConnectUrl = `${gpSessionConnectUrl}&asserted_login_identity=${token}`;
    this.$store.dispatch('http/isLoadingExternalSite');
    window.location = fullGpSessionConnectUrl;
  },
};
</script>
