<template>
  <a :href="generateMessageUrl(sender)"
     :class="$style['nhs-app-message__link']"
     :aria-label="messageLabel"
     @click.stop.prevent="goToMessages(sender)">
    <h2 class="nhsuk-heading-xs" aria-hidden="true">
      {{ sender }}
      <span :class="{
        [$style['nhs-app-message__date']]: true,
        [$style['nhs-app-message__date--unread']]: unreadCount
      }">{{ message.sentTime | formatDate('DD/MM/YYYY') }}</span>
    </h2>
    <p class="nhsuk-body-s" :class="$style['nhs-app-message__subject-line']"
       aria-hidden="true">
      <span v-if="unreadCount" :class="$style['nhs-app-message__meta']">
        <span :class="$style['nhs-app-message__count']">{{ unreadCount }}</span>
      </span>
      {{ sanitizedContent }}
    </p>
  </a>
</template>

<script>
import { createUri } from '@/lib/noJs';
import { formatDate } from '@/plugins/filters';
import { redirectTo, stripHtml } from '@/lib/utils';
import { MESSAGING_MESSAGES } from '@/lib/routes';

export default {
  name: 'SummaryMessage',
  props: {
    message: {
      type: Object,
      required: true,
    },
    sender: {
      type: String,
      required: true,
    },
    unreadCount: {
      type: Number,
      required: true,
    },
  },
  computed: {
    messageLabel() {
      let label = this.$t('messaging.index.hidden.intro')
        .replace('{sender}', this.sender)
        .replace('{date}', formatDate(this.message.sentTime, 'DD MMMM YYYY'));

      if (this.unreadCount > 0) {
        label += this.$t('messaging.index.hidden.unread')
          .replace('{count}', this.unreadCount)
          .replace('{plural}', this.unreadCount > 1 ? 's' : '');
      }
      return label;
    },
    sanitizedContent() {
      return stripHtml(this.message.body);
    },
  },
  methods: {
    generateMessageUrl(sender) {
      const noJs = {
        messaging: {
          selectedSender: sender,
        },
      };

      return createUri({ path: MESSAGING_MESSAGES.path, noJs });
    },
    goToMessages(sender) {
      this.$store.dispatch('messaging/selectSender', sender);
      redirectTo(this, MESSAGING_MESSAGES.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/settings/typography';
@import '~nhsuk-frontend/packages/core/tools/functions';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import '~nhsuk-frontend/packages/core/tools/typography';

@mixin overflow-ellipsis {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.nhs-app-message__link {
  @include nhsuk-responsive-padding(3);
  padding-left: $nhsuk-gutter-half;
  text-decoration: none;
  &:hover,
  &:focus {
    background-color: transparent;
    box-shadow: inset 0 0 0 $nhsuk-box-shadow-spread #ffcd60;
    outline: none;
  }
  h2 {
    @include nhsuk-responsive-padding(0);
    @include nhsuk-responsive-margin(0);
    width: calc(100% - 130px);
    overflow-wrap: break-word;
  }
}

.nhs-app-message__date {
  @include nhsuk-typography-responsive(16);
  position: absolute;
  right: nhsuk-spacing(7);
  top: 3px;
  @include nhsuk-responsive-margin(3, "top");
  font-weight: $nhsuk-font-normal;
  color: $nhsuk-secondary-text-color;
  @include govuk-media-query($until: tablet) {
    top: 1px;
  }
  &.nhs-app-message__date--unread {
    font-weight: $nhsuk-font-bold;
    color: $nhsuk-text-color;
  }
}

.nhs-app-message__subject-line {
  @include nhsuk-responsive-padding(1, "top");
  @include nhsuk-responsive-padding(0, "bottom");
  @include nhsuk-responsive-margin(0, "bottom");
  padding-right: 80px;
  color: $nhsuk-secondary-text-color;
  @include overflow-ellipsis;
  @include govuk-media-query($until: tablet) {
    max-width: 90%;
  }
}

.nhs-app-message__meta {
  float: right;
  position: absolute;
  right: nhsuk-spacing(7);
  .nhs-app-message__count {
    @include nhsuk-responsive-padding(1, "left");
    @include nhsuk-responsive-padding(1, "right");
    @include nhsuk-typography-responsive(14);
    font-weight: $nhsuk-font-bold;
    background-color: #FFC107;
    border-radius: nhsuk-spacing(3);
    color: $nhsuk-text-color;
    border: 1px solid #B58F1C;
    display: inline-block;
    min-width: nhsuk-spacing(4);
    text-align: center;
    @include govuk-media-query($until: tablet) {
      padding-top: 1px;
      padding-bottom: 1px;
    }
  }
}
</style>
