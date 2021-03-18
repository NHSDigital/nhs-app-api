<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-two-thirds">
      <div id="before-you-start" class="nhsuk-u-margin-top-1">
        <h2 v-if="shouldShowHeader" class="nhsuk-u-margin-bottom-3">
          {{ $t('login.beforeYouStart') }}
        </h2>
        <warning-callout :title="$t('login.notTheNhsCovidApp')">
          <p>
            {{ $t('login.toDownloadTheNhsCovidApp') }}
            <a href="https://covid19.nhs.uk" target="_blank" rel="noopener noreferrer">covid19.nhs.uk</a>.
          </p>
        </warning-callout>
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
import WarningCallout from '@/components/widgets/WarningCallout';

export default {
  name: 'PreRegistrationInformation',
  components: {
    CollapsibleDetails,
    WarningCallout,
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
  @import "@/style/custom/pre-registration-information";
</style>
