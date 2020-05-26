<template>
  <div>
    <Medicines :medicines="acuteMedicines" :show-error="showError"/>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="getBackPath"
      :button-text="'rp03.backButton'"
      @clickAndPrevent="backButtonClicked"/>
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
      return this.acuteMedicines.hasErrored
             || (this.acuteMedicines && this.acuteMedicines.length === 0);
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.medications) {
      await store.dispatch('myRecord/load');
    }
    return {
      acuteMedicines: store.state.myRecord.record.medications.data.acuteMedications,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>
