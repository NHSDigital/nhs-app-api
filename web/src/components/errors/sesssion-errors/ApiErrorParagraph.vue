<template>
  <div v-if="messageText!==''">
    <message-text v-if="messageText.isVisible"
                  :aria-label="messageText.label"
                  :override-style="overrideStyle"
                  data-purpose="msg-text">
      {{ messageText.textBefore }}
      <span v-if="messageText.errorCode!=''" id="errorCode"
            :class="$style['errorCode']">
        {{ messageText.errorCode }}</span>{{ messageText.textAfter }}
    </message-text>
  </div>
</template>

<script>
import MessageText from '@/components/widgets/MessageText';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import isObject from 'lodash/fp/isObject';

export default {
  name: 'ApiErrorParagraph',
  components: {
    MessageText,
  },
  mixins: [ErrorMessageMixin],
  props: {
    from: {
      type: String,
      default: '',
    },
    variable: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
    message() {
      return this.from !== '' ? this.getText(this.from) : '';
    },
    messageText() {
      if (this.variable && isObject(this.variable) && this.variable.swap && this.variable.text !== '') {
        const textPath = `${this.from}.text`;
        const labelPath = `${this.from}.label`;
        const textReturned = this.$t(textPath, { errorCode: this.variable.text });
        const spacedLabel = this.variable.text.replace(/(.{1})/g, '$1 ');
        const labelReturned = this.$t(labelPath, { errorCode: spacedLabel });
        const textArray = textReturned.split(this.variable.text);
        return {
          isVisible: true,
          textBefore: textArray[0],
          errorCode: this.variable.text,
          textAfter: textArray[1],
          label: labelReturned,
        };
      }
      if (isObject(this.message)) {
        if (this.message.text.toLowerCase().includes('{errorcode}')) {
          return {
            isVisible: false,
            textBefore: this.message.text,
            errorCode: '',
            textAfter: '',
            label: this.message.label,
          };
        }
        return {
          isVisible: true,
          textBefore: this.message.text,
          errorCode: '',
          textAfter: '',
          label: this.message.label,
        };
      }
      return {
        isVisible: false,
        textBefore: '',
        errorCode: '',
        textAfter: '',
        label: '',
      };
    },
    messageLabel() {
      return this.message.label;
    },
  },
};
</script>

<style module lang="scss" scoped>
  .errorCode {
    font-weight: bold;
  }
</style>
