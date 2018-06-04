<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <ul :class="$style.allergyAndAdverseReactions">
      <li v-for="item in allergies" :key="item.name">
        <label>{{ item.date | longDate }}</label>
        <p>{{ item.symptom }}</p>
      </li>
    </ul>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    ...mapGetters({
      allergies: 'myRecord/allergies',
    }),
  },
};

</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/elements';

  .recordContent { @include record-content };

  .allergyAndAdverseReactions li{
    border-bottom: 1px solid #e8edee;
  }
</style>
