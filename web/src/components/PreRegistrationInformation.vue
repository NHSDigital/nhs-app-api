<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-two-thirds">
      <div id="before-you-start" class="nhsuk-u-margin-top-1">
        <div v-if="shouldShowFullContent">
          <h2 class="nhsuk-u-margin-bottom-3">
            {{ $t('login.beforeYouStart') }}
          </h2>
          <p>{{ $t('login.toUseThisServiceYouMustBe') }}</p>
          <ul>
            <li class="nhsuk-u-margin-bottom-3">
              {{ $t('login.registeredWithSurgeryInEngland') }}
            </li>
            <li class="nhsuk-u-margin-bottom-3">
              {{ $t('login.aged13OrOver') }}
            </li>
          </ul>
          <p>
            <a :href="whoCanUseAppUrl" target="_blank" rel="noopener noreferrer">
              {{ $t('login.findOutMoreAboutWhoCanUseTheNHSApp') }}
            </a>
          </p>
        </div>
        <div v-else>
          <p>{{ $t('login.youMustBe13OrOver') }}</p>
        </div>
      </div>
    </div>
  </div>
</template>
<script>

export default {
  name: 'PreRegistrationInformation',
  components: {
  },
  props: {
    shouldShowFullContent: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      whoCanUseAppUrl: this.$store.$env.WHO_CAN_USE_APP_URL,
    };
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/pre-registration-information";
</style>
