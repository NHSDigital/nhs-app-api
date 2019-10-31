<template>
  <div>
    <Medicines :medicines="discontinuedMedicines" :show-error="showError"/>

    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="getBackPath"
      :button-text="'rp03.backButton'"
      @clickAndPrevent="backButtonClicked"
    />
    <glossary v-if="!showError"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Medicines from '@/components/gp-medical-record/SharedComponents/Medicines';
import { MEDICINES } from '@/lib/routes';
import Glossary from '@/components/Glossary';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    Glossary,
    Medicines,
  },
  computed: {
    getBackPath() {
      return MEDICINES.path;
    },
    showError() {
      return this.discontinuedMedicines.hasErrored
             || (this.discontinuedMedicines && this.discontinuedMedicines.length === 0);
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.medications) {
      await store.dispatch('myRecord/load');
    }
    return {
      discontinuedMedicines:
        store.state.myRecord.record.medications.data.discontinuedRepeatMedications,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>

<style module scoped lang="scss">
@import "../../../style/colours";
@import "../../../style/desktopWeb/accessibility";
</style>
