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
import {
  CLINICAL_ABBREVIATIONS_HELP_PATH,
} from '@/router/externalLinks';

export default {
  name: 'Glossary',
  components: {
    NhsArrowBanner,
  },
  data() {
    return {
      glossaryLinkURL: `${this.$store.$env.BASE_NHS_APP_HELP_URL}${CLINICAL_ABBREVIATIONS_HELP_PATH}`,
    };
  },
};
</script>
