<template>
  <div v-if="showTemplate && summariesLoaded" id="mainDiv">
    <p v-if="hasNoSummaries">{{ $t('im01.noMessages') }}</p>
    <template v-else>
      <h2>{{ $t('im01.subheader') }}</h2>
      <ul id="patientPracticeInboxMessages" :class="$style['nhs-app-message']">
        <li v-for="(summary, index) in summaries"
            :key="`summary-${index}`"
            :class="$style['nhs-app-message__item']">
          <summary-message :id="summary.id"
                           :title="summary.recipient"
                           :sub-title="summary.subject"
                           :date-time="summary.lastMessageDateTime"
                           :aria-label="getMessageLabel(summary)"
                           :has-unread-messages="true"
                           date-format="D MMMM YYYY"
                           @click="goToMessageDetails(summary.id, summary.recipient)"/>
        </li>
      </ul>
    </template>
  </div>
</template>

<script>
import SummaryMessage from '@/components/messaging/SummaryMessage';
import { INDEX, PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE } from '@/lib/routes';
import { isFalsy, redirectTo } from '@/lib/utils';
import { formatDate } from '@/plugins/filters';

export default {
  layout: 'nhsuk-layout',
  components: {
    SummaryMessage,
  },
  data() {
    return {
      summaries: this.$store.state.patientPracticeMessaging.messageSummaries,
    };
  },
  computed: {
    summariesLoaded() {
      return this.$store.state.patientPracticeMessaging.loaded;
    },
    hasNoSummaries() {
      return !(this.summaries && this.summaries.length > 0);
    },
  },
  async fetch({ store, redirect }) {
    if (isFalsy(store.app.$env.PATIENT_PRACTICE_MESSAGING_ENABLED)) {
      return redirect(INDEX.path);
    }
    return store.dispatch('patientPracticeMessaging/load');
  },
  methods: {
    getMessageLabel(summary) {
      return this.$t('im01.summary.hidden', {
        recipient: summary.recipient,
        subject: summary.subject,
        date: formatDate(summary.lastMessageDateTime, 'D MMMM YYYY'),
      });
    },
    goToMessageDetails(id, recipient) {
      this.$store.dispatch('patientPracticeMessaging/setSelectedMessageID', id);
      this.$store.dispatch('patientPracticeMessaging/setSelectedRecipient', recipient);
      redirectTo(this, PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '../../style/arrow';
@import '../../style/messaging';
</style>
