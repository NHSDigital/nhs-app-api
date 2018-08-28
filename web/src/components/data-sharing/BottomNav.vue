<template>
  <div>
    <button v-if="showNext" id="next-button" :class="[$style.button]" @click="nextClicked">
      {{ $t('ds01.nextButton') }}
    </button>
    <button v-if="showPrevious" id="previous-button" :class="[$style.button, $style.grey]"
            @click="previousClicked">
      {{ $t('ds01.previousButton') }}
    </button>
  </div>
</template>

<script>
import _ from 'lodash';

export default {
  props: {
    currentPage: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      pageIds: _.keys(this.$t('ds01.titles')),
    };
  },
  computed: {
    showNext() {
      return (
        _.indexOf(this.pageIds, this.currentPage) < this.pageIds.length - 1
      );
    },
    showPrevious() {
      return _.indexOf(this.pageIds, this.currentPage) > 0;
    },
  },
  methods: {
    previousClicked() {
      this.$emit('previous-page');
    },
    nextClicked() {
      this.$emit('next-page');
    },
  },
};
</script>

<style module scoped lang='scss'>
@import '../../style/_buttons';
</style>
