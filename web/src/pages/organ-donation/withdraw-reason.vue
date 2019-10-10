<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <message-dialog v-if="showErrors" id="errors">
      <message-text data-purpose="error-heading">
        {{ $t('organDonation.withdrawReason.errorMessageHeader') }}
      </message-text>
      <message-list data-purpose="reason-error">
        <li>{{ $t('organDonation.withdrawReason.errorMessageText') }}</li>
      </message-list>
    </message-dialog>
    <div :class="$style.info">
      <h2>{{ $t('organDonation.withdrawReason.subheader') }}</h2>
      <p v-for="(item, index) in $t('organDonation.withdrawReason.bodyItems')" :key="index">
        {{ item }}
      </p>
      <error-group :show-error="showErrors">
        <label :class="[$style.label, $style['mb-2']]" for="reason">
          {{ $t('organDonation.withdrawReason.reason.label') }}
        </label>
        <error-message v-if="showErrors">
          {{ $t('organDonation.withdrawReason.errorMessageText') }}
        </error-message>
        <select-dropdown v-model="reasonId"
                         :class="[$style.select, $style['mb-4']]"
                         :required="true"
                         select-id="reason">
          <option v-for="option in reasons"
                  :key="option.id"
                  :value="option.id"
                  :disabled="option.value===''"
                  :selected="option.value===''">
            {{ option.displayName }}
          </option>
        </select-dropdown>
      </error-group>
      <generic-button id="continue-button"
                      :class="['nhsuk-button']"
                      @click.stop.prevent="continueClicked">
        {{ $t('organDonation.withdrawReason.continueButton') }}
      </generic-button>
      <generic-button v-if="!$store.state.device.isNativeApp"
                      id="back-button"
                      :class="['nhsuk-button', 'nhsuk-button--secondary']"
                      @click.stop.prevent="goBack" >
        {{ $t('generic.backButton.text') }}
      </generic-button>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import ErrorGroup from '@/components/ErrorGroup';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import { INDEX, ORGAN_DONATION, ORGAN_DONATION_REVIEW_YOUR_DECISION } from '@/lib/routes';
import { isNativeApp } from '@/components/NativeOnlyMixin';

export default {
  components: {
    GenericButton,
    ErrorGroup,
    ErrorMessage,
    MessageDialog,
    MessageList,
    MessageText,
    SelectDropdown,
  },
  data() {
    return {
      hasTriedToContinue: false,
      reasonId: get('organDonation.withdrawReasonId')(this.$store.state),
    };
  },
  computed: {
    reasons() {
      return [
        { id: '', displayName: this.$t('organDonation.withdrawReason.reason.placeholder') },
        ...get('organDonation.referenceData.withdrawReasons')(this.$store.state),
      ];
    },
    showErrors() {
      return (this.hasTriedToContinue && !this.reasonId);
    },
  },
  fetch({ redirect, route, store }) {
    if (!isNativeApp({ route, store })) {
      redirect(INDEX.path);
    } else if (!store.state.organDonation.isWithdrawing) {
      redirect(ORGAN_DONATION.path);
    }
  },
  methods: {
    continueClicked() {
      this.hasTriedToContinue = true;

      if (this.showErrors) {
        window.scrollTo(0, 0);
        return;
      }

      this.$store.dispatch('organDonation/setWithdrawReasonId', this.reasonId);
      this.$router.push(ORGAN_DONATION_REVIEW_YOUR_DECISION.path);
    },
    goBack() {
      this.$store.dispatch('organDonation/withdrawCancel');
      this.$router.push(ORGAN_DONATION.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/info";

  .label {
    margin-top: $one;
  }

  .select {
    margin-bottom: $three;
  }
</style>
