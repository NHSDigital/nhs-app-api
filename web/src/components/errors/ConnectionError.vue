<template>
  <div v-if="showError"
       :class="[!isNativeApp && $style.desktopWeb, 'pull-content']">
    <message-dialog override-style="plain">
      <message-text>
        {{ $t('generic.errors.checkYourConnection') }}</message-text>
      <message-text>{{ $t('generic.errors.ifTheProblemContinues') }}</message-text>
      <message-text
        :aria-label="$t('generic.errors.urgentMedicalAdvice.label')">
        {{ $t('generic.errors.urgentMedicalAdvice.forUrgentMedicalAdvice') }}
        <a :href="oneOneOneUrl" target="_blank" rel="noopener noreferrer" style="display:inline">
          {{ $t('generic.errors.urgentMedicalAdvice.nhs111Link') }}</a>
        {{ $t('generic.errors.urgentMedicalAdvice.orCall111') }}
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
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
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
      oneOneOneUrl: this.$store.$env.SYMPTOM_CHECKER_URL,
    };
  },
  computed: {
    showError() {
      return this.$store.state.errors.hasConnectionProblem;
    },
  },
  watch: {
    showError(value) {
      if (value) {
        EventBus.$emit(UPDATE_HEADER, 'generic.errors.internetConnectionError');
        EventBus.$emit(UPDATE_TITLE, 'generic.errors.internetConnectionError');
      }
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
  @import "@/style/custom/connection-error";
</style>
