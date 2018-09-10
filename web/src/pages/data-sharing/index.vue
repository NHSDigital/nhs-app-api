<template>
  <div class="content">
    <div :class="$style['page']">
      <Contents :page-index="pageIndex" @change-page="changePage"/>
      <Overview v-if="pageId === 'p1'" @manage-choices="goToManageChoices"/>
      <ManageChoice v-if="pageId === 'p2'"/>
    </div>
    <button v-if="pageId === 'p2'" id="start-now-button" :class="[$style.button, $style.green]"
            @click="startNowClicked">
      {{ $t('ds01.startNowButton') }}
    </button>
    <BottomNav :class="$style['bottom-nav']" :current-page="pageId"
               @next-page="changePage(++pageIndex)" @previous-page="changePage(--pageIndex)"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import Contents from '@/components/data-sharing/Contents';
import BottomNav from '@/components/data-sharing/BottomNav';
import Overview from '@/components/data-sharing/Overview';
import ManageChoice from '@/components/data-sharing/ManageChoice';

import _ from 'lodash';

export default {
  components: {
    Contents,
    BottomNav,
    Overview,
    ManageChoice,
  },
  data() {
    return {
      pageIds: _.keys(this.$t('ds01.titles')),
      pageIndex: 0,
    };
  },
  computed: {
    pageId() {
      return this.pageIds[this.pageIndex];
    },
  },
  mounted() {
    if (!this.$store.state.navigation.menuItemStatusAt[4]) {
      this.$store.dispatch('navigation/setNewMenuItem', 4);
    }
  },
  methods: {
    changePage(index) {
      window.scrollTo(0, 0);
      this.pageIndex = index;
    },
    goToManageChoices() {
      this.changePage(_.indexOf(this.pageIds, 'p2'));
    },
    startNowClicked() {
      // this.$store.app.$http.getV1PatientNdop({}).then(p => {
      //   axios({
      //     url: process.env.DATA_PREFERENCES_URL,
      //     method: 'GET',
      //     headers: { Authorization: 'Bearer ' + p.response.token }
      //   })
      //   .then(response => {})
      //   .catch(error => {});
      // });
    },
  },
};
</script>

<style module scoped lang='scss'>
@import '../../style/buttons';
@import '../../style/datasharing';
</style>
