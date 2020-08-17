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
        <collapsible-details id="age-info">
          <template slot="header">
            {{ $t('web.home.aged13To15InformationTitle') }}
          </template>
          <p>{{ $t('web.home.aged13To15Description') }}</p>
        </collapsible-details>
      </div>
    </div>
  </div>
</template>
<script>
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';

export default {
  name: 'PreRegistrationInformation',
  components: {
    CollapsibleDetails,
  },
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

    a {
        display: inline-block;
    }

</style>
