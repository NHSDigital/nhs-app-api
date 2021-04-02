<template>
  <div>
    <p>{{ $t('glossary.youMaySeeAbbreviationsYouAreNotFamilarWith') }}</p>
    <nhs-arrow-banner id="glossary-link"
                      :banner-text="$t('glossary.helpWithAbbreviations')"
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
      glossaryLinkURL: this.$store.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
};
</script>
