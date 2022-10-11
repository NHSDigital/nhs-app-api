<template>
  <div id="wait-Times-Title">
    <p id="wait-Times-Content">
      {{ waitingListCount ===1 ? $t('wayfinder.waitTimes.youAreOnOneWaitingList') : $t('wayfinder.waitTimes.youAreOnMultipleWaitingLists',null,{waitingListCount}) }}
    </p>
    <h2 id="something-Wrong">
      {{ $t('wayfinder.waitTimes.isSomethingWrong') }}
    </h2>
    <help-link
      id="wayfinder-help-jump-off-link-wait-times"
      route-crumb="waitTimes"
      :path="wayfinderHelpPath"
      :back-link-override="wayfinderWaitingListsPath"
      :text="$t('wayfinder.waitTimes.missingIncorrectOrNotChangedOrCancelled')"/>
  </div>
</template>

<script>
import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import { WAYFINDER_HELP_PATH, WAYFINDER_WAITING_LISTS_PATH } from '@/router/paths';

export default {
  name: 'WayfinderWaitingLists',
  components: {
    HelpLink,
  },
  data() {
    return {
      result: null,
      wayfinderHelpPath: WAYFINDER_HELP_PATH,
      wayfinderWaitingListsPath: WAYFINDER_WAITING_LISTS_PATH,
    };
  },
  computed: {
    waitingListCount() {
      return this.result ? this.result.waitTimes.length : 0;
    },
  },
  async mounted() {
    await this.$store.dispatch('wayfinder/loadWaitTimes');
    this.result = this.$store.state.wayfinder.waitTimes;
  },
};
</script>
