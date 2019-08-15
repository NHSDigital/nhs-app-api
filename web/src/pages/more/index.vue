<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <ul :class="$style['list-menu']">
      <sjr-if journey="cdssAdmin" tag="li">
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
      </sjr-if>
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
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import SjrIf from '@/components/SjrIf';
import { DATA_SHARING_PREFERENCES, APPOINTMENT_ADMIN_HELP, MORE } from '@/lib/routes';
import { createUri } from '@/lib/noJs';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    OrganDonationLink,
    SjrIf,
  },
  computed: {
    dataSharingPath() {
      return DATA_SHARING_PREFERENCES.path;
    },
    requestAdminHelpPath() {
      return createUri({
        path: APPOINTMENT_ADMIN_HELP.path,
        noJs: { onlineConsultations: { previousRoute: MORE.path } },
      });
    },
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
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
