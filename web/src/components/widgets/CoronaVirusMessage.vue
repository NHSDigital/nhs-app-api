<template>
  <message-dialog
    message-id="corona-virus-warning"
    message-type="warning"
    :icon-text="$t('coronaVirus.title')">
    <p class="nhsuk-u-margin-bottom-0">
      {{ $t('coronaVirus.paragraphText1') }}
    </p>
    <p class="nhsuk-u-margin-bottom-0">
      {{ $t('coronaVirus.paragraphText2') }}
    </p>
    <p class="nhsuk-u-margin-bottom-0">
      <analytics-tracked-tag
        href="https://111.nhs.uk/PWCorona/35fa201f-99cb-4327-b2a5-fda8fb673bf6/COVID-19/about"
        :text="$t('coronaVirus.linkText1')"
        tag="a"
        target="_blank">
        {{ $t('coronaVirus.linkText1') }}
      </analytics-tracked-tag>
    </p>
  </message-dialog>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'CoronaVirusMessage',
  components: {
    MessageDialog,
    AnalyticsTrackedTag,
  },
};
</script>
<style module lang="scss" scoped>
  a {
    display: inline;
    cursor: pointer;
  }
  p {
    padding: 1em 1em 0 1.4em;
    display: block;
  }
  ul {
    padding: 0 1em 0 2.5em;
  }
</style>
