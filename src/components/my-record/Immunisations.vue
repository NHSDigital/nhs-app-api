<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <div v-if="data.hasErrored">
      <p>  {{ $t('myRecord.genericErrorMessage') }} </p>
    </div>
    <div v-else>
      <div v-if="data.hasAccess">
        <div v-if="data.data.length > 0">
          <ul :class="$style.immunisations">
            <li v-for="item in data.data" :key="item.name">
              <label>{{ item.date | longDate }}</label>
              <p class="immunisationTerm">{{ item.term }}</p>
            </li>
          </ul>
        </div>
        <div v-else>
          <p> {{ $t('myRecord.genericNoDataMessage') }} </p>
        </div>
      </div>
      <div v-else>
        <p> {{ $t('myRecord.genericNoAccessMessage') }} </p>
      </div>
    </div>
    <hr>
  </div>
</template>

<script>
export default {
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    data: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
  },
};

</script>

<style lang="scss" module>
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/elements';

  .recordContent { @include record-content };

  .immunisations li{
    border-bottom: 1px solid #e8edee;
  }
</style>
