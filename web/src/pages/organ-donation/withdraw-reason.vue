<template>
  <div v-if="showTemplate" id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div>
        <message-dialog v-if="showErrors" id="errors"
                        message-type="error"
                        role="alert"
                        :focusable="true">
          <message-text data-purpose="error-heading">
            {{ $t('organDonation.thereIsAProblem') }}
          </message-text>
          <message-list data-purpose="reason-error">
            <li>{{ $t('organDonation.withdrawReason.giveAReasonForWithdrawing') }}</li>
          </message-list>
        </message-dialog>
        <div :class="[$style.form]">
          <h2>{{ $t('organDonation.withdrawReason.withdrawYourPreviousDecision') }}</h2>
          <p>{{ $t('organDonation.withdrawReason.differentToOptingOut') }}</p>
          <p>{{ $t('organDonation.withdrawReason.youAreConsideredToBeAnOrganDonorUnless') }}</p>
          <ul>
            <li>{{ $t('organDonation.withdrawReason.youHaveRecordedADscisionNotToDonate') }}</li>
            <li>{{ $t('organDonation.withdrawReason.youAreInAnExcludedGroup') }}</li>
          </ul>
          <p>
            {{ $t('organDonation.withdrawReason.findOutMoreAboutThe') }}
            <analytics-tracked-tag id="law-change"
                                   :href="lawChangeUrl"
                                   :text="$t('organDonation.withdrawReason.lawAndExcludedGroups')"
                                   class="inline"
                                   tag="a"
                                   target="_blank">
              {{ $t('organDonation.withdrawReason.lawAndExcludedGroups') }}</analytics-tracked-tag>.
          </p>
          <p>
            {{ $t('organDonation.withdrawReason.ifYouDoNotWantTheWayToTellUsIs') }}
            <a id="update" href="#" class="inline" @click.stop.prevent="amendDecision">
              {{ $t('organDonation.withdrawReason.updateYourDecision') }}</a>.
            {{ $t('organDonation.withdrawReason.youCanChangeYouDecisionAtAnyTime') }}
          </p>
          <p>{{ $t('organDonation.withdrawReason.makeSureYourFamilyKnow') }}</p>
          <error-group :show-error="showErrors">
            <label for="reason">
              {{ $t('organDonation.withdrawReason.reasonForWithdrawing') }}
            </label>
            <error-message v-if="showErrors">
              {{ $t('organDonation.withdrawReason.giveAReasonForWithdrawing') }}
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
            {{ $t('generic.continue') }}
          </generic-button>
          <generic-button v-if="!$store.state.device.isNativeApp"
                          id="back-button"
                          :class="['nhsuk-button', 'nhsuk-button--secondary']"
                          @click.stop.prevent="goBack" >
            {{ $t('generic.back') }}
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
import {
  INDEX_PATH,
  ORGAN_DONATION_PATH,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
  ORGAN_DONATION_AMEND_PATH,
} from '@/router/paths';
import { isNativeApp } from '@/components/NativeOnlyMixin';
import { redirectTo } from '@/lib/utils';
import {
  ORGAN_DONATION_LAW_CHANGE_URL,
} from '@/router/externalLinks';

export default {
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
      lawChangeUrl: ORGAN_DONATION_LAW_CHANGE_URL,
      reasonId: get('organDonation.withdrawReasonId')(this.$store.state),
    };
  },
  computed: {
    reasons() {
      return [
        { id: '', displayName: this.$t('organDonation.withdrawReason.selectReason') },
        ...get('organDonation.referenceData.withdrawReasons')(this.$store.state),
      ];
    },
    showErrors() {
      return (this.hasTriedToContinue && !this.reasonId);
    },
  },
  mounted() {
    if (!isNativeApp({ route: this.$route, store: this.$store })) {
      redirectTo(this, INDEX_PATH);
    } else if (!this.$store.state.organDonation.isWithdrawing) {
      redirectTo(this, ORGAN_DONATION_PATH);
    }
  },
  methods: {
    amendDecision() {
      this.$store.dispatch('organDonation/amendStart');
      redirectTo(this, ORGAN_DONATION_AMEND_PATH);
    },
    continueClicked() {
      this.hasTriedToContinue = true;

      if (this.showErrors) {
        window.scrollTo(0, 0);
        return;
      }

      this.$store.dispatch('organDonation/setWithdrawReasonId', this.reasonId);
      redirectTo(this, ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH);
    },
    goBack() {
      this.$store.dispatch('organDonation/withdrawCancel');
      redirectTo(this, ORGAN_DONATION_PATH);
    },
  },
};
</script>

<style module lang="scss" scoped>
 @import "../../style/forms";
</style>
