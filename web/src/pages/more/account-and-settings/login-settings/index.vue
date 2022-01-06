<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="!biometricSupported" class="nhsuk-u-padding-top-4">
        <p>
          {{ $t('loginSettings.biometrics.noBiometricType.information.paragraph1') }}
        </p>
        <p>
          {{ $t('loginSettings.biometrics.noBiometricType.information.paragraph2') }}
        </p>
      </div>
      <div v-else>
        <p id="biometricInformation" class="nhsuk-u-padding-bottom-3">
          {{ biometricInformation }}
        </p>
        <message-dialog message-type="warning" :icon-text="$t('generic.important')">
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
import { MORE_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'LoginSettingsPage',
  components: {
    MessageDialog,
    MessageText,
    LabelledToggle,
  },
  data() {
    return {
      bannerDismissed: this.$store.state.biometricBanner.dismissed,
    };
  },
  computed: {
    biometricType() {
      return this.$store.getters['loginSettings/biometricType'];
    },
    toggleLabel() {
      return this.$t(`loginSettings.biometrics.toggleLabel.${this.biometricType}`);
    },
    biometricInformation() {
      return this.$t(`loginSettings.biometrics.biometricInformation.${this.biometricType}`);
    },
    biometricWarningText() {
      return this.$t(`loginSettings.biometrics.warningText.${this.biometricType}`);
    },
    biometricSupported() {
      return this.$store.getters['loginSettings/biometricSupported'];
    },
    isWaiting() {
      return this.$store.getters['loginSettings/isWaiting'];
    },
    registered: {
      get() {
        return this.$store.getters['loginSettings/biometricRegistered'];
      },
      set() {
        this.$store.dispatch('spinner/prevent', true);
        this.$store.dispatch('loginSettings/updateRegistration');

        if (!this.bannerDismissed) {
          this.$store.dispatch('biometricBanner/dismiss');
        }
      },
    },
  },
  async created() {
    if (await this.$store.dispatch('loginSettings/missingBiometricState')) {
      redirectTo(this, MORE_PATH);
    }
  },
  mounted() {
    this.$store.dispatch('loginSettings/clearErrorCode');
  },
};
</script>
