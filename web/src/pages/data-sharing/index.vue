<!-- eslint-disable vue/no-template-shadow -->
<template>
  <div v-if="showTemplate" class="content">
    <div :class="$style['page']" aria-live="polite">
      <h2 :class="$style['pageTitle']">
        {{ $t('ds01.mainHeader') }}
      </h2>
      <ul id="contents" :class="$style['list-menu']">
        <li v-for="pageId in pageIds" :key="pageId">
          <a :class="isLinkActive(pageId)" tabindex="0" role="link" @click="goToPage(pageId)"
             @keypress="contentsKeyPressed($event, pageId)">{{ $t('ds01.titles.' + pageId) }}</a>
        </li>
      </ul>
      <h2 :key="`${pageId}header`" :class="$style['title']">
        {{ $t('ds01.titles.' + pageId) }}
      </h2>
      <Overview v-if="pageId === 'p1'"/>
      <WhereConfidentialPatientInformationIsUsed v-if="pageId === 'p2'"/>
      <WhereYourChoiceDoesNotApply v-if="pageId === 'p3'"/>
      <MakeYourChoice v-if="pageId === 'p4'"/>
    </div>

    <analytics-tracked-tag v-if="pageId === 'p4' && device === 'ios'"
                           id="start-now-button"
                           :class="[$style.button, $style.green]"
                           :text="$t('ds01.startNowButton')"
                           :destination="dataPreferencesUrl"
                           data-purpose="button"
                           tag="button"
                           @click.native="startNowClickedIos">
      {{ $t('ds01.startNowButton') }}
    </analytics-tracked-tag>

    <form v-if="pageId === 'p4' && device !== 'ios'" id="ndop-token-form"
          :action="dataPreferencesUrl" :target="formTarget" method="POST"
          name="ndopTokenForm">
      <input v-model="ndopToken" type="hidden" name="token">
      <analytics-tracked-tag id="start-now-button"
                             :class="[$style.button, $style.green]"
                             :text="$t('ds01.startNowButton')"
                             :destination="dataPreferencesUrl"
                             data-purpose="button"
                             tag="button">
        {{ $t('ds01.startNowButton') }}
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
import WhereConfidentialPatientInformationIsUsed from '@/components/data-sharing/WhereConfidentialPatientInformationIsUsed';
import WhereYourChoiceDoesNotApply from '@/components/data-sharing/WhereYourChoiceDoesNotApply';
import MakeYourChoice from '@/components/data-sharing/MakeYourChoice';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

import _ from 'lodash';

export default {
  components: {
    BottomNav,
    Overview,
    WhereConfidentialPatientInformationIsUsed,
    WhereYourChoiceDoesNotApply,
    MakeYourChoice,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      pageIds: _.keys(this.$t('ds01.titles')),
      pageIndex: 0,
      dataPreferencesUrl: this.$store.app.$env.DATA_PREFERENCES_URL,
      ndopToken: undefined,
    };
  },
  computed: {
    pageId() {
      return this.pageIds[this.pageIndex];
    },
    device() {
      return this.$store.state.device.source;
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
    async changePage(index) {
      window.scrollTo(0, 0);
      this.pageIndex = index;
      const scope = this;
      if (!scope.ndopToken && scope.pageId === 'p4') {
        await this.getNdopToken();
      }
    },
    goToPage(pageId) {
      this.changePage(_.indexOf(this.pageIds, pageId));
    },
    startNowClickedIos() {
      NativeCallbacks.postNdopToken(this.ndopToken);
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
    async getNdopToken() {
      const scope = this;
      await scope.$store.app.$http
        .getV1PatientNdop({})
        .then((p) => {
          scope.ndopToken = p.response.token;
        });
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
