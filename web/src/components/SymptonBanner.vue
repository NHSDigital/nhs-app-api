<template>
  <div class="symptom_banner">
    <h2>{{ $t('symptomBanner.howAreYouFeeling') }}</h2>
    <a id="btn_home_symptoms" :href="symptomCheckerUrl" class="button white">
      {{ $t('symptomBanner.checker') }}
    </a>
  </div>
</template>

<script>
export default {
  data() {
    return {
      symptomCheckerUrl: process.env.SYMPTOM_CHECKER_URL,
    };
  },
};
</script>

<style lang="scss"  scoped>
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
