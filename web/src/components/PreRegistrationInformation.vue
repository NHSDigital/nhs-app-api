<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-two-thirds">
      <div id="before-you-start" class="nhsuk-u-margin-top-1">
        <h2 v-if="shouldShowHeader" class="nhsuk-u-margin-bottom-3">
          {{ $t('login.beforeYouStart') }}</h2>
        <p>{{ $t('login.toUseThisServiceYouMustBe') }}</p>
        <ul>
          <li class="nhsuk-u-margin-bottom-3">
            {{ $t('login.registeredWithSurgeryInEngland') }}
          </li>
          <li class="nhsuk-u-margin-bottom-3">
            {{ $t('login.aged13AndOver') }}
          </li>
        </ul>
        <collapsible-details id="age-info">
          <template slot="header">
            {{ $t('login.whatToDoIfYouAre13To15') }}
          </template>
          <p>{{ $t('login.youWillNeedToContactSurgeryToRequestAccess') }}</p>
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
