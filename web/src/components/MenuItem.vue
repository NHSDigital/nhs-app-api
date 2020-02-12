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
      <span :class="[$style.listMenuItemContainer, showCount ? $style['countWidth'] : '']">
        <div :class="$style['internalWrapper']">
          <component :is="headerTag"
                     :class="['nhsuk-heading-s']"
                     :aria-label="ariaText">{{ text }}</component>
          <div v-if="showCount"
               id="count"
               :class="['nhsuk-u-font-weight-regular',
                        $style['count']]"
               aria-hidden="true">{{ count }}</div>
          <p v-if="description"
             :id="descriptionId"
             :data-sid="descriptionDataSid"
             class="nhsuk-u-margin-bottom-3">{{ description }}</p>
        </div>
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
    descriptionId: {
      type: String,
      default: undefined,
    },
    descriptionDataSid: {
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
      type: [String, Object],
      default: undefined,
    },
    count: {
      type: Number,
      default: undefined,
    },
  },
  computed: {
    showCount() {
      return this.count !== undefined;
    },
    ariaText() {
      return this.showCount ? `${`${this.text} (${this.count} `}${this.$t('my_record.records')})` : this.text;
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
  @import '../style/arrow';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/settings/colours';

  .listMenuItemLink {
    @include icon-arrow-left-white-background;
    display: block;
    box-sizing: border-box;
    margin-left: 0;

    border-top: 1px $border_grey solid;
    border-bottom: 1px $border_grey solid;

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
    display: block;
    margin-bottom: 5px;

    :focus {
      outline: none;
    }

    .listMenuItemContainer {
      padding: 0.2em 0.5em;
      display: block;
      cursor: pointer;

      h2, p {
        padding-left:10px;
        width: 98%;
        display: inline-block;
      }

      h2 {
        margin: 0;
      }

      h3 {
        @include h3;
      }

      h4 {
        @include h4;
        padding-bottom: 0.5em;
        padding-top: 0.5em;
      }

      p {
        color: #000;
      }
    }
  }

  // Please be careful when changing the below 2 classes these are needed
  // to keep the medical record count in alignment!!!
  .internalWrapper {
    display: block;
    padding-right: 28px;
    width: 100%;
    position: relative;
  }

  .count {
    color: $nhsuk-text-color;
    font-weight: normal;
    position: absolute;
    display: inline-block;
    top: 50%;
    transform: translate(-50%, -50%);
  }
  @media screen and (device-aspect-ratio: 2/3) {
    .count {
      display: none;
    }
  }

  @media screen and (device-aspect-ratio: 40/71) {
    .count {
      display: none;
    }
  }
</style>
