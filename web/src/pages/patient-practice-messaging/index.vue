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
                             :has-subject="hasSubject"
                             :date-time="summary.lastMessageDateTime"
                             :aria-label="getMessageLabel(summary)"
                             :has-unread-messages="summary.unreadReplyInfo.present"
                             :list-index="index"
                             :unread-count="getUnreadCount(summary.unreadReplyInfo.count)"
                             @click="goToMessageDetails(summary)"/>
          </li>
        </ul>

        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          id="desktopBackLink"
          :path="messagesPath"
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
  MESSAGES,
  PATIENT_PRACTICE_MESSAGING_URGENCY,
  PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE,
  INDEX,
} from '@/lib/routes';
import { redirectTo, isNumber } from '@/lib/utils';
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
      messagesPath: MESSAGES.path,
      summaries: this.$store.state.patientPracticeMessaging.messageSummaries,
    };
  },
  computed: {
    summariesLoaded() {
      return this.$store.state.patientPracticeMessaging.loadedMessages;
    },
    hasNoSummaries() {
      return !(this.summaries && this.summaries.length > 0);
    },
    additionalDetailsCallRequired() {
      return srjIf({ $store: this.$store, journey: 'requiredDetailsCallPatientPracticeMessage' });
    },
    hasSubject() {
      return srjIf({ $store: this.$store, journey: 'sendMessageSubject' });
    },
  },
  async asyncData({ store, redirect }) {
    if (!store.state.practiceSettings.im1MessagingEnabled) {
      redirect(INDEX.path);
      return;
    }

    store.dispatch('patientPracticeMessaging/clearSelectedRetainingId');
    store.dispatch('patientPracticeMessaging/clearSelectedRecipient');

    await store.dispatch('patientPracticeMessaging/loadMessages');
  },
  mounted() {
    this.$store.dispatch('patientPracticeMessaging/setUrgencyChoice', undefined);
  },
  methods: {
    getUnreadCount(unreadCount) {
      if (!isNumber(unreadCount) || unreadCount < 1) {
        return undefined;
      }

      return unreadCount;
    },
    sendMessage() {
      redirectTo(this, PATIENT_PRACTICE_MESSAGING_URGENCY.path);
    },
    getSubtitle(summary) {
      const { subject, content } = summary;
      if (this.hasSubject) {
        return subject;
      }
      return (content.length > 64) ? `${content.substring(0, 64)}...` : content;
    },
    getMessageLabel(summary) {
      const { subject, recipient, lastMessageDateTime } = summary;

      return (this.hasSubject) ?
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
      redirectTo(this, this.messagesPath);
    },
    goToMessageDetails(message) {
      this.$store.dispatch('patientPracticeMessaging/setSelectedMessageID', message.messageId);
      this.$store.dispatch('patientPracticeMessaging/setSelectedRecipient', { name: message.recipient });

      if (!this.additionalDetailsCallRequired) {
        this.$store.dispatch('patientPracticeMessaging/setMessageDetails', { messageDetails: message });
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
