<template>
  <message-dialog
    message-id="corona-virus-warning"
    message-type="warning"
    :icon-text="$t('coronaVirus.title')">
    <p class="nhsuk-u-margin-bottom-0">
      <span class="nhsuk-u-font-weight-bold">
        {{ $t('coronaVirus.paragraph1Highlight') }}
      </span>
      {{ $t('coronaVirus.paragraph1') }}
    </p>
    <ul class="nhsuk-list nhsuk-list--bullet nhsuk-u-margin-bottom-0">
      <li class="nhsuk-u-margin-bottom-0">{{ $t('coronaVirus.listItem1') }}</li>
      <li class="nhsuk-u-margin-bottom-0">{{ $t('coronaVirus.listItem2') }}</li>
    </ul>
    <p class="nhsuk-u-font-weight-bold nhsuk-u-padding-top-0">{{ $t('coronaVirus.paragraph2') }}</p>
    <p class=" nhsuk-u-padding-top-0"><a :href="coronaVirusUrl" target="_blank">
      {{ $t('coronaVirus.paragraph3') }}</a>.
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
  data() {
    return {
      coronaVirusUrl: this.$store.app.$env.CORONAVIRUS_INFORMATION_URL,
    };
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
    padding: 0em 0 0 2.5em;
  }
</style>
