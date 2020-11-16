<template>
  <div v-if="showError"
       :class="[!isNativeApp && $style.desktopWeb, 'pull-content']">
    <message-dialog message-type="error">
      <message-text>
        {{ $t('generic.errors.thereIsAProblemWithYourInternetConnection') }}</message-text>
      <message-text
        :aria-label="$t('generic.errors.checkYourConnectionAndTryAgainOneOneOne')">
        {{ $t('generic.errors.checkYourConnectionAndTryAgain111') }}
        <a href="https://111.nhs.uk" target="_blank" rel="noopener noreferrer"
           style="display:inline">
          {{ $t('generic.nhs111Link') }}
        </a>
        {{ $t('generic.orCall') }}
      </message-text>
    </message-dialog>
    <generic-button :class="['nhsuk-button']"
                    @click.stop.prevent="onRetryButtonClicked">
      {{ $t('generic.tryAgain') }}
    </generic-button>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { redirectTo } from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';

export default {
  name: 'ConnectionError',
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
    };
  },
  computed: {
    showError() {
      const isNativeVersionAfter = this.$store.getters['appVersion/isNativeVersionAfter'];
      if (isNativeVersionAfter('1.41.0')) {
        return this.$store.state.errors.hasConnectionProblem && !this.isNativeApp;
      }
      return this.$store.state.errors.hasConnectionProblem;
    },
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
