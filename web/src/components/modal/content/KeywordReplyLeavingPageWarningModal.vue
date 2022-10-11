<template>
  <div>
    <div :class="$style.modalBody">
      <h2 data-sid="pageLeaveWarningHeader">
        {{ $t('navigation.keywordReplyLeavePage.title') }} </h2>
      <p data-sid="pageLeaveWarningText">
        {{ $t('navigation.keywordReplyLeavePage.message') }} </p>
    </div>
    <generic-button id="modalStayOnPage"
                    :button-classes="['nhsuk-button', $style['nhsuk-button-full-width']]"
                    @click.prevent="stayOnPage">
      {{ $t('navigation.keywordReplyLeavePage.stayButton') }}
    </generic-button>

    <div :class="$style.leavePanel">
      <a id="modalLeavePage"
         tabindex="0"
         :class="[$style['nhsuk-action-link__link'], $style.leaveLink]"
         :href="redirectPath"
         data-purpose="leavePage"
         @click.prevent="leavePage">
        {{ $t('navigation.keywordReplyLeavePage.leaveButton') }}
      </a>
    </div>

  </div>

</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';

export default {
  name: 'KeywordReplyLeavingPageWarningModal',
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
