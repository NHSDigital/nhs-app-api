<template v-if="hasLoaded">
  <div>
    <template v-if="hasErrored">
      <p id="technicalProblem">{{ $t('wayfinder.waitTimes.errors.technicalProblem') }}</p>
      <p id="informationCurrentlyUnavailable">{{ $t('wayfinder.waitTimes.errors.informationCurrentlyUnavailable') }}</p>
      <p id="cannotViewTryAgain">{{ $t('wayfinder.waitTimes.errors.cannotViewTryAgain') }}</p>
    </template>
    <template v-else>
      <div id="wait-times-title">
        <p id="wait-times-content" class="nhsuk-u-padding-top-3">
          {{ waitingListCount === 1 ? $t('wayfinder.waitTimes.youAreOnOneWaitingList') : $t('wayfinder.waitTimes.youAreOnMultipleWaitingLists',null,{waitingListCount}) }}
        </p>
      </div>
      <card-group v-if="waitingListCount > 0" class="nhsuk-grid-row nhsuk-u-margin-bottom-3">
        <card-group-item
          v-for="(item, index) in waitingLists"
          :key="`actionable-item-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
          <wait-times-card :item="item" />
        </card-group-item>
      </card-group>
      <h2 id="something-wrong" class="nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
        {{ $t('wayfinder.waitTimes.isSomethingWrong') }}
      </h2>
      <help-link
        id="wayfinder-help-jump-off-link-wait-times"
        route-crumb="waitTimes"
        :path="wayfinderHelpPath"
        :back-link-override="wayfinderWaitingListsPath"
        :text="$t('wayfinder.waitTimes.missingIncorrectOrNotChangedOrCancelled')"/>
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

import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import WaitTimesCard from '@/components/appointments/hospital-referrals-appointments/appointments/WaitTimesCard';
import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import { WAYFINDER_HELP_PATH, WAYFINDER_WAITING_LISTS_PATH, WAYFINDER_PATH } from '@/router/paths';
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

const loadData = async (store) => {
  store.dispatch('wayfinder/clearApiError');
  await store.dispatch('wayfinder/loadWaitTimes');
};

export default {
  name: 'WayfinderWaitingLists',
  components: {
    CardGroup,
    CardGroupItem,
    WaitTimesCard,
    HelpLink,
    DesktopGenericBackLink,
  },
  data() {
    return {
      wayfinderPath: WAYFINDER_PATH,
      wayfinderHelpPath: WAYFINDER_HELP_PATH,
      wayfinderWaitingListsPath: WAYFINDER_WAITING_LISTS_PATH,
      isNativeApp: this.$store.state.device.isNativeApp,
      waitTimesResults: [],
    };
  },
  computed: {
    waitingLists() {
      if (this.waitTimesResults && this.waitTimesResults.waitTimes) {
        return this.waitTimesResults.waitTimes;
      }
      return [];
    },
    waitingListCount() {
      return this.waitingLists.length;
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
    await loadData(this.$store);
    this.waitTimesResults = this.$store.state.wayfinder;

    if (this.hasErrored) {
      const header = 'wayfinder.waitTimes.errors.cannotView';
      EventBus.$emit(UPDATE_HEADER, header);
      EventBus.$emit(UPDATE_TITLE, header);
    }
  },
};
</script>
