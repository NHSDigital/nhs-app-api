<template>
  <li :class="$style['message-panel__item']">
    <div>
      <h3 :class="$style['message-panel__sender']">{{ message.sender }}</h3>
      <div :class="$style['message-panel__content']">
        <linkify-content class="panel-content" :content="message.body" tag="p" />
      </div>
      <formatted-date-time :class="$style['message-panel__time']"
                           :date-time="message.sentTime" />
    </div>
  </li>
</template>

<script>
import FormattedDateTime from '@/components/widgets/FormattedDateTime';
import LinkifyContent from '@/components/widgets/LinkifyContent';

export default {
  name: 'Message',
  components: {
    FormattedDateTime,
    LinkifyContent,
  },
  props: {
    message: {
      type: Object,
      required: true,
    },
  },
  created() {
    if (!this.message.read) {
      this.$store.dispatch('messaging/markAsRead', this.message.id);
    }
  },
};
</script>

<style lang="scss">
  p.panel-content > a{
    display: inline;
    font-weight: normal;
    vertical-align: unset;
  }
</style>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/settings/typography';
@import '~nhsuk-frontend/packages/core/tools/functions';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/mixins';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import '~nhsuk-frontend/packages/core/tools/typography';

.message-panel__item{
  @include nhsuk-responsive-margin(4, "bottom");
  list-style: none;
  width: 80%;
  &:last-child {
    @include nhsuk-responsive-margin(0, "bottom");
  }
}

.message-panel__sender{
  @include nhsuk-responsive-padding(0);
  @include nhsuk-responsive-margin(0);
  @include nhsuk-typography-responsive(16);
  color: $color_nhsuk-grey-1;
}

.message-panel__time{
  @include nhsuk-responsive-padding(0);
  @include nhsuk-responsive-margin(0);
  @include nhsuk-typography-responsive(14);
}

.message-panel__content{
  @include panel($color_nhsuk-white, $nhsuk-text-color);
  @include nhsuk-responsive-padding(2);
  @include nhsuk-responsive-margin(0);
  border-radius: nhsuk-spacing(1);
  border: 1px solid $color_nhsuk-grey-4;
  overflow-wrap: break-word;
  word-wrap: break-word;
  -ms-word-break: break-all;
  word-break: break-all;
  word-break: break-word;
}
</style>
