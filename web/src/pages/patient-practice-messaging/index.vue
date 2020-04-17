<template>
  <div v-if="showTemplate && summariesLoaded" id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list class="nhsuk-u-margin-bottom-3">
        <menu-item id="sendMessageButton"
                   :text="$t('im01.sendMessageButtonText')"
                   :click-func="sendMessage"
                   header-tag="h2"
                   href="#"/>
      </menu-item-list>
      <p v-if="hasNoSummaries">{{ $t('im01.noMessages') }}</p>
      <template v-else>
        <menu-item-list-header id="messages_list_header"
                               header-tag="h2"
                               :text="$t('im01.subheader')"/>
        <p v-if="hasNoSummaries">{{ $t('im01.noMessages') }}</p>
        <ul id="patientPracticeInboxMessages" :class="$style['nhs-app-message']">
          <li v-for="(summary, index) in summaries"
              :key="`summary-${index}`"
              :class="$style['nhs-app-message__item']">
            <summary-message :id="summary.messageId"
                             :title="summary.recipient"
                             :sub-title="getSubtitle(summary)"
                             :date-time="summary.lastMessageDateTime"
                             :aria-label="getMessageLabel(summary)"
                             :has-unread-messages="summary.hasUnreadReplies"
                             :list-index="index"
                             :unread-count="getUnreadCount(summary.unreadCount)"
                             @click="goToMessageDetails(summary)"/>
          </li>
        </ul>
        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          id="desktopBackLink"
          :path="morePath"
          :button-text="'patient_practice_messaging.view_details.backButtonText.text'"
          @clickAndPrevent="backLinkClicked"/>
      </template>
    </div>
  </div>
</template>

<script>
import SummaryMessage from '@/components/messaging/SummaryMessage';
import MenuItemListHeader from '@/components/MenuItemListHeader';
import MenuItemList from '@/components/MenuItemList';
import MenuItem from '@/components/MenuItem';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import {
  MORE,
  PATIENT_PRACTICE_MESSAGING_URGENCY,
  PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE,
  INDEX,
} from '@/lib/routes';
import { redirectTo, datePart } from '@/lib/utils';
import { formatDate } from '@/plugins/filters';
import srjIf from '@/lib/sjrIf';

export default {
  layout: 'nhsuk-layout',
  components: {
    SummaryMessage,
    MenuItemListHeader,
    MenuItemList,
    MenuItem,
    DesktopGenericBackLink,
  },
  data() {
    return {
      summaries: this.$store.state.patientPracticeMessaging.messageSummaries,
    };
  },
  computed: {
    summariesLoaded() {
      return this.$store.state.patientPracticeMessaging.loadedMessages;
    },
    morePath() {
      return MORE.path;
    },
    hasNoSummaries() {
      return !(this.summaries && this.summaries.length > 0);
    },
    additionalDetailsCallRequired() {
      return srjIf({ $store: this.$store, journey: 'requiredDetailsCallPatientPracticeMessage' });
    },
  },
  async asyncData({ store, redirect }) {
    if (!store.state.practiceSettings.im1MessagingEnabled) {
      redirect(INDEX.path);
      return;
    }
    await store.dispatch('patientPracticeMessaging/loadMessages');
  },
  mounted() {
    this.$store.dispatch('patientPracticeMessaging/setUrgencyChoice', undefined);
  },
  methods: {
    getUnreadCount(unreadCount) {
      return (unreadCount > 0) ? unreadCount : undefined;
    },
    sendMessage() {
      redirectTo(this, PATIENT_PRACTICE_MESSAGING_URGENCY.path);
    },
    getSubtitle(summary) {
      return (summary.subject) ? summary.subject : this.$t('im01.lastMessageRecieved',
        { date: datePart(summary.lastMessageDateTime, 'YearMonthDayTime') });
    },
    getMessageLabel(summary) {
      const { subject, recipient, lastMessageDateTime } = summary;
      return (subject) ?
        this.$t('im01.summary.hiddenWithSubject', {
          recipient,
          subject,
          date: formatDate(lastMessageDateTime, 'D MMMM YYYY'),
        }) :
        this.$t('im01.summary.hiddenWithoutSubject', {
          recipient,
          date: formatDate(lastMessageDateTime, 'D MMMM YYYY'),
        });
    },
    backLinkClicked() {
      redirectTo(this, this.morePath);
    },
    goToMessageDetails(message) {
      this.$store.dispatch('patientPracticeMessaging/setSelectedMessageID', message.messageId);
      this.$store.dispatch('patientPracticeMessaging/setSelectedRecipient', { name: message.recipient });

      if (!this.additionalDetailsCallRequired) {
        this.$store.dispatch('patientPracticeMessaging/setMessageDetails',
          { messageDetails: {
            content: message.content,
            sentDateTime: message.sentDateTime,
            sender: message.sender,
            messageReplies: message.replies,
            attachmentId: message.attachmentId,
            outboundMessage: message.outboundMessage },
          });
      }
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
