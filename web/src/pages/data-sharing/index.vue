<template>
  <div class="content">
    <div :class="$style['page']">
      <Contents :page-index="pageIndex" @change-page="changePage"/>
      <Overview v-if="pageId === 'p1'" @manage-choices="goToManageChoices"/>
      <ManageChoice v-if="pageId === 'p2'"/>
    </div>
    <form id="ndop-token-form" :action="dataPreferencesUrl" method="POST" name="ndopTokenForm"
          target="_blank">
      <button v-if="pageId === 'p2'" id="start-now-button" :class="[$style.button, $style.green]"
              @click="startNowClicked($event)">
        {{ $t('ds01.startNowButton') }}
      </button>
    </form>
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
      dataPreferencesUrl: process.env.DATA_PREFERENCES_URL,
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
    startNowClicked(event) {
      event.preventDefault();
      this.$store.app.$http.getV1PatientNdop({}).then((p) => {
        if (this.$store.state.device.source === 'ios') {
          window.nativeApp.postNdopToken(p.response.token);
        } else {
          const ndopTokenForm = document.getElementById('ndop-token-form');
          const startNowButton = document.getElementById('start-now-button');

          const tokenInput = document.createElement('input');
          tokenInput.setAttribute('type', 'hidden');
          tokenInput.setAttribute('name', 'token');
          tokenInput.setAttribute('value', p.response.token);

          ndopTokenForm.insertBefore(tokenInput, startNowButton);
          ndopTokenForm.submit();
          ndopTokenForm.removeChild(tokenInput);
        }
      });
    },
  },
};
</script>

<style module scoped lang='scss'>
@import '../../style/buttons';
@import '../../style/datasharing';
</style>
