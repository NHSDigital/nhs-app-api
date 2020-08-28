<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <message-dialog :icon-text="$t('organDonation.withdrawn.decisionWithdrawn')"
                      message-id="success-dialog" message-type="success">
        <message-text
          v-for="(item, index) in $t('organDonation.withdrawn.youNoLongerHaveADecisionRecorded')"
          :key="index">
          {{ item }}
        </message-text>
        <message-text>
          <analytics-tracked-tag
            :href="lawChangeUrl"
            :text="$t('organDonation.withdrawn.moreInformationAboutLawChanges')"
            class="inline"
            tag="a"
            target="_blank">
            {{ $t('organDonation.withdrawn.moreInformationAboutLawChanges') }}
          </analytics-tracked-tag>
        </message-text>
        <message-text>
          {{ $t('organDonation.withdrawn.youCanRecordADecisionAtAnyTime') }}
        </message-text>
      </message-dialog>
      <div>
        <h2>{{ $t('organDonation.withdrawn.whatToDoNext') }}</h2>
        <p>{{ $t('organDonation.withdrawn.letYourFamilyKnow') }}</p>
      </div>
      <other-things-to-do :can-withdraw="false"/>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NativeOnlyMixin from '@/components/NativeOnlyMixin';
import OtherThingsToDo from '@/components/organ-donation/OtherThingsToDo';
import {
  ORGAN_DONATION_LAW_CHANGE_URL,
} from '@/router/externalLinks';

export default {
  components: {
    AnalyticsTrackedTag,
    MessageDialog,
    MessageText,
    OtherThingsToDo,
  },
  mixins: [NativeOnlyMixin],
  data() {
    return {
      lawChangeUrl: ORGAN_DONATION_LAW_CHANGE_URL,
    };
  },
  created() {
    this.$store.dispatch('organDonation/init');
  },
};
</script>
