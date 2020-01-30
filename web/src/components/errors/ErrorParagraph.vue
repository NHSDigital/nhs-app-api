<template>
  <p v-if="messageText.isVisible" :aria-label="messageLabel" data-purpose="msg-text">
    {{ messageText.textBefore }}
    <span v-if="messageText.errorCode!=''" id="errorCode"
          class="nhsuk-u-font-weight-bold">{{ messageText.errorCode }}</span>
    {{ messageText.textAfter }}
  </p>
</template>

<script>
import { isObject } from 'lodash/fp';

export default {
  name: 'ErrorParagraph',
  props: {
    from: {
      type: String,
      required: true,
    },
    variable: {
      type: String,
      default: '',
    },
  },
  computed: {
    message() {
      return this.$t(this.from);
    },
    messageLabel() {
      if (isObject(this.message)) {
        return this.variable
          ? this.$t(`${this.from}.label`, { errorCode: this.variable })
          : this.message.label;
      }
      return undefined;
    },
    messageText() {
      if (this.variable) {
        const textPath = isObject(this.message) ? `${this.from}.text` : this.from;
        const text = this.$t(textPath, { errorCode: this.variable });
        const textArray = text.split(this.variable);
        return {
          isVisible: true,
          textBefore: textArray[0],
          errorCode: this.variable,
          textAfter: textArray[1],
        };
      }

      const text = isObject(this.message) ? this.message.text : this.message;

      return {
        isVisible: !text.toLowerCase().includes('{errorcode}'),
        textBefore: text,
        errorCode: '',
        textAfter: '',
      };
    },
  },
};
</script>
