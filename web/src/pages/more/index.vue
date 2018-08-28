<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">

    <ul :class="$style['list-menu']">
      <li>
        <a id="btn_choices" :href="organDonationUrl"
           style="text-decoration: none" target="_blank">
          <h2>{{ $t('sc04.organDonation.subheader') }}</h2>
          <p>{{ $t('sc04.organDonation.body') }}</p>
        </a>
      </li>
      <li>
        <a id="btn_111" href="/data-sharing"
           style="text-decoration: none"
           @click="navigate($event)">
          <h2>{{ $t('sc04.dataSharing.subheader') }}</h2>
          <p>{{ $t('sc04.dataSharing.body') }}</p>
        </a>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  data() {
    return {
      organDonationUrl: process.env.ORGAN_DONATION_URL,
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
.no-padding {
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
}

</style>
