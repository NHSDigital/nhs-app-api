<template>
  <div>
    <p>{{ $t('rp01.glossary.headerText') }}</p>
    <nhs-arrow-banner :banner-text="$t('rp01.glossary.linkText')"
                      :click-action="glossaryLinkURL"
                      :is-analytics-tracked="true"/>
  </div>
</template>

<script>
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';

export default {
  name: 'Glossary',
  components: {
    NhsArrowBanner,
  },
  data() {
    return {
      glossaryLinkURL: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
};
</script>
