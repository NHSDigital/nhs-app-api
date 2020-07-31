<template>
  <div :class="$style.info" data-purpose="info">
    <message-dialog message-type="warning" :icon-text="$t('messageIconText.important')">
      <message-text id="warning-text">
        {{ $t('nominatedPharmacyNotFound.warningText') }}
      </message-text>
    </message-dialog>
    <p id="instruction">
      {{ $t('nominatedPharmacyNotFound.line') }}
    </p>
    <a id="link-to-nominate-pharmacy"
       :class="[$style.checkFeaturesLink, $style['link']]"
       :href="nominatedPharmacyInterruptPath"
       tag="a"
       @click.prevent="goToAddOrChangeNominatedPharmacy">
      {{ $t('nominatedPharmacyNotFound.nominatedPharmacyLink') }}
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
@import "../../style/colours";
@import '../../style/info';

.link {
  margin-top: 0.5em;
  margin-bottom: 1.5em;
  cursor: pointer;
  text-decoration: underline;
  color: $nhs_blue;
  display: block;
  font-weight: bold;
}

</style>
