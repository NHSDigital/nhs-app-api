<template>
  <div v-if="showTemplate && loaded">
    <div v-if="error">
      <error-container>
        <error-title title="messages.error.messageError" />
        <error-paragraph from="messages.error.problemGettingMessage" />
        <error-button from="generic.tryAgain" @click="reload" />
      </error-container>
    </div>
    <div v-else>
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <page-title css-class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
            <span class="nhsuk-caption-l nhsuk-u-margin-bottom-0">
              {{ $t('messages.messageFrom') }}
            </span>
            {{ sender }}
          </page-title>
        </div>
      </div>

      <div :class="$style['message-panel__item']">
        <formatted-date-time :class="$style['message-panel__time']"
                             :date-time="message.sentTime" />
        <div :class="$style['message-panel__content']">
          <markdown-content v-if="isMarkdown" class="panel-content" :content="message.body"
                            :message-id="message.id" />
          <linkify-content v-else class="panel-content" :content="message.body" tag="p" />
        </div>
      </div>

      <desktopGenericBackLink v-if="!isNativeApp"
                              data-purpose="back-link"
                              :path="backLink"
                              @clickAndPrevent="backClicked"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import FormattedDateTime from '@/components/widgets/FormattedDateTime';
import LinkifyContent from '@/components/widgets/LinkifyContent';
import MarkdownContent from '@/components/widgets/MarkdownContent';
import PageTitle from '@/components/widgets/PageTitle';
import get from 'lodash/fp/get';
import { HEALTH_INFORMATION_UPDATES_PATH, HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH } from '@/router/paths';
import { messageVersion, redirectTo } from '@/lib/utils';

export default {
  name: 'AppMessagingAppMessagePage',
  components: {
    DesktopGenericBackLink,
    ErrorButton,
    ErrorContainer,
    ErrorParagraph,
    ErrorTitle,
    FormattedDateTime,
    LinkifyContent,
    MarkdownContent,
    PageTitle,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      backLink: HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH,
      isNativeApp: this.$store.state.device.isNativeApp,
      loaded: false,
    };
  },
  computed: {
    error() {
      return this.$store.state.messaging.error;
    },
    isMarkdown() {
      return get('version')(this.message) === messageVersion.Markdown;
    },
    isUnread() {
      return get('read')(this.message) === false;
    },
    message() {
      return this.$store.state.messaging.message;
    },
    sender() {
      return get('sender')(this.message);
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.loadMessage();
    },
    isUnread: async function watchRead(value) {
      if (value) {
        this.$store.dispatch('messaging/markAsRead', this.message.id);
      }
    },
    sender: function watchSender(value) {
      this.$store.dispatch('messaging/selectSender', value);
    },
  },
  async created() {
    await this.loadMessage();
  },
  methods: {
    async loadMessage() {
      this.loaded = false;
      this.$store.dispatch('messaging/clear');
      const messageId = get('messageId')(this.$route.query);

      if (!messageId) {
        redirectTo(this, HEALTH_INFORMATION_UPDATES_PATH);
        return;
      }

      await this.$store.dispatch('messaging/loadMessage', { messageId });

      if (!this.error && !this.message) {
        redirectTo(this, HEALTH_INFORMATION_UPDATES_PATH);
        return;
      }

      this.loaded = true;
    },
    backClicked() {
      redirectTo(this, this.backLink, { sender: this.sender });
    },
  },
};
</script>

<style lang="scss">
p.panel-content > a{
  display: inline;
  font-weight: normal;
  vertical-align: unset;
}

div.panel-content{
  ol{
    padding-left:1.5em;
  }

  p > a{
    display: inline;
    font-weight: normal;
    vertical-align: unset;
  }

  img{
    display: block;
    max-width: 100%;
  }
}
</style>

<style module lang='scss' scoped>
  @import "@/style/custom/app-message";
  @import "@/style/custom/message";
</style>
