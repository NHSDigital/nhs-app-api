<template>
  <div v-if="showTemplate" id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div>
        <div role="alert" aria-atomic="true">
          <message-dialog v-if="showErrors" id="errors"
                          message-type="error"
                          :focusable="true">
            <message-text data-purpose="error-heading">
              {{ $t('organDonation.thereIsAProblem') }}
            </message-text>
            <message-list data-purpose="reason-error">
              <li>{{ $t('organDonation.withdrawReason.giveAReasonForWithdrawing') }}</li>
            </message-list>
          </message-dialog>
        </div>
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
            <label class="nhsuk-label" for="reason">
              {{ $t('organDonation.withdrawReason.reasonForWithdrawing') }}
            </label>
            <error-message v-if="showErrors" id="reason-dropdown-error">
              {{ $t('organDonation.withdrawReason.giveAReasonForWithdrawing') }}
            </error-message>
            <select-dropdown v-model="reasonId"
                             :required="true"
                             :a-described-by="showErrors ? 'reason-dropdown-error' : undefined"
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
          <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                     :path="organDonationPath"
                                     :button-text="'generic.back'"/>
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
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
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
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';

export default {
  components: {
    AnalyticsTrackedTag,
    ErrorGroup,
    ErrorMessage,
    GenericButton,
    DesktopGenericBackLink,
    MessageDialog,
    MessageList,
    MessageText,
    SelectDropdown,
  },
  data() {
    return {
      hasTriedToContinue: false,
      lawChangeUrl: this.$store.$env.ORGAN_DONATION_LAW_CHANGE_URL,
      reasonId: get('organDonation.withdrawReasonId')(this.$store.state),
      organDonationPath: ORGAN_DONATION_PATH,
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
    if (!isNativeApp({ store: this.$store })
    && !this.$store.$env.ORGAN_DONATION_DESKTOP_ENABLED) {
      redirectTo(this, INDEX_PATH);
    } else if (!this.$store.getters['organDonation/canWithdraw']) {
      redirectTo(this, ORGAN_DONATION_PATH);
    }

    this.$store.dispatch('organDonation/amendCancel');
    this.$store.dispatch('organDonation/withdrawCancel');
  },
  methods: {
    amendDecision() {
      this.$store.dispatch('organDonation/amendStart');
      redirectTo(this, ORGAN_DONATION_AMEND_PATH);
    },
    continueClicked() {
      this.hasTriedToContinue = false;

      this.$nextTick(() => {
        this.hasTriedToContinue = true;

        if (this.showErrors) {
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
          return;
        }
        this.$store.dispatch('organDonation/withdrawStart');
        this.$store.dispatch('organDonation/setWithdrawReasonId', this.reasonId);
        redirectTo(this, ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH);
      });
    },
  },
};
</script>

<style module lang="scss" scoped>
 @import "../../style/forms";
</style>
