<template>
  <div>
    <p>{{ $t('glossary.headerText') }}</p>
    <nhs-arrow-banner :banner-text="$t('glossary.linkText')"
                      :click-action="glossaryLinkURL"
                      :is-analytics-tracked="true"/>
  </div>
</template>

<script>
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';
import {
  CLINICAL_ABBREVIATIONS_URL,
} from '@/router/externalLinks';

export default {
  name: 'Glossary',
  components: {
    NhsArrowBanner,
  },
  data() {
    return {
      glossaryLinkURL: CLINICAL_ABBREVIATIONS_URL,
    };
  },
};
</script>
