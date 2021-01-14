<template>
  <div :class="$style.info" data-purpose="info">
    <message-dialog message-type="warning" :icon-text="$t('generic.important')">
      <message-text id="warning-text">
        {{ $t('nominatedPharmacy.notFound.warningText') }}
      </message-text>
    </message-dialog>
    <p id="instruction">
      {{ $t('nominatedPharmacy.notFound.line') }}
    </p>
    <a id="link-to-nominate-pharmacy"
       :class="[$style.checkFeaturesLink, $style['link']]"
       :href="nominatedPharmacyInterruptPath"
       tag="a"
       @click.prevent="goToAddOrChangeNominatedPharmacy">
      {{ $t('nominatedPharmacy.notFound.nominatedPharmacyLink') }}
    </a>
  </div>
</template>

<script>
/* eslint-disable global-require */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { NOMINATED_PHARMACY_INTERRUPT_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'NoNominatedPharmacyWarning',
  components: {
    MessageDialog,
    MessageText,
  },
  data() {
    return {
      nominatedPharmacyInterruptPath: NOMINATED_PHARMACY_INTERRUPT_PATH,
    };
  },
  methods: {
    goToAddOrChangeNominatedPharmacy() {
      redirectTo(this, this.nominatedPharmacyInterruptPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/no-nominated-pharmacy-warning";
</style>
