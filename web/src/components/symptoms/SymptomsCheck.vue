<template>
  <div v-if="showTemplate" id="mainDiv">
    <main :class="$style['panel-title']">
      <div>
        <ul :class="$style['list-menu']">
          <li>
            <a id="btn_choices" :href="conditionsCheckerUrl" style="text-decoration: none">
              <h2>{{ $t('symptoms.healthAZHeader') }}</h2>
              <p>{{ $t('symptoms.healthAZText') }}</p>
            </a>
          </li>
          <li>
            <a id="btn_111" :href="symptomsCheckerUrl" style="text-decoration: none">
              <h2>{{ $t('symptoms.medicalHelpHeader') }}</h2>
              <p>{{ $t('symptoms.medicalHelpText') }}</p>
            </a>
          </li>
        </ul>
      </div>
    </main>
  </div>
</template>

<script>

export default {
  data() {
    return {
      symptomsCheckerUrl: process.env.SYMPTOM_CHECKER_URL,
      conditionsCheckerUrl: process.env.CONDITIONS_CHECKER_URL,
    };
  },
};
</script>

<style module lang="scss">
@import "../../style/html";
@import "../../style/fonts";
@import "../../style/buttons";
@import "../../style/elements";

.list-menu li a h2, .form a h2, .panel-title a h2 {
    color: #005EB8;
}

ul.list-menu li {
    box-sizing: border-box;
    background: transparent url('~/assets/icon_arrow_left.svg') no-repeat center right;
    background-position: right 1em center;
    position: relative;
    margin-bottom: 0em;
    margin-left: 0em;
    padding: 1em 3em 0.250em 1em;
    border-bottom: 1px #D8DDE0 solid;
}

.menu li a {
    display: block;
    font-weight: normal;
    font-size: 1em;
    line-height: 1.5em;
    color: #212B32;
    font-size: 1.250em;
    display: block;
    text-decoration: underline;
    color: #005EB8;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
}

menu li h2, .form h2, .panel-title h2 {
    font-family: "FrutigerLTW01-65Bold", Arial, sans-serif;
    font-weight: 700;
    font-size: 1.375em;
    line-height: 1.375em;
    color: #425563;
    padding-bottom: 0.2em;
    padding-top: 0.5em;
}

main.content.nopadding {
    padding: 3.125em 3em 3.125em 0em;
}
</style>
