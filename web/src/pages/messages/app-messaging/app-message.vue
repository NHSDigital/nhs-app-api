<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <page-title css-class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
          <span class="nhsuk-caption-l nhsuk-u-margin-bottom-0">
            {{ $t('app_messaging.messages.titlePrefix') }}
          </span>
          {{ sender }}
        </page-title>
      </div>
    </div>

    <ul v-if="hasReadMessages" id="readSection" :class="$style['message-panel__list']">
      <message v-for="(message, index) in readMessages" :key="index" :message="message" />
    </ul>

    <template v-if="hasUnreadMessages">
      <scroll-to-anchor id="unreadMessages" />
      <page-divider :text="$t('app_messaging.messages.unreadMessages')" />

      <ul id="unreadSection" :class="$style['message-panel__list']">
        <message v-for="(message, index) in unreadMessages" :key="index" :message="message" />
      </ul>
    </template>
  </div>
</template>

<script>
import Message from '@/components/messaging/Message';
import PageDivider from '@/components/widgets/PageDivider';
import PageTitle from '@/components/widgets/PageTitle';
import ScrollToAnchor from '@/components/widgets/ScrollToAnchor';
import { redirectTo } from '@/lib/utils';
import { HEALTH_INFORMATION_UPDATES } from '@/lib/routes';
import get from 'lodash/fp/get';
import first from 'lodash/fp/first';
import takeWhile from 'lodash/fp/takeWhile';
import dropWhile from 'lodash/fp/dropWhile';

export default {
  layout: 'nhsuk-layout',
  components: {
    Message,
    PageDivider,
    PageTitle,
    ScrollToAnchor,
  },
  data() {
    return {
      messages: get('messages')(first(this.$store.state.messaging.senderMessages)) || [],
      sender: this.$store.state.messaging.selectedSender,
    };
  },
  computed: {
    hasReadMessages() {
      return this.readMessages.length > 0;
    },
    hasUnreadMessages() {
      return this.unreadMessages.length > 0;
    },
    readMessages() {
      return takeWhile(m => m.read)(this.messages);
    },
    unreadMessages() {
      return dropWhile(m => m.read)(this.messages);
    },
  },
  async fetch({ redirect, store }) {
    const sender = store.state.messaging.selectedSender;

    if (!sender) {
      redirect(HEALTH_INFORMATION_UPDATES.path);
      return;
    }

    await store.dispatch('messaging/load', sender);
  },
  created() {
    if (!this.messages.length) {
      redirectTo(this, HEALTH_INFORMATION_UPDATES.path);
    }
  },
};
</script>

<style module lang='scss' scoped>
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';

.message-panel__list {
  @include nhsuk-responsive-margin(2, "top");
  @include nhsuk-responsive-margin(2, "bottom");
  @include nhsuk-responsive-padding(0, "left");
  list-style: none;
}
</style>
