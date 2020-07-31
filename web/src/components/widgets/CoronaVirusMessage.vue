<template>
  <message-dialog
    message-id="corona-virus-warning"
    message-type="warning"
    :icon-text="$t('coronaVirus.title')">
    <message-text>
      {{ $t('coronaVirus.paragraphText1') }}
    </message-text>
    <message-text>
      {{ $t('coronaVirus.paragraphText2') }}
    </message-text>
    <message-text>
      <analytics-tracked-tag
        href="https://111.nhs.uk/PWCorona/35fa201f-99cb-4327-b2a5-fda8fb673bf6/COVID-19/about"
        :text="$t('coronaVirus.linkText1')"
        tag="a"
        target="_blank">
        {{ $t('coronaVirus.linkText1') }}
      </analytics-tracked-tag>
    </message-text>
  </message-dialog>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'CoronaVirusMessage',
  components: {
    MessageDialog,
    MessageText,
    AnalyticsTrackedTag,
  },
};
</script>
