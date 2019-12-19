<template>
  <div v-if="isVisible"
       :class="[!$store.state.device.isNativeApp && $style.desktopWeb, 'pull-content']">
    <message-dialog message-type="error">
      <message-text>{{ subheader }}</message-text>
      <message-text :aria-label="messageLabel">{{ messageText }}</message-text>
    </message-dialog>
    <form method="get">
      <generic-button :class="['nhsuk-button']"
                      @click.stop.prevent="onRetryButtonClicked">
        {{ retryButtonText }}
      </generic-button>
    </form>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import isObject from 'lodash/fp/isObject';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import { INDEX } from '@/lib/routes';

export default {
  name: 'ConnectionError',
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
  },
  mixins: [ErrorMessageMixin],
  props: {
    withTitle: {
      type: Boolean,
      required: false,
      default: () => false,
    },
  },
  computed: {
    isVisible() {
      return this.showError();
    },
    header() {
      return this.getMessage('header');
    },
    subheader() {
      return this.getMessage('subheader');
    },
    message() {
      return this.getMessage('message');
    },
    messageLabel() {
      return isObject(this.message) ? this.message.label : undefined;
    },
    messageText() {
      return isObject(this.message) ? this.message.text : this.message;
    },
    retryButtonText() {
      return this.getMessage('retryButtonText');
    },
  },
  updated() {
    this.$store.dispatch('errors/setConnectionProblem', process.client && !navigator.onLine);
    this.$store.dispatch('header/updateHeaderText', this.getMessage('header'));
    this.$store.dispatch('pageTitle/updatePageTitle', this.getMessage('header'));
  },
  methods: {
    onRetryButtonClicked() {
      if (this.$store.getters['session/isProxying']) {
        this.$router.push(INDEX.path);
      } else {
        this.$router.go();
      }
    },
    showError() {
      return this.hasConnectionError;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/buttons';

  .desktopWeb {
    button.button {
      width: auto;
      min-width: 12.5em;
    }
  }
</style>
