<template>
  <div v-if="showTemplate"
       id="mainDiv"
       :class="[!$store.state.device.isNativeApp && $style.desktopWeb]">
    <message-dialog message-type="warning" :icon-text="$t('messageIconText.important')">
      <message-text :class="$style.warningText">
        {{ $t('my_record.personalRecordText.warningText.wt1') }}
      </message-text>
      <message-text :class="$style.warningText">
        {{ $t('my_record.personalRecordText.warningText.wt2') }}
      </message-text>
    </message-dialog>
    <div :class="$style.info" data-purpose="info">
      <p>{{ $t('my_record.personalRecordText.body') }}</p>
      <p>{{ $t('my_record.personalRecordText.bulletPointHeader') }}</p>
      <ul>
        <li>{{ $t('my_record.personalRecordText.bulletPoints.bp1') }}</li>
        <li>{{ $t('my_record.personalRecordText.bulletPoints.bp2') }}</li>
      </ul>
    </div>
    <form :action="myRecordPath" method="get">
      <input :value="JSON.stringify({ myRecord: { hasAcceptedTerms: true }})"
             type="hidden"
             name="nojs">

      <generic-button class="nhsuk-button"
                      @click="onContinueButtonClicked">
        {{ $t('my_record.personalRecordText.agreeButtonText') }}
      </generic-button>
    </form>

    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="indexPath"
      :button-text="'my_record.personalRecordText.backButtonText'"
      @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { INDEX, MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

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
      indexPath: INDEX.path,
      myRecordPath: MYRECORD.path,
    };
  },
  methods: {
    onContinueButtonClicked(event) {
      event.preventDefault();
      sessionStorage.setItem('hasAgreedToMedicalWarning', true);
      this.$store.dispatch('myRecord/acceptTerms');
      this.$store.dispatch('myRecord/load');
      EventBus.$emit(FOCUS_NHSAPP_ROOT);
    },
    onBackButtonClicked(event) {
      event.preventDefault();
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
