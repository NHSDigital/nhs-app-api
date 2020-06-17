<template>
  <div v-if="showError"
       :class="[!isNativeApp && $style.desktopWeb, 'pull-content']">
    <message-dialog message-type="error">
      <message-text>{{ this.$t('noConnection.subheader') }}</message-text>
      <message-text :aria-label="messageLabel">{{ messageText }}</message-text>
    </message-dialog>
    <generic-button :class="['nhsuk-button']"
                    @click.stop.prevent="onRetryButtonClicked">
      {{ $t('noConnection.retryButtonText') }}
    </generic-button>
  </div>
</template>

<script>
import isObject from 'lodash/fp/isObject';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { redirectTo } from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  name: 'ConnectionError',
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
  },
  props: {
    withTitle: {
      type: Boolean,
      required: false,
      default: () => false,
    },
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
    };
  },
  computed: {
    message() {
      return this.$t('noConnection.message');
    },
    messageLabel() {
      return isObject(this.message) ? this.message.label : undefined;
    },
    messageText() {
      return isObject(this.message) ? this.message.text : this.message;
    },
    showError() {
      return this.$store.state.errors.hasConnectionProblem && !this.isNativeApp;
    },
  },
  updated() {
    if (this.showError) {
      this.$store.dispatch('errors/setConnectionProblem', !navigator.onLine);
      EventBus.$emit(UPDATE_HEADER, 'noConnection.header');
      EventBus.$emit(UPDATE_TITLE, 'noConnection.header');
    }
  },
  methods: {
    onRetryButtonClicked() {
      if (!navigator.onLine) { return; }

      if (this.$store.getters['session/isProxying']) {
        redirectTo(this, INDEX_PATH);
      } else {
        this.$router.go();
      }
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
