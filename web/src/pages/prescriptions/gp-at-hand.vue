<template>
  <div v-if="showTemplate"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]" >
    <div>
      <h2 id="guidance_sub_header">
        {{ $t('prescriptions.gp_at_hand.header') }}
      </h2>
      <div data-purpose="info" :class="$style.info">
        <p v-for="(content, index) in $t('prescriptions.gp_at_hand.paragraphs')"
           :key="index">
          {{ content.prefix }}
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
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  components: {
    AnalyticsTrackedTag,
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";
@import "../../style/nhsuklinks";
@import "../../style/textstyles";

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
