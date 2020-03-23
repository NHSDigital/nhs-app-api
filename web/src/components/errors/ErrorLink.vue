<template>
  <p v-if="isVisible">
    <a :href="actionUrl"
       :rel="target === '_blank' ? 'noopener noreferrer': undefined"
       :target="target"
       :aria-label="messageLabel"
       :data-purpose="dataPurpose"
       @click.stop.prevent="onRetryButtonClicked">
      {{ messageText }}
    </a>
  </p>
</template>

<script>
import { isObject } from 'lodash/fp';

export default {
  name: 'ErrorLink',
  props: {
    action: {
      type: String,
      default: '',
    },
    dataPurpose: {
      type: String,
      default: 'main-back-button',
    },
    desktopOnly: {
      type: Boolean,
      default: false,
    },
    from: {
      type: String,
      required: true,
    },
    queryParam: {
      type: Object,
      default: () => ({
        param: undefined,
        value: undefined,
      }),
    },
    target: {
      type: String,
      default: '',
    },
  },
  computed: {
    isVisible() {
      return !this.desktopOnly || !this.$store.state.device.isNativeApp;
    },
    actionUrl() {
      let url = this.action;
      if (this.queryParam.param && this.queryParam.value) {
        url += `?${this.queryParam.param}=${this.queryParam.value}`;
      }
      return url;
    },
    message() {
      return this.$t(this.from);
    },
    messageLabel() {
      if (isObject(this.message)) {
        return this.$t(`${this.from}.label`);
      }
      return undefined;
    },
    messageText() {
      const textPath = isObject(this.message) ? `${this.from}.text` : this.from;
      return this.$t(textPath);
    },
  },
  methods: {
    onRetryButtonClicked() {
      if (this.target === '_blank') {
        window.open(this.actionUrl, '');
        return;
      }
      this.goToUrl(this.actionUrl);
    },
  },
};
</script>
