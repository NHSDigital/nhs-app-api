<template>
  <message-dialog
    message-id="corona-virus-warning"
    message-type="warning"
    :icon-text="$t('appointments.coronavirus.coronavirus')">
    <message-text>
      {{ $t('appointments.coronavirus.doNotVisitIfYouThinkYouMightHave') }}
    </message-text>
    <message-text>
      {{ $t('appointments.coronavirus.stayAtHomeAndAvoidCloseContact') }}
    </message-text>
    <message-text>
      <contact-111
        :text="$t('appointments.coronavirus.forUrgentMedicalAdvice.text')"
        :aria-label="$t('appointments.coronavirus.forUrgentMedicalAdvice.label')"/>
    </message-text>
    <message-text>
      <analytics-tracked-tag
        :href="coronaConditionsUrl"
        :text="$t('appointments.coronavirus.findOutWhatToDo')"
        tag="a"
        target="_blank">
        {{ $t('appointments.coronavirus.findOutWhatToDo') }}
      </analytics-tracked-tag>
    </message-text>
  </message-dialog>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'CoronaVirusMessage',
  components: {
    Contact111,
    MessageDialog,
    MessageText,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      coronaConditionsUrl: this.$store.$env.CORONA_CONDITIONS_URL,
    };
  },
};
</script>
