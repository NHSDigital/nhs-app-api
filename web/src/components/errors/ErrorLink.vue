<template>
  <p v-if="isVisible">
    <a :href="actionUrl"
       :rel="target === '_blank' ? 'noopener noreferrer': undefined"
       :target="target"
       data-purpose="main-back-button"
       @click.stop.prevent="onRetryButtonClicked">
      {{ $t(from) }}
    </a>
  </p>
</template>

<script>
export default {
  name: 'ErrorLink',
  props: {
    action: {
      type: String,
      default: '',
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
