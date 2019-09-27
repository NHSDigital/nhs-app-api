<template>
  <li :class="$style['nhsuk-panel-group__item']">
    <div>
      <h3>{{ message.sender }}</h3>
      <div :class="$style['nhsuk-panel']">
        <!-- eslint-disable-next-line vue/no-v-html -->
        <p v-html="content"/>
      </div>
      <time :datetime="sentTimeAttribute">Sent {{ sentTime }}</time>
    </div>
  </li>
</template>

<script>
import moment from 'moment-timezone';

export default {
  name: 'Message',
  props: {
    message: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      content: this.message.body.replace(/\n/g, '<br />'),
      sentTime: moment.tz(this.message.sentTime, 'Europe/London').format('h:mma, DD MMMM YYYY', { trim: false }),
      sentTimeAttribute: moment.tz(this.message.sentTime, 'Europe/London').format('YYYY-MM-DD h:mma', { trim: false }),
    };
  },
};
</script>
<style module lang="scss" scoped>
  @import "~nhsuk-frontend/packages/nhsuk";

  .nhsuk-panel-group__item{
    @extend .nhsuk-panel-group__item;
    margin-bottom: nhsuk-spacing(3);
    width: 100%;
    > div {
      width: 100%;
    }
    h3{
      @extend %nhsuk-heading-xs;
      margin: nhsuk-spacing(0) nhsuk-spacing(0) nhsuk-spacing(2) nhsuk-spacing(0);
      padding: nhsuk-spacing(0);
    }
    .nhsuk-panel {
      margin: nhsuk-spacing(0);
      padding: nhsuk-spacing(2);
      border-radius: nhsuk-spacing(1);
      border: 1px solid $color_nhsuk-grey-4;
      width: 80%;
      overflow-wrap: break-word;
      word-wrap: break-word;
      -ms-word-break: break-all;
      word-break: break-all;
      word-break: break-word;
      -ms-hyphens: auto;
      -moz-hyphens: auto;
      -webkit-hyphens: auto;
      hyphens: auto;
    }
    time{
      @include nhsuk-typography-responsive(14);
      margin: nhsuk-spacing(0);
    }
  }

</style>
