<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list>
        <find-out-more-link/>
      </menu-item-list>
      <make-decision/>
      <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                              id="back-link"
                              :path="organDonationPath"
                              :button-text="'generic.back'"
                              @clickAndPrevent="backButtonClicked"/>
    </div>
  </div>
</template>

<script>
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import MenuItemList from '@/components/MenuItemList';
import { INDEX_PATH, ORGAN_DONATION_PATH } from '@/router/paths';
import { isNativeApp } from '@/components/NativeOnlyMixin';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    FindOutMoreLink,
    DesktopGenericBackLink,
    MakeDecision,
    MenuItemList,
  },
  data() {
    return {
      organDonationPath: ORGAN_DONATION_PATH,
    };
  },
  mounted() {
    if (!isNativeApp({ store: this.$store })
    && !this.$store.$env.ORGAN_DONATION_DESKTOP_ENABLED) {
      redirectTo(this, INDEX_PATH);
    } else if (!this.$store.state.organDonation.isAmending) {
      redirectTo(this, ORGAN_DONATION_PATH);
    }
  },
  methods: {
    backButtonClicked() {
      this.$store.dispatch('organDonation/amendCancel');
      redirectTo(this, ORGAN_DONATION_PATH);
    },
  },
};
</script>
