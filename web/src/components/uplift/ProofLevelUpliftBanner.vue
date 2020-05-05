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
      const authorisationService = new AuthorisationService(this.$store.app.$env);
      const { upliftUrl } = authorisationService.generateUpliftUrl({
        isNativeApp: this.$store.state.device.isNativeApp,
        cookies: this.$cookies,
      });

      return upliftUrl;
    },
  },
  methods: {
    async onUpliftClick() {
      await this.$store.app.$http
        .postV1PatientAssertedLoginIdentity({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: window.location.hostname,
            action: 'UpliftStarted',
          },
        })
        .then(({ token }) => {
          window.location = `${this.upliftUrl}&asserted_login_identity=${token}`;
        });
    },
  },
};
</script>

<style lang="scss">
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/mixins';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';

.banner-panel--blue {
  @include panel($color_shade_nhsuk-blue-35, $color_nhsuk-white);
  @include nhsuk-responsive-padding(2, 'top');
  /* overriding this to prevent elements style file forcing black text*/
  p {
    color: $color_nhsuk-white;
  }
}
</style>
