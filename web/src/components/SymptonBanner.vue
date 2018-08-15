<template>
  <div :class="$style.symptomBanner">
    <h2>{{ $t('symptomBanner.howAreYouFeeling') }}</h2>
    <button id="btn_home_symptoms" :class="[$style.button, $style.white]"
            @click="checkSymptomsButtonClicked()">
      {{ $t('symptomBanner.checker') }}
    </button>
  </div>
</template>
<script>
export default {
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
