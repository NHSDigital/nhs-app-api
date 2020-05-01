<template>
  <div v-if="showTemplate" id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div :class="[$style.form]">
        <message-dialog v-if="showErrors" id="errors">
          <message-text data-purpose="error-heading">
            {{ $t('organDonation.withdrawReason.errorMessageHeader') }}
          </message-text>
          <message-list data-purpose="reason-error">
            <li>{{ $t('organDonation.withdrawReason.errorMessageText') }}</li>
          </message-list>
        </message-dialog>
        <div>
          <h2>{{ $t('organDonation.withdrawReason.subheader') }}</h2>
          <p v-for="(item, index) in $t('organDonation.withdrawReason.explanations')"
             :key="index">
            {{ item }}
          </p>
          <ul>
            <li v-for="(item, index) of $t('organDonation.withdrawReason.exclusions')" :key="index">
              {{ item }}
            </li>
          </ul>
          <p>
            {{ $t('organDonation.withdrawReason.moreAboutLawText') }}
            <analytics-tracked-tag id="law-change"
                                   :href="lawChangeUrl"
                                   :text="$t('organDonation.withdrawReason.moreAboutLawLinkText')"
                                   class="inline"
                                   tag="a"
                                   target="_blank">
              {{ $t('organDonation.withdrawReason.moreAboutLawLinkText') }}</analytics-tracked-tag>.
          </p>
          <p>
            {{ $t('organDonation.withdrawReason.amendBeforeLink') }}
            <a id="update" href="#" class="inline" @click.stop.prevent="amendDecision">
              {{ $t('organDonation.withdrawReason.amendLink') }}</a>.
            {{ $t('organDonation.withdrawReason.amendAfterLink') }}
          </p>
          <p>{{ $t('organDonation.withdrawReason.familyText') }}</p>
          <error-group :show-error="showErrors">
            <label for="reason">
              {{ $t('organDonation.withdrawReason.reason.label') }}
            </label>
            <error-message v-if="showErrors">
              {{ $t('organDonation.withdrawReason.errorMessageText') }}
            </error-message>
            <select-dropdown v-model="reasonId"
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
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import ErrorGroup from '@/components/ErrorGroup';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import { INDEX, ORGAN_DONATION, ORGAN_DONATION_REVIEW_YOUR_DECISION, ORGAN_DONATION_AMEND } from '@/lib/routes';
import { isNativeApp } from '@/components/NativeOnlyMixin';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    AnalyticsTrackedTag,
    ErrorGroup,
    ErrorMessage,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    SelectDropdown,
  },
  data() {
    return {
      hasTriedToContinue: false,
      lawChangeUrl: this.$store.app.$env.ORGAN_DONATION_LAW_CHANGE_URL,
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
    amendDecision() {
      this.$store.dispatch('organDonation/amendStart');
      redirectTo(this, ORGAN_DONATION_AMEND.path);
    },
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
 @import "../../style/forms";
</style>
