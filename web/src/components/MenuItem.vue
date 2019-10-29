<template>
  <li :class="$style.listMenuItem">
    <analytics-tracked-tag :id="id"
                           :href="href"
                           :text="text"
                           :aria-label="ariaLabel || text"
                           :tag="tag"
                           :target="target"
                           :class="[$style['no-decoration'], $style.listMenuItemLink]"
                           :click-param="clickParam"
                           :prevent-default="preventDefault"
                           :click-func="clickFunc">
      <span :class="$style.listMenuItemContainer">
        <component :is="headerTag">{{ text }}</component>
        <p v-if="description">{{ description }}</p>
        <slot/>
      </span>
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
    target: {
      type: String,
      default: undefined,
    },
    preventDefault: {
      type: Boolean,
      default: true,
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
@import '../style/accessibility';
@import '../style/desktopWeb/accessibility';
@import '../style/textstyles';
@import '../style/fonts';
@import '../style/colours';

.listMenuItemLink {
  display: block;
  box-sizing: border-box;
  background: $white url(~assets/icon_arrow_left.svg) no-repeat center right;
  background-position: right 1em center;
  margin-left: 0;

  border-top: 1px $border_grey solid;
  border-bottom: 1px $border_grey solid;

  font-size: 1em;

  &:hover {
    @include outlineStyleLightMenuItem;
    color: #000;
  }

  &:focus {
    @include focusStyleLightMenuItem;
    color: #000;
  }

  &.active {
    outline: none;
    text-decoration: underline;
  }
}

button.listMenuItemLink {
  display: block;
  width: 100%;
  color: $nhs_blue;
  text-align: left;
  font-weight: bold;
  border-left: none;
  border-right: none;
}

.no-decoration {
  text-decoration: none;
}

.listMenuItem {
  font-family: $default-web;
  font-weight: lighter;
  display: block;
  margin-bottom: 5px;

  :focus {
    outline: none;
  }

  .listMenuItemContainer {
    padding: 1em;
    display: block;

    h2 {
      margin: 0;
    }

    h3 {
      @include h3;
      padding-top: 1em;
    }

    h4 {
      @include h4;
      padding-bottom: 0.5em;
      padding-top: 0.5em;
    }

    p {
      padding-bottom: 0.5em;
      padding-top: 0.5em;
      margin-bottom: 0;
      width: 90%;
    }
  }
}
</style>

