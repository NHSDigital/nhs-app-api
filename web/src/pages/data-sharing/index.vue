<template>
  <div class="content">
    <div :class="$style['page']">
      <h1 :class="$style['pageTitle']" :key="`${pageId}header`">
        {{ $t('ds01.titles.' + pageId) }}
      </h1>
      <ul id="contents" :class="$style['list-menu']">
        <li v-for="pageId in pageIds" :key="pageId">
          <a :class="isLinkActive(pageId)" tabindex="0" role="link" @click="goToPage(pageId)"
             @keypress="contentsKeyPressed($event, pageId)">{{ $t('ds01.titles.' + pageId) }}</a>
        </li>
      </ul>
      <Overview v-if="pageId === 'p1'" @manage-choices="goToPage('p5')"/>
      <Benefits v-if="pageId === 'p2'"/>
      <DataUse v-if="pageId === 'p3'"/>
      <OptOutNotApply v-if="pageId === 'p4'"/>
      <ManageChoice v-if="pageId === 'p5'"/>
    </div>
    <form id="ndop-token-form" :action="dataPreferencesUrl" :target="formTarget" method="POST"
          name="ndopTokenForm">
      <generic-button v-if="pageId === 'p5'" id="start-now-button"
                      :class="[$style.button, $style.green]"
                      @click="startNowClicked($event)">
        {{ $t('ds01.startNowButton') }}
      </generic-button>
    </form>
    <BottomNav :class="$style['bottom-nav']" :current-page="pageId"
               @next-page="changePage(++pageIndex)" @previous-page="changePage(--pageIndex)"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import BottomNav from '@/components/data-sharing/BottomNav';
import Overview from '@/components/data-sharing/Overview';
import Benefits from '@/components/data-sharing/Benefits';
import DataUse from '@/components/data-sharing/DataUse';
import OptOutNotApply from '@/components/data-sharing/OptOutNotApply';
import ManageChoice from '@/components/data-sharing/ManageChoice';
import GenericButton from '@/components/widgets/GenericButton';

import _ from 'lodash';

export default {
  components: {
    GenericButton,
    BottomNav,
    Overview,
    Benefits,
    DataUse,
    OptOutNotApply,
    ManageChoice,
  },
  data(app) {
    return {
      pageIds: _.keys(this.$t('ds01.titles')),
      pageIndex: 0,
      dataPreferencesUrl: app.$env.DATA_PREFERENCES_URL,
    };
  },
  computed: {
    pageId() {
      return this.pageIds[this.pageIndex];
    },
    formTarget() {
      return !this.$store.state.device.isNativeApp ? '_self' : '_blank';
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
    goToPage(pageId) {
      this.changePage(_.indexOf(this.pageIds, pageId));
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
    isLinkActive(pageId) {
      return pageId === this.pageIds[this.pageIndex] ? this.$style.active : undefined;
    },
    contentsKeyPressed(event, pageId) {
      if (event.key === 'Enter') {
        event.preventDefault();
        this.goToPage(pageId);
      }
    },
  },
};
</script>

<style module lang='scss'>
@import '../../style/listmenu';
@import '../../style/buttons';
@import '../../style/datasharing';
</style>
