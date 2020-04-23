<template>
  <div class="banner-panel--blue">
    <h2>{{ $t('components.proofLevelUpliftBanner.heading') }}</h2>
    <p>{{ $t('components.proofLevelUpliftBanner.description') }}</p>
    <generic-button class="nhsuk-button nhsuk-button--reverse">
      {{ $t('components.proofLevelUpliftBanner.buttonText') }}
    </generic-button>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';

export default {
  name: 'ProofLevelUpliftBanner',
  components: { GenericButton },
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
