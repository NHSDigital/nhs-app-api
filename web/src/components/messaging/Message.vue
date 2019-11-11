<template>
  <li :class="$style['nhsuk-panel-group__item']">
    <div>
      <h3>{{ message.sender }}</h3>
      <div :class="$style['nhsuk-panel']">
        <linkify-content class="panel-content" :content="message.body" tag="p" />
      </div>
      <time :datetime="sentTime | formatDate('YYYY-MM-DD h:mma')"
      >Sent {{ sentTime | formatDate('h:mma, DD MMMM YYYY') }}</time>
    </div>
  </li>
</template>

<script>
import LinkifyContent from '@/components/widgets/LinkifyContent';

export default {
  name: 'Message',
  components: {
    LinkifyContent,
  },
  props: {
    message: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      content: this.message.body,
      sentTime: this.message.sentTime,
    };
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
