<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <ul :class="$style['list-menu']">
      <li>
        <organ-donation-link id="btn_organ_donation" :class-name="[$style['no-decoration']]">
          <h2>{{ $t('sc04.organDonation.subheader') }}</h2>
          <p>{{ $t('sc04.organDonation.body') }}</p>
        </organ-donation-link>
      </li>
      <li>
        <analytics-tracked-tag :text="$t('sc04.dataSharing.subheader')"
                               :href="dataSharingPath"
                               :tabindex="-1"
                               data-purpose="text_link">
          <a id="btn_data_sharing"
             :class="$style['no-decoration']"
             :href="dataSharingPath"
             @click="navigate($event)">
            <h2>{{ $t('sc04.dataSharing.subheader') }}</h2>
            <p>{{ $t('sc04.dataSharing.body') }}</p>
          </a>
        </analytics-tracked-tag>
      </li>
      <li v-if="isOnlineConsultationsEnabled">
        <analytics-tracked-tag :text="$t('sc04.requestGpHelp.subheader')"
                               data-purpose="text_link">
          <a id="btn_gp_help"
             :href="requestAdminHelpPath"
             :class="$style['no-decoration']"
             @click="navigate($event)">
            <h2>{{ $t('sc04.requestGpHelp.subheader') }}</h2>
            <p>{{ $t('sc04.requestGpHelp.body') }}</p>
          </a>
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import get from 'lodash/fp/get';
import flow from 'lodash/fp/flow';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { DATA_SHARING_PREFERENCES, INDEX, APPOINTMENT_ADMIN_HELP, MORE } from '@/lib/routes';
import { createUri } from '@/lib/noJs';
import { isTruthy, redirectTo } from '@/lib/utils';

const getOnlineConsultationEnabled = flow(
  get('$store.app.$env.ONLINE_CONSULTATIONS_ENABLED'),
  isTruthy,
);

export default {
  components: {
    AnalyticsTrackedTag,
    OrganDonationLink,
  },
  computed: {
    dataSharingPath() {
      return DATA_SHARING_PREFERENCES.path;
    },
    isOnlineConsultationsEnabled() {
      return getOnlineConsultationEnabled(this);
    },
    requestAdminHelpPath() {
      const noJsData = {
        onlineConsultations: {
          previousRoute: MORE.path,
        },
      };
      return createUri({ path: APPOINTMENT_ADMIN_HELP.path, noJs: noJsData });
    },
  },
  created() {
    this.redirectIfDesktop();
  },
  methods: {
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname, null);
      event.preventDefault();

      if (event.currentTarget.pathname === this.requestAdminHelpPath) {
        this.$store.dispatch('navigation/setNewMenuItem', 4);
        this.$store.dispatch('onlineConsultations/setPreviousRoute', MORE.path);
      }
    },
    redirectIfDesktop() {
      if (!this.$store.state.device.isNativeApp) {
        redirectTo(this, INDEX.path, null);
      }
    },
  },
};
</script>

<style module lang="scss">
@import '../../style/listmenu';

.no-decoration {
  text-decoration: none;
}

.no-padding {
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
}

</style>
