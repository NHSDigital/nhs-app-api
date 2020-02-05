<template>
  <div>
    <div>
      <generic-button v-if="showNext" id="next-button" :class="['nhsuk-button']" click-delay="none"
                      @click="nextClicked">
        {{ $t('ds01.nextButton') }}
      </generic-button>
    </div>
    <generic-button v-if="showPrevious" id="previous-button"
                    :class="['nhsuk-button', 'nhsuk-button--secondary']"
                    click-delay="none" @click="previousClicked">
      {{ $t('ds01.previousButton') }}
    </generic-button>
  </div>
</template>

<script>
import keys from 'lodash/fp/keys';
import indexOf from 'lodash/indexOf';

import GenericButton from '@/components/widgets/GenericButton';

export default {
  name: 'BottomNav',
  components: {
    GenericButton,
  },
  props: {
    currentPage: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      pageIds: keys(this.$t('ds01.titles')),
    };
  },
  computed: {
    showNext() {
      return (
        indexOf(this.pageIds, this.currentPage) < this.pageIds.length - 1
      );
    },
    showPrevious() {
      return indexOf(this.pageIds, this.currentPage) > 0;
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
@import '../../style/buttons';
@import '../../style/desktopcomponentsizes';
</style>
