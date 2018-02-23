<template>
  <div class="symptom_banner">
    <h2>{{ $t('symptom.howAreYouFeeling') }}</h2>
    <button class="button white" id="btn_home_symptoms">
      {{ $t('symptom.checker') }}
    </button>
  </div>
</template>

<style lang="scss"  scoped>
  @import '../style/colours';
  @import '../style/textstyles';
  @import '../style/fonts';
  @import '../style/buttons';
  @import '../style/icons';
  .symptom_banner {
    text-align: center;
    width: 100%;
    vertical-align: bottom;

    button.button {
      margin-bottom: 5px;
    }
  }
</style>
