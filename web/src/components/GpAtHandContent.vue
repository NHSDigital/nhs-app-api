<template>
  <div :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]" >
    <h2 id="guidance_sub_header">
      {{ headerText }}
    </h2>
    <div data-purpose="info" :class="$style.info">
      <p v-for="(content, index) in $t('gp_at_hand.content.paragraphs')"
         :key="index">
        {{ replaceContentTag(content.prefix) }}
        <analytics-tracked-tag
          :href="content.linkUrl"
          :text="content.linkText"
          tag="a" target="_blank">
          {{
            content.linkText
          }}</analytics-tracked-tag>{{
          content.suffix }}
      </p>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'GpAtHandContent',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    headerTag: {
      type: String,
      required: true,
    },
    contentTag: {
      type: String,
      required: true,
    },
  },
  computed: {
    contentTagText() {
      return this.$t(this.contentTag);
    },
    headerText() {
      const headerTagText = this.$t(this.headerTag);
      return this.$t('gp_at_hand.content.header').replace('{headerTag}', headerTagText);
    },
  },
  methods: {
    replaceContentTag(content) {
      return content.replace('{contentTag}', this.contentTagText);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/info";
@import "../style/nhsuklinks";
@import "../style/textstyles";
.pull-content{
  display: block;
}
a {
  text-decoration: underline;
}
div {
  .info {
    a {
      display: inline;
      vertical-align: unset;
      font-weight: normal;
      padding: 0;
    }
  }
  .desktopWeb {
    max-width: 540px;
    .info {
      p {
        font-family: $default-web;
        font-weight: lighter;
      }
    }
  }
}
</style>
