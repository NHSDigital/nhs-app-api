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

    <form v-if="pageId === 'p4'" id="ndop-token-form"
          ref="ndopTokenForm" :action="dataPreferencesUrl" target="_self"
          method="POST" name="ndopTokenForm">
      <input v-model="ndopToken" type="hidden" name="token">
      <analytics-tracked-tag id="startNowButton"
                             :class="[$style.button, $style.green]"
                             :text="$t('ds01.startNowButton')"
                             data-purpose="startNowButton"
                             tag="button"
                             :click-func="startNow">
        {{ $t('ds01.startNowButton') }}
      </analytics-tracked-tag>
    </form>

    <BottomNav :class="$style['bottom-nav']" :current-page="pageId"
               @next-page="changePage(++pageIndex)" @previous-page="changePage(--pageIndex)"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import BottomNav from '@/components/data-sharing/BottomNav';
import Overview from '@/components/data-sharing/Overview';
import WhereConfidentialPatientInformationIsUsed from '@/components/data-sharing/WhereConfidentialPatientInformationIsUsed';
import WhereYourChoiceDoesNotApply from '@/components/data-sharing/WhereYourChoiceDoesNotApply';
import MakeYourChoice from '@/components/data-sharing/MakeYourChoice';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

import keys from 'lodash/fp/keys';

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
      pageIds: keys(this.$t('ds01.titles')),
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
      this.changePage(this.pageIds.indexOf(pageId));
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
    async startNow() {
      const scope = this;
      await this.$store.app.$http
        .getV1PatientNdop({})
        .then((p) => {
          scope.ndopToken = p.response.token;
        });

      this.$refs.ndopTokenForm.submit();
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
