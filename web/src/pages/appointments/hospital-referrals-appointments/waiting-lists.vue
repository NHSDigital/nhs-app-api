<template v-if="hasLoaded">
  <div id="wait-Times-Title">
    <p id="wait-Times-Content">
      {{ waitingListCount ===1 ? $t('wayfinder.waitTimes.youAreOnOneWaitingList') : $t('wayfinder.waitTimes.youAreOnMultipleWaitingLists',null,{waitingListCount}) }}
    </p>
    <help-link
      id="wayfinder-help-jump-off-link-wait-times"
      route-crumb="waitTimes"
      :path="wayfinderHelpPath"
      :back-link-override="wayfinderWaitingListsPath"
      :text="$t('wayfinder.waitTimes.missingIncorrectOrNotChangedOrCancelled')"/>
    <template v-if="hasErrored">
      <p id="technicalProblem">{{ $t('wayfinder.waitTimes.errors.technicalProblem') }}</p>
      <p id="informationCurrentlyUnavailable">{{ $t('wayfinder.waitTimes.errors.informationCurrentlyUnavailable') }}</p>
      <p id="cannotViewTryAgain">{{ $t('wayfinder.waitTimes.errors.cannotViewTryAgain') }}</p>
    </template>
    <template v-else>
      <p id="wait-Times-Content" class="nhsuk-u-padding-top-3">
        {{ waitingListCount === 1 ? $t('wayfinder.waitTimes.youAreOnOneWaitingList') : $t('wayfinder.waitTimes.youAreOnMultipleWaitingLists',null,{waitingListCount}) }}
      </p>
    </template>
    <desktop-generic-back-link
      v-if="!isNativeApp"
      id="desktopBackLink"
      data-purpose="back-to-hospital-referrals-appointments-button"
      :path="wayfinderPath"
      :button-text="'generic.back'"/>
  </div>
</template>

<script>
import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import { WAYFINDER_HELP_PATH, WAYFINDER_WAITING_LISTS_PATH, WAYFINDER_PATH } from '@/router/paths';
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  name: 'WayfinderWaitingLists',
  components: {
    HelpLink,
    DesktopGenericBackLink,
  },
  data() {
    return {
      wayfinderPath: WAYFINDER_PATH,
      wayfinderHelpPath: WAYFINDER_HELP_PATH,
      wayfinderWaitingListsPath: WAYFINDER_WAITING_LISTS_PATH,
      isNativeApp: this.$store.state.device.isNativeApp,
      waitTimesResults: null,
    };
  },
  computed: {
    waitingListCount() {
      if (this.waitTimesResults) {
        return this.waitTimesResults.length;
      }
      return 0;
    },
    apiError() {
      return this.$store.state.wayfinder.apiError;
    },
    hasErrored() {
      return this.apiError !== null;
    },
    hasLoaded() {
      return this.$store.state.wayfinder.hasWaitTimesLoaded;
    },
  },
  async mounted() {
    await this.$store.dispatch('wayfinder/loadWaitTimes');
    this.waitTimesResults = this.$store.state.wayfinder.waitTimes;

    if (this.hasErrored) {
      const header = 'wayfinder.waitTimes.errors.cannotView';
      EventBus.$emit(UPDATE_HEADER, header);
      EventBus.$emit(UPDATE_TITLE, header);
    }
  },
};
</script>
