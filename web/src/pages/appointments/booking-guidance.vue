<template>
  <main v-if="showTemplate" :class="$style.main">
    <div :class="$style.info">
      <h2>{{ $t('appointments.guidance.header') }}</h2>
      <p>{{ $t('appointments.guidance.text') }}</p>
      <ol>
        <li>
          <strong>{{ $t('appointments.guidance.li1.header') }}</strong>
          <p>{{ $t('appointments.guidance.li1.text') }}</p>
        </li>
        <li>
          <strong>{{ $t('appointments.guidance.li2.header') }}</strong>
          <p>{{ $t('appointments.guidance.li2.text') }}</p>
        </li>
        <li>
          <strong>{{ $t('appointments.guidance.li3.header') }}</strong>
          <p>{{ $t('appointments.guidance.li3.text') }}</p>
        </li>
      </ol>

      <button id="btn_check_symptoms" :class="$style.button" @click="onCheckSymptomClicked">
        {{ $t('appointments.guidance.symptomButttonText') }}
      </button>
      <button id="btn_appointment" :class="[$style.button, $style.green]"
              @click="onBookButtonClicked">
        {{ $t('appointments.guidance.bookButtonText') }}
      </button>
    </div>
  </main>
</template>

<script>
export default {
  middleware: ['auth', 'meta'],
  data() {
    return {
      symptomsCheckerUrl: process.env.SYMPTOM_CHECKER_URL,
    };
  },
  methods: {
    onCheckSymptomClicked() {
      window.open(this.symptomsCheckerUrl, '_blank');
    },
    onBookButtonClicked() {
      this.$router.push('/appointments/booking');
    },
  },
};

</script>

<style module lang="scss">
@import "../../style/spacings";
@import "../../style/colours";
@import "../../style/textstyles";
@import "../../style/buttons";
.main {
  @include space(padding, all, $three);
  .info {
    h2 {
      @include h2Blue;
      margin-bottom: 0.5em
    }
    p {
      @include default_text;
      font-size: 1em;
      display: block;
      margin-bottom: 1em;
    }
    ol {
      li {
        font-weight: bold;
        list-style-type: decimal;
        list-style-position: inside;
        p {
          font-weight: normal;
        }
      }
    }
  }
}

</style>
