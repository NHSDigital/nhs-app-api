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
        {{ $t('generic.backButton.text') }}
      </generic-button>
    </div>
  </div>
</template>

<script>
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import GenericButton from '@/components/widgets/GenericButton';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import MenuItemList from '@/components/MenuItemList';
import { INDEX, ORGAN_DONATION } from '@/lib/routes';
import { isNativeApp } from '@/components/NativeOnlyMixin';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    FindOutMoreLink,
    GenericButton,
    MakeDecision,
    MenuItemList,
  },
  fetch({ redirect, route, store }) {
    if (!isNativeApp({ route, store })) {
      redirect(INDEX.path);
    } else if (!store.state.organDonation.isAmending) {
      redirect(ORGAN_DONATION.path);
    }
  },
  methods: {
    goBack() {
      this.$store.dispatch('organDonation/amendCancel');
      redirectTo(this, ORGAN_DONATION.path);
    },
  },
};
</script>
