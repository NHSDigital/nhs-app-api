<template>
  <div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <page-title>
          <span :class="$style['messages-title']">
            <span :class="$style['messages-title-prefix']">
              {{ $t('messaging.messages.titlePrefix') }}
            </span>{{ sender }}</span>
        </page-title>
      </div>
    </div>

    <ul v-if="hasReadMessages" id="readSection" :class="$style['panel-group-nomargin']">
      <message v-for="(message, index) in readMessages" :key="index" :message="message" />
    </ul>

    <page-divider v-if="hasUnreadMessages" :text="$t('messaging.messages.unreadMessages')" />

    <ul v-if="hasUnreadMessages" id="unreadSection" :class="$style['panel-group-nomargin']">
      <message v-for="(message, index) in unreadMessages" :key="index" :message="message" />
    </ul>
  </div>
</template>

<script>
import Message from '@/components/messaging/Message';
import PageDivider from '@/components/widgets/PageDivider';
import PageTitle from '@/components/widgets/PageTitle';
import { dropWhile, first, get, takeWhile } from 'lodash/fp';
import { redirectTo } from '@/lib/utils';
import { MESSAGING } from '@/lib/routes';

export default {
  layout: 'nhsuk-layout',
  components: {
    Message,
    PageDivider,
    PageTitle,
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
      redirect(MESSAGING.path);
      return;
    }

    await store.dispatch('messaging/load', sender);
  },
  created() {
    if (!this.messages.length) {
      redirectTo(this, MESSAGING.path);
    }
  },
};
</script>

<style module lang='scss' scoped>
@import '~nhsuk-frontend/packages/nhsuk';

ul.panel-group-nomargin {
  @extend .nhsuk-panel-group;
  padding-left: 0;
}

.messages-title {
  @include nhsuk-typography-responsive(32);
  .messages-title-prefix {
    @include nhsuk-typography-responsive(19);
    display: block;
    color: $nhsuk-secondary-text-color;
    font-weight: 400;
  }
}
</style>
