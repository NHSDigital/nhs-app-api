
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

        <ul id="gpInboxMessages" :class="$style['nhs-app-message']">
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
          :button-text="'gp_messages.view_details.backButtonText.text'"
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
import { redirectTo, isNumber, isEmptyArray } from '@/lib/utils';
import { formatDate } from '@/plugins/filters';
import srjIf from '@/lib/sjrIf';
import last from 'lodash/fp/last';
import {
  MESSAGES_PATH,
  INDEX_PATH,
  GP_MESSAGES_URGENCY_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
} from '@/router/paths';

export default {
  name: 'GpMessagesPage',
  components: {
    SummaryMessage,
    MenuItemListHeader,
    MenuItemList,
    MenuItem,
    DesktopGenericBackLink,
  },
  data() {
    return {
      messagesPath: MESSAGES_PATH,
    };
  },
  computed: {
    summaries() {
      return this.$store.state.gpMessages.messageSummaries;
    },
    summariesLoaded() {
      return this.$store.state.gpMessages.loadedMessages;
    },
    hasNoSummaries() {
      return !(this.summaries && this.summaries.length > 0);
    },
    additionalDetailsCallRequired() {
      return srjIf({ $store: this.$store, journey: 'requiredDetailsCallGpMessages' });
    },
    hasSubject() {
      return srjIf({ $store: this.$store, journey: 'sendMessageSubject' });
    },
  },
  created() {
    if (!this.$store.state.practiceSettings.im1MessagingEnabled) {
      redirectTo(this, INDEX_PATH);
      return;
    }

    this.$store.dispatch('gpMessages/setUrgencyChoice', undefined);
    this.$store.dispatch('gpMessages/clearSelectedRetainingId');
    this.$store.dispatch('gpMessages/clearSelectedRecipient');
    this.$store.dispatch('gpMessages/loadMessages');
  },
  methods: {
    getUnreadCount(unreadCount) {
      if (!isNumber(unreadCount) || unreadCount < 1) {
        return undefined;
      }

      return unreadCount;
    },
    sendMessage() {
      redirectTo(this, GP_MESSAGES_URGENCY_PATH);
    },
    getSubtitle(summary) {
      const { subject, content } = summary;
      if (this.hasSubject) {
        return subject;
      }

      const subtitle = isEmptyArray(summary.replies) ? content :
        last(summary.replies).replyContent;

      return (subtitle.length > 64) ? `${subtitle.substring(0, 64)}...` : subtitle;
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
      this.$store.dispatch('gpMessages/setSelectedMessageID', message.messageId);
      this.$store.dispatch('gpMessages/setSelectedRecipient', { name: message.recipient });

      if (!this.additionalDetailsCallRequired) {
        this.$store.dispatch('gpMessages/setMessageDetails', { messageDetails: message });
      }

      redirectTo(this, `${GP_MESSAGES_VIEW_MESSAGE_PATH}${
        message.unreadReplyInfo.present ? '#unreadMessages' : ''
      }`);
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
@import '../../../style/arrow';
@import '../../../style/messaging';
</style>
