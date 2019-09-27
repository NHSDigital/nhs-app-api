<template>
  <div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <page-title>
          <span :class="$style['messages-title']">
            <span :class="$style['messages-title-prefix']">
              {{ $t('messaging.messages.titlePrefix') }}
            </span>
            Everyone
          </span>
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

export default {
  layout: 'nhsuk-layout',
  components: {
    Message,
    PageDivider,
    PageTitle,
  },
  data() {
    return {
      readMessages: this.$store.state.messaging.readMessages,
      unreadMessages: this.$store.state.messaging.unreadMessages,
    };
  },
  computed: {
    hasReadMessages() {
      return this.readMessages.length > 0;
    },
    hasUnreadMessages() {
      return this.unreadMessages.length > 0;
    },
  },
  async fetch({ store }) {
    await store.dispatch('messaging/load');
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
    color: #425563;
    font-weight: 400;
  }
}
</style>
