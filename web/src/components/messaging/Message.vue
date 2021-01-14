<template>
  <li :class="$style['message-panel__item']">
    <div>
      <h3 :class="$style['message-panel__sender']">{{ message.sender }}</h3>
      <div :class="$style['message-panel__content']">
        <markdown-content v-if="isMarkdown" class="panel-content" :content="message.body" />
        <linkify-content v-else class="panel-content" :content="message.body" tag="p" />
      </div>
      <formatted-date-time :class="$style['message-panel__time']"
                           :date-time="message.sentTime" />
    </div>
  </li>
</template>

<script>
import FormattedDateTime from '@/components/widgets/FormattedDateTime';
import LinkifyContent from '@/components/widgets/LinkifyContent';
import MarkdownContent from '@/components/widgets/MarkdownContent';
import { messageVersion } from '@/lib/utils';

export default {
  name: 'Message',
  components: {
    FormattedDateTime,
    LinkifyContent,
    MarkdownContent,
  },
  props: {
    message: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      isMarkdown: this.message.version === messageVersion.Markdown,
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

<style module lang="scss" scoped>
  @import "@/style/custom/message";
</style>
