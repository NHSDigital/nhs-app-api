<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="!hasBiometricType" class="nhsuk-u-padding-top-4">
        <p>
          {{ $t('loginSettings.biometrics.noBiometricType.information.paragraph1') }}
        </p>
        <p>
          {{ $t('loginSettings.biometrics.noBiometricType.information.paragraph2') }}
        </p>
      </div>
      <div v-else>
        <p class="nhsuk-u-padding-bottom-3">
          {{ biometricInformation }}
        </p>
        <message-dialog message-type="warning" :icon-text="$t('messageIconText.important')">
          <message-text>
            {{ biometricWarningText }}
          </message-text>
        </message-dialog>
        <div class="nhsuk-u-margin-top-5">
          <labelled-toggle v-model="registered"
                           checkbox-id="updateBiometricReg"
                           :is-waiting="isWaiting"
                           :label="toggleLabel"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import LabelledToggle from '@/components/widgets/LabelledToggle';

export default {
  layout: 'nhsuk-layout',
  components: {
    MessageDialog,
    MessageText,
    LabelledToggle,
  },
  data() {
    const biometricType = this.$t(this.$store.getters['loginSettings/getDeviceBiometricNameString']);
    return {
      biometricType,
      toggleLabel: this.$t('loginSettings.biometrics.toggleLabel', { biometricType }),
      biometricInformation: this.$t(this.$store.getters['loginSettings/getBiometricInformation']),
      biometricWarningText: this.$t(this.$store.getters['loginSettings/getBiometricWarningText']),
      hasBiometricType: this.$store.getters['loginSettings/deviceBiometricType'] !== undefined,
    };
  },
  computed: {
    isWaiting() {
      return this.$store.state.loginSettings.isWaiting;
    },
    registered: {
      get() {
        return this.$store.getters['loginSettings/biometricState'];
      },
      set() {
        this.$store.dispatch('spinner/prevent', true);
        this.$store.dispatch('loginSettings/updateRegistration');
      },
    },
  },
  mounted() {
    this.$store.dispatch('loginSettings/clearErrorCode');
  },
};
</script>
