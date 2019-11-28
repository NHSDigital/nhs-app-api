<template>
  <div>
    <div class="nhsuk-back-link">
      <a :class="[$style['nhsuk-back-link__link'], $style['desktopBackLink']]"
         :href="retryUrl"
         :target="target"
         data-purpose="main-back-button"
         @click.prevent="onRetryButtonClicked">
        {{ buttonText.text }}
      </a>
    </div>
  </div>
</template>

<script>

import { getDynamicStyle } from '@/lib/desktop-experience';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';

export default {
  name: 'ApiErrorButton',

  mixins: [ErrorMessageMixin],
  props: {
    from: {
      type: String,
      default: '',
    },
    action: {
      type: String,
      default: '',
    },
    target: {
      type: String,
      default: '',
    },
  },
  computed: {
    buttonText() {
      return this.getText(this.from);
    },
    retryUrl() {
      return this.action;
    },
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args);
    },
    onRetryButtonClicked() {
      if (this.target === '_blank') {
        window.open(this.retryUrl, '');
        return false;
      }
      this.goToUrl(this.retryUrl, this.statusCode);
      return false;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/all.scss';
@import '~nhsuk-frontend/packages/components/back-link/back-link';
@import '../../../style/buttons';

.desktopBackLink {
  font-size: 1.125em;
  line-height: 1.125em;
  font-weight: normal;
  vertical-align: middle;
  padding-left: 0;
 }
</style>
