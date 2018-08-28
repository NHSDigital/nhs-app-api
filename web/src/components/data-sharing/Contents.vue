<template>
  <div :class="$style['content']">
    <h1 :class="$style['pageTitle']">{{ $t('ds01.titles.' + pageId) }}</h1>
    <p :class="$style['pageSubtitle']">{{ $t('ds01.subtitle') }}</p>
    <ul :class="$style['contents']">
      <li v-for="(title, key) in $t('ds01.titles')"
          :key="(title, key)" @click="changePage(key)">
        <span :class="{[$style['active']]: key === pageId}">{{ title }}</span>
      </li>
    </ul>
  </div>
</template>

<script>
import _ from 'lodash';

export default {
  props: {
    pageIndex: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      pageIds: _.keys(this.$t('ds01.titles')),
    };
  },
  computed: {
    pageId() {
      return this.pageIds[this.pageIndex];
    },
  },
  methods: {
    changePage(key) {
      this.$emit('change-page', _.indexOf(this.pageIds, key));
    },
  },
};
</script>

<style module lang='scss' scoped>
@import '../../style/datasharingcontents';
</style>
