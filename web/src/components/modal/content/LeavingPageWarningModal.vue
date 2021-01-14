<template>
  <div>
    <div :class="$style.modalBody">
      <h2 data-sid="pageLeaveWarningHeader">
        {{ $t('navigation.leavePage.leaveThisPageQuestion') }} </h2>
      <p data-sid="pageLeaveWarningText">
        {{ $t('navigation.leavePage.ifYouEnteredInformationItWillNotBeSaved') }} </p>
    </div>
    <generic-button id="modalStayOnPage"
                    :button-classes="['nhsuk-button', $style['nhsuk-button-full-width']]"
                    @click.prevent="stayOnPage">
      {{ $t('navigation.leavePage.stayOnThisPage') }}
    </generic-button>

    <div :class="$style.leavePanel">
      <a id="modalLeavePage"
         :class="[$style['nhsuk-action-link__link'], $style.leaveLink]"
         :href="redirectPath"
         data-purpose="leavePage"
         @click.prevent="leavePage">
        {{ $t('navigation.leavePage.leaveThisPage') }}
      </a>
    </div>

  </div>

</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';

export default {
  name: 'LeavingPageWarningModal',
  components: {
    GenericButton,
  },
  data() {
    return {
      redirectPath: this.$store.state.pageLeaveWarning.attemptedRedirectRoute,
    };
  },
  methods: {
    stayOnPage() {
      this.$store.dispatch('pageLeaveWarning/stayOnPage');
    },
    leavePage() {
      this.$store.dispatch('pageLeaveWarning/leavePage');
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/leaving-page-warning-modal";
</style>
