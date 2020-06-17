<template>
  <p data-purpose="msg-text">
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
  },
};
</script>

<style lang="scss" module scoped>
a {
  display: inline-block;
  vertical-align: unset;
  padding: 0;
}
</style>
