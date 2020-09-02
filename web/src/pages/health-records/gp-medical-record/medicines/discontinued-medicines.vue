<template>
  <div>
    <Medicines :medicines="discontinuedMedicines" :show-error="showError"/>

    <glossary v-if="!showError"/>
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="getBackPath"
      :button-text="'generic.back'"
      @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Medicines from '@/components/gp-medical-record/SharedComponents/Medicines';
import { MEDICINES_PATH } from '@/router/paths';
import Glossary from '@/components/Glossary';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    DesktopGenericBackLink,
    Glossary,
    Medicines,
  },
  data() {
    return {
      discontinuedMedicines: null,
    };
  },
  computed: {
    getBackPath() {
      return MEDICINES_PATH;
    },
    showError() {
      return this.discontinuedMedicines &&
        (this.discontinuedMedicines.hasErrored || this.discontinuedMedicines.length === 0);
    },
  },
  async mounted() {
    if (!this.$store.state.myRecord.record.medications) {
      await this.$store.dispatch('myRecord/load');
    }
    this.discontinuedMedicines =
      this.$store.state.myRecord.record.medications.data.discontinuedRepeatMedications;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>
