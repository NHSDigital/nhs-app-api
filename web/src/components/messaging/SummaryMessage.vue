<template>
  <a :href="generateMessageUrl(sender)"
     :class="$style['nhs-app-message__link']"
     :aria-label="messageLabel"
     @click.stop.prevent="goToMessages(sender)">
    <h2 :class="$style['nhsuk-heading-xs']" aria-hidden="true">
      {{ sender }}
      <span :class="{
        [$style['nhs-app-message__date']]: true,
        [$style['nhs-app-message__date--unread']]: unreadCount
      }">{{ message.sentTime | formatDate('DD/MM/YYYY') }}</span>
    </h2>
    <p :class="[$style['nhsuk-body-s'], $style['nhs-app-message__subject-line']]"
       aria-hidden="true">
      <span v-if="unreadCount" :class="$style['nhs-app-message__meta']">
        <span :class="$style['nhs-app-message__count']">{{ unreadCount }}</span>
      </span>
      {{ sanitizedContent }}
    </p>
  </a>
</template>

<script>
import { formatDate } from '@/plugins/filters';
import { createUri } from '@/lib/noJs';
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
@import '~nhsuk-frontend/packages/nhsuk';

@mixin overflow-ellipsis {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.nhs-app-message__link {
  font-size: 1em;
  display: block;
  text-decoration: none;
  font-weight: $nhsuk-font-bold;
  padding: 0.8em 1em;
  &:hover,
  &:focus {
    background-color: transparent;
    box-shadow: inset 0 0 0 $nhsuk-box-shadow-spread #ffcd60;
    outline: none;
  }
  h2 {
    padding: 0 6.5em 0 0;
    font-family: $nhsuk-font, $nhsuk-font-fallback;
    margin: nhsuk-spacing(0);
    position: relative;
    @include overflow-ellipsis;
  }
}

.nhs-app-message__meta {
  float: right;
  position: absolute;
  right: 42px;
}

.nhs-app-message__subject-line {
  padding-top: 2px;
  padding-bottom: nhsuk-spacing(0);
  padding-right: 3em;
  margin-bottom: nhsuk-spacing(0);
  color: $nhsuk-secondary-text-color;
  @include overflow-ellipsis;
  @include govuk-media-query($until: tablet) {
    max-width: 90%;
  }
}

.nhs-app-message__date {
  float: right;
  font-size: 0.875rem;
  font-weight: $nhsuk-font-normal;
  position: absolute;
  right: nhsuk-spacing(4);
  top: 2px;
  color: $nhsuk-secondary-text-color;
}

.nhs-app-message__count {
  font-size: 0.75rem;
  font-weight: $nhsuk-font-bold;
  background-color: #FFC107;
  border-radius: nhsuk-spacing(3);
  padding: 1px nhsuk-spacing(1);
  color: $nhsuk-text-color;
  border: 1px solid #B58F1C;
    display: inline-block;
    min-width: nhsuk-spacing(4);
    text-align: center;
}

.nhs-app-message__count--long {
  padding: nhsuk-spacing(1) 6px;
}

.nhs-app-message__date--unread {
  font-weight: $nhsuk-font-bold;
  color: $nhsuk-text-color;
}
</style>
