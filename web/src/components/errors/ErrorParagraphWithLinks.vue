<template>
  <p data-purpose="msg-text" :aria-label="getAriaLabel()">
    <template v-for="(content, index) in $t(from)">
      {{ content.text }}
      <analytics-tracked-tag v-if="content.linkText"
                             :key="index"
                             :href="getURL(content)"
                             :text="content.linkText"
                             tag="a" target="_blank">
        {{ content.linkText }}
      </analytics-tracked-tag>
    </template>
  </p>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'ErrorParagraphWithLinks',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    from: {
      type: String,
      required: true,
    },
    queryParam: {
      type: Object,
      default: () => ({
        param: undefined,
        value: undefined,
      }),
    },
  },
  methods: {
    getURL(content) {
      let url;
      if (content.linkUrl) {
        url = content.linkUrl;
      } else {
        url = this.$store.$env.CONTACT_US_URL;
      }
      if (this.queryParam.param) {
        url += `?${this.queryParam.param}=${this.queryParam.value}`;
      }
      return url;
    },
    getAriaLabel() {
      const sections = this.$t(this.from);
      let ariaLabel = '';
      sections.forEach((section) => {
        if (section.label) {
          ariaLabel += section.label;
        }
      });
      return ariaLabel.length > 0 ? ariaLabel : undefined;
    },
  },
};
</script>

<style lang="scss" module scoped>
  @import "@/style/custom/error-paragraph-with-links"
</style>
