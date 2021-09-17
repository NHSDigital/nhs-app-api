<template>
  <div class="banner-panel--blue">
    <h2>{{ $t('components.proofLevelUpliftBanner.heading') }}</h2>
    <p>{{ $t(`components.proofLevelUpliftBanner.description.${description}`) }}</p>
    <generic-button class="nhsuk-button nhsuk-button--reverse"
                    @click.prevent.stop="onUpliftClick">
      {{ $t('components.proofLevelUpliftBanner.buttonText') }}
    </generic-button>
  </div>
</template>

<script>
import AuthorisationService from '@/services/authorisation-service';
import GenericButton from '@/components/widgets/GenericButton';
import NativeApp from '@/services/native-app';

export default {
  name: 'ProofLevelUpliftBanner',
  components: { GenericButton },
  props: {
    description: {
      type: String,
      default: 'default',
    },
  },
  computed: {
    upliftUrl() {
      const authorisationService = new AuthorisationService(this.$store.$env);
      const { upliftUrl } = authorisationService.generateUpliftUrl({
        cookies: this.$store.$cookies,
      });

      return upliftUrl;
    },
  },
  methods: {
    async onUpliftClick() {
      await this.$store.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: window.location.hostname,
            action: 'UpliftStarted',
          },
        })
        .then(({ token }) => {
          if (NativeApp.supportsNativeNhsLoginUplift()) {
            NativeApp.startNhsLoginUplift(token);
          } else {
            this.$store.dispatch('http/isLoadingExternalSite');
            window.location = `${this.upliftUrl}&asserted_login_identity=${token}`;
          }
        });
    },
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/proof-level-uplift-banner";
</style>
