<template>
  <div class="symptom_banner">
    <h2>{{ $t('symptomBanner.howAreYouFeeling') }}</h2>
    <button id="btn_home_symptoms" class="button white" @click="checkSymptomsButtonClicked()">
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
<style lang="scss" scoped>
@import "../style/textstyles";
@import "../style/buttons";
.symptom_banner {
  text-align: center;
  width: 100%;
  vertical-align: bottom;

  h2 {
    @include h2;
    margin-bottom: 16px;
  }

  button.button,
  a.button {
    margin-bottom: 5px;
  }
}
</style>
