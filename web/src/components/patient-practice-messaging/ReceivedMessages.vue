<template>
  <div>
    <ul :class="[$style['nhsuk-app-chat'],
                 'nhsuk-u-margin-top-2',
                 'nhsuk-u-margin-bottom-4',
                 'nhsuk-u-padding-left-0']">
      <li v-for="(readReply, readIndex) in readMessages"
          :id="`readMessageReply`+readIndex"
          :key="`readMessageReply`+readIndex"
          :class="[$style['nhsuk-panel-group__item'], 'nhsuk-u-padding-bottom-3']">
        <message-panel :index="readIndex" :id-prefix="'read'" :message="readReply"/>
      </li>
    </ul>
    <page-divider v-if="hasUnreadMessages" id="receivedMessagesDivider" :text="unreadText" />
    <ul :class="[$style['nhsuk-app-chat'],
                 'nhsuk-u-margin-top-2',
                 'nhsuk-u-margin-bottom-4',
                 'nhsuk-u-padding-left-0']">
      <li v-for="(unReadReply, unReadIndex) in unreadMessages"
          :id="`unReadMessageReply`+unReadIndex"
          :key="`unReadMessageReply`+unReadIndex"
          :class="[$style['nhsuk-panel-group__item'], 'nhsuk-u-padding-bottom-3']">
        <message-panel :index="unReadIndex" :id-prefix="'unread'" :message="unReadReply"/>
      </li>
    </ul>
  </div>
</template>
<script>
import dropWhile from 'lodash/fp/dropWhile';
import takeWhile from 'lodash/fp/takeWhile';
import MessagePanel from '@/components/patient-practice-messaging/ReceivedMessagePanel';
import PageDivider from '@/components/widgets/PageDivider';

export default {
  name: 'ReceivedMessage',
  components: { MessagePanel, PageDivider },
  props: {
    dividerVisible: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    getReplies() {
      return this.$store.state.patientPracticeMessaging
        .selectedMessageDetails.messageDetails.messageReplies;
    },
    readMessages() {
      return takeWhile(m => !m.isUnread)(this.getReplies);
    },
    unreadMessages() {
      return dropWhile(m => !m.isUnread)(this.getReplies);
    },
    hasUnreadMessages() {
      return this.unreadMessages.length > 0;
    },
    unreadText() {
      if (this.unreadMessages.length > 1) {
        return this.$t('patient_practice_messaging.view_details.unreadMessages');
      }
      return this.$t('patient_practice_messaging.view_details.unreadMessage');
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/settings/all';
  @import '~nhsuk-frontend/packages/core/tools/all';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/components/panel/panel';
  @import "../../style/colours";

  .nhsuk-panel-group__item {
    padding-right: 15%;
  }
  .nhsuk-app-chat {
    list-style-type: none;
  }

</style>
