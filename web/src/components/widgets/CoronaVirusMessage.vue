<template>
  <message-dialog
    message-id="corona-virus-warning"
    message-type="warning"
    :icon-text="$t('coronaVirus.title')">
    <p class="nhsuk-u-margin-bottom-0">
      <span>
        {{ $t('coronaVirus.paragraphText1') }}
      </span>
      <a href="https://www.gov.uk/government/publications/covid-19-specified-countries-and-areas/covid-19-specified-countries-and-areas-with-implications-for-returning-travellers-or-visitors-arriving-in-the-uk" target="_blank">
        {{ $t('coronaVirus.linkText1') }}</a>
      <span>
        {{ $t('coronaVirus.paragraphText2') }}
      </span>
    </p>
    <br>
    <p class=" nhsuk-u-padding-top-0"><a href="https://www.nhs.uk/conditions/coronavirus-covid-19/" target="_blank">
      {{ $t('coronaVirus.linkText2') }}</a>.
    </p>
  </message-dialog>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';

export default {
  name: 'CoronaVirusMessage',
  components: {
    MessageDialog,
  },
};
</script>
<style module lang="scss" scoped>
  a {
    display: inline;
    cursor: pointer;
  }
  p {
    padding: 1em 1em 0 1.4em;
    display: block;
  }
  ul {
    padding: 0 1em 0 2.5em;
  }
</style>
