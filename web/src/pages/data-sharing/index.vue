<template>
  <div v-if="showTemplate" class="content">
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
      <Overview v-if="pageId === 'p1'"/>
      <WhereConfidentialPatientInformationIsUsed v-if="pageId === 'p2'"/>
      <WhereYourChoiceDoesNotApply v-if="pageId === 'p3'"/>
    </div>
    <form id="ndop-token-form" :action="dataPreferencesUrl" :target="formTarget" method="POST"
          name="ndopTokenForm">
      <analytics-tracked-tag v-if="pageId === 'p3'"
                             id="start-now-button"
                             :text="$t('ds01.startNowButton')"
                             :destination="dataPreferencesUrl"
                             data-purpose="generic-button"
                             tag="generic-button"
                             @click="startNowClicked($event)">
        <button :class="[$style.button, $style.green]">
          {{ $t('ds01.startNowButton') }}
        </button>
      </analytics-tracked-tag>
    </form>
    <BottomNav :class="$style['bottom-nav']" :current-page="pageId"
               @next-page="changePage(++pageIndex)" @previous-page="changePage(--pageIndex)"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import NativeCallbacks from '@/services/native-app';
import BottomNav from '@/components/data-sharing/BottomNav';
import Overview from '@/components/data-sharing/Overview';
import Benefits from '@/components/data-sharing/Benefits';
import WhereConfidentialPatientInformationIsUsed from '@/components/data-sharing/WhereConfidentialPatientInformationIsUsed';
import WhereYourChoiceDoesNotApply from '@/components/data-sharing/WhereYourChoiceDoesNotApply';
import GenericButton from '@/components/widgets/GenericButton';

import _ from 'lodash';

export default {
  components: {
    GenericButton,
    BottomNav,
    Overview,
    Benefits,
    WhereConfidentialPatientInformationIsUsed,
    WhereYourChoiceDoesNotApply,
  },
  data() {
    return {
      pageIds: _.keys(this.$t('ds01.titles')),
      pageIndex: 0,
      dataPreferencesUrl: this.$store.app.$env.DATA_PREFERENCES_URL,
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
          NativeCallbacks.postNdopToken(p.response.token);
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
@import '../../style/desktopcomponentsizes';
</style>
