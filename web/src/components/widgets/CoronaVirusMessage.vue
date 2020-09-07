<template>
  <message-dialog
    message-id="corona-virus-warning"
    message-type="warning"
    :icon-text="$t('appointments.coronavirus.coronavirus')">
    <message-text>
      {{ $t('appointments.coronavirus.doNotBookIfYouThinkYouMightHave') }}
    </message-text>
    <message-text>
      {{ $t('appointments.coronavirus.stayAtHomeAndAvoidCloseContact') }}
    </message-text>
    <message-text>
      <analytics-tracked-tag
        href="https://111.nhs.uk/PWCorona/35fa201f-99cb-4327-b2a5-fda8fb673bf6/COVID-19/about"
        :text="$t('appointments.coronavirus.useThe111CoronavirusService')"
        tag="a"
        target="_blank">
        {{ $t('appointments.coronavirus.useThe111CoronavirusService') }}
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
