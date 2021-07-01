<template>
  <div/>
</template>

<script>
import AuthorisationService from '@/services/authorisation-service';

export default {
  layout: 'nhsuk-layout',
  async mounted() {
    const authorisationService = new AuthorisationService(this.$store.$env);
    const { gpSessionConnectUrl } = authorisationService.generateGpSessionUrl({
      redirectTo: this.$router.currentRoute.query.targetPage,
      cookies: this.$store.$cookies,
    });
    const { token } = await this.$store.app.$http
      .postV1PatientAssertedLoginIdentity({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: window.location.hostname,
        },
      });
    const fullGpSessionConnectUrl = `${gpSessionConnectUrl}&asserted_login_identity=${token}`;
    this.$store.dispatch('http/isLoadingExternalSite');
    window.location = fullGpSessionConnectUrl;
  },
};
</script>
