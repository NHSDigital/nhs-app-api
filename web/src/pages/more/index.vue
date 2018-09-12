<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">

    <ul :class="$style['list-menu']">
      <li>
        <analytics-tracked-tag id="btn_organ_donation" :href="organDonationUrl"
                               :class="$style['no-decoration']"
                               :text="$t('sc04.organDonation.subheader')"
                               tag="a" target="_blank">
          <h2>{{ $t('sc04.organDonation.subheader') }}</h2>
          <p>{{ $t('sc04.organDonation.body') }}</p>
        </analytics-tracked-tag>
      </li>
      <li>
        <analytics-tracked-tag :text="$t('sc04.dataSharing.subheader')" data-purpose="text_link">
          <a id="btn_data_sharing"
             :class="$style['no-decoration']"
             href="/data-sharing"
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

export default {
  components: {
    AnalyticsTrackedTag,
  },
  data(app) {
    return {
      organDonationUrl: app.$env.ORGAN_DONATION_URL,
    };
  },
  methods: {
    navigate(event) {
      this.$router.push(event.currentTarget.pathname);
      event.preventDefault();
    },
  },
};
</script>

<style module lang="scss" scoped>
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
