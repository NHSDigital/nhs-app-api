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

.viewFullMessageText {
  color: #425563;
}

.nhs-app-message__link {
  padding: nhsuk-spacing(3);
  padding-right: nhsuk-spacing(7);
  text-decoration: none;
  &:hover,
  &:focus {
    background-color: transparent;
    box-shadow: inset 0 0 0 $nhsuk-box-shadow-spread $color_nhsuk-warm-yellow;
    outline: none;
  }

  .flex-baseline-container {
    display: flex;
    align-items: baseline;
    justify-content: space-between;

    .nhs-app-message__title {
      @include nhsuk-responsive-padding(0);
      @include nhsuk-responsive-margin(0);
      overflow-wrap: break-word;
      word-break: break-word;
    }

    .nhs-app-message__date {
      margin-left: nhsuk-spacing(2);
      @include nhsuk-typography-responsive(16);
      font-weight: $nhsuk-font-normal;
      color: $nhsuk-secondary-text-color;
      word-break: break-word;
      justify-content: right;

      &.nhs-app-message__date--unread {
        font-weight: $nhsuk-font-bold;
        color: $nhsuk-text-color;
      }
    }

    .nhs-app-message__subject-line {
      flex-grow: 1;
      flex-shrink: 1;
      @include nhsuk-responsive-padding(1, "top");
      @include nhsuk-responsive-padding(0, "bottom");
      @include nhsuk-responsive-margin(0, "bottom");
      color: $nhsuk-secondary-text-color;
      @include overflow-ellipsis;

      &.nhs-app-message__subject-line--unread{
        font-weight: $nhsuk-font-bold;
      }
    }

    .nhs-app-message__meta{
      flex-shrink: 0;
      margin-left: nhsuk-spacing(2);
      .nhs-app-message__count {
        @include nhsuk-responsive-padding(1, "left");
        @include nhsuk-responsive-padding(1, "right");
        @include nhsuk-typography-responsive(14);
        font-weight: $nhsuk-font-bold;
        background-color: $color_nhsuk-red;
        border-radius: nhsuk-spacing(3);
        color: $color_nhsuk-white;
        display: inline-block;
        min-width: nhsuk-spacing(4);
        min-height: nhsuk-spacing(4);
        text-align: center;
        @include govuk-media-query($until: tablet) {
          padding-top: 1px;
          padding-bottom: 1px;
        }
      }
    }
  }
}
</style>
