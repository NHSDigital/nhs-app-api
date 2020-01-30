<template>
  <div v-if="showTemplate" class="content">
    <div :class="$style['page']" aria-live="polite">
      <h2 :class="$style['pageTitle']">
        {{ $t('ds01.mainHeader') }}
      </h2>
      <ul id="contents" :class="$style['list-menu']">
        <li v-for="_pageId in pageIds" :key="_pageId">
          <a :class="isLinkActive(_pageId)" tabindex="0" role="link" @click="goToPage(_pageId)"
             @keypress.enter.prevent="goToPage(_pageId)">{{ $t('ds01.titles.' + _pageId) }}</a>
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
                             data-purpose="startNowButton"
                             :text="this.$t('ds01.startNowButton')"
                             :tabindex="-1"
                             class="nhsuk-u-margin-padding-0">
        <generic-button :class="['nhsuk-button']" @click.prevent="startNow">
          {{ this.$t('ds01.startNowButton') }}
        </generic-button>
      </analytics-tracked-tag>
    </form>

    <BottomNav :class="$style['bottom-nav']" :current-page="pageId"
               @next-page="changePage(++pageIndex)" @previous-page="changePage(--pageIndex)"/>
  </div>
</template>

<script>
import keys from 'lodash/fp/keys';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import BottomNav from '@/components/data-sharing/BottomNav';
import GenericButton from '@/components/widgets/GenericButton';
import MakeYourChoice from '@/components/data-sharing/MakeYourChoice';
import Overview from '@/components/data-sharing/Overview';
import WhereConfidentialPatientInformationIsUsed from '@/components/data-sharing/WhereConfidentialPatientInformationIsUsed';
import WhereYourChoiceDoesNotApply from '@/components/data-sharing/WhereYourChoiceDoesNotApply';

export default {
  components: {
    AnalyticsTrackedTag,
    BottomNav,
    GenericButton,
    MakeYourChoice,
    Overview,
    WhereConfidentialPatientInformationIsUsed,
    WhereYourChoiceDoesNotApply,
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
@import '../../style/accessibility';
  li a:hover, li a:focus {
    background-color: transparent;
  }
  a:hover, a:focus {
    border: none;
  }

</style>

