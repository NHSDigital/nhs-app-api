<template>
  <div>
    <Medicines :medicines="currentMedicines" :show-error="showError"/>

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
      return this.currentMedicines.hasErrored
             || (this.currentMedicines && this.currentMedicines.length === 0);
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.medications) {
      await store.dispatch('myRecord/load');
    }
    return {
      currentMedicines: store.state.myRecord.record.medications.data.currentRepeatMedications,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath, null);
    },
  },
};
</script>

<style module scoped lang="scss">
@import "../../../style/colours";
@import "../../../style/desktopWeb/accessibility";
</style>
