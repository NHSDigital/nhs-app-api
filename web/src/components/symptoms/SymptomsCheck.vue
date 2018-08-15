<template>
  <div v-if="showTemplate" :class="$style['no-padding']" data-purpose="">
    <ul :class="$style['list-menu']">
      <li>
        <a id="btn_choices" :href="conditionsCheckerUrl"
           style="text-decoration: none" target="_blank">
          <h2>{{ $t('sy01.a_z.subheader') }}</h2>
          <p>{{ $t('sy01.a_z.body') }}</p>
        </a>
      </li>
      <li>
        <a id="btn_111" :href="symptomsCheckerUrl"
           style="text-decoration: none" target="_blank">
          <h2>{{ $t('sy01.111.subheader') }}</h2>
          <p>{{ $t('sy01.111.body') }}</p>
        </a>
      </li>
    </ul>
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

<style module lang="scss" scoped>
@import '../../style/buttons';
@import '../../style/listmenu';
.no-padding {
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
}

</style>
