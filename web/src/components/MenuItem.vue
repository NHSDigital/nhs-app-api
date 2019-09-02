<template>
  <li :class="$style.listMenuItem">
    <analytics-tracked-tag :id="id"
                           :href="href"
                           :text="text"
                           :aria-label="text"
                           :tag="tag"
                           :class="$style['no-decoration']"
                           :click-param="clickParam"
                           :click-func="clickFunc">
      <component :is="headerTag">{{ text }}</component>
      <p>{{ description }}</p>
    </analytics-tracked-tag>
  </li>
</template>
<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'MenuItem',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    href: {
      type: String,
      default: undefined,
    },
    ariaLabel: {
      type: String,
      default: undefined,
    },
    theClass: {
      type: String,
      default: undefined,
    },
    text: {
      type: String,
      default: undefined,
    },
    description: {
      type: String,
      default: undefined,
    },
    tag: {
      type: String,
      default: 'a',
    },
    headerTag: {
      type: String,
      default: 'span',
    },
    clickFunc: {
      type: Function,
      default: undefined,
    },
    clickParam: {
      type: String,
      default: '',
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "../style/accessibility";
@import "../style/listmenu";
@import "../style/colours";
@import "../style/webshared";
@import "../style/info";

.listMenuItem {
  font-family: $default-web;
  font-weight: lighter;

  .no-decoration {
    text-decoration: none;
  }

  a {
    @extend .focusBorder;
    &:hover {
      color: #000;
    }
   padding-top: 0.5em;
   padding-bottom: 0.5em;
  }
}
</style>

