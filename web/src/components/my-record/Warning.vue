<template>
  <div v-if="showTemplate"
       id="mainDiv"
       :class="[!$store.state.device.isNativeApp && $style.desktopWeb]">
    <message-dialog message-type="warning" :icon-text="$t('generic.important')">
      <message-text :class="$style.warningText">
        {{ $t('myRecord.warning.yourRecordMayContainSensitiveInformation') }}
      </message-text>
      <message-text :class="$style.warningText">
        {{ $t('myRecord.warning.youHaveALegalRightToAccess') }}
      </message-text>
    </message-dialog>
    <div :class="$style.info" data-purpose="info">
      <p>{{ $t('myRecord.warning.yourRecordShowsPersonalData') }}</p>
      <p>{{ $t('myRecord.warning.youMayAlsoSee') }}</p>
      <ul>
        <li>{{ $t('myRecord.warning.yourMedicalHistory') }}</li>
        <li>{{ $t('myRecord.warning.testResultsYouMayNotHaveDiscussedWithYourDoctor') }}</li>
      </ul>
    </div>
    <generic-button class="nhsuk-button"
                    @click.prevent="onContinueButtonClicked">
      {{ $t('generic.continue') }}
    </generic-button>

    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="indexPath"
      :button-text="'myRecord.warning.backToHome'"
      @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import NativeApp from '@/services/native-app';
import { INDEX_PATH } from '@/router/paths';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';

export default {
  name: 'Warning',
  components: {
    GenericButton,
    MessageDialog,
    MessageText,
    DesktopGenericBackLink,
  },
  props: {
    id: {
      type: String,
      default: '',
    },
  },
  data() {
    return {
      indexPath: INDEX_PATH,
    };
  },
  methods: {
    onContinueButtonClicked() {
      sessionStorage.setItem('agreedToMedicalWarning', true);
      this.$store.dispatch('myRecord/acceptTerms');
      this.$store.dispatch('myRecord/load');
      EventBus.$emit(FOCUS_NHSAPP_ROOT);
      if (this.$store.state.device.isNativeApp) {
        NativeApp.resetPageFocus();
      }
    },
    onBackButtonClicked() {
      redirectTo(this, this.indexPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  .h2 {
    padding-bottom: 0.5em;
  }
</style>
