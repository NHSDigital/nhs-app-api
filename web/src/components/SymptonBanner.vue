<template>
  <div :class="$style.symptomBanner">
    <h2>{{ $t('symptomBanner.howAreYouFeeling') }}</h2>
    <generic-button id="btn_home_symptoms" :class="[$style.button, $style.white]"
                    @click="checkSymptomsButtonClicked()">
      {{ $t('symptomBanner.checker') }}
    </generic-button>
  </div>
</template>
<script>

import GenericButton from '@/components/widgets/GenericButton';

export default {
  components: {
    GenericButton,
  },
  methods: {
    checkSymptomsButtonClicked() {
      this.$store.dispatch('device/goToCheckSymptoms');
      // this method will be refactored to the following:
      // const sourceValue = this.$store.state.device.source;
      // this.$router.push({
      //   path: '/check-your-symptoms',
      //   query: { source: sourceValue },
      // });
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/buttons";
@import "../style/home";
</style>
