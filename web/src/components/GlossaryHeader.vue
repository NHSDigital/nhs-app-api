<template>
  <div :class="$style.info">
    <p>{{ $t('rp01.glossary.headerText') }}</p>
    <a :href="gloasaryLinkURL" target="_blank">
      <abbreviations-arrow-right-icon />
      {{ $t('rp01.glossary.linkText') }}
    </a>
    <hr>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AbbreviationsArrowRightIcon from './icons/AbbreviationsArrowRightIcon';

export default {
  components: {
    AbbreviationsArrowRightIcon,
  },
  data() {
    return {
      gloasaryLinkURL: process.env.GLOSSARY_LINK_LOCATION,
    };
  },
};
</script>

<style module scoped lang="scss">
    .info {
        margin-bottom: 0.5em;

        p, a {
            padding-bottom: 0.5em;
            padding-top: 0.5em;
        }
    }
</style>
