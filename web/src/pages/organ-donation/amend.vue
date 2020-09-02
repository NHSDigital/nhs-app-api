<template>
  <div id="mainDiv" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list>
        <find-out-more-link/>
      </menu-item-list>
      <make-decision/>
      <generic-button v-if="!$store.state.device.isNativeApp"
                      id="back-button"
                      :class="['nhsuk-button', 'nhsuk-button--secondary']"
                      @click="goBack" >
        {{ $t('generic.back') }}
      </generic-button>
    </div>
  </div>
</template>

<script>
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import GenericButton from '@/components/widgets/GenericButton';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import MenuItemList from '@/components/MenuItemList';
import { INDEX_PATH, ORGAN_DONATION_PATH } from '@/router/paths';
import { isNativeApp } from '@/components/NativeOnlyMixin';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    FindOutMoreLink,
    GenericButton,
    MakeDecision,
    MenuItemList,
  },
  mounted() {
    if (!isNativeApp({ route: this.$route, store: this.$store })) {
      redirectTo(this, INDEX_PATH);
    } else if (!this.$store.state.organDonation.isAmending) {
      redirectTo(this, ORGAN_DONATION_PATH);
    }
  },
  methods: {
    goBack() {
      this.$store.dispatch('organDonation/amendCancel');
      redirectTo(this, ORGAN_DONATION_PATH);
    },
  },
};
</script>
