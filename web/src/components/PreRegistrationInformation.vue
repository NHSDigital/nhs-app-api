<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-two-thirds">
      <div id="before-you-start" class="nhsuk-u-margin-top-1">
        <h2 v-if="shouldShowHeader" class="nhsuk-u-margin-bottom-3">
          {{ $t('web.home.beforeYouStartTitle') }}</h2>
        <p>{{ $t('web.home.beforeYouStartBulletListDescription') }}</p>
        <ul>
          <li class="nhsuk-u-margin-bottom-3">
            {{ $t('web.home.beforeYouStartBullets.one') }}
          </li>
          <li class="nhsuk-u-margin-bottom-3">
            {{ $t('web.home.beforeYouStartBullets.two') }}
          </li>
        </ul>
        <details id="age-info" class="nhsuk-details">
          <summary class="nhsuk-details__summary">
            <span class="nhsuk-details__summary-text">
              {{ $t('web.home.aged13To15InformationTitle') }}
            </span>
          </summary>
          <div class="nhsuk-details__text">
            <p>{{ $t('web.home.aged13To15Description') }}</p>
          </div>
        </details>
      </div>
    </div>
  </div>
</template>
<script>

export default {
  name: 'PreRegistrationInformation',
  props: {
    shouldShowHeader: {
      type: Boolean,
      default: true,
    },
  },
};
</script>
<style module lang="scss" scoped>
    @import "../style/home";
    @import "../style/spacings";
    @import "../style/webshared";

    @include desktopAndBelow() {
    }

    @include fromDesktop() {
    }

    a {
        display: inline-block;
    }

</style>
