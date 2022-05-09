<template>
  <menu-item
    id="btn_messagesAdmin"
    data-purpose="text_link"
    header-tag="h2"
    :href="messagesPath"
    :text="$t(text)"
    :description="$t(description)"
    :click-func="navigate"
    :aria-label="ariaLabelCaption(text, description)"/>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { MESSAGES_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'MessagesMenuItem',
  components: {
    MenuItem,
  },
  props: {
    text: {
      type: String,
      required: true,
      default: undefined,
    },
    description: {
      type: String,
      required: true,
      default: undefined,
    },
  },
  data() {
    return {
      messagesPath: MESSAGES_PATH,
    };
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    navigate() {
      redirectTo(this, this.messagesPath);
    },
  },
};
</script>
