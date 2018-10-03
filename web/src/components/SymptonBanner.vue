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
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/buttons";
@import "../style/home";
</style>
