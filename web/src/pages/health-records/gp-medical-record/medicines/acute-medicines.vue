<template>
  <div>
    <Medicines :medicines="acuteMedicines" :show-error="showError"/>
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
      acuteMedicines: null,
    };
  },
  computed: {
    getBackPath() {
      return MEDICINES_PATH;
    },
    showError() {
      return this.acuteMedicines &&
        (this.acuteMedicines.hasErrored || this.acuteMedicines.length === 0);
    },
  },
  async mounted() {
    if (!this.$store.state.myRecord.record.medications) {
      await this.$store.dispatch('myRecord/load');
    }
    this.acuteMedicines =
      this.$store.state.myRecord.record.medications.data.acuteMedications;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>
