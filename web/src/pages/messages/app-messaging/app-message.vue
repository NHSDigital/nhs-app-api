<template>
  <div v-if="showTemplate && loaded">
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

    <desktopGenericBackLink v-if="!isNativeApp"
                            data-purpose="back-link"
                            :path="backLink"
                            :button-text="$t('app_messaging.messages.backLink')"
                            @clickAndPrevent="backClicked"/>

  </div>
</template>

<script>
import Message from '@/components/messaging/Message';
import PageDivider from '@/components/widgets/PageDivider';
import PageTitle from '@/components/widgets/PageTitle';
import ScrollToAnchor from '@/components/widgets/ScrollToAnchor';
import { redirectTo } from '@/lib/utils';
import { HEALTH_INFORMATION_UPDATES_PATH } from '@/router/paths';
import get from 'lodash/fp/get';
import first from 'lodash/fp/first';
import takeWhile from 'lodash/fp/takeWhile';
import dropWhile from 'lodash/fp/dropWhile';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  name: 'AppMessagingAppMessagePage',
  components: {
    Message,
    PageDivider,
    PageTitle,
    ScrollToAnchor,
    DesktopGenericBackLink,
  },
  data() {
    return {
      loaded: false,
      sender: this.$store.state.messaging.selectedSender,
      isNativeApp: this.$store.state.device.isNativeApp,
      backLink: HEALTH_INFORMATION_UPDATES_PATH,
    };
  },
  computed: {
    messages() {
      return get('messages')(first(this.$store.state.messaging.senderMessages)) || [];
    },
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
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.loadMessage();
    },
  },
  async created() {
    await this.loadMessage();
  },
  methods: {
    async loadMessage() {
      const sender = this.$store.state.messaging.selectedSender;

      if (!sender) {
        redirectTo(this, HEALTH_INFORMATION_UPDATES_PATH);
        return;
      }

      await this.$store.dispatch('messaging/load', { sender });

      if (!this.messages.length) {
        redirectTo(this, HEALTH_INFORMATION_UPDATES_PATH);
      }

      this.loaded = true;
    },
    backClicked() {
      redirectTo(this, this.backLink);
    },
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
