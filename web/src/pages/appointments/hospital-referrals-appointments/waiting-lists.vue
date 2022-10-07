<template>
  <div id="wait-Times-Title">
    <p id="wait-Times-Content">
      {{ waitingListCount ===1 ? $t('wayfinder.waitTimes.youAreOnOneWaitingList') : $t('wayfinder.waitTimes.youAreOnMultipleWaitingLists',null,{waitingListCount}) }}
    </p>
    <h2 id="something-Wrong">
      {{ $t('wayfinder.waitTimes.isSomethingWrong') }}
    </h2>
  </div>
</template>

<script>

export default {
  name: 'WayfinderWaitingLists',
  data() {
    return {
      result: null,
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
