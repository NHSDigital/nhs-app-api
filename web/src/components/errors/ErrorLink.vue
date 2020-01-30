<template>
  <div v-if="isVisible" class="nhsuk-back-link nhsuk-u-margin-bottom-3">
    <a class="nhsuk-back-link__link nhsuk-u-padding-left-0" :class="$style['error-link']"
       :href="actionUrl"
       :target="target"
       data-purpose="main-back-button"
       @click.stop.prevent="onRetryButtonClicked">
      {{ $t(from) }}
    </a>
  </div>
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

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/typography';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/typography';

.error-link {
  @include nhsuk-typography-responsive(19);
}
</style>
