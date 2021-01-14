<template>
  <a :href="href"
     :class="$style['nhs-app-message__link']"
     :aria-label="ariaLabel"
     tabindex="0"
     @click.stop.prevent="$emit('click')">
    <div :class="$style['flex-baseline-container']" aria-hidden="true">
      <h2 :class="['nhsuk-heading-xs', $style['nhs-app-message__title']]">
        {{ title }}
      </h2>
      <formatted-date-time
        :class="{
          [$style['nhs-app-message__date']]: true,
          [$style['nhs-app-message__date--unread']]: unreadCount || hasUnreadMessages
        }"
        :date-time="dateTime"
        summary-time-format />
    </div>
    <div :class="$style['flex-baseline-container']" aria-hidden="true">
      <p id="subject" :class="{
        'nhsuk-body-s': true,
        [$style['nhs-app-message__subject-line']]: true,
        [$style['nhs-app-message__subject-line--unread']]: unreadCount|| hasUnreadMessages
      }">
        {{ subTitle }}
      </p>
      <br>
      <span v-if="hasUnreadMessages" :class="$style['nhs-app-message__meta']">
        <span :id="'unreadIndicator' + listIndex"
              :class="$style['nhs-app-message__count']">{{ unreadCount }}</span>
      </span>
    </div>
  </a>
</template>

<script>
import FormattedDateTime from '@/components/widgets/FormattedDateTime';

export default {
  name: 'SummaryMessage',
  components: {
    FormattedDateTime,
  },
  props: {
    title: {
      type: String,
      required: true,
    },
    subTitle: {
      type: String,
      required: true,
    },
    text: {
      type: String,
      required: false,
      default: undefined,
    },
    dateTime: {
      type: String,
      required: true,
    },
    hasUnreadMessages: {
      type: Boolean,
      required: false,
      default: false,
    },
    unreadCount: {
      type: Number,
      required: false,
      default: undefined,
    },
    ariaLabel: {
      type: String,
      required: false,
      default: undefined,
    },
    href: {
      type: String,
      required: false,
      default: '#',
    },
    listIndex: {
      type: Number,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/summary-message";
</style>
